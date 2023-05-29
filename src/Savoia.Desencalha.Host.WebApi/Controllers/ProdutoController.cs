using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Domain.Models;
using Savoia.Desencalha.Domain.Repositories;
using Savoia.Desencalha.Domain.Resources;
using Savoia.Desencalha.Host.WebApi.Dtos.Produto;
using Savoia.Desencalha.Host.WebApi.Extensions;
using Savoia.Desencalha.Host.WebApi.Filters;
using Savoia.Desencalha.Host.WebApi.Mappers;
using Savoia.Desencalha.Host.WebApi.Messages;
using Savoia.Desencalha.Host.WebApi.Messages.Produto;
using Savoia.Desencalha.Host.WebApi.Util;
using Savoia.Desencalha.Host.WebApi.Validators;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static Savoia.Desencalha.Host.WebApi.Helpers.ResponseHelper;

namespace Savoia.Desencalha.Host.WebApi.Controllers
{
    [Route("produtos")]
    public class ProdutoController : BaseController
    {
        internal IProdutoValidator produtoValidator;
        internal IProdutoRepository produtoRepository;
        internal ICategoriaRepository categoriaRepository;
        internal IClienteRepository clienteRepository;
        readonly IWebHostEnvironment environment;
        internal string pathImagensProduto = "imagens";

        public ProdutoController(IProdutoValidator produtoValidator,
                           IProdutoRepository produtoRepository,
                           IWebHostEnvironment environment,
                           ICategoriaRepository categoriaRepository,
                           IClienteRepository clienteRepository)
        {
            this.produtoValidator = produtoValidator;
            this.produtoRepository = produtoRepository;
            this.environment = environment;
            this.categoriaRepository = categoriaRepository;
            this.clienteRepository = clienteRepository;
        }


        [HttpPost]
        [ProducesResponseType(typeof(ManterProdutoResponse), statusCode: StatusCodes.Status200OK)]
        [Authorize(Roles = ConstantesUtil.USUARIO_TIPO_CLI_ADM + "," + ConstantesUtil.USUARIO_TIPO_ADM)]
        public async Task<ActionResult> ManterProdutoAsync([FromBody] ManterProdutoRequest request)
        {
            try
            {
                var response = produtoValidator.Validar(request);

                if (response.Valido)
                {

                    //se for o cliente logado, forçar carregar o idcliente no request
                    if (UsuarioAutenticacaoTipo.Equals(ConstantesWebUtil.UsuarioAutenticacaoTipo.Cliente))
                        request.IdCliente = ClienteAutenticado.IdCliente;

                    var model = request.Mapper();

                    if (string.IsNullOrEmpty(request.IdProduto))
                    {
                        if (UsuarioAutenticacaoTipo.Equals(ConstantesWebUtil.UsuarioAutenticacaoTipo.Admin))
                            model.UsuarioCadastro = UsuarioAutenticado.Nome;
                        else
                            model.UsuarioCadastro = ClienteAutenticado.Nome;


                        model.DataCadastro = DateTime.Now;

                       
                        if(UsuarioAutenticacaoTipo.Equals(ConstantesWebUtil.UsuarioAutenticacaoTipo.Cliente))
                            model.IdCliente = ClienteAutenticado?.IdCliente;  // se for admin, vem no request
                  

                    }
                    else
                    {
                        var modelOriginal = produtoRepository.ObterAsync(request.IdProduto).Result;
                        model.UsuarioCadastro = modelOriginal.UsuarioCadastro;
                        model.DataCadastro = modelOriginal.DataCadastro;


                        if (UsuarioAutenticacaoTipo.Equals(ConstantesWebUtil.UsuarioAutenticacaoTipo.Admin))
                            model.UsuarioAlteracao = UsuarioAutenticado.Nome;
                        else
                            model.UsuarioAlteracao = ClienteAutenticado.Nome;
    
                        model.DataAlteracao = DateTime.Now;

                        if (model.ValorVenda != modelOriginal.ValorVenda ||
                            model.Altura != modelOriginal.Altura)
                        {
                            model.PrecosLog = modelOriginal?.PrecosLog?.OrderByDescending(x => x.DataAlteracao)?.Take(5)?.ToList();

                            if (model.PrecosLog == null)
                                model.PrecosLog = new List<Domain.Models.ProdutoPrecoLogModel>();

                            model.PrecosLog.Add(new Domain.Models.ProdutoPrecoLogModel()
                            {
                                DataAlteracao = DateTime.Now,
                                UsuarioAlteracao = UsuarioAutenticado.Nome,
                                ValorVenda = modelOriginal.ValorVenda
                            });
                        }

                    }

                    var resultado = await produtoRepository.ManterAsync(model);
                    response.Id = resultado.IdProduto;

                    if (response.Valido)
                    {
                        //Movendo imagens qd novo
                        if (string.IsNullOrEmpty(request.IdProduto))
                        {
                            //movendo imagens para a pasta definitiva
                            string caminhoOriginal = Path.Combine(environment.WebRootPath, pathImagensProduto,"$temp");
                            string caminhoNovo = Path.Combine(environment.WebRootPath, pathImagensProduto, response.Id);

                            if (!Directory.Exists(caminhoNovo))
                                Directory.CreateDirectory(caminhoNovo);

                            request.Imagens.ForEach(x =>
                            {
                                var extensao = x.Nome.Substring(x.Nome.Length - 3, 3);

                                System.IO.File.Copy(caminhoOriginal + @"\" + x.Nome, caminhoNovo+@"\" + x.Nome, true);
                                System.IO.File.Delete(caminhoOriginal + @"\" + x.Nome);

                                System.IO.File.Copy(caminhoOriginal + @"\" + x.Nome.Replace($".{extensao}", $"_M.{extensao}", StringComparison.InvariantCultureIgnoreCase) , caminhoNovo + @"\" + x.Nome.Replace($".{extensao}", $"_M.{extensao}", StringComparison.InvariantCultureIgnoreCase), true);
                                System.IO.File.Delete(caminhoOriginal + @"\" + x.Nome.Replace($".{extensao}", $"_M.{extensao}", StringComparison.InvariantCultureIgnoreCase));

                            });
                            
                        }
                        else
                        {
                            //Apagando imagens que não existem mais no cadastro
                            string caminhoNovo = Path.Combine(environment.WebRootPath, pathImagensProduto, response.Id);

                            string[] files = Directory.GetFiles(caminhoNovo);
                            foreach (string file in files)
                            {

                                if ( !(request.Imagens.Where(x => x.Nome.ToLower() == Path.GetFileName(file).ToLower() 
                                        || x.Nome.Replace(".png", "_m.png").Replace(".jpg", "_m.jpg").ToLower() == Path.GetFileName(file).ToLower()).Any())) 
                                {
                                    System.IO.File.Delete(caminhoNovo + @"\" + Path.GetFileName(file));
                                }
                            }

                        }

                        response.Mensagens.Add(new Messages.MensagemSistema()
                        {
                            Campo = "",
                            Mensagem = (string.IsNullOrEmpty(request?.IdProduto) ? MessageResource.COMUM_REGISTRO_INSERIDO_SUCESSO : MessageResource.COMUM_REGISTRO_ATUALIZADO_SUCESSO)
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
        [Route("precos")]
        [ProducesResponseType(typeof(ManterProdutoPrecoResponse), statusCode: StatusCodes.Status200OK)]
        [Authorize(Roles = ConstantesUtil.USUARIO_TIPO_CLI_ADM + "," + ConstantesUtil.USUARIO_TIPO_ADM)]
        public async Task<ActionResult> ManterProdutoPrecoAsync([FromBody] ManterProdutoPrecoRequest request)
        {
            try
            {

                var response = produtoValidator.Validar(request);

                if (response.Valido)
                {

                    ProdutoModel resultado = new ProdutoModel();

                    request?.Produtos.ForEach(x =>
                    {

                        var modelOriginal = produtoRepository.ObterAsync(x.IdProduto).Result;
                        var modelNovo = modelOriginal.MapperClone();

                        if (x.ValorVenda != modelOriginal.ValorVenda)
                        {
                            modelNovo.PrecosLog = modelOriginal?.PrecosLog?.OrderByDescending(y => y.DataAlteracao)?.Take(5)?.ToList();

                            if (modelNovo.PrecosLog == null)
                                modelNovo.PrecosLog = new List<Domain.Models.ProdutoPrecoLogModel>();

                            modelNovo.PrecosLog.Add(new Domain.Models.ProdutoPrecoLogModel()
                            {
                                DataAlteracao = DateTime.Now,
                                UsuarioAlteracao = UsuarioAutenticado.Nome,
                                ValorVenda = modelOriginal.ValorVenda
                            });

                            modelNovo.ValorVenda = x.ValorVenda;
                            resultado = produtoRepository.ManterAsync(modelNovo).Result;
                        }

                    });


                    if (response.Valido)
                        response.Mensagens.Add(new Messages.MensagemSistema()
                        {
                            Campo = "",
                            Mensagem = MessageResource.COMUM_REGISTROS_ATUALIZADOS_SUCESSO
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
        [Route("ativos")]
        [ProducesResponseType(typeof(ListarProdutoResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> ListarProdutoAtivosAsync([FromQuery] string titulo, [FromQuery] string idCategoria = null,
                                                           [FromQuery] string ordenacao = null, [FromQuery] int? paginaAtual = null, [FromQuery] int? numeroRegistros = null)
        {
            try
            {
                //Se vier externo, vamos sempre pegar os ativos
                bool acessoCliente = false;
                string idClienteParametro = ClienteAutenticado?.IdCliente;

                var response = produtoValidator.Validar(titulo, idCategoria, idClienteParametro);

                if (response.Valido)
                {
                    ClienteModel cliente = await clienteRepository.ObterAsync(idClienteParametro);

                    List<ProdutoModel> result = null;
                    List<string> categorias = null;

                    if (cliente?.Categorias != null)
                        categorias = cliente.Categorias.ToList();

                    result = await produtoRepository.ListarAtivosAsync(titulo, idCategoria, categorias, idClienteParametro, cliente?.IdRamoAtividade, ordenacao, paginaAtual, numeroRegistros);

                    response.Dados = result.Mapper();

                    response?.Dados.ForEach(produto =>
                    {
                        produto?.Imagens?.ForEach(x =>
                        {
                            x.Caminho = "/imagens/" + produto.IdProduto;
                        });

                    });

                    if (response?.Dados?.Count > 0)
                        response.TotalRegistros = result[0].TotalRegistros;

                }

                return this.GetHttpResponse(response);
            }
            catch (Exception ex)
            {
                return this.GetHttpResponseError(GetResponseError<BaseResponse>(ex));

            }

        }


        [HttpGet]
        [ProducesResponseType(typeof(ListarProdutoResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> ListarProdutoAsync([FromQuery] string codInterno, [FromQuery]string titulo, [FromQuery]string idCategoria = null,
                                                           [FromQuery] bool? ativo = null, [FromQuery] bool? promocao = null, [FromQuery] bool? lancamento = null,
                                                           [FromQuery] bool? recentes = null,
                                                           [FromQuery] bool? direcionados = null,
                                                           [FromQuery] string ordenacao = null, [FromQuery] int? paginaAtual = null, [FromQuery] int? numeroRegistros = null, bool? webAppAdmin = null,
                                                           [FromQuery] string idCliente = null)
        {


            try
            {
                //Se vier externo, vamos sempre pegar os ativos
                bool acessoCliente = false;
                string idClienteParametro = string.Empty;

                if (!string.IsNullOrEmpty(ClienteAutenticado?.IdCliente)) {

                    acessoCliente = true;
                    if(numeroRegistros == null) numeroRegistros = 40;

                    if (!webAppAdmin.HasValue || !webAppAdmin.Value)
                    {
                        ativo = true;
                    }
                    
                }

                //Pegando o tipo do usuario para validar de acordo
                if (UsuarioAutenticado == null)
                    ativo = true;

                if (UsuarioAutenticacaoTipo.Equals(ConstantesWebUtil.UsuarioAutenticacaoTipo.Admin))
                    idClienteParametro = idCliente;
                else
                    idClienteParametro = ClienteAutenticado?.IdCliente;

                var response = produtoValidator.Validar(codInterno, titulo, idCategoria, ativo, promocao, lancamento,idClienteParametro);

                if (response.Valido)
                {
                    var cliente = await clienteRepository.ObterAsync(idClienteParametro);

                    List<ProdutoModel> result = null;

                    if (recentes!=null && recentes==true)
                    {
                        result =  produtoRepository.ListarAsync(codInterno, titulo, idCategoria, ativo, null, null, null, paginaAtual, 20,
                               idClienteParametro, webAppAdmin, cliente?.Categorias?.ToList()).Result.OrderByDescending(x =>x.DataCadastro).ToList();
                    }
                    else if(direcionados!=null && direcionados == true)
                    {
                        result = produtoRepository.ListarPorRamoAtividadeAsync(cliente.IdRamoAtividade,ordenacao,paginaAtual,numeroRegistros,cliente.IdCliente).Result.OrderByDescending(x => x.DataCadastro).ToList();
                    }
                    else
                    {
                        result = await produtoRepository.ListarAsync(codInterno, titulo, idCategoria, ativo, promocao, lancamento, ordenacao, paginaAtual, numeroRegistros,
                        idClienteParametro, webAppAdmin, cliente?.Categorias?.ToList());

                    }

                    response.Dados = result.Mapper();

                    response?.Dados.ForEach(produto =>
                    {
                        produto?.Imagens?.ForEach(x =>
                        {
                            x.Caminho = "/imagens/" + produto.IdProduto;
                        });

                    });

                    if (response?.Dados?.Count > 0)
                        response.TotalRegistros = result[0].TotalRegistros;

                }

                return this.GetHttpResponse(response);
            }
            catch (Exception ex)
            {
                return this.GetHttpResponseError(GetResponseError<BaseResponse>(ex));

            }

        }

        [HttpGet]
        [Route("{idProduto}")]
        [ProducesResponseType(typeof(ObterProdutoResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> ObterProdutoAsync([FromRoute] string idProduto)
        {
            try
            {

                var response = new ObterProdutoResponse();

                if (response.Valido)
                {
                    var result = await produtoRepository.ObterAsync(idProduto);

                    if (UsuarioAutenticado == null && result.Ativo == false)
                        return null;

                    response.Dados = result.Mapper();

                    //retornando a categoria
                    var categoria = categoriaRepository.ObterAsync(response.Dados.IdCategoria).Result;

                    if (response.Dados!=null && categoria!=null)
                        response.Dados.DescCategoria = categoria.Titulo;

                    response?.Dados?.Imagens?.ForEach(x =>
                    {
                        x.Caminho = "/imagens/" + response.Dados.IdProduto;
                    });

                    //if (AcessoExterno)
                    //{
                        List<ProdutoDto> listaCalculada = new List<ProdutoDto>();
                        listaCalculada.Add(response?.Dados);

                        //CalcularValorProdutosExterno(listaCalculada);

                        response.Dados = listaCalculada[0];
                    //}
                }

                return this.GetHttpResponse(response);
            }
            catch (Exception ex)
            {
                return this.GetHttpResponseError(GetResponseError<ObterProdutoResponse>(ex));
            }

        }

        [HttpDelete]
        [Route("{idProduto}")]
        [Authorize(Roles = ConstantesUtil.USUARIO_TIPO_CLI_ADM + "," + ConstantesUtil.USUARIO_TIPO_ADM)]
        [ProducesResponseType(typeof(ExcluirProdutoResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> ExcluirProdutoAsync([FromRoute] string idProduto)
        {
            try
            {

                var response = new ExcluirProdutoResponse();

                if (response.Valido)
                {
                    var result = await produtoRepository.ExcluirAsync(idProduto);
                    response.RegistrosExcluidos = result;
                }

                if (response.Valido)
                {
                    response.Mensagens.Add(new Messages.MensagemSistema()
                    {
                        Campo = "",
                        Mensagem = MessageResource.COMUM_REGISTRO_EXCLUIDO_SUCESSO
                    });

                    //Apagando imagens que não existem mais no cadastro
                    Directory.Delete(Path.Combine(environment.WebRootPath, pathImagensProduto, idProduto),true);

                }

                return this.GetHttpResponse(response);
            }
            catch (Exception ex)
            {
                return this.GetHttpResponseError(GetResponseError<ExcluirProdutoResponse>(ex));
            }

            
        }

        //[HttpPost(pathImagensProduto)]
        //public async Task SubirImagem(IFormFile file)
        //{
        //    var uploads = Path.Combine(_environment.WebRootPath, "images");

        //    if (file.Length > 0)
        //    {
        //        using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
        //        {
        //            await file.CopyToAsync(fileStream);
        //        }
        //    }
        //}

        [HttpPost("imagens")]
        [Authorize(Roles = ConstantesUtil.USUARIO_TIPO_CLI_ADM + "," + ConstantesUtil.USUARIO_TIPO_ADM)]
        public ActionResult SubirImagemVarias(List<IFormFile> files, [FromQuery] string idProduto)
        {
            try
            {
                idProduto = idProduto?.Trim();
                //Validando todos os arquivos
                foreach (IFormFile item in files)
                {
                    if (item.Length > 0)
                    {
                        string fileName = ContentDispositionHeaderValue.Parse(item.ContentDisposition).FileName.Trim('"');

                        string extensaoArquivo = fileName.Substring(fileName.Length - 3, 3);
                        string nomeArquivo = fileName.Substring(0, fileName.Length - 4);

                        if (fileName.Length < 4 ||
                            (extensaoArquivo.ToUpper() != "JPG") && (extensaoArquivo.ToUpper() != "PNG") )
                        {
                            return this.GetHttpResponse(new BaseResponse()
                            {
                                Valido = false,
                                Mensagens = new List<MensagemSistema>() {
                                        new MensagemSistema(){
                                        Campo  = "",
                                        Mensagem = "Só são permitidos arquivos de extensão JPG e PNG"
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

                    //criando a pasta de acordo com a categoria
                    if (!string.IsNullOrEmpty(idProduto))
                    {
                        var produto = produtoRepository.ObterAsync(idProduto).Result;

                        if (produto != null)
                            subPasta = @"/imagens/" + produto.IdProduto;
                        else
                            subPasta = @"/imagens/$ProdutoSemCategoria";
                    }
                    else
                        subPasta = @"/imagens/$Temp";

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
                            string nomeArquivoCompactado = nomeArquivo + "_M." + extensaoArquivo;
                            nomeArquivo = nomeArquivo + "." + extensaoArquivo;

                            var continuar = true;
                            int contador = 1;
                            while (continuar)
                            {
                                if (System.IO.File.Exists(newPath + subPasta + "/" + nomeArquivo)) {
                                    nomeArquivo = fileName.Substring(0, fileName.Length - 4) + "(" + contador + ")." + extensaoArquivo;
                                    nomeArquivoCompactado = fileName.Substring(0, fileName.Length - 4) + "(" + contador + ")_M." + extensaoArquivo;
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
                            System.Threading.Thread.Sleep(300);
                            //Compactando a versão que subiu
                            using (var image = SixLabors.ImageSharp.Image.Load(Path.Combine(newPath + subPasta, nomeArquivo)))
                            {
                                image.Mutate(x => x
                                     .Resize(image.Width / 3, image.Height / 3)
                                     .Quantize());

                                FileStream file = new FileStream(Path.Combine(newPath + subPasta, nomeArquivoCompactado), FileMode.Create);
                                image.SaveAsJpeg(file, new JpegEncoder() { Quality = 80});
                                file.Close();
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

                    //return this.Content("OK");
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

                //return this.Content("Fail");
            }
            catch (Exception ex)
            {
                return this.GetHttpResponseError(GetResponseError<BaseResponse>(ex));
            }

        }

    }
}
