using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Domain.Models;
using Savoia.Desencalha.Domain.Repositories;
using Savoia.Desencalha.Domain.Resources;
using Savoia.Desencalha.Host.WebApi.Dtos.Frete;
using Savoia.Desencalha.Host.WebApi.Extensions;
using Savoia.Desencalha.Host.WebApi.Filters;
using Savoia.Desencalha.Host.WebApi.Mappers;
using Savoia.Desencalha.Host.WebApi.Messages;
using Savoia.Desencalha.Host.WebApi.Messages.Frete;
using Savoia.Desencalha.Host.WebApi.Util;
using Savoia.Desencalha.Host.WebApi.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Savoia.Desencalha.Host.WebApi.Helpers.ResponseHelper;

namespace Savoia.Desencalha.Host.WebApi.Controllers
{
    [Route("fretes")]
    public class FreteController : BaseController
    {
        internal IFreteEstadoRepository freteEstadoRepository;
        internal IFreteValidator validator;

        public FreteController(IFreteEstadoRepository freteEstadoRepository, IFreteValidator validator)
        {
            this.freteEstadoRepository = freteEstadoRepository;
            this.validator = validator;
        }

        [HttpGet]
        [Route("tipos")]
        [ProducesResponseType(typeof(ListarFreteTipoResponse), StatusCodes.Status200OK)]
        [Authorize(Roles = ConstantesUtil.USUARIO_TIPO_CLI_ADM + "," + ConstantesUtil.USUARIO_TIPO_ADM)]
        public async Task<ActionResult> ListarFreteTipoAsync()
        {
            try
            {
                var response = new ListarFreteTipoResponse();

                response.Dados = ColecoesWebUtil.ListarFreteTipo();

                return this.GetHttpResponse(response);
            }
            catch (Exception ex)
            {
                return this.GetHttpResponseError(GetResponseError<BaseResponse>(ex));

            }
        }

        [HttpGet]
        [Route("~/clientes/{idCliente}/frestes/estados")]
        [Authorize(Roles = ConstantesUtil.USUARIO_TIPO_CLI_ADM + "," + ConstantesUtil.USUARIO_TIPO_ADM)]
        [ProducesResponseType(typeof(ListarFreteEstadoResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> ListarFreteEstado([FromRoute]string idCliente)
        {
            try
            {

                List<FreteEstadoDto> lista = new List<FreteEstadoDto>();

                if (UsuarioAutenticacaoTipo.Equals(ConstantesWebUtil.UsuarioAutenticacaoTipo.Cliente))
                {
                    idCliente = ClienteAutenticado.IdCliente;
                }

                var response = validator.Validar(idCliente);

                if (response.Valido)
                {

                    freteEstadoRepository.ListarAsync(idCliente).Result.ForEach(x =>
                    {
                        lista.Add(new FreteEstadoDto()
                        {
                            IdFreteEstado = x.IdFreteEstado,
                            Valor = x.Valor,
                            UF = x.UF,
                            ValorPedidoFreteGratis = x.ValorPedidoFreteGratis,
                            DiasPrazoEntrega = x.DiasPrazoEntrega
                        });
                    });

                    //gambi pra carregar lista idCliente = ""

                    if (lista?.Count < 1)
                    {
                        lista = ColecoesWebUtil.ListarFreteEstado();

                    }
                }
                

                response.Dados = lista;
                

                return this.GetHttpResponse(response);
            }
            catch (Exception ex)
            {
                return this.GetHttpResponseError(GetResponseError<BaseResponse>(ex));

            }
        }





        [HttpPost]
        [Route("estados")]
        [ProducesResponseType(typeof(ManterFreteEstadoResponse), statusCode: StatusCodes.Status200OK)]
        [Authorize(Roles = ConstantesUtil.USUARIO_TIPO_CLI_ADM + "," + ConstantesUtil.USUARIO_TIPO_ADM)]
        public async Task<ActionResult> ManterFreteAsync([FromBody] ManterFreteEstadoRequest request)
        {
            try
            {

                if (UsuarioAutenticacaoTipo.Equals(ConstantesWebUtil.UsuarioAutenticacaoTipo.Cliente))
                {
                    request.IdCliente = ClienteAutenticado.IdCliente;
                }

                var response = new ManterFreteEstadoResponse();
                if (response.Valido)
                {
                    request.Fretes.ForEach(x =>
                    {
                        var model = new FreteEstadoModel()
                        {
                            IdFreteEstado = x.IdFreteEstado,
                            Valor = x.Valor,
                            ValorPedidoFreteGratis = x.ValorPedidoFreteGratis,
                            UF = x.UF,
                            DiasPrazoEntrega = x.DiasPrazoEntrega,
                            IdCliente = request.IdCliente
                        };

                        freteEstadoRepository.ManterAsync(model);
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
        [Route("tipos/{codFreteTipo}")]
        [ProducesResponseType(typeof(ObterFreteTipoResponse), StatusCodes.Status200OK)]
        [Authorize(Roles = ConstantesUtil.USUARIO_TIPO_CLI_ADM + "," + ConstantesUtil.USUARIO_TIPO_ADM)]
        public async Task<ActionResult> ObterFreteTipoAsync([FromRoute] int codFreteTipo)
        {
            
            try
            {
                var response = new ObterFreteTipoResponse();
                response.Dados = ColecoesWebUtil.ListarFreteTipo().Where(x => x.CodFreteTipo == codFreteTipo).FirstOrDefault();
                return this.GetHttpResponse(response);
            }
            catch (Exception ex)
            {
                return this.GetHttpResponseError(GetResponseError<ObterFreteTipoResponse>(ex));

            }


        }
  

    }
}
