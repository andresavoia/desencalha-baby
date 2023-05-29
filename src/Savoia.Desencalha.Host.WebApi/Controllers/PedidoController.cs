using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver.Linq;
using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Domain.Models;
using Savoia.Desencalha.Domain.Repositories;
using Savoia.Desencalha.Domain.Resources;
using Savoia.Desencalha.Host.WebApi.Dtos.Pedido;
using Savoia.Desencalha.Host.WebApi.Dtos.Produto;
using Savoia.Desencalha.Host.WebApi.Extensions;
using Savoia.Desencalha.Host.WebApi.Filters;
using Savoia.Desencalha.Host.WebApi.Mappers;
using Savoia.Desencalha.Host.WebApi.Messages;
using Savoia.Desencalha.Host.WebApi.Messages.Pedido;
using Savoia.Desencalha.Host.WebApi.Util;
using Savoia.Desencalha.Host.WebApi.Validators;
using Savoia.Desencalha.Infrastructure.Mongo.Mappers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Transactions;
using static Savoia.Desencalha.Common.Util.ConstantesUtil;
using static Savoia.Desencalha.Host.WebApi.Helpers.ResponseHelper;

namespace Savoia.Desencalha.Host.WebApi.Controllers
{
    [Route("pedidos")]
    public class PedidoController : BaseController
    {

        internal IPedidoValidator pedidoValidator;
        internal IProdutoRepository produtoRepository;
        internal IPedidoRepository pedidoRepository;
        internal IClienteRepository clienteRepository;
        internal IFreteEstadoRepository freteEstadoRepository;
        readonly IWebHostEnvironment environment;

        public PedidoController(IPedidoValidator pedidoValidator,
                           IPedidoRepository pedidoRepository,
                           IClienteRepository clienteRepository,
                           IFreteEstadoRepository freteEstadoRepository,
                           IProdutoRepository produtoRepository,
                           IEmailRepository emailRepository,
                           IConfiguration configuration,
                           IWebHostEnvironment environment)    
        {
            this.pedidoValidator = pedidoValidator;
            this.pedidoRepository = pedidoRepository;
            this.clienteRepository = clienteRepository;
            this.freteEstadoRepository = freteEstadoRepository;
            this.produtoRepository = produtoRepository;
            this.emailRepository = emailRepository;
            this.configuration = configuration;
            this.environment = environment;
        }

        [HttpPatch]
        [ProducesResponseType(typeof(ManterPedidoResponse), statusCode: StatusCodes.Status200OK)]
        [Authorize(Roles = ConstantesUtil.USUARIO_TIPO_CLI_ADM + "," + ConstantesUtil.USUARIO_TIPO_ADM)]
        public async Task<ActionResult> ManterPedidoAsync([FromBody] ManterPedidoRequest request)
        {
            try
            {
                var response = pedidoValidator.Validar(request);

                if (response.Valido)
                {

                    var pedidoBase = pedidoRepository.ObterAsync(request.IdPedido).Result;

                    string idCliente = null;

                    if (UsuarioAutenticacaoTipo.Equals(ConstantesWebUtil.UsuarioAutenticacaoTipo.Cliente))
                    {
                        idCliente = ClienteAutenticado.IdCliente;
                    }
                    else
                    {
                        idCliente = request.IdVendedor;
                    }

                    var pedidoVendedor = pedidoRepository.ObterPedidoVendedorAsync(idCliente, request.IdPedido, true).Result.FirstOrDefault();

                    //retornando itens para o estoque
                    if(pedidoBase.CodPedidoStatus != (int)ConstantesUtil.PedidoVendedorStatus.Cancelado &&
                        request.CodPedidoStatus == (int)ConstantesUtil.PedidoVendedorStatus.Cancelado)
                    {
                        foreach(var item in pedidoVendedor.Itens)
                        {
                            await produtoRepository.AtualizarEstoqueAsync(item.IdProduto,item.Qt * (-1));
                        }
                    }

                    #region -- Pedido Vendedor --

                    if (UsuarioAutenticacaoTipo.Equals(ConstantesWebUtil.UsuarioAutenticacaoTipo.Cliente))
                        pedidoVendedor.UsuarioAlteracao = ClienteAutenticado.Nome;
                    else
                        pedidoVendedor.UsuarioAlteracao = UsuarioAutenticado.Nome;


                    pedidoVendedor.DataAlteracao = DateTime.Now;

                    pedidoVendedor.CodPedidoVendedorStatus = request.CodPedidoStatus;
                    pedidoVendedor.Observacao = request.Observacao;

                    //Quando o pedido for enviado, salva os dados de rastreio
                    if (pedidoVendedor.CodPedidoVendedorStatus == (int)ConstantesUtil.PedidoVendedorStatus.PedidoEnviado)
                    {
                        pedidoVendedor.Rastreio = new PedidoRastreioModel();
                        pedidoVendedor.Rastreio.CodigoRastreio = request.CodigoRastreio;
                        pedidoVendedor.Rastreio.LinkRastreio = request.LinkRastreio;
                    }

                    pedidoVendedor.Anexos = new List<PedidoAnexoModel>();

                    request.Anexos.ForEach(x =>
                    {
                        pedidoVendedor.Anexos.Add(new PedidoAnexoModel()
                        {
                            CodTipoAnexo = x.CodTipoAnexo,
                            Nome = x.Nome,
                            Caminho = x.Caminho
                        });
                    });

                    //adicionando um novo log
                    if (pedidoVendedor.Logs == null) pedidoVendedor.Logs = new List<PedidoLogModel>();

                    pedidoVendedor.Logs.Add(new PedidoLogModel()
                    {
                        CodPedidoStatus = request.CodPedidoStatus,
                        DataAlteracao = DateTime.Now,
                        Observacao = request.Observacao,
                        UsuarioAlteracao = pedidoVendedor.UsuarioAlteracao
                    });

                    var resultadoPedidoVendedor = await pedidoRepository.ManterPedidoVendedorAsync(pedidoVendedor, false);


                    var pedidoVendedorDto = resultadoPedidoVendedor.MapperToPedidoVendedorDto();

                    List<string> listaAnexos = null;
                    string mensagemAdicional = string.Empty;

                    //pegando os anexos dependendo do status
                    if (pedidoVendedorDto.CodPedidoVendedorStatus == (int)ConstantesUtil.PedidoVendedorStatus.AguardandoPagto || pedidoVendedorDto.CodPedidoVendedorStatus == (int)ConstantesUtil.PedidoVendedorStatus.NotaFiscalGerada)
                    {
                        listaAnexos = new List<string>();

                        pedidoVendedorDto.Anexos.Where(x => x.CodTipoAnexo ==
                        (pedidoVendedorDto.CodPedidoVendedorStatus == (int)ConstantesUtil.PedidoVendedorStatus.AguardandoPagto ? (int)ConstantesUtil.PedidoAnexo.Boleto : (int)ConstantesUtil.PedidoAnexo.NotaFiscal)

                        ).ToList()?.ForEach(xx =>
                        {
                            listaAnexos.Add(Path.Combine(environment.WebRootPath) + xx.Caminho);
                        });

                        mensagemAdicional = (pedidoVendedorDto.CodPedidoVendedorStatus == (int)ConstantesUtil.PedidoVendedorStatus.AguardandoPagto ? "<BR><BR><strong>Verifique o(s) boleto(s) em anexo</strong>" : "<BR><BR><strong>Verifique a nota fiscal em anexo</strong>");

                        //Se o status for Agaurdando Pagamento, envia sempre o ultimo boleto adicionado
                        if (pedidoVendedorDto.CodPedidoVendedorStatus == (int)ConstantesUtil.PedidoVendedorStatus.AguardandoPagto)
                        {
                            if (listaAnexos.Count > 1)
                            {
                                listaAnexos.RemoveRange(0, listaAnexos.Count - 1);
                            }
                        }
                    }

                    if (pedidoVendedorDto.CodPedidoVendedorStatus == (int)ConstantesUtil.PedidoVendedorStatus.PedidoEnviado)
                    {
                        mensagemAdicional = "<BR><BR><strong>Verifique o rastreio do pedido</strong>";
                        mensagemAdicional += $"<BR><strong>Código de rastreio:</strong> {pedidoVendedorDto.Rastreio.CodigoRastreio}";
                        mensagemAdicional += $"<BR><strong>Link de rastreio:</strong> {pedidoVendedorDto.Rastreio.LinkRastreio}";
                    }


                    #endregion

                    //Atualiza Pedido Base de acordo com status do PedidoVendedor
                    var listaPedidoVendedor = await pedidoRepository.ListarPedidoVendedorAsync(null, request.IdPedido);
                    
                    if (listaPedidoVendedor != null && listaPedidoVendedor.Any())
                    {
                        //Se todos PedidoVendedor estiverem com status de ENTREGUE, seta pra status Finalizado o PedidoBase
                        var pedidoFinalizado = listaPedidoVendedor.All(lpv => lpv.CodPedidoVendedorStatus == (int)ConstantesUtil.PedidoVendedorStatus.Entregue);
                        if (pedidoFinalizado)
                        {
                            pedidoBase.CodPedidoStatus = (int)ConstantesUtil.PedidoStatus.Finalizado;
                            await pedidoRepository.ManterAsync(pedidoBase, false);

                        }
                    }
                    

                    var pedidoDto = pedidoBase.MapperToPedidoDto();
                    pedidoDto.PedidoVendedor = new List<PedidoVendedorDto>();
                    pedidoDto.PedidoVendedor.Add(pedidoVendedorDto);

                    response.Id = pedidoDto.IdPedido;
                    response.DescPedidoStatus = pedidoVendedorDto.DescPedidoStatus;



                    var infoVendedor = ObterListaVendedor(resultadoPedidoVendedor.Itens, pedidoDto.Cliente.EnderecoEntrega.Cidade.Estado.UF).FirstOrDefault(v => v.IdCliente == idCliente);

                    //Enviando email
                    string corpoEmail = string.Format(@"
                                        Olá,
                                        <BR><BR>
                                        O status do seu pedido do vendedor <b>{1}</b> foi alterado.
                                        {0}<BR><BR>
                                          
                                        {2}", mensagemAdicional, infoVendedor.NomeClienteVendedor, ObterCorpoEmailPedidoVendedor(pedidoDto, infoVendedor));

                    EnviarEmailAsync(corpoEmail, "Pedido Desencalha Estoque - N° " + pedidoDto.IdPedido, new List<string> { pedidoDto.Cliente.EmailPrincipal },listaAnexos);

                }


                response.Mensagens.Add(new Messages.MensagemSistema()
                {
                    Campo = "",
                    Mensagem = MessageResource.COMUM_REGISTRO_ATUALIZADO_SUCESSO
                });

                return this.GetHttpResponse(response);
            }
            catch (Exception ex)
            {
                return this.GetHttpResponseError(GetResponseError<BaseResponse>(ex));

            }

        }

        [HttpPost]
        [ProducesResponseType(typeof(CriarPedidoResponse), statusCode: StatusCodes.Status200OK)]
        [Authorize(Roles = ConstantesUtil.USUARIO_TIPO_CLI_ADM + "," + ConstantesUtil.USUARIO_TIPO_ADM)]
        public async Task<ActionResult> CriarPedidoAsync([FromBody] CriarPedidoRequest request)
        {
            try
            {                
                var response = pedidoValidator.Validar(request, ClienteAutenticado.IdCliente);

                if (response.Valido)
                {
                    var cliente = clienteRepository.ObterAsync(ClienteAutenticado.IdCliente).Result;
                    var carrinho = pedidoRepository.ObterCarrinhoAsync(ClienteAutenticado.IdCliente).Result;

                    if (carrinho == null || carrinho.DataExpiracaoToken <= DateTime.Now || carrinho.Itens == null || carrinho.Itens.Count<=0)
                    {
                        response.Valido = false;
                        response.Mensagens.Add(new Messages.MensagemSistema()
                        {
                            Mensagem = (carrinho.Itens == null ? "Não existem itens no seu carrinho" : "Carrinho expirado")
                        });
                        return this.GetHttpResponse(response);
                    }

                    //pegando o novo endereço de entrega
                    cliente.EnderecoEntrega = new EnderecoModel();
                    cliente.EnderecoEntrega.Endereco = request.EnderecoEntrega.Endereco;
                    cliente.EnderecoEntrega.Complemento = request.EnderecoEntrega.Complemento;
                    cliente.EnderecoEntrega.Bairro = request.EnderecoEntrega.Bairro;
                    cliente.EnderecoEntrega.Cep = request.EnderecoEntrega.Cep;
                    cliente.EnderecoEntrega.Cidade = new CidadeModel();
                    cliente.EnderecoEntrega.Cidade.CodCidade = request.EnderecoEntrega.Cidade.CodCidade;
                    cliente.EnderecoEntrega.Cidade.Titulo = request.EnderecoEntrega.Cidade.Titulo;
                    cliente.EnderecoEntrega.Cidade.Estado = new EstadoModel();
                    cliente.EnderecoEntrega.Cidade.Estado.CodEstado = request.EnderecoEntrega.Cidade.Estado.CodEstado;
                    cliente.EnderecoEntrega.Cidade.Estado.UF = request.EnderecoEntrega.Cidade.Estado.UF;


                    var listaVendedor = ObterListaVendedor(carrinho.Itens, request.EnderecoEntrega.Cidade.Estado.UF);

                    using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        var idPedidoUltimo = pedidoRepository.ObterPedidoUltimoAsync().Result;
                        idPedidoUltimo = (idPedidoUltimo == null ? 1  :idPedidoUltimo+1);

                        cliente.Login = null;
                        cliente.Senha = null;

                        //Pegando os dados que foram calculados no carrinho, para passar para o pedido.. sem consultar tudo novamente
                        var model = new PedidoModel()
                        {
                            Cliente = cliente,
                            CodPedidoStatus = (int)ConstantesUtil.PedidoStatus.EmAndamento,
                            DataCadastro = DateTime.Now,
                            IdPedido = (int)idPedidoUltimo,
                            Observacao = request.Observacao,
                            ValorTotal = carrinho.ValorTotal,
                            ValorTotalComFrete = carrinho.ValorTotalComFrete,
                            ValorTotalFrete = carrinho.ValorTotalFrete,
                            UsuarioAlteracao = ClienteAutenticado.Nome
                        };

                        //Agrupamento de vendedores
                        var agrupamentoVendedor = carrinho.Itens.GroupBy(v => v.IdCliente);

                        if (agrupamentoVendedor.Count() > 1)
                        {
                            model.Observacao = ConstantesUtil.PEDIDO_MULTIPLOS_VENDEDORES;
                        }

                        var resultado = await pedidoRepository.ManterAsync(model,true);


                        //Cria PedidoVendedor
                        PedidoVendedorModel pedidoVendedor = null;
                        foreach (var vendedor in agrupamentoVendedor)
                        {
                            //Consulta dados do vendedor
                            var infoVendedor = listaVendedor.FirstOrDefault(lv => lv.IdCliente == vendedor.Key);

                            pedidoVendedor = new PedidoVendedorModel();

                            pedidoVendedor.IdPedido = resultado.IdPedido;
                            pedidoVendedor.IdCliente = vendedor.Key;
                            pedidoVendedor.CodPedidoVendedorStatus = (int)ConstantesUtil.PedidoVendedorStatus.Pendente;
                            pedidoVendedor.CodFreteTipoUtilizado = infoVendedor.CodFreteTipo;
                            pedidoVendedor.UsuarioAlteracao = ClienteAutenticado.Nome;
                            pedidoVendedor.DataAlteracao = model.DataCadastro;


                            pedidoVendedor.Itens = new List<PedidoItemModel>();
                            foreach (var item in vendedor)
                            {
                                var pedidoItem = new PedidoItemModel();
                                pedidoItem.IdCliente = item.IdCliente;
                                pedidoItem.IdProduto = item.IdProduto;
                                pedidoItem.Titulo = item.Titulo;
                                pedidoItem.Qt = item.Qt;
                                pedidoItem.Valor = item.Valor;
                                pedidoItem.ValorTotal = item.ValorTotal;
                                pedidoItem.DiasPrazoEntrega = item.DiasPrazoEntrega;

                                pedidoVendedor.Itens.Add(pedidoItem);
                            }

                            var valorTotalFreteVendedor = ObterValorFreteCliente(new List<InfoVendedor>() { infoVendedor }, pedidoVendedor.Itens, request.EnderecoEntrega.Cidade.Estado.UF);

                            pedidoVendedor.ValorTotal = pedidoVendedor.Itens.Sum(i => i.ValorTotal);
                            pedidoVendedor.ValorTotalFrete = valorTotalFreteVendedor; 
                            pedidoVendedor.ValorTotalComFrete = pedidoVendedor.ValorTotal + pedidoVendedor.ValorTotalFrete;

                            pedidoVendedor.DataPrevisaoEntrega = DateTime.Now.AddDays(infoVendedor.DiasPrazoEntregaEscolhido);


                            await pedidoRepository.ManterPedidoVendedorAsync(pedidoVendedor, true);


                            //dando baixa no estoque do produto

                            foreach (var item in vendedor)
                            {
                                await produtoRepository.AtualizarEstoqueAsync(item.IdProduto, item.Qt);
                            }

                        }


                        response.Dados = new CriarPedidoDados();                            
                        response.Dados.Id = resultado.IdPedido;
                        var pedidoDto = resultado.MapperToPedidoDto();
                        pedidoDto.Itens = carrinho.Itens.MapperToPedidoItemDto();

                        //se der certo, excluir carrinho
                        if (resultado.IdPedido != 0)
                            await pedidoRepository.RemoverCarrinhoAsync(ClienteAutenticado.IdCliente);

                        scope.Complete();
                        
                        //Enviando email
                        string corpoEmail = string.Format(@"
                                        Olá,
                                        <BR><BR>
                                        Nós acabamos de receber o seu pedido. Em breve entraremos em contato.<BR><BR>
                                          
                                        {0}", ObterCorpoEmailPadrao(pedidoDto, listaVendedor));

                        EnviarEmailAsync(corpoEmail, "Pedido Desencalha Estoque - N° " + resultado.IdPedido, new List<string> { cliente.EmailPrincipal, configuration.GetValue<string>("AppSettings:email-email") });
                         
                    }


                }

                return this.GetHttpResponse(response);
            }
            catch (Exception ex)
            {
                return this.GetHttpResponseError(GetResponseError<BaseResponse>(ex));

            }
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(ListarPedidoResponse), StatusCodes.Status200OK)]
        [Authorize(Roles = ConstantesUtil.USUARIO_TIPO_CLI_ADM + "," + ConstantesUtil.USUARIO_TIPO_ADM)]
        public async Task<ActionResult> ListarPedidoAsync([FromQuery]int? idPedido = null, [FromQuery]int? codPedidoStatus = null, [FromQuery]string cliente = null, [FromQuery]DateTime? dataInicial = null, [FromQuery] DateTime? dataFinal = null,
                                                          [FromQuery]string idVendedor = null,  
                                                           [FromQuery]string cnpjCliente = null, [FromQuery] int? paginaAtual = null, [FromQuery] int? numeroRegistros = null)
        {
            try
            {
                string idCliente = string.Empty;
                string ordenacao = string.Empty;
                bool ordenacaoAsc = true;
                bool? isVendedor = null;

                var idClienteParametro = string.Empty;


                if (ClienteAutenticado != null && !string.IsNullOrEmpty(ClienteAutenticado.IdCliente))
                {
                    idCliente = ClienteAutenticado.IdCliente;
                    ordenacao = "IdPedido";
                    ordenacaoAsc = false;
                    isVendedor = ClienteAutenticado.IsVendedor;
                }
                else
                {
                    idCliente = idVendedor;
                    isVendedor = true;
                }
                 

                var response = new ListarPedidoResponse();
                List<PedidoVendedorModel> pedidosVendedor;
                List<PedidoModel> infoPedidosBase;
                List<int> idsPedido = null;

                //Se true, a consulta esta sendo realizada pelo WebAppAdmin/vendedor, senão, a consulta esta sendo realizada pelo WebAppSite/comprador
                if (isVendedor.HasValue && isVendedor.Value)
                {
                    pedidosVendedor = await pedidoRepository.ListarPedidoVendedorAsync(idCliente, idPedido, codPedidoStatus, ordenacao: ordenacao, ordenacaoAsc: ordenacaoAsc, isVendedor: isVendedor);

                    if (pedidosVendedor != null && pedidosVendedor.Any())
                    {
                        idsPedido = pedidosVendedor.Select(pv => pv.IdPedido).ToList();
                        infoPedidosBase = await pedidoRepository.ListarInformacaoPedidoBaseAsync(idsPedido, ordenacao: ordenacao, ordenacaoAsc: ordenacaoAsc, isVendedor: isVendedor);


                        response.Dados = pedidosVendedor.Mapper(infoPedidosBase);

                        if (ClienteAutenticado != null && !string.IsNullOrEmpty(ClienteAutenticado.IdCliente) && response?.Dados?.Count > 20)
                        {
                            response.Dados = response.Dados.Take(20).ToList();
                        }

                    }
                }
                else
                {
                    if (idPedido != null)
                    {
                        idsPedido = new List<int>();
                        idsPedido.Add(idPedido.Value);
                    }

                    infoPedidosBase = await pedidoRepository.ListarInformacaoPedidoBaseAsync(idsPedido, ordenacao: ordenacao, ordenacaoAsc: ordenacaoAsc, isVendedor: isVendedor, idClienteComprador: idCliente);

                    response.Dados = infoPedidosBase.MapperPedidoBaseComprador();
                    
                }

                return this.GetHttpResponse(response);

            }
            catch (Exception ex)
            {
                return this.GetHttpResponseError(GetResponseError<BaseResponse>(ex));

            }

        }

        [HttpGet]
        [Route("{idPedido}")]
        [Authorize(Roles = ConstantesUtil.USUARIO_TIPO_CLI_ADM + "," + ConstantesUtil.USUARIO_TIPO_ADM)]
        [ProducesResponseType(typeof(ObterPedidoResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> ObterPedidoAsync([FromRoute] int idPedido, [FromQuery] string idVendedor = null)
        {
            try
            {
                var response = new ObterPedidoResponse();

                if (response.Valido)
                {

                    var pedidosVendedor = await pedidoRepository.ObterPedidoVendedorAsync(!string.IsNullOrEmpty(idVendedor)? idVendedor : ClienteAutenticado?.IdCliente, idPedido, !string.IsNullOrEmpty(idVendedor) ? true: ClienteAutenticado?.IsVendedor);
                    var pedidoBase = await pedidoRepository.ObterAsync(idPedido);

                    response.Dados = pedidoBase.MapperToPedidoDto();
                    response.Dados.PedidoVendedor = pedidosVendedor.Mapper();

                    foreach (var vendedor in response.Dados.PedidoVendedor)
                    {
                        var nomeVendedor = clienteRepository.ObterAsync(vendedor.IdVendedor).Result.ApelidoOuFantasia;
                        vendedor.NomeVendedor = nomeVendedor;
                    }


                    var produtos = new List<ProdutoModel>();

                    foreach (var pv in pedidosVendedor)
                    {
                        produtos.AddRange(produtoRepository.ListarAsync(pv.Itens.Select(x => x.IdProduto).ToList()).Result);
                    }

                    foreach (var item in response?.Dados?.PedidoVendedor)
                    {
                        item.Itens?.ForEach(x =>
                        {
                            var imagemPrincipal = produtos.Where(aa => aa.IdProduto == x.IdProduto)?.FirstOrDefault()?.Imgs?.OrderBy(xa => xa.Principal).FirstOrDefault();
                            if (imagemPrincipal == null)
                                imagemPrincipal = produtos.Where(aa => aa.IdProduto == x.IdProduto)?.FirstOrDefault()?.Imgs?.Where(xx => xx.Principal == false)?.FirstOrDefault();

                            x.CaminhoImagemPrincipal = "/imagens/" + x.IdProduto + "/" + imagemPrincipal.Nome;
                        });

                    }


                    if (response?.Dados?.PedidoVendedor != null)
                    {
                        foreach (var item in response.Dados.PedidoVendedor)
                        {
                            item.Logs = item.Logs?.OrderByDescending(x => x.DataAlteracao).ToList();
                        }
                    }

                }

                return this.GetHttpResponse(response);
            }
            catch (Exception ex)
            {
                return this.GetHttpResponse(GetResponseError<ObterPedidoResponse>(ex));
            }

            
        }

        [HttpGet]
        [Route("/pedido-status")]
        [Authorize(Roles = ConstantesUtil.USUARIO_TIPO_CLI_ADM + "," + ConstantesUtil.USUARIO_TIPO_ADM)]
        [ProducesResponseType(typeof(ListarPedidoStatusResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> ListarPedidoStatusAsync()
        {
            try
            {
                var response = new ListarPedidoStatusResponse();

                response.Dados = ListarPedidoStatus();

                return this.GetHttpResponse(response);
            }
            catch (Exception ex)
            {
                return this.GetHttpResponseError(GetResponseError<BaseResponse>(ex));

            }
        }

        [HttpGet]
        [Route("carrinho")]
        //[Authorize(Roles = ConstantesUtil.USUARIO_TIPO_CLI_ADM + "," + ConstantesUtil.USUARIO_TIPO_ADM)]
        [ProducesResponseType(typeof(ObterCarrinhoResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> ObterCarrinhoAsync([FromHeader] string sessao)
        {
            try
            {

                var response = pedidoValidator.ValidarCarrinho( (ClienteAutenticado==null ? sessao : ClienteAutenticado.IdCliente));

                if (response.Valido)
                {
                    CarrinhoModel result;

                    if (ClienteAutenticado != null)
                        result = await pedidoRepository.ObterCarrinhoAsync(ClienteAutenticado.IdCliente);
                    else
                        result = await pedidoRepository.ObterCarrinhoPorTokenAsync(sessao);

                    if (result != null && result.Itens != null && result.Itens.Any())
                    {
                        response.Dados = result.MapperToDto();

                        if (result.Itens != null && result.Itens.Any())
                        {
                            var produtos = produtoRepository.ListarAsync(result.Itens.Select(x => x.IdProduto).ToList()).Result;

                            response?.Dados?.Itens?.ForEach(x =>
                            {
                                var imagemPrincipal = produtos?.Where(aa => aa.IdProduto == x.IdProduto)?.FirstOrDefault()?.Imgs?.OrderBy(xa => xa.Principal)?.FirstOrDefault();
                                var estoqueProduto = produtos?.Where(aa => aa.IdProduto == x.IdProduto)?.FirstOrDefault()?.Estoque;
                                if (imagemPrincipal == null)
                                    imagemPrincipal = produtos?.Where(aa => aa.IdProduto == x.IdProduto)?.FirstOrDefault()?.Imgs?.Where(xx => xx.Principal == false)?.FirstOrDefault();

                                x.CaminhoImagemPrincipal = "/imagens/" + x.IdProduto + "/" + imagemPrincipal?.Nome;
                                x.Largura = produtos?.Where(aa => aa.IdProduto == x.IdProduto)?.FirstOrDefault()?.Largura;
                                x.EstoqueDisponivel = (int)estoqueProduto;

                            });
                        }
                    }
                }

                return this.GetHttpResponse(response);

            }
            catch (Exception ex)
            {
                return this.GetHttpResponse(GetResponseError<ObterCarrinhoResponse>(ex));
            }
        }

        [HttpPatch]
        [Route("carrinho")]
        [ProducesResponseType(typeof(ManterCarrinhoResponse), statusCode: StatusCodes.Status200OK)]
        //[Authorize(Roles = ConstantesUtil.USUARIO_TIPO_CLI_ADM + "," + ConstantesUtil.USUARIO_TIPO_ADM)]
        public async Task<ActionResult> ManterCarrinhoAsync([FromBody] ManterCarrinhoRequest request, [FromHeader] string sessao)
        {
            try
            {
                var response = pedidoValidator.ValidarCarrinhoManter(request);

               
                if (response.Valido)
                {

                    CarrinhoModel carrinhoOriginal;
                    ClienteModel cliente = null;

                    if (ClienteAutenticado == null)
                        carrinhoOriginal = pedidoRepository.ObterCarrinhoPorTokenAsync(sessao).Result;
                    else
                    {
                        cliente = clienteRepository.ObterAsync(ClienteAutenticado.IdCliente).Result;
                        carrinhoOriginal = pedidoRepository.ObterCarrinhoAsync(ClienteAutenticado.IdCliente).Result;
                    }

                    if (carrinhoOriginal?.Itens == null) return this.GetHttpResponse(response);



                    //verificando qt no estoque, e colocando qt maximo
                    foreach (var item in carrinhoOriginal.Itens)
                    {
                        var produto = await produtoRepository.ObterAsync(item.IdProduto);

                        if (item.Qt > produto.Estoque)
                            item.Qt = produto.Estoque;
                    }

                    CarrinhoModel model;
                    //se for sem o cliente logado
                    if (ClienteAutenticado != null)
                    {
                        CalcularValorProdutos(carrinhoOriginal?.Itens);

                        //Consulta informações do vendedor
                        List<InfoVendedor> ListaInfoVendedor = ObterListaVendedor(carrinhoOriginal?.Itens, cliente.EnderecoEntrega.Cidade.Estado.UF);

                        double valorFrete =  ObterValorFreteCliente(ListaInfoVendedor, carrinhoOriginal?.Itens, request.UF);

                        model = new CarrinhoModel()
                        {
                            IdCliente = ClienteAutenticado.IdCliente,
                            Token = carrinhoOriginal.Token,
                            DataExpiracaoToken = carrinhoOriginal.DataExpiracaoToken,
                            DataCadastro = DateTime.Now,
                            ValorTotal = carrinhoOriginal.Itens.Sum(x => x.ValorTotal),
                            ValorTotalComFrete = carrinhoOriginal.Itens.Sum(x => x.ValorTotal) + valorFrete,
                            ValorTotalFrete = valorFrete,
                            Itens = carrinhoOriginal.Itens
                        };
                    }
                    else
                    {
                        model = new CarrinhoModel()
                        {
                            IdCliente = null,
                            Token = sessao,
                            DataExpiracaoToken = carrinhoOriginal.DataExpiracaoToken,
                            DataCadastro = DateTime.Now,
                            ValorTotal = carrinhoOriginal.Itens.Sum(x => x.ValorTotal),
                            ValorTotalComFrete = 0,
                            ValorTotalFrete = 0,
                            Itens = carrinhoOriginal.Itens
                        };
                    }

                    var resultado = await pedidoRepository.ManterCarrinhoAsync(model, (ClienteAutenticado == null ? true : false));

                    //Se for sucesso, voltaremos o proprio carrinho calculado no response
                    if (resultado)
                    {
                        response.Dados = model.MapperToDto();

                        var produtos = produtoRepository.ListarAsync(response.Dados.Itens.Select(x => x.IdProduto).ToList()).Result;

                        response?.Dados?.Itens?.ForEach(x =>
                        {

                            var imagemPrincipal = produtos.Where(aa => aa.IdProduto == x.IdProduto)?.FirstOrDefault()?.Imgs?.OrderBy(xa => xa.Principal).FirstOrDefault();
                            if (imagemPrincipal == null)
                                imagemPrincipal = produtos.Where(aa => aa.IdProduto == x.IdProduto)?.FirstOrDefault()?.Imgs?.Where(xx => xx.Principal == false)?.FirstOrDefault();

                            x.CaminhoImagemPrincipal = "/imagens/" + x.IdProduto + "/" + imagemPrincipal.Nome;

                        });
                    }
                }


                return this.GetHttpResponse(response);
            }
            catch (Exception ex)
            {
                return this.GetHttpResponseError(GetResponseError<BaseResponse>(ex));

            }
        }

        [HttpPost]
        [Route("carrinho/itens")]
        [ProducesResponseType(typeof(CriarCarrinhoRequest), statusCode: StatusCodes.Status200OK)]
        //[Authorize(Roles = ConstantesUtil.USUARIO_TIPO_CLI_ADM + "," + ConstantesUtil.USUARIO_TIPO_ADM)]
        public async Task<ActionResult> ManterCarrinhoItemAsync([FromBody] CriarCarrinhoRequest request, [FromHeader] string sessao)
        {
            try
            {
                var response = pedidoValidator.ValidarCarrinhoCriar(request);

                if (response.Valido)
                {

                    
                    CarrinhoModel carrinhoOriginal;
                    ClienteModel cliente = null;

                    if (ClienteAutenticado == null)
                        carrinhoOriginal = pedidoRepository.ObterCarrinhoPorTokenAsync(sessao).Result;
                    else
                    {
                        cliente = clienteRepository.ObterAsync(ClienteAutenticado.IdCliente).Result;
                        carrinhoOriginal = pedidoRepository.ObterCarrinhoAsync(ClienteAutenticado.IdCliente).Result;
                    }

                    if (carrinhoOriginal == null)
                        carrinhoOriginal = new CarrinhoModel()
                        {
                            DataExpiracaoToken = DateTime.UtcNow.AddMinutes(ConstantesUtil.TIMEOUT_SESSAO_MINUTOS)
                        };



                    //Consulta informações do vendedor

                    List<PedidoItemModel> itens = null;

                    if (carrinhoOriginal.Itens != null && carrinhoOriginal.Itens.Any())
                    {
                        itens = carrinhoOriginal.Itens;
                    }
                    else
                    {
                        itens = request.Itens.Mapper();
                    }

                    List<InfoVendedor> ListaInfoVendedor = ObterListaVendedor(itens, cliente?.EnderecoEntrega?.Cidade?.Estado?.UF);

                    //pegando os itens enviados como novos, e adicionando na lsita do carrinho existente
                    request.Itens.ForEach(x =>
                    {
                        if (request.AtualizacaoDosItens == true)
                        {
                            var produtoCarrinho = carrinhoOriginal?.Itens?.Where(a => a.IdProduto == x.IdProduto).FirstOrDefault();
                            if (produtoCarrinho != null && !string.IsNullOrEmpty(produtoCarrinho.IdProduto))
                                produtoCarrinho.Qt = x.Qt;
                        }
                        else
                        {
                            var produtoCarrinho = carrinhoOriginal?.Itens?.Where(a => a.IdProduto == x.IdProduto).FirstOrDefault();
                            if (produtoCarrinho != null && !string.IsNullOrEmpty(produtoCarrinho.IdProduto))
                                produtoCarrinho.Qt += x.Qt;
                            else
                            {
                                if (carrinhoOriginal?.Itens == null)
                                    carrinhoOriginal.Itens = new List<PedidoItemModel>();


                                if (!ListaInfoVendedor.Any(liv => liv.IdCliente == x.IdCliente))
                                {
                                    ListaInfoVendedor.AddRange(ObterListaVendedor(request.Itens.Mapper(), cliente?.EnderecoEntrega?.Cidade?.Estado?.UF));
                                }

                                carrinhoOriginal.Itens.Add(new PedidoItemModel()
                                {
                                    IdProduto = x.IdProduto,
                                    Qt = x.Qt,
                                    IdCliente = x.IdCliente,
                                    DiasPrazoEntrega = ListaInfoVendedor.FirstOrDefault(liv => liv.IdCliente == x.IdCliente).DiasPrazoEntregaEscolhido
                                }); 
                            }
                        }
                    });

                    //verificando qt no estoque, e colocando qt maximo
                    foreach(var item in carrinhoOriginal.Itens)
                    {
                        var produto = await  produtoRepository.ObterAsync(item.IdProduto);

                        if (item.Qt > produto.Estoque)
                            item.Qt = produto.Estoque;
                    }


                    CalcularValorProdutos(carrinhoOriginal.Itens);

                    double valorFrete = ObterValorFreteCliente(ListaInfoVendedor, carrinhoOriginal?.Itens, cliente?.EnderecoEntrega?.Cidade?.Estado?.UF);

                    CarrinhoModel model;
                    //se for sem o cliente logado
                    if (ClienteAutenticado != null)
                    {
                        model = new CarrinhoModel()
                        {
                            IdCliente = ClienteAutenticado.IdCliente,
                            Token = carrinhoOriginal.Token,
                            DataExpiracaoToken = carrinhoOriginal.DataExpiracaoToken,
                            DataCadastro = DateTime.Now,
                            ValorTotal = carrinhoOriginal.Itens.Sum(x => x.ValorTotal),
                            ValorTotalComFrete = carrinhoOriginal.Itens.Sum(x => x.ValorTotal) + valorFrete,
                            ValorTotalFrete = valorFrete,
                            Itens = carrinhoOriginal.Itens
                        };
                    }
                    else
                    {
                        model = new CarrinhoModel()
                        {
                            IdCliente = null,
                            Token = sessao,
                            DataExpiracaoToken = carrinhoOriginal.DataExpiracaoToken,
                            DataCadastro = DateTime.Now,
                            ValorTotal = carrinhoOriginal.Itens.Sum(x => x.ValorTotal),
                            ValorTotalComFrete = 0,
                            ValorTotalFrete = 0,
                            Itens = carrinhoOriginal.Itens
                        };
                    }

                    var resultado = await pedidoRepository.ManterCarrinhoAsync(model, (ClienteAutenticado==null ? true : false));

                    //Se for sucesso, voltaremos o proprio carrinho calculado no response
                    if (resultado)
                        response.Dados = model.MapperToDto();

                }


                return this.GetHttpResponse(response);
            }
            catch (Exception ex)
            {
                return this.GetHttpResponseError(GetResponseError<BaseResponse>(ex));

            }
        }

        [HttpDelete]
        [Route("carrinho/{idProduto}")]
        [ProducesResponseType(typeof(BaseResponse), statusCode: StatusCodes.Status200OK)]
        //[Authorize(Roles = ConstantesUtil.USUARIO_TIPO_CLI_ADM + "," + ConstantesUtil.USUARIO_TIPO_ADM)]
        public async Task<ActionResult> ExcluirCarrinhoAsync([FromRoute] string idProduto, [FromHeader] string sessao)
        {
            try
            {
                var response = new BaseResponse();
                response.Valido = true;
              
                if (response.Valido)
                {
                    CarrinhoModel carrinhoOriginal;
                    if (ClienteAutenticado == null)
                        carrinhoOriginal = pedidoRepository.ObterCarrinhoPorTokenAsync(sessao).Result;
                    else
                        carrinhoOriginal = pedidoRepository.ObterCarrinhoAsync(ClienteAutenticado.IdCliente).Result;

                    var item = carrinhoOriginal.Itens.Where(x => x.IdProduto == idProduto).FirstOrDefault();

                    var IdClienteVendedor = item.IdCliente;

                    if (item != null)
                    {
                        carrinhoOriginal.Itens.Remove(item);
                    }

                    //Se não tiver mais nenhum item no carrinho desse vendedor, o Valor de total do frete deve ser atualizado
                    if (!carrinhoOriginal.Itens.Any(i => i.IdCliente == IdClienteVendedor))
                    {
                        if (carrinhoOriginal.Itens.Any())
                        {
                            var clienteComprador = clienteRepository.ObterAsync(carrinhoOriginal.IdCliente).Result;
                            var listaInfoVendedor = ObterListaVendedor(carrinhoOriginal.Itens, clienteComprador.EnderecoEntrega.Cidade.Estado.UF);

                            var valorFrete = ObterValorFreteCliente(listaInfoVendedor, carrinhoOriginal.Itens, clienteComprador.EnderecoEntrega.Cidade.Estado.UF);

                            carrinhoOriginal.ValorTotal = carrinhoOriginal.Itens.Sum(x => x.ValorTotal);
                            carrinhoOriginal.ValorTotalComFrete = carrinhoOriginal.Itens.Sum(x => x.ValorTotal) + valorFrete;
                            carrinhoOriginal.ValorTotalFrete = valorFrete;
                        }
                        else
                        {
                            carrinhoOriginal.ValorTotal = 0;
                            carrinhoOriginal.ValorTotalComFrete = 0;
                            carrinhoOriginal.ValorTotalFrete = 0;
                        }

                    }

                    var resultado = await pedidoRepository.ManterCarrinhoAsync(carrinhoOriginal);

                }
                return this.GetHttpResponse(response);
            }
            catch (Exception ex)
            {
                return this.GetHttpResponseError(GetResponseError<BaseResponse>(ex));

            }
        }

        [HttpPost()]
        [Route("{idPedido}/arquivos")]
        //[AutorizacaoFilter(PerfisAcesso = new string[] { ConstantesUtil.USUARIO_TIPO_ECM_ADM })]
        public ActionResult SubirArquivosAsync(List<IFormFile> files, [FromRoute] int idPedido, [FromQuery] string tipo)
        {
            try
            {
                //Validando todos os arquivos
                foreach (IFormFile item in files)
                {
                    if (item.Length > 0)
                    {
                        string fileName = ContentDispositionHeaderValue.Parse(item.ContentDisposition).FileName.Trim('"');

                        string extensaoArquivo = fileName.Substring(fileName.Length - 3, 3);
                        string nomeArquivo = fileName.Substring(0, fileName.Length - 4);

                        if (fileName.Length < 4 ||
                            (extensaoArquivo.ToUpper() != "PDF"))
                        {
                            return this.GetHttpResponse(new BaseResponse()
                            {
                                Valido = false,
                                Mensagens = new List<MensagemSistema>() {
                                        new MensagemSistema(){
                                        Campo  = "",
                                        Mensagem = "Só são permitidos arquivos de extensão PDF"
                                    }
                                    }
                            });
                        }
                    }
                }


                if (files != null && files.Count > 0)
                {
                    string newPath = Path.Combine(environment.WebRootPath, "");
                    string subPasta = string.Empty;
                    List<string> arquivos = new List<string>();
                    string dataFormatada = pedidoRepository.ObterAsync(idPedido).Result?.DataCadastro.ToString("yyyyMM");
                   
                    subPasta = @"/anexos/" + dataFormatada  + "/" + idPedido + "/" + Guid.NewGuid() + "/";

                    if (!Directory.Exists(newPath + subPasta))
                    {
                        Directory.CreateDirectory(newPath + subPasta);
                    }

                    foreach (IFormFile item in files)
                    {
                        if (item.Length > 0)
                        {
                            string fileName = ContentDispositionHeaderValue.Parse(item.ContentDisposition).FileName.Trim('"');

                            string extensaoArquivo = fileName.Substring(fileName.Length - 3, 3);
                            string nomeArquivo = fileName.Substring(0, fileName.Length - 4); 
                            nomeArquivo = nomeArquivo + "." + extensaoArquivo;

                            var continuar = true;
                            int contador = 1;
                            while (continuar)
                            {
                                if (System.IO.File.Exists(newPath + subPasta + "/" + nomeArquivo))
                                {
                                    nomeArquivo = fileName.Substring(0, fileName.Length - 4) + "(" + contador + ")." + extensaoArquivo; 
                                }
                                else
                                    continuar = false;

                                contador++;
                            }

                            arquivos.Add(nomeArquivo + "|" + subPasta + "/" + nomeArquivo);

                            string fullPath = Path.Combine(newPath + subPasta, nomeArquivo);
                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                item.CopyTo(stream);
                            }

                        }
                    }

                    List<MensagemSistema> mensagens = new List<MensagemSistema>();

                    arquivos.ForEach(x =>
                    {
                        mensagens.Add(new MensagemSistema()
                        {
                            Mensagem = x
                        });
                    });

                    return this.GetHttpResponse(new BaseResponse()
                    {
                        Valido = true,
                        Mensagens = mensagens
                    });
                     
                }

                return this.GetHttpResponse(new BaseResponse()
                {
                    Valido = false,
                    Mensagens = new List<MensagemSistema>() {
                        new MensagemSistema(){
                            Campo  = "",
                            Mensagem = "Erro ao realizar o upload"
                        }
                    }
                });
                 
            }
            catch (Exception ex)
            {
                return this.GetHttpResponseError(GetResponseError<BaseResponse>(ex));

            }
        }



        [HttpPatch()]
        [Route("anexo")]
        [Authorize(Roles = ConstantesUtil.USUARIO_TIPO_CLI_ADM + "," + ConstantesUtil.USUARIO_TIPO_ADM)]
        public async Task<ActionResult> ExcluirAnexoAsync([FromBody] ExcluirAnexoRequest request)
        {
            try
            {
                var response = pedidoValidator.Validar(request);

                if (response.Valido)
                {
                    var pedidosVendedor = await pedidoRepository.ObterPedidoVendedorAsync(ClienteAutenticado.IdCliente, request.IdPedido, true);
                    var pedidoVendedor = pedidosVendedor.FirstOrDefault();

                    if (pedidoVendedor != null)
                    {
                        var anexo = pedidoVendedor.Anexos.FirstOrDefault(a => a.Nome == request.Anexo.Nome);

                        if (anexo != null)
                        {
                            //Remove o Anexo
                            var anexoRemovido = pedidoVendedor.Anexos.Remove(anexo);

                            if (anexoRemovido)
                            {
                                var msgLogSistema = $" - [Arquivo removido: {request.Anexo.Nome}]";

                                //adicionando um novo log
                                if (pedidoVendedor.Logs == null) pedidoVendedor.Logs = new List<PedidoLogModel>();
                                pedidoVendedor.Logs.Add(new PedidoLogModel()
                                {
                                    CodPedidoStatus = pedidoVendedor.CodPedidoVendedorStatus,
                                    DataAlteracao = DateTime.Now,
                                    Observacao = request.Observacao + msgLogSistema,
                                    UsuarioAlteracao = pedidoVendedor.UsuarioAlteracao
                                });

                                await pedidoRepository.ManterPedidoVendedorAsync(pedidoVendedor, false);


                                string subPasta = request.Anexo.Caminho.Substring(0, request.Anexo.Caminho.LastIndexOf('/'));
                                string path = Path.Combine(environment.WebRootPath, "") + subPasta;
                                

                                if (Directory.Exists(path))
                                {
                                    Directory.Delete(path, true);
                                }

                                response.Mensagens.Add(new Messages.MensagemSistema()
                                {
                                    Campo = "",
                                    Mensagem = "Arquivo excluído com sucesso!"
                                });
                            }
                        }
                    }
                }

                return this.GetHttpResponse(response);

            }
            catch (Exception ex)
            {
                return this.GetHttpResponseError(GetResponseError<BaseResponse>(ex));
            }
        }





        #region Métodos auxiliares

        private List<InfoVendedor> ObterListaVendedor(List<PedidoItemModel> itens, string UF)
        {
            //Consulta informações do vendedor
            List<InfoVendedor> ListaInfoVendedor = new List<InfoVendedor>();
            var agrupamentoVendedor = itens.GroupBy(v => v.IdCliente);

            foreach (var vendedor in agrupamentoVendedor)
            {
                var infoVendedor = new InfoVendedor();

                //Consulta dados do vendedor
                var clienteVendedor = clienteRepository.ObterAsync(vendedor.Key).Result;

                infoVendedor.IdCliente = vendedor.Key;
                infoVendedor.NomeClienteVendedor = clienteVendedor.ApelidoOuFantasia;
                infoVendedor.CodFreteTipo = clienteVendedor.CodFreteTipo.Value;


                if (infoVendedor.CodFreteTipo == (int)FreteTipo.ValorFixo)
                {
                    infoVendedor.ValorFreteFixo = clienteVendedor.ValorFreteFixo.Value;
                    infoVendedor.DiasPrazoEntregaFixo = clienteVendedor.DiasPrazoEntregaFixo.Value;
                }
                else
                {
                    if (!string.IsNullOrEmpty(UF))
                    {
                        var fretes = freteEstadoRepository.ListarAsync(vendedor.Key).Result;
                        infoVendedor.FreteEstado = fretes?.FirstOrDefault(fpe => fpe.UF == UF.Trim());
                    }
                }

                ListaInfoVendedor.Add(infoVendedor);
            }

            return ListaInfoVendedor;
        }



        private void CalcularValorProdutos(List<PedidoItemModel> itens)
        {
            var produtos = produtoRepository.ListarAsync(itens.Select(x => x.IdProduto).ToList()).Result;

            itens.ForEach(x =>
            {
                var produto = produtos.Where(a => a.IdProduto == x.IdProduto).FirstOrDefault();


                x.Valor = (double)produto.ValorVenda;

                x.ValorTotal = (double)x.Valor * x.Qt;
                x.Titulo = produto.Titulo;
            });
        }

        private double ObterValorFreteCliente(List<InfoVendedor> listaInfoVendedor, List<PedidoItemModel> itens, string UF)
        {
            double valorFrete = 0;

            if (itens != null)
            {
                foreach (var vendedor in listaInfoVendedor)
                {
                    var valorTotal = itens.Where(i => i.IdCliente == vendedor.IdCliente).Sum(i => i.ValorTotal);

                    if (vendedor.CodFreteTipo.Equals((int)ConstantesUtil.FreteTipo.PorEstado))
                    {
                        var freteEstado = vendedor.FreteEstado;

                        if (freteEstado == null)
                            valorFrete = 999999; //se caso ocorrer um erro, lançar um valor de frete absurdo para o cliente reclamar
                        else
                        {
                            if (freteEstado.ValorPedidoFreteGratis > 0 &&
                                valorTotal >= freteEstado.ValorPedidoFreteGratis)
                                valorFrete += 0;
                            else
                                valorFrete += freteEstado.Valor;

                        }
                    }
                    else
                    {
                        valorFrete += Convert.ToDouble(vendedor.ValorFreteFixo);

                    }

                }
            }

            return valorFrete;
        }


        private string ObterCorpoEmailPadrao(PedidoDto pedidoDto, List<InfoVendedor> listaVendedor = null)
        {

            var endereco = "";

            if (pedidoDto.Cliente.EnderecoEntrega.Endereco != null)
                endereco += pedidoDto.Cliente.EnderecoEntrega.Endereco;

            if (pedidoDto.Cliente.EnderecoEntrega.Complemento != null)
                endereco += " - " + pedidoDto.Cliente.EnderecoEntrega.Complemento;

            if (pedidoDto.Cliente.EnderecoEntrega.Cep != null)
                endereco += " - " + pedidoDto.Cliente.EnderecoEntrega.Cep;

            if (pedidoDto.Cliente.EnderecoEntrega.Cidade != null)
                endereco += " - " + pedidoDto.Cliente.EnderecoEntrega.Cidade.Titulo;

            if (pedidoDto.Cliente.EnderecoEntrega.Cidade.Estado.UF != null)
                endereco += " - " + pedidoDto.Cliente.EnderecoEntrega.Cidade.Estado.UF;

            var produtos = produtoRepository.ListarAsync(pedidoDto.Itens.Select(x => x.IdProduto).ToList()).Result;


            pedidoDto?.Itens?.ForEach(x =>
            {

                var imagemPrincipal = produtos.Where(aa => aa.IdProduto == x.IdProduto)?.FirstOrDefault()?.Imgs?.OrderBy(xa => xa.Principal).FirstOrDefault();
                if (imagemPrincipal == null)
                    imagemPrincipal = produtos.Where(aa => aa.IdProduto == x.IdProduto)?.FirstOrDefault()?.Imgs?.Where(xx => xx.Principal == false)?.FirstOrDefault();

                x.CaminhoImagemPrincipal = configuration.GetValue<string>("AppSettings:url-api") + "/imagens/" + x.IdProduto + "/" + imagemPrincipal.Nome;

            });

            string corpoEmailComum = $@"
                            <div style='width:800px'><table  style='width: 100%'  cellspacing='1' cellpadding='1'>
                             <tr><TD colspan=2><h3><strong>Dados do Pedido</strong></h3></td></tr>
                            <tr><TD><strong>Pedido N° {pedidoDto.IdPedido}</strong></td><TD  align='right'><strong>Data Emissão:</strong> {pedidoDto.DataCadastro.ToString("dd/MM/yyyy HH:mm:ss")}</td></tr>
                            <tr><TD colspan=2  style='color: orange'><strong>Status:</strong> {ConstantesUtil.ObterPedidoStatus(pedidoDto.CodPedidoStatus)}</td></tr>
                            {(!string.IsNullOrEmpty(pedidoDto.Observacao) ? $"<tr><TD colspan=2 ><strong>Observação:</strong> {pedidoDto.Observacao}</td></tr>" : "") }
                            </table>
                            <BR>
                            <table  style='width: 100%'  cellspacing='1' cellpadding='1'>
                            <tr><TD colspan=2><h3><strong>Dados do Cliente</strong></h3></td></tr>
                            <tr><TD><strong>Razão:</strong> {pedidoDto.Cliente.NomeOuRazao}</td><TD  align='right'><strong>CNPJ:</strong> {pedidoDto.Cliente.CpfOuCnpj}</td></tr>
                            <tr><TD><strong>Telefone 1:</strong> {pedidoDto.Cliente.Telefone1}</td><TD  align='right'><strong>Telefone 2:</strong> {pedidoDto.Cliente.Telefone2}</td></tr>
                            <tr><TD><strong>Email:</strong> {pedidoDto.Cliente.EmailPrincipal}</td><TD  align='right'><strong>Email Alt:</strong> {pedidoDto.Cliente.EmailAlternativo}</td></tr>
                            <tr><TD colspan=2><strong>Endereço Entrega:</strong> {endereco}</td></tr>
                            </table>
                            <table  style='width: 100%'  cellspacing='1' cellpadding='1'>
                            <tr><TD colspan=2><h3><strong>Produtos</strong></h3></td></tr>
                            <tr><TD>
                            <table style='width:100%' cellspacing='0' cellpadding='0'>
                                <tr>
                                    <td></td>
                                    <td><strong>Produto</strong></td>
                                    <td><strong>Valor</strong></td>
                                    <td><strong>Qt</strong></td>
                                    <td><strong>Valor Total</strong></td>
                                </tr>
                                ";


            pedidoDto.Itens.ForEach(x =>
            {

                var nomeClienteVendedor = listaVendedor.FirstOrDefault(lv => lv.IdCliente == x.IdCliente).NomeClienteVendedor;
                var dataPrevisaoEntrega = pedidoDto.DataCadastro.AddDays(x.DiasPrazoEntrega).ToString("dd/MM/yyyy");

                corpoEmailComum += $@"
                                    <tr >
                                        <td style='border-top:1px solid silver'><img src='{x.CaminhoImagemPrincipal}' style='margin-top:4px;height:70px; width:70px'></td>
                                        <td style='border-top:1px solid silver'>{x.Titulo}</td>
                                        <td style='border-top:1px solid silver'>{NumeroUtil.ObterValorBrasil(x.Valor, true)}</td>
                                        <td style='border-top:1px solid silver'>{x.Qt}</td>
                                        <td style='border-top:1px solid silver'>{NumeroUtil.ObterValorBrasil(x.ValorTotal, true)}</td>
                                    </tr>
                                    <tr>
                                        <td colspan='5'>Vendedor: <strong>{nomeClienteVendedor}</strong></td>
                                    </tr>
                                    <tr>
                                        <td colspan='5'>Previsão de entrega: <strong>{dataPrevisaoEntrega}</strong></td>
                                    </tr>
                                    <br />
                                    ";
            });


            corpoEmailComum += $@"</table>

                            </td></tr>
                            </table>
                            <table  style='width: 100%'  cellspacing='1' cellpadding='1'>
                            <tr><TD  align='right'><strong>Valor Frete:</strong> {NumeroUtil.ObterValorBrasil(pedidoDto.ValorTotalFrete, true)}</td></tr>
                            <tr><TD  align='right'><strong>Valor Total:</strong> {NumeroUtil.ObterValorBrasil(pedidoDto.ValorTotal, true)}</td></tr>
                            <tr><TD  align='right'><strong>Valor Total com Frete:</strong> {NumeroUtil.ObterValorBrasil(pedidoDto.ValorTotalComFrete, true)}</td></tr>
                            </table>

                           
                        " + 
                        ObterRodapePadraoEmail();

            return corpoEmailComum;

        }


        private string ObterCorpoEmailPedidoVendedor(PedidoDto pedidoDto, InfoVendedor infoVendedor = null)
        {

            var endereco = "";

            if (pedidoDto.Cliente.EnderecoEntrega.Endereco != null)
                endereco += pedidoDto.Cliente.EnderecoEntrega.Endereco;

            if (pedidoDto.Cliente.EnderecoEntrega.Complemento != null)
                endereco += " - " + pedidoDto.Cliente.EnderecoEntrega.Complemento;

            if (pedidoDto.Cliente.EnderecoEntrega.Cep != null)
                endereco += " - " + pedidoDto.Cliente.EnderecoEntrega.Cep;

            if (pedidoDto.Cliente.EnderecoEntrega.Cidade != null)
                endereco += " - " + pedidoDto.Cliente.EnderecoEntrega.Cidade.Titulo;

            if (pedidoDto.Cliente.EnderecoEntrega.Cidade.Estado.UF != null)
                endereco += " - " + pedidoDto.Cliente.EnderecoEntrega.Cidade.Estado.UF;

                        
            var pedidoVendedorDto = pedidoDto.PedidoVendedor.FirstOrDefault();

            var produtos = produtoRepository.ListarAsync(pedidoVendedorDto.Itens.Select(x => x.IdProduto).ToList()).Result;


            pedidoVendedorDto?.Itens?.ForEach(x =>
            {

                var imagemPrincipal = produtos.Where(aa => aa.IdProduto == x.IdProduto)?.FirstOrDefault()?.Imgs?.OrderBy(xa => xa.Principal).FirstOrDefault();
                if (imagemPrincipal == null)
                    imagemPrincipal = produtos.Where(aa => aa.IdProduto == x.IdProduto)?.FirstOrDefault()?.Imgs?.Where(xx => xx.Principal == false)?.FirstOrDefault();

                x.CaminhoImagemPrincipal = configuration.GetValue<string>("AppSettings:url-api") + "/imagens/" + x.IdProduto + "/" + imagemPrincipal.Nome;

            });

            string corpoEmailComum = $@"
                            <div style='width:800px'><table  style='width: 100%'  cellspacing='1' cellpadding='1'>
                             <tr><TD colspan=2><h3><strong>Dados do Pedido</strong></h3></td></tr>
                            <tr><TD><strong>Pedido N° {pedidoDto.IdPedido}</strong></td><TD  align='right'><strong>Data Emissão:</strong> {pedidoDto.DataCadastro.ToString("dd/MM/yyyy HH:mm:ss")}</td></tr>
                            <tr><TD colspan=2  style='color: orange'><strong>Status:</strong> {ConstantesUtil.ObterPedidoVendedorStatus(pedidoVendedorDto.CodPedidoVendedorStatus)}</td></tr>
                            {(!string.IsNullOrEmpty(pedidoVendedorDto.Observacao) ? $"<tr><TD colspan=2 ><strong>Observação:</strong> {pedidoVendedorDto.Observacao}</td></tr>" : "")}
                            </table>
                            <BR>
                            <table  style='width: 100%'  cellspacing='1' cellpadding='1'>
                            <tr><TD colspan=2><h3><strong>Dados do Cliente</strong></h3></td></tr>
                            <tr><TD><strong>Razão:</strong> {pedidoDto.Cliente.NomeOuRazao}</td><TD  align='right'><strong>CNPJ:</strong> {pedidoDto.Cliente.CpfOuCnpj}</td></tr>
                            <tr><TD><strong>Telefone 1:</strong> {pedidoDto.Cliente.Telefone1}</td><TD  align='right'><strong>Telefone 2:</strong> {pedidoDto.Cliente.Telefone2}</td></tr>
                            <tr><TD><strong>Email:</strong> {pedidoDto.Cliente.EmailPrincipal}</td><TD  align='right'><strong>Email Alt:</strong> {pedidoDto.Cliente.EmailAlternativo}</td></tr>
                            <tr><TD colspan=2><strong>Endereço Entrega:</strong> {endereco}</td></tr>
                            </table>
                            <table  style='width: 100%'  cellspacing='1' cellpadding='1'>
                            <tr><TD colspan=2><h3><strong>Produtos</strong></h3></td></tr>
                            <tr><TD>
                            <table style='width:100%' cellspacing='0' cellpadding='0'>
                                <tr>
                                    <td></td>
                                    <td><strong>Produto</strong></td>
                                    <td><strong>Valor</strong></td>
                                    <td><strong>Qt</strong></td>
                                    <td><strong>Valor Total</strong></td>
                                </tr>
                                ";


            pedidoVendedorDto.Itens.ForEach(x =>
            {

                var nomeClienteVendedor = infoVendedor.NomeClienteVendedor;
                var dataPrevisaoEntrega = pedidoVendedorDto.DataPrevisaoEntrega.ToString("dd/MM/yyyy");

                corpoEmailComum += $@"
                                    <tr >
                                        <td style='border-top:1px solid silver'><img src='{x.CaminhoImagemPrincipal}' style='margin-top:4px;height:70px; width:70px'></td>
                                        <td style='border-top:1px solid silver'>{x.Titulo}</td>
                                        <td style='border-top:1px solid silver'>{NumeroUtil.ObterValorBrasil(x.Valor, true)}</td>
                                        <td style='border-top:1px solid silver'>{x.Qt}</td>
                                        <td style='border-top:1px solid silver'>{NumeroUtil.ObterValorBrasil(x.ValorTotal, true)}</td>
                                    </tr>
                                    <tr>
                                        <td colspan='5'>Vendedor: <strong>{nomeClienteVendedor}</strong></td>
                                    </tr>
                                    <tr>
                                        <td colspan='5'>Previsão de entrega: <strong>{dataPrevisaoEntrega}</strong></td>
                                    </tr>
                                    <br />
                                    ";
            });


            corpoEmailComum += $@"</table>

                            </td></tr>
                            </table>
                            <table  style='width: 100%'  cellspacing='1' cellpadding='1'>
                            <tr><TD  align='right'><strong>Valor Frete:</strong> {NumeroUtil.ObterValorBrasil(pedidoVendedorDto.ValorTotalFrete, true)}</td></tr>
                            <tr><TD  align='right'><strong>Valor Total:</strong> {NumeroUtil.ObterValorBrasil(pedidoVendedorDto.ValorTotal, true)}</td></tr>
                            <tr><TD  align='right'><strong>Valor Total com Frete:</strong> {NumeroUtil.ObterValorBrasil(pedidoVendedorDto.ValorTotalComFrete, true)}</td></tr>
                            </table>

                           
                        " +
                        ObterRodapePadraoEmail();

            return corpoEmailComum;

        }







        private List<PedidoStatusDto> ListarPedidoStatus()
        {
            return new List<PedidoStatusDto>()
                {
                   new PedidoStatusDto(){
                       CodPedidoStatus = (int)ConstantesUtil.PedidoVendedorStatus.Cancelado,
                       Titulo = ConstantesUtil.GetEnumDescription(ConstantesUtil.PedidoVendedorStatus.Cancelado)
                   },
                   new PedidoStatusDto(){
                       CodPedidoStatus = (int)ConstantesUtil.PedidoVendedorStatus.AguardandoPagto,
                       Titulo = ConstantesUtil.GetEnumDescription(ConstantesUtil.PedidoVendedorStatus.AguardandoPagto)
                   },
                   new PedidoStatusDto(){
                       CodPedidoStatus = (int)ConstantesUtil.PedidoVendedorStatus.Pendente,
                       Titulo = ConstantesUtil.GetEnumDescription(ConstantesUtil.PedidoVendedorStatus.Pendente)
                   }
                   ,
                   new PedidoStatusDto(){
                       CodPedidoStatus = (int)ConstantesUtil.PedidoVendedorStatus.Entregue,
                       Titulo = ConstantesUtil.GetEnumDescription(ConstantesUtil.PedidoVendedorStatus.Entregue)
                   },
                   new PedidoStatusDto(){
                       CodPedidoStatus = (int)ConstantesUtil.PedidoVendedorStatus.NotaFiscalGerada,
                       Titulo = ConstantesUtil.GetEnumDescription(ConstantesUtil.PedidoVendedorStatus.NotaFiscalGerada)
                   }

                };
        }

        #endregion

    }
}
