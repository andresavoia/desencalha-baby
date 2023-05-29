using Savoia.Desencalha.Host.WebApi.Messages.Usuario;
using Savoia.Desencalha.Host.WebApi.Validators;
using Savoia.Desencalha.Domain.Repositories;
using Savoia.Desencalha.Domain.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using static Savoia.Desencalha.Host.WebApi.Helpers.ResponseHelper;
using Savoia.Desencalha.Host.WebApi.Messages;
using Savoia.Desencalha.Common.Util;
using System.Text.RegularExpressions;

namespace Savoia.Desencalha.Host.WebApi.Validators
{
    public class UsuarioValidator : IUsuarioValidator
    {
        internal IUsuarioRepository usuarioRepository;

        public UsuarioValidator(IUsuarioRepository usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository;
        }

        public ManterUsuarioResponse Validar(ManterUsuarioRequest request)
        {
            if (request == null) return GetRequestIsNullResponse<ManterUsuarioResponse>();

            if (string.IsNullOrWhiteSpace(request.Nome))
            {
                return GetResponseErrorValidation<ManterUsuarioResponse>("Nome", "Preencha o campo Nome");
            }

            if (request.CodUsuarioTipo!= ConstantesUtil.USUARIO_TIPO_ADM &&
                request.CodUsuarioTipo != ConstantesUtil.USUARIO_TIPO_CLI_ADM)
            {
                return GetResponseErrorValidation<ManterUsuarioResponse>("Tipo", "Preencha corretamente o campo Tipo");
            }

            if (usuarioRepository.ListarAsync()?.Result?.Where(x=>x.Nome==request.Nome && x.CodUsuarioTipo == request.CodUsuarioTipo && x.IdUsuario!=request.IdUsuario)?.FirstOrDefault()!=null)
                return GetResponseErrorValidation<ManterUsuarioResponse>("",MessageResource.COMUM_REGISTRO_EXISTENTE);

            return new ManterUsuarioResponse();
        }
        public AutenticarUsuarioResponse Validar(AutenticarUsuarioRequest request)
        {
            if (request == null) return GetRequestIsNullResponse<AutenticarUsuarioResponse>();
            List<MensagemSistema> mensagens = new List<MensagemSistema>();


            if (string.IsNullOrWhiteSpace(request.Login))
            {
                mensagens.Add(new MensagemSistema() { Campo = nameof(request.Login), Mensagem = "Preencha o campo Login" });
            }

            if (string.IsNullOrWhiteSpace(request.Senha))
            {
                mensagens.Add(new MensagemSistema() { Campo = nameof(request.Login), Mensagem = "Preencha o campo Senha" });
            }

            if (mensagens?.Count > 0)
                return GetResponseErrorValidation<AutenticarUsuarioResponse>(mensagens);

            return new AutenticarUsuarioResponse();
        }


        public BaseResponse Validar(AlterarSenhaUsuarioRequest request)
        {
            if (request == null) return GetRequestIsNullResponse<ManterUsuarioResponse>();

            if (string.IsNullOrWhiteSpace(request.SenhaNova) ||
                string.IsNullOrWhiteSpace(request.SenhaConfirmacao)
                ||
                string.IsNullOrWhiteSpace(request.SenhaAtual))
            {
                return GetResponseErrorValidation<BaseResponse>("SenhaAtual", "Preencha todos os campos corretamente");
            }

            if (request.SenhaNova != request.SenhaConfirmacao)
            {
                return GetResponseErrorValidation<BaseResponse>("SenhaConfirmacao", "Nova senha e confirmação não coincidem");
            }

            string strRegex = @"^(?=.{6,})(?=.*?[A-Z])(?=.*?[a-z])(?=.*?\d)(?=.*?\W).*$";
            Regex re = new Regex(strRegex);

            if (!re.IsMatch(request.SenhaNova))
            {
                return GetResponseErrorValidation<BaseResponse>(nameof(AlterarSenhaUsuarioRequest.SenhaAtual), "Deve conter uma letra maiúscula e um caracter especial, além de ter no minimo 6 caracteres.");
            }



            return new BaseResponse();
        }

    }
}
