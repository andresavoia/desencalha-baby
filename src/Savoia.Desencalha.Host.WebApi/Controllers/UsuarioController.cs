using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Domain.Repositories;
using Savoia.Desencalha.Host.WebApi.Dtos.Usuario;
using Savoia.Desencalha.Host.WebApi.Extensions;
using Savoia.Desencalha.Host.WebApi.Mappers;
using Savoia.Desencalha.Host.WebApi.Messages;
using Savoia.Desencalha.Host.WebApi.Messages.Usuario;
using Savoia.Desencalha.Host.WebApi.Util;
using Savoia.Desencalha.Host.WebApi.Validators;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Savoia.Desencalha.Host.WebApi.Helpers.ResponseHelper;

namespace Savoia.Desencalha.Host.WebApi.Controllers
{
    [Route("usuarios")]
    public class UsuarioController : BaseController
    {
        internal IUsuarioValidator usuarioValidator;
        internal IUsuarioRepository usuarioRepository;

        public UsuarioController(IUsuarioValidator usuarioValidator,
                           IUsuarioRepository usuarioRepository,
                           IConfiguration configuration)
        {
            this.usuarioValidator = usuarioValidator;
            this.usuarioRepository = usuarioRepository;
            this.configuration = configuration;
        } 

        [HttpPost]
        [ProducesResponseType(typeof(ManterUsuarioResponse), statusCode: StatusCodes.Status200OK)]
        public async Task<ActionResult> ManterUsuarioAsync([FromBody] ManterUsuarioRequest request)
        {
            try
            {
                var response = usuarioValidator.Validar(request);

                if (response.Valido)
                {
                    //Gerando hash de senha
                    var salt = CriptografiaUtil.GerarSalt();
                    var senha = CriptografiaUtil.CriptografarSenha(request.Senha, salt);
                    var model = request.Mapper();

                    model.Salt = salt;
                    model.Senha = senha;

                    var resultado = await usuarioRepository.ManterAsync(model);
                    response.Id = resultado.IdUsuario;
                }
                return this.GetHttpResponse(response);
            }
            catch (Exception ex)
            {
                return this.GetHttpResponse(GetResponseError<ManterUsuarioResponse>(ex));
            }

        }

        [HttpPost]
        [Route("autenticacao")]
        [ProducesResponseType(typeof(AutenticarUsuarioResponse), statusCode: StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult> AutenticarUsuarioAsync([FromBody] AutenticarUsuarioRequest request)
        {
            try
            {
                var response = usuarioValidator.Validar(request);

                if (response.Valido)
                {
                    var usuario = usuarioRepository.ObterPorLoginAsync(request.Login).Result;

                    var senhaValidada = CriptografiaUtil.ValidarSenha(request.Senha, usuario?.Salt, usuario?.Senha);

                    if (usuario != null && !string.IsNullOrEmpty(usuario.IdUsuario) && usuario.Ativo && senhaValidada)
                    {
                        var usuarioSessao = new UsuarioSessaoDto();
                        usuarioSessao.Nome = usuario.Nome;
                        usuarioSessao.CodUsuarioTipo = usuario.CodUsuarioTipo;
                        usuarioSessao.Login = usuario.Login;
                        usuarioSessao.Token  = SecurityWebUtil.GerarToken(configuration, usuario.Nome, usuario.IdUsuario, usuario.CodUsuarioTipo, usuario.Login, DateTime.UtcNow.AddMinutes(ConstantesUtil.TIMEOUT_SESSAO_MINUTOS), null);
                         
                        response.Dados = usuarioSessao;
                        response.Valido = true;
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
                return this.GetHttpResponseError(GetResponseError<AutenticarUsuarioResponse>(ex));
            }

        }

        [HttpGet]
        [Route("{idUsuario}")]
        [Authorize(Roles = ConstantesUtil.USUARIO_TIPO_ADM)]
        [ProducesResponseType(typeof(ObterUsuarioResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> ObterUsuarioAsync([FromRoute] string idUsuario)
        {
            try
            {
                var response = new ObterUsuarioResponse();

                if (response.Valido)
                {
                    var result = await usuarioRepository.ObterAsync(idUsuario);
                    response.Dados = result.Mapper();
                }
                return this.GetHttpResponse(response);
            }
            catch (Exception ex)
            {
                return this.GetHttpResponseError(GetResponseError<ObterUsuarioResponse>(ex));
            }
        }

        [HttpPatch]
        [Route("senhas")]
        [Authorize(Roles = ConstantesUtil.USUARIO_TIPO_ADM)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> AlterarSenhaAsync([FromBody] AlterarSenhaUsuarioRequest request)
        {
            try
            {
                var response = usuarioValidator.Validar(request);

                if (response.Valido)
                {

                    //Gerando hash de senha
                    var salt = CriptografiaUtil.GerarSalt();
                    var senha = CriptografiaUtil.CriptografarSenha(request.SenhaNova, salt);

                    var model = await usuarioRepository.ObterPorLoginAsync(UsuarioAutenticado.Login);

                    var senhaValidada = CriptografiaUtil.ValidarSenha(request.SenhaAtual, model.Salt, model.Senha);

                    if (!senhaValidada)
                    {
                        response.Mensagens.Add(new Messages.MensagemSistema()
                        {
                            Campo = "",
                            Mensagem = "Senha atual inválida"
                        });
                    }
                    else
                    {
                        model.Salt = salt;
                        model.Senha = senha;

                        var resultado = await usuarioRepository.ManterAsync(model);

                        response.Valido = (resultado != null);

                        if (response.Valido)
                            response.Mensagens.Add(new Messages.MensagemSistema()
                            {
                                Campo = "",
                                Mensagem = "Senha alterada com sucesso"
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

        //[HttpGet]
        //[ProducesResponseType(typeof(ListarUsuarioResponse), StatusCodes.Status200OK)]
        //[Authorize(Roles = ConstantesUtil.USUARIO_TIPO_ADM)]
        //public async Task<ActionResult> ListarUsuarioAsync([FromQuery] int? paginaAtual = null, [FromQuery] int? numeroRegistros = null)
        //{

        //    try
        //    {
        //        var response = new ListarUsuarioResponse();

        //        if (response.Valido)
        //        {
        //            var result = await usuarioRepository.ListarAsync(paginaAtual, numeroRegistros);
        //            response.Dados = result.Mapper();
        //        }
        //        return this.GetHttpResponse(response);
        //    }
        //    catch (Exception ex)
        //    {

        //        return this.GetHttpResponseError(GetResponseError<ListarUsuarioResponse>(ex));
        //    }



        //}

        //[HttpDelete]
        //[Route("{idUsuario}")]
        //[Authorize(Roles = ConstantesUtil.USUARIO_TIPO_ADM)]
        //[ProducesResponseType(typeof(ExcluirUsuarioResponse), StatusCodes.Status200OK)]
        //public async Task<ActionResult> ExcluirUsuarioAsync([FromRoute] string idUsuario)
        //{
        //    try
        //    {
        //        var response = new ExcluirUsuarioResponse();

        //        if (response.Valido)
        //        {
        //            var result = await usuarioRepository.ExcluirAsync(idUsuario);
        //            response.RegistrosExcluidos = result;
        //        }
        //        return this.GetHttpResponse(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return this.GetHttpResponseError(GetResponseError<ExcluirUsuarioResponse>(ex));
        //    }


        //}

    }
}
