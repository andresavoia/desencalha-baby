using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Domain.Repositories;
using Savoia.Desencalha.Domain.Resources;
using Savoia.Desencalha.Host.WebApi.Extensions;
using Savoia.Desencalha.Host.WebApi.Filters;
using Savoia.Desencalha.Host.WebApi.Mappers;
using Savoia.Desencalha.Host.WebApi.Messages;
using Savoia.Desencalha.Host.WebApi.Messages.RamoAtividade;
using Savoia.Desencalha.Host.WebApi.Validators;
using static Savoia.Desencalha.Host.WebApi.Helpers.ResponseHelper;

namespace Savoia.Desencalha.Host.WebApi.Controllers
{
    [Route("ramos-atividades")]
    public class RamoAtividadeController : BaseController
    {

        internal IRamoAtividadeValidator RamoAtividadeValidator;
        internal IRamoAtividadeRepository RamoAtividadeRepository;

        public RamoAtividadeController(IRamoAtividadeValidator RamoAtividadeValidator,
                           IRamoAtividadeRepository RamoAtividadeRepository)
        {
            this.RamoAtividadeValidator = RamoAtividadeValidator;
            this.RamoAtividadeRepository = RamoAtividadeRepository;
        }

        [HttpPost]
        [Authorize(Roles = ConstantesUtil.USUARIO_TIPO_ADM)]
        [ProducesResponseType(typeof(ManterRamoAtividadeResponse), statusCode: StatusCodes.Status200OK)]
        public async Task<ActionResult> ManterRamoAtividadeAsync([FromBody] ManterRamoAtividadeRequest request)
        {
            try
            {
                var response = RamoAtividadeValidator.Validar(request);
                if (response.Valido)
                {
                    var model = request.Mapper();

                    if (string.IsNullOrEmpty(request.IdRamoAtividade)) {
                        model.UsuarioCadastro = UsuarioAutenticado.Nome;
                        model.DataCadastro = DateTime.Now;
                    }
                    else
                    {
                        var modelOriginal = RamoAtividadeRepository.ObterAsync(request.IdRamoAtividade).Result;

                        model.UsuarioCadastro = modelOriginal.UsuarioCadastro;
                        model.DataCadastro = modelOriginal.DataCadastro;
                        model.UsuarioAlteracao = UsuarioAutenticado.Nome;
                        model.DataAlteracao = DateTime.Now;
                    }

                    var resultado = await RamoAtividadeRepository.ManterAsync(model);
                    response.Id = resultado.IdRamoAtividade;

                    if (response.Valido)
                        response.Mensagens.Add(new Messages.MensagemSistema()
                        {
                            Campo = "",
                            Mensagem = (string.IsNullOrEmpty(request?.IdRamoAtividade) ?  MessageResource.COMUM_REGISTRO_INSERIDO_SUCESSO : MessageResource.COMUM_REGISTRO_ATUALIZADO_SUCESSO)
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
        [ProducesResponseType(typeof(ListarRamoAtividadeResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> ListarRamoAtividadeAsync([FromQuery] string idRamoAtividade, [FromQuery] string titulo, [FromQuery] bool? ativo = null, [FromQuery] int? paginaAtual = null, [FromQuery] int? numeroRegistros = null)
        {
            try
            {
                var response = RamoAtividadeValidator.Validar(titulo, ativo);

                if (response.Valido)
                {
                    var result = await RamoAtividadeRepository.ListarAsync(idRamoAtividade, titulo,ativo, paginaAtual, numeroRegistros);
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
        [Route("{idRamoAtividade}")]
        [ProducesResponseType(typeof(ObterRamoAtividadeResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> ObterRamoAtividadeAsync([FromRoute] string IdRamoAtividade)
        {
            
            try
            {
                var response = new ObterRamoAtividadeResponse();
                if (response.Valido)
                {
                    var result = await RamoAtividadeRepository.ObterAsync(IdRamoAtividade);
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
        [Route("{idRamoAtividade}")]
        [ProducesResponseType(typeof(ExcluirRamoAtividadeResponse), StatusCodes.Status200OK)]
        [Authorize(Roles = ConstantesUtil.USUARIO_TIPO_ADM)]
        public async Task<ActionResult> ExcluirRamoAtividadeAsync([FromRoute] string idRamoAtividade)
        {
            try
            {
                var response = new ExcluirRamoAtividadeResponse();

                if (response.Valido)
                {
                    var result = await RamoAtividadeRepository.ExcluirAsync(idRamoAtividade);
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
