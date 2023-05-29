using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Domain.Repositories;
using Savoia.Desencalha.Host.WebApi.Dtos.Cliente;
using Savoia.Desencalha.Host.WebApi.Dtos.Usuario;
using Savoia.Desencalha.Host.WebApi.Util;
using System.Security.Claims;

namespace Savoia.Desencalha.Host.WebApi.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected IEmailRepository emailRepository;
        protected IConfiguration configuration;

        public UsuarioSessaoDto UsuarioAutenticado
        {

            get
            {
                if (User.Identity.IsAuthenticated && User.FindFirst(x => x.Type == ClaimTypes.Role).Value == ConstantesUtil.USUARIO_TIPO_ADM)
                {
                    return new UsuarioSessaoDto()
                    {
                        Nome = User.Identity.Name,
                        IdUsuario = User.FindFirst(x => x.Type == ClaimTypes.Sid).Value,
                        Login = User.FindFirst(x => x.Type == ClaimTypes.Email).Value,
                        CodUsuarioTipo = User.FindFirst(x => x.Type == ClaimTypes.Role).Value
                    };
                }
                else
                    return null;
            }


        }

        public ClienteSessaoDto ClienteAutenticado
        {
            get
            {
                try
                {
                    if (User.Identity.IsAuthenticated && User.FindFirst(x => x.Type == ClaimTypes.Role).Value ==ConstantesUtil.USUARIO_TIPO_CLI_ADM)
                    {
                        return new ClienteSessaoDto()
                        {
                            Nome = User.Identity.Name,
                            IdCliente = User.FindFirst(x => x.Type == ClaimTypes.Sid).Value,
                            IsVendedor = IsVendedor(User.FindFirst(x => x.Type == ClaimTypes.AuthorizationDecision).Value)
                        };
                    }
                    else
                        return null;
                     
                }
                catch(Exception e)
                {
                    return null;
                }

            }

        }

        private bool? IsVendedor(string valor)
        {
            bool? isVendedor = null;

            if (!string.IsNullOrEmpty(valor))
            {
                bool parse;

                bool.TryParse(valor, out parse);

                isVendedor = parse;

            }

            return isVendedor;
        }


        public ConstantesWebUtil.UsuarioAutenticacaoTipo UsuarioAutenticacaoTipo
        {
            get
            {
                if (UsuarioAutenticado != null && !string.IsNullOrEmpty(UsuarioAutenticado.IdUsuario))
                    return ConstantesWebUtil.UsuarioAutenticacaoTipo.Admin;
                else if (ClienteAutenticado != null && !string.IsNullOrEmpty(ClienteAutenticado.IdCliente))
                    return ConstantesWebUtil.UsuarioAutenticacaoTipo.Cliente;
                else
                    return ConstantesWebUtil.UsuarioAutenticacaoTipo.Anonimo;
            }
        }


        #region Gambi para não replicar em todos os controllers.. depois criar um EmailHelper 
        protected string ObterRodapePadraoEmail()
        {
            string site = configuration.GetValue<string>("AppSettings:url-site");

            return $@"
                            <BR><BR>
                            <table  style='width: 100%'  cellspacing='1' cellpadding='1'><TR><TD>
                            Atenciosamente<BR>
                            Desencalha Estoque LTDA<BR>
                            <a href='{site}'>{site}</a>
                            </td></tr>
                            </div>
            ";
        }
        protected async Task EnviarEmailAsync(string corpo, string assunto, List<string> emailsPara, List<string> anexos = null)
        {
            
            //Enviando email
           await emailRepository.EnviarAsync(
                configuration.GetValue<string>("AppSettings:email-email"),
                configuration.GetValue<string>("AppSettings:email-nome"),
                emailsPara,
                null,
                configuration.GetValue<string>("AppSettings:email-copia-oculta").Split(";")?.ToList(),
                anexos, //anexos
                assunto,
                corpo,
                configuration.GetValue<string>("AppSettings:email-smtp"),
                configuration.GetValue<string>("AppSettings:email-usuario"),
                configuration.GetValue<string>("AppSettings:email-senha"),
                configuration.GetValue<int>("AppSettings:email-porta"),
                configuration.GetValue<bool>("AppSettings:email-ssl"),
                true,
                true);
        }
        #endregion

    }

}
