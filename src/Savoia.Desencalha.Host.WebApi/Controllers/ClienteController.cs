using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Domain.Models;
using Savoia.Desencalha.Domain.Repositories;
using Savoia.Desencalha.Domain.Resources;
using Savoia.Desencalha.Host.WebApi.Dtos.Cliente;
using Savoia.Desencalha.Host.WebApi.Dtos.Frete;
using Savoia.Desencalha.Host.WebApi.Extensions;
using Savoia.Desencalha.Host.WebApi.Filters;
using Savoia.Desencalha.Host.WebApi.Mappers;
using Savoia.Desencalha.Host.WebApi.Messages;
using Savoia.Desencalha.Host.WebApi.Messages.Cliente;
using Savoia.Desencalha.Host.WebApi.Util;
using Savoia.Desencalha.Host.WebApi.Validators;
using System.Text.RegularExpressions;
using System.Transactions;
using static Savoia.Desencalha.Host.WebApi.Helpers.ResponseHelper;

namespace Savoia.Desencalha.Host.WebApi.Controllers
{
    [Route("clientes")]
    public class ClienteController : BaseController
    {
        internal IClienteValidator ClienteValidator;
        internal IClienteRepository ClienteRepository;
        internal IPedidoRepository pedidoRepository;
        internal IFreteEstadoRepository freteEstadoRepository;
        string chaveTokenEsqueciSenha = "asdfa234923DFDFD#$#$#$#$9sdfioagujasgopuas90ju89u234562464k]]";

        public ClienteController(IClienteValidator ClienteValidator,
                           IClienteRepository ClienteRepository,
                           IPedidoRepository pedidoRepository,
                           IEmailRepository emailRepository,
                           IConfiguration configuration,
                           IFreteEstadoRepository freteEstadoRepository)
        {
            this.ClienteValidator = ClienteValidator;
            this.ClienteRepository = ClienteRepository;
            this.pedidoRepository = pedidoRepository;
            this.emailRepository = emailRepository;
            this.configuration = configuration;
            this.freteEstadoRepository = freteEstadoRepository;
        }

        [HttpPost]
        [Route("fale-conosco")]
        [ProducesResponseType(typeof(BaseResponse), statusCode: StatusCodes.Status200OK)]
        public async Task<ActionResult> EnviarFaleConoscoAsync([FromBody] EnviarFaleConoscoRequest request)
        {
            try
            {
                var response = ClienteValidator.Validar(request);

                if (response.Valido)
                {
                    Regex regex = new Regex(@"(\r\n|\r|\n)+");

                    request.Observacao = regex.Replace(request.Observacao, "<br />");

                    //Enviando email
                    string corpoEmail = string.Format(@"
                                        Olá,
                                        <BR><BR>
                                        Acabaram de enviar uma mensagem pelo Fale Conosco.<BR><BR>
                                        <strong>Nome:</strong> {0}<BR>
                                        <strong>Empresa:</strong> {1}<BR>
                                        <strong>Celular:</strong> {2}<BR>
                                        <strong>Telefone:</strong> {3}<BR>
                                        <strong>Email:</strong> {4}<BR><BR>
                                        <strong>Mensagem:</strong> {5}<BR>><BR>
                                        {6}
                                        ", request.Nome, 
                                        (string.IsNullOrEmpty(request.Empresa) ? "Não preencheu" : request.Empresa), 
                                        request.Celular,
                                        (string.IsNullOrEmpty(request.Telefone) ? "Não preencheu" : request.Telefone),
                                        request.Email,
                                        request.Observacao,
                                        ObterRodapePadraoEmail());

                    await EnviarEmailAsync(corpoEmail, "Fale Conosco", new List<string> { configuration.GetValue<string>("AppSettings:email-email") });
                }

                return this.GetHttpResponse(response);
            }
            catch (Exception ex)
            {
                return this.GetHttpResponseError(GetResponseError<BaseResponse>(ex));

            }

        }


        [HttpPatch]
        [Route("senhas")]
        [ProducesResponseType(typeof(BaseResponse), statusCode: StatusCodes.Status200OK)]
        [Authorize(Roles = ConstantesUtil.USUARIO_TIPO_CLI_ADM + "," + ConstantesUtil.USUARIO_TIPO_ADM)]
        public async Task<ActionResult> AlterarSenhaAsync([FromBody] AlterarSenhaRequest request)
        {
            try
            {
                var response = ClienteValidator.Validar(request);

                if (response.Valido)
                {
                    var clienteAutenticado = ClienteAutenticado?.IdCliente;
                    ClienteAlterarSenhaDto clienteDeserializado = null;
                    string idCliente = string.Empty;

                    //se não for em branco, quer dizer que veio de token de alteração.. tem que consistir o objeto
                    if (!string.IsNullOrEmpty(request.Token))
                    {
                        clienteDeserializado = JsonConvert.DeserializeObject<ClienteAlterarSenhaDto>(CriptografiaUtil.Decriptar(StringUtil.DecodificarUrl(request.Token), chaveTokenEsqueciSenha));
                        idCliente = clienteDeserializado.IdCliente;
                        var dataExpiracao = clienteDeserializado.DataAutorizacao;

                        if (DateTime.Now > dataExpiracao)
                            idCliente = string.Empty;
                    }
                    else
                    {
                        idCliente = ClienteAutenticado?.IdCliente;
                    }

                    if (string.IsNullOrEmpty(idCliente))
                    {
                        //se não for, e tirar sem autenticação, retornar não autorizado
                        response.Valido = false;
                        response.Mensagens.Add(new MensagemSistema() { Mensagem = "Token expirado. Solicite novamente o token para alteração." });
                        return this.GetHttpResponse(response);
                    }
                    else
                    {
                        var clienteOriginal = ClienteRepository.ObterAsync(idCliente).Result;
                        
                        //Gerando hash de senha
                        var salt = CriptografiaUtil.GerarSalt();
                        var senha = CriptografiaUtil.CriptografarSenha(request.Senha, salt);

                        clienteOriginal.Salt = salt;
                        clienteOriginal.Senha = senha;

                        var result = ClienteRepository.ManterAsync(clienteOriginal).Result;
                        if (response.Valido)
                        {
                            response.Mensagens.Add(new Messages.MensagemSistema()
                            {
                                Campo = "",
                                Mensagem = MessageResource.COMUM_REGISTRO_ATUALIZADO_SUCESSO
                            });
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

        [HttpPost]
        [Route("esqueci-minha-senha")]
        [ProducesResponseType(typeof(BaseResponse), statusCode: StatusCodes.Status200OK)]
        public async Task<ActionResult> EnviarTokenSenha([FromBody] EnviarTokenSenhaRequest request)
        {
            try
            {
                var response = ClienteValidator.Validar(request);

                if (response.Valido)
                {
                    var cliente = ClienteRepository.ObterPorLoginAsync(request.Email).Result;

                    string token = StringUtil.CodificarUrl(CriptografiaUtil.Encriptar(JsonConvert.SerializeObject(new ClienteAlterarSenhaDto() {
                        IdCliente = cliente.IdCliente,
                        DataAutorizacao = DateTime.Now.AddMinutes(30)
                   }),chaveTokenEsqueciSenha));

                    //Enviando email
                    string corpoEmail = string.Format(@"
                                        Olá {0},
                                        <BR><BR>
                                        Você solicitou a troca de senha para acessar o site. Esse token para troca é valido por 30 minutos.<BR><BR>
                                        Clique no link abaixo, para realizar a troca da senha. <BR>
                                        <strong><a href='{1}'>Trocar Senha</a></strong>
                                        <BR><BR>
                                        Caso não consiga alterar a senha, ou tenha algum dúvida, entre em contato conosco através da opção fale conosco do site.

                                        {3}
                                        ", 
                                        cliente.NomeOuRazao,
                                        configuration.GetValue<string>("AppSettings:url-site") + "/cliente/alterar-senha?acesso-email=true&token=" + token + "&complemento=8PliHLXdcVMOv3BqM1K5e1ux4wP0rghJgcqnD4CkKAPLpFx9zzOlLNeJ1kdT7QM9aZVM2fBXKe73Z4ZQrJyqtfkE4kbrujpYN2Sy",
                                        configuration.GetValue<string>("AppSettings:url-site") + "/cliente/alterar-senha?acesso-email=true&token=" + token,
                                        ObterRodapePadraoEmail());

                    await EnviarEmailAsync(corpoEmail, "Esqueci minha senha", new List<string> { cliente.EmailPrincipal });
                }

                return this.GetHttpResponse(response);
            }
            catch (Exception ex)
            {
                return this.GetHttpResponseError(GetResponseError<BaseResponse>(ex));

            }

        }

        [HttpPost]
        [ProducesResponseType(typeof(CriarClienteResponse), statusCode: StatusCodes.Status200OK)]
        public async Task<ActionResult> CriarClienteAsync([FromBody] CriarClienteRequest request)
        {
            try
            {
                var response = ClienteValidator.Validar(request);

                if (response.Valido)
                {
                    var model = request.Mapper();

                    model.UsuarioCadastro = "Via Site";
                    model.DataCadastroSite = DateTime.Now;
                    model.CodClienteStatus = (int)ConstantesUtil.ClienteStatus.Pendente;

                    //Gerando hash de senha
                    var salt = CriptografiaUtil.GerarSalt();
                    var senha = CriptografiaUtil.CriptografarSenha(request.Senha, salt);

                    model.Salt = salt;
                    model.Senha = senha;

                    var resultado = await ClienteRepository.ManterAsync(model);
                    response.Id = resultado.IdCliente;

                    if (response.Valido)
                    {
                        response.Mensagens.Add(new Messages.MensagemSistema()
                        {
                            Campo = "",
                            Mensagem = "Cadastro efetuado com sucesso"
                        });

                        //Enviando email
                        string corpoEmail = string.Format(@"
                                        Olá {0},
                                        <BR><BR>
                                        O seu cadastro foi realizado com sucesso. Os dados serão analisados e em breve entrarem em contato.
                                        {1}                                               
                                        ", model.NomeOuRazao, ObterRodapePadraoEmail());

                        await EnviarEmailAsync(corpoEmail, "Cadastro de cliente -" + model.NomeOuRazao, new List<string> { model.EmailPrincipal });
                    }
                }

                return this.GetHttpResponse(response);
            }
            catch (Exception ex)
            {
                return this.GetHttpResponseError(GetResponseError<BaseResponse>(ex));

            }

        }


        [HttpPatch]
        [ProducesResponseType(typeof(ManterClienteResponse), statusCode: StatusCodes.Status200OK)]
        [Authorize(Roles = ConstantesUtil.USUARIO_TIPO_CLI_ADM + "," + ConstantesUtil.USUARIO_TIPO_ADM)]
        public async Task<ActionResult> ManterClienteAsync([FromBody] ManterClienteRequest request)
        {
            try
            {
                //Pegando o tipo do usuario para validar de acordo
                ConstantesWebUtil.ClienteAtualizacaoTipo tipoAtualizacao = ConstantesWebUtil.ClienteAtualizacaoTipo.PeloCliente;
                if (UsuarioAutenticado!=null)
                    tipoAtualizacao = ConstantesWebUtil.ClienteAtualizacaoTipo.PorAdmin;

                if (tipoAtualizacao == ConstantesWebUtil.ClienteAtualizacaoTipo.PeloCliente)
                {
                    request.IdCliente = ClienteAutenticado.IdCliente;

                }

                var modelOriginal = await ClienteRepository.ObterAsync(request.IdCliente);

                if (tipoAtualizacao == ConstantesWebUtil.ClienteAtualizacaoTipo.PeloCliente)
                {
                    request.CpfOuCnpj = modelOriginal.CpfOuCnpj;
                    request.TipoPessoa = modelOriginal.TipoPessoa;
                }

                var response = ClienteValidator.Validar(request, tipoAtualizacao);

                if (response.Valido)
                {

                    var model = request.Mapper(modelOriginal, tipoAtualizacao);

                    if (tipoAtualizacao == ConstantesWebUtil.ClienteAtualizacaoTipo.PeloCliente)
                        model.UsuarioAlteracao = ClienteAutenticado.Nome;
                    else
                        model.UsuarioAlteracao = UsuarioAutenticado.Nome;

                    model.DataAlteracao = DateTime.Now;

                    //Limpando campo se for tipo estado
                    if (model.CodFreteTipo == (int)ConstantesUtil.FreteTipo.PorEstado)
                    {
                        model.ValorFreteFixo = null;
                        model.DiasPrazoEntregaFixo = null;

                        //gambi para inserir os fretes por estado para cliente que não tenha nada
                        if (modelOriginal.CodClienteStatus == (int)ConstantesUtil.ClienteStatus.Pendente &&
                            model.CodClienteStatus == (int)ConstantesUtil.ClienteStatus.Ativo)
                        {
                            var lista = ColecoesWebUtil.ListarFreteEstado();

                            lista.ForEach(x =>
                            {
                                var model = new FreteEstadoModel()
                                {
                                    IdFreteEstado = x.IdFreteEstado,
                                    Valor = 29.99,
                                    ValorPedidoFreteGratis = 1000,
                                    UF = x.UF,
                                    DiasPrazoEntrega = 10,
                                    IdCliente = request.IdCliente
                                };

                                freteEstadoRepository.ManterAsync(model);
                            });
                        }

                    }

                    var resultado = await ClienteRepository.ManterAsync(model);
                    response.Id = resultado.IdCliente;

                    if (response.Valido)
                    {
                        response.Mensagens.Add(new Messages.MensagemSistema()
                        {
                            Campo = "",
                            Mensagem = MessageResource.COMUM_REGISTRO_ATUALIZADO_SUCESSO
                        });

                        //enviar o email, caso o cliente tenha passado de pendente para ativo
                        if (modelOriginal.CodClienteStatus == (int)ConstantesUtil.ClienteStatus.Pendente &&
                            resultado.CodClienteStatus == (int)ConstantesUtil.ClienteStatus.Ativo
                            )
                        {
                            string corpoEmail = string.Format(@"
                                        Olá {0},
                                        <BR><BR>
                                        O seu cadastro foi aprovado para acessar o nosso site. Boas compras :-)
                                        {1}                                               
                                        ", model.NomeOuRazao, ObterRodapePadraoEmail());

                            await EnviarEmailAsync(corpoEmail, "Cadastro aprovado", new List<string> { model.EmailPrincipal });
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

        [HttpGet]
        [Authorize(Roles = ConstantesUtil.USUARIO_TIPO_ADM)]
        [ProducesResponseType(typeof(ListarClienteResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> ListarClienteAsync([FromQuery] string codInterno, [FromQuery]string nomeOuRazao, [FromQuery]string cpfOuCnpj, [FromQuery]int? codClienteStatus = null, [FromQuery]int ?codFreteTipo = null,
                                                           [FromQuery] int? paginaAtual = null, [FromQuery] int? numeroRegistros = null)
        {
            try
            {

                var response = ClienteValidator.Validar(codInterno, nomeOuRazao, codFreteTipo, cpfOuCnpj, codClienteStatus);

                if (response.Valido)
                {
                    var result = await ClienteRepository.ListarAsync(codInterno, nomeOuRazao, codFreteTipo, cpfOuCnpj, codClienteStatus, paginaAtual, numeroRegistros);
                    response.Dados = result.Mapper();

                    List<ClienteStatusDto> clienteStatus = ColecoesWebUtil.ListarClienteStatus();
                    List<FreteTipoDto> freteTipo = ColecoesWebUtil.ListarFreteTipo();
                    //recuperando status e tipofrete
                    response.Dados.ForEach(x => {
                        x.ClienteStatus = clienteStatus.Where(a => a.CodClienteStatus == x.CodClienteStatus)?.FirstOrDefault()?.Titulo;
                        x.FreteTipo = freteTipo.Where(a => a.CodFreteTipo == x.CodFreteTipo)?.FirstOrDefault()?.Titulo;
                    });
                }

                return this.GetHttpResponse(response);
            }
            catch (Exception ex)
            {
                return this.GetHttpResponseError(GetResponseError<BaseResponse>(ex));

            }
        }

        [HttpGet]
        [Route("{idCliente}")]
        [ProducesResponseType(typeof(ObterClienteResponse), StatusCodes.Status200OK)]
        [Authorize(Roles = ConstantesUtil.USUARIO_TIPO_CLI_ADM + "," + ConstantesUtil.USUARIO_TIPO_ADM)]
        public async Task<ActionResult> ObterClienteAsync([FromRoute] string idCliente)
        {
            try
            {
                var response = new ObterClienteResponse();

                //Pegando o tipo do usuario para validar de acordo
                if (ClienteAutenticado!=null && !string.IsNullOrEmpty(ClienteAutenticado.IdCliente))
                    idCliente = ClienteAutenticado.IdCliente;

                if (response.Valido && !string.IsNullOrEmpty(idCliente))
                {
                    var result = await ClienteRepository.ObterAsync(idCliente);
                    response.Dados = result.Mapper();
                }

                return this.GetHttpResponse(response);
            }
            catch (Exception ex)
            {
                return this.GetHttpResponseError(GetResponseError<BaseResponse>(ex));

            }
        }

 
        [HttpGet]
        [Route("/cliente-status")]
        [ProducesResponseType(typeof(ListarClienteStatusResponse), StatusCodes.Status200OK)]
        [Authorize(Roles = ConstantesUtil.USUARIO_TIPO_CLI_ADM + "," + ConstantesUtil.USUARIO_TIPO_ADM)]
        public async Task<ActionResult> ListarClienteStatusAsync()
        {
            try
            {
                var response = new ListarClienteStatusResponse();

                response.Dados = ColecoesWebUtil.ListarClienteStatus();

                return this.GetHttpResponse(response);
            }
            catch (Exception ex)
            {
                return this.GetHttpResponseError(GetResponseError<BaseResponse>(ex));

            }
        }


        [HttpPost]
        [Route("autenticacao")]
        [ProducesResponseType(typeof(AutenticarClienteResponse), statusCode: StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult> AutenticarClienteAsync([FromBody] AutenticarClienteRequest request, [FromHeader] string sessao)
        {
            try
            {
                var response = ClienteValidator.Validar(request);

                if (response.Valido)
                {
                    var senha = CriptografiaUtil.EncriptarPadrao(request.Senha);

                    var cliente = ClienteRepository.ObterPorLoginAsync(request.Login).Result;

                    var senhaValidada = CriptografiaUtil.ValidarSenha(request.Senha, cliente?.Salt, cliente?.Senha);

                    if (cliente!=null && !string.IsNullOrEmpty(cliente.IdCliente) && cliente.CodClienteStatus == (int)ConstantesUtil.ClienteStatus.Ativo &&
                        senhaValidada)
                    {

                        DateTime dataExpiracao = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("AppSettings:jwt-expire-min"));

                        var clienteSessao = new ClienteSessaoDto();
                        clienteSessao.Nome = cliente.NomeOuRazao;
                        clienteSessao.CEP = cliente.EnderecoEntrega?.Cep;
                        clienteSessao.CodUsuarioTipo = ConstantesUtil.USUARIO_TIPO_CLI_ADM;
                        clienteSessao.Categorias = cliente.Categorias.ToList();
                        clienteSessao.Token = SecurityWebUtil.GerarToken(configuration, cliente.NomeOuRazao, cliente.IdCliente, ConstantesUtil.USUARIO_TIPO_CLI_ADM, cliente.Login, dataExpiracao);
                        clienteSessao.IsVendedor = request.PerfilVendedor.HasValue ? true : false;
                        clienteSessao.Token = SecurityWebUtil.GerarToken(configuration, cliente.NomeOuRazao, cliente.IdCliente, ConstantesUtil.USUARIO_TIPO_CLI_ADM, cliente.Login, dataExpiracao, clienteSessao.IsVendedor);

                        #region carregando dados no carrinho

                        CarrinhoModel carrinho = pedidoRepository.ObterCarrinhoAsync(cliente.IdCliente).Result;
                        
                        //Se não tiver carrinho para o cliente, vamos criar 
                        if (carrinho == null )
                                carrinho = pedidoRepository.ObterCarrinhoPorTokenAsync(sessao).Result;

                        if (carrinho == null)
                        {
                            carrinho = new CarrinhoModel();
                        }


                        carrinho.IdCliente = cliente.IdCliente;
                        carrinho.DataCadastro = DateTime.Now;
                        carrinho.DataExpiracaoToken = dataExpiracao;
                        carrinho.Token = clienteSessao.Token;

                        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            //Atualizando carrinho com os dados que estavam na sessão
                            await pedidoRepository.ManterCarrinhoAsync(carrinho);

                            //Apagando os antigos da sessão
                            await pedidoRepository.RemoverCarrinhoPorTokenAsync(sessao);

                            scope.Complete();
                        }

                        #endregion

                        if (!string.IsNullOrEmpty(clienteSessao.Token))
                        {
                            response.Dados = clienteSessao;
                            response.Valido = true;
                        }
                        else
                        {
                            response.Valido = false;
                            response.Mensagens.Add(new Messages.MensagemSistema()
                            {
                                Campo = nameof(request.Login),
                                Mensagem = "Erro ao gerar token do cliente"
                            });
                        }
                    }
                    else
                    {
                        response.Valido = false;
                        response.Mensagens.Add(new Messages.MensagemSistema()
                        {
                            Mensagem = "Login ou senha incorretos"
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

        private bool GerarCarrinhoCliente(ClienteModel cliente, ClienteSessaoDto clienteSessao, DateTime dataExpiracao, string sessao)
        {
            bool resultado = false;

            //Criando carrinho que guardara a sessao do cliente
            CarrinhoModel carrinho;

            if (cliente != null)
                carrinho = pedidoRepository.ObterCarrinhoAsync(cliente.IdCliente).Result;
            else
                carrinho = pedidoRepository.ObterCarrinhoPorTokenAsync(sessao).Result;
             
            if (carrinho == null)
            {
                carrinho = new CarrinhoModel()
                {
                    IdCliente = cliente.IdCliente,
                    DataCadastro = DateTime.Now,
                    DataExpiracaoToken = dataExpiracao
                };
                 
            }

            carrinho.DataExpiracaoToken = dataExpiracao;
            carrinho.Token = clienteSessao.Token;

            //carrinho.DataExpiracaoToken = dataExpiracao;
            //carrinho.Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            resultado = pedidoRepository.ManterCarrinhoAsync(carrinho).Result;

            return resultado;
        }

    }
}
