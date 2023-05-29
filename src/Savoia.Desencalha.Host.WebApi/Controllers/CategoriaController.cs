using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Domain.Repositories;
using Savoia.Desencalha.Domain.Resources;
using Savoia.Desencalha.Host.WebApi.Extensions;
using Savoia.Desencalha.Host.WebApi.Filters;
using Savoia.Desencalha.Host.WebApi.Mappers;
using Savoia.Desencalha.Host.WebApi.Messages;
using Savoia.Desencalha.Host.WebApi.Messages.Categoria;
using Savoia.Desencalha.Host.WebApi.Validators;
using static Savoia.Desencalha.Host.WebApi.Helpers.ResponseHelper;

namespace Savoia.Desencalha.Host.WebApi.Controllers
{
    [Route("categorias")]
    public class CategoriaController : BaseController
    {

        internal ICategoriaValidator categoriaValidator;
        internal IClienteRepository clienteRepository;
        internal ICategoriaRepository categoriaRepository;

        public CategoriaController(ICategoriaValidator categoriaValidator,
                           ICategoriaRepository categoriaRepository,
                           IClienteRepository clienteRepository)
        {
            this.categoriaValidator = categoriaValidator;
            this.categoriaRepository = categoriaRepository;
            this.clienteRepository = clienteRepository;
        }

        [HttpPost]
        [Authorize(Roles = ConstantesUtil.USUARIO_TIPO_ADM)]
        [ProducesResponseType(typeof(ManterCategoriaResponse), statusCode: StatusCodes.Status200OK)]
        public async Task<ActionResult> ManterCategoriaAsync([FromBody] ManterCategoriaRequest request)
        {
            try
            {
                var response = categoriaValidator.Validar(request);
                if (response.Valido)
                {
                    var model = request.Mapper();

                    if (string.IsNullOrEmpty(request.IdCategoria)) {
                        model.UsuarioCadastro = UsuarioAutenticado.Nome;
                        model.DataCadastro = DateTime.Now;
                    }
                    else
                    {
                        var modelOriginal = categoriaRepository.ObterAsync(request.IdCategoria).Result;

                        model.UsuarioCadastro = modelOriginal.UsuarioCadastro;
                        model.DataCadastro = modelOriginal.DataCadastro;
                        model.UsuarioAlteracao = UsuarioAutenticado.Nome;
                        model.DataAlteracao = DateTime.Now;
                    }

                    var resultado = await categoriaRepository.ManterAsync(model);
                    response.Id = resultado.IdCategoria;

                    if (response.Valido)
                        response.Mensagens.Add(new Messages.MensagemSistema()
                        {
                            Campo = "",
                            Mensagem = (string.IsNullOrEmpty(request?.IdCategoria) ?  MessageResource.COMUM_REGISTRO_INSERIDO_SUCESSO : MessageResource.COMUM_REGISTRO_ATUALIZADO_SUCESSO)
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
        [ProducesResponseType(typeof(ListarCategoriaResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> ListarCategoriaAsync([FromQuery] string idCategoria, [FromQuery] string codInterno, [FromQuery] string titulo, [FromQuery] bool? ativo = null, [FromQuery] int? paginaAtual = null, [FromQuery] int? numeroRegistros = null)
        {
            try
            {
                var response = categoriaValidator.Validar(codInterno, titulo, ativo);

                if (response.Valido)
                {
                    var result = await categoriaRepository.ListarAsync(idCategoria, codInterno,titulo,ativo, paginaAtual, numeroRegistros);
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
        [Route("{idCategoria}")]
        [ProducesResponseType(typeof(ObterCategoriaResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> ObterCategoriaAsync([FromRoute] string IdCategoria)
        {
            
            try
            {
                var response = new ObterCategoriaResponse();
                if (response.Valido)
                {
                    var result = await categoriaRepository.ObterAsync(IdCategoria);
                    response.Dados = result.Mapper();
                }

                return this.GetHttpResponse(response);
            }
            catch (Exception ex)
            {
                return this.GetHttpResponseError(GetResponseError<BaseResponse>(ex));

            }

        }

        [HttpDelete]
        [Route("{idCategoria}")]
        [ProducesResponseType(typeof(ExcluirCategoriaResponse), StatusCodes.Status200OK)]
        [Authorize(Roles = ConstantesUtil.USUARIO_TIPO_ADM)]
        public async Task<ActionResult> ExcluirCategoriaAsync([FromRoute] string idCategoria)
        {
            try
            {
                var response = new ExcluirCategoriaResponse();

                if (response.Valido)
                {
                    var result = await categoriaRepository.ExcluirAsync(idCategoria);
                    response.RegistrosExcluidos = result;
                }


                if (response.Valido)
                    response.Mensagens.Add(new Messages.MensagemSistema()
                    {
                        Campo = "",
                        Mensagem = MessageResource.COMUM_REGISTRO_EXCLUIDO_SUCESSO
                    });

                return this.GetHttpResponse(response);
            }
            catch (Exception ex)
            {
                return this.GetHttpResponseError(GetResponseError<BaseResponse>(ex));

            }

        }
    }
}
