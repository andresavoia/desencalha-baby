using Savoia.Desencalha.Host.WebApi.Messages.Usuario;
using Savoia.Desencalha.Host.WebApi.Validators;
using Savoia.Desencalha.Domain.Repositories;
using Savoia.Desencalha.Domain.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using static Savoia.Desencalha.Host.WebApi.Helpers.ResponseHelper;
using Savoia.Desencalha.Host.WebApi.Messages.Cliente;
using Savoia.Desencalha.Host.WebApi.Messages;
using Savoia.Desencalha.Common.Util;
using Savoia.Desencalha.Host.WebApi.Util;
using Savoia.Desencalha.Host.WebApi.Dtos;

namespace Savoia.Desencalha.Host.WebApi.Validators
{
    public class ClienteValidator : IClienteValidator
    {
        internal IClienteRepository clienteRepository;

        public ClienteValidator(IClienteRepository clienteRepository)
        {
            this.clienteRepository = clienteRepository;
        }

        public BaseResponse Validar(EnviarTokenSenhaRequest request)
        {
            if (request == null) return GetRequestIsNullResponse<BaseResponse>();
            List<MensagemSistema> mensagens = new List<MensagemSistema>();

            if (string.IsNullOrEmpty(request.Email) ||
                !StringUtil.ValidarEmail(request.Email))
            {
                mensagens.Add(new MensagemSistema() { Campo = nameof(request.Email), Mensagem = "Preencha corretamente o campo email" });
            }

            var cliente = clienteRepository.ObterPorLoginAsync(request.Email).Result;

            if (cliente == null || cliente?.CodClienteStatus != (int)ConstantesUtil.ClienteStatus.Ativo)
            {
                mensagens.Add(new MensagemSistema() {Mensagem = "Cliente não encontrado" });
            }

            if (mensagens?.Count > 0)
                return GetResponseErrorValidation<BaseResponse>(mensagens);

            return new BaseResponse();
        }

        public BaseResponse Validar(EnviarFaleConoscoRequest request)
        {
            if (request == null) return GetRequestIsNullResponse<BaseResponse>();
            List<MensagemSistema> mensagens = new List<MensagemSistema>();


            if (string.IsNullOrWhiteSpace(request.Nome))
            {
                mensagens.Add(new MensagemSistema() { Campo = nameof(request.Nome), Mensagem = "Preencha o campo Nome" });
            }

            if (string.IsNullOrWhiteSpace(request.Celular))
            {
                mensagens.Add(new MensagemSistema() { Campo = nameof(request.Nome), Mensagem = "Preencha o campo Celular" });
            }

            if (string.IsNullOrEmpty(request.Email) ||
                !StringUtil.ValidarEmail(request.Email))
            {
                mensagens.Add(new MensagemSistema() { Campo = nameof(request.Email), Mensagem = "Preencha corretamente o campo email" });
            }

            if (mensagens?.Count > 0)
                return GetResponseErrorValidation<CriarClienteResponse>(mensagens);

            return new BaseResponse();
        }


        public BaseResponse Validar(Messages.Cliente.AlterarSenhaRequest request)
        {
            if (request == null) return GetRequestIsNullResponse<CriarClienteResponse>();
            List<MensagemSistema> mensagens = new List<MensagemSistema>();

            
            if (string.IsNullOrEmpty(request.Senha) && string.IsNullOrEmpty(request.SenhaConfirmacao))
                mensagens.Add(new MensagemSistema() { Campo = nameof(request.Senha), Mensagem = "Senha inválida" });
            else
                //verificando campos de senha
                if (request.Senha != request.SenhaConfirmacao)
                mensagens.Add(new MensagemSistema() { Campo = nameof(request.Senha), Mensagem = "Senha e confirmação não são idênticas" });

            if (mensagens?.Count > 0)
                return GetResponseErrorValidation<BaseResponse>(mensagens);

            return new BaseResponse();
        }

        public CriarClienteResponse Validar(CriarClienteRequest request)
        {
            if (request == null) return GetRequestIsNullResponse<CriarClienteResponse>();
            List<MensagemSistema> mensagens = new List<MensagemSistema>();

            if (string.IsNullOrWhiteSpace(request.NomeOuRazao))
            {
                mensagens.Add(new MensagemSistema() { Campo = nameof(request.NomeOuRazao), Mensagem = "Preencha o campo Razão" });
            }


            if (string.IsNullOrWhiteSpace(request.ApelidoOuFantasia))
            {
                mensagens.Add(new MensagemSistema() { Campo = nameof(request.ApelidoOuFantasia), Mensagem = "Preencha o campo Fantasia" });
            }

            if (string.IsNullOrWhiteSpace(request.IdRamoAtividade))
            {
                mensagens.Add(new MensagemSistema() { Campo = nameof(request.IdRamoAtividade), Mensagem = "Preencha o campo Ramo de Atividade" });
            }

            if (request.TipoPessoa!=ConstantesUtil.PESSOA_TIPO_FISICA &&
                request.TipoPessoa != ConstantesUtil.PESSOA_TIPO_JURIDICA)
            {
                mensagens.Add(new MensagemSistema() { Campo = nameof(request.TipoPessoa), Mensagem = "Preencha corretamente o campo Tipo Pessoa (PF ou PJ)" });
            }

            if (request.TipoPessoa == ConstantesUtil.PESSOA_TIPO_FISICA)
            {
                var valido = NumeroUtil.ValidarCpf(request.CpfOuCnpj);
                if(!valido)
                    mensagens.Add(new MensagemSistema() { Campo = nameof(request.CpfOuCnpj), Mensagem = "Preencha corretamente o campo CPF"});
            }

            if (request.TipoPessoa == ConstantesUtil.PESSOA_TIPO_JURIDICA)
            {
                var valido = NumeroUtil.ValidarCnpj(request.CpfOuCnpj);
                if (!valido)
                    mensagens.Add(new MensagemSistema() { Campo = nameof(request.CpfOuCnpj), Mensagem = "Preencha corretamente o campo CNPJ" });
            }

            if (string.IsNullOrEmpty(request.EmailPrincipal) &&
                string.IsNullOrEmpty(request.EmailAlternativo))
            {
                var valido = NumeroUtil.ValidarCnpj(request.EmailPrincipal);
                if (!valido)
                    mensagens.Add(new MensagemSistema() { Campo = nameof(request.EmailPrincipal), Mensagem = "Preencha pelo menos um email" });
            }
            else
            {
                if(!string.IsNullOrEmpty(request.EmailPrincipal) && !StringUtil.ValidarEmail(request.EmailPrincipal))
                    mensagens.Add(new MensagemSistema() { Campo = nameof(request.EmailPrincipal), Mensagem = "Preencha corretamente o email principal" });

                if (!string.IsNullOrEmpty(request.EmailAlternativo) && !StringUtil.ValidarEmail(request.EmailAlternativo))
                    mensagens.Add(new MensagemSistema() { Campo = nameof(request.EmailAlternativo), Mensagem = "Preencha corretamente o email alternativo" });

            }

            if (string.IsNullOrWhiteSpace(request.Telefone1))
            {
                mensagens.Add(new MensagemSistema() { Campo = nameof(request.Telefone1), Mensagem = "Preencha o campo Telefone 1" });
            }

            //Validando Endereço Entrega
            ValidarEndereco(request.EnderecoCobranca, mensagens,"EnderecoCobranca");

            //Validando Endereço Entrega
            ValidarEndereco(request.EnderecoEntrega, mensagens, "EnderecoEntrega");

            if (string.IsNullOrEmpty(request.Login) || !StringUtil.ValidarEmail(request.Login))
                mensagens.Add(new MensagemSistema() { Campo = nameof(request.Login), Mensagem = "Preencha o login com email válido" });

            if (string.IsNullOrEmpty(request.Senha) && string.IsNullOrEmpty(request.SenhaConfirmar))
                mensagens.Add(new MensagemSistema() { Campo = nameof(request.Senha), Mensagem = "Senha inválida" });
            else
                //verificando campos de senha
                if (request.Senha != request.SenhaConfirmar)
                    mensagens.Add(new MensagemSistema() { Campo = nameof(request.Senha), Mensagem = "Senha e confirmação não são idênticas." });

            if (mensagens?.Count > 0)
                return GetResponseErrorValidation<CriarClienteResponse>(mensagens);

            if (clienteRepository.ConsistirAsync(null,null,request.CpfOuCnpj,request.Login).Result)
                return GetResponseErrorValidation<CriarClienteResponse>("","Cliente já cadastrado. Verifique o CNPJ, código e o login utilizado.");

            return new CriarClienteResponse();
        }

        public ManterClienteResponse Validar(ManterClienteRequest request, ConstantesWebUtil.ClienteAtualizacaoTipo tipoAtualizacao)
        {
            if (request == null) return GetRequestIsNullResponse<ManterClienteResponse>();
            List<MensagemSistema> mensagens = new List<MensagemSistema>();

            if (tipoAtualizacao.Equals(ConstantesWebUtil.ClienteAtualizacaoTipo.PorAdmin))
            {

                if (request.CodClienteStatus==null)
                {
                    mensagens.Add(new MensagemSistema() { Campo = nameof(request.CodClienteStatus), Mensagem = "Preencha o campo status" });
                }

                if (request.CodFreteTipo == null || request.CodFreteTipo == 0)
                {
                    mensagens.Add(new MensagemSistema() { Campo = nameof(request.CodFreteTipo), Mensagem = "Preencha o campo Tipo de Frete" });
                }


            }

            if (string.IsNullOrWhiteSpace(request.ApelidoOuFantasia))
            {
                mensagens.Add(new MensagemSistema() { Campo = nameof(request.ApelidoOuFantasia), Mensagem = "Preencha o campo Fantasia" });
            }

            if (string.IsNullOrWhiteSpace(request.IdRamoAtividade))
            {
                mensagens.Add(new MensagemSistema() { Campo = nameof(request.IdRamoAtividade), Mensagem = "Preencha o campo Ramo de Atividade" });
            }

            if (request.TipoPessoa != ConstantesUtil.PESSOA_TIPO_FISICA &&
                request.TipoPessoa != ConstantesUtil.PESSOA_TIPO_JURIDICA)
            {
                mensagens.Add(new MensagemSistema() { Campo = nameof(request.TipoPessoa), Mensagem = "Preencha corretamente o campo Tipo Pessoa (PF ou PJ)" });
            }

            if (string.IsNullOrEmpty(request.EmailPrincipal) &&
                string.IsNullOrEmpty(request.EmailAlternativo))
            {
                mensagens.Add(new MensagemSistema() { Campo = nameof(request.EmailPrincipal), Mensagem = "Preencha pelo menos um email" });
            }
            else
            {
                if (!string.IsNullOrEmpty(request.EmailPrincipal) && !StringUtil.ValidarEmail(request.EmailPrincipal))
                    mensagens.Add(new MensagemSistema() { Campo = nameof(request.EmailPrincipal), Mensagem = "Preencha corretamente o email principal" });

                if (!string.IsNullOrEmpty(request.EmailAlternativo) && !StringUtil.ValidarEmail(request.EmailAlternativo))
                    mensagens.Add(new MensagemSistema() { Campo = nameof(request.EmailAlternativo), Mensagem = "Preencha corretamente o email alternativo" });

            }


            if (request.TipoPessoa == ConstantesUtil.PESSOA_TIPO_FISICA)
            {
                var valido = NumeroUtil.ValidarCpf(request.CpfOuCnpj);
                if (!valido)
                    mensagens.Add(new MensagemSistema() { Campo = nameof(request.CpfOuCnpj), Mensagem = "Preencha corretamente o campo CPF" });
            }

            if (request.TipoPessoa == ConstantesUtil.PESSOA_TIPO_JURIDICA)
            {
                var valido = NumeroUtil.ValidarCnpj(request.CpfOuCnpj);
                if (!valido)
                    mensagens.Add(new MensagemSistema() { Campo = nameof(request.CpfOuCnpj), Mensagem = "Preencha corretamente o campo CNPJ" });
            }

            //Validando Endereço Entrega
            ValidarEndereco(request.EnderecoCobranca, mensagens, "EnderecoCobranca");

            //Validando Endereço Entrega
            ValidarEndereco(request.EnderecoEntrega, mensagens, "EnderecoEntrega");


            if (mensagens?.Count > 0)
                return GetResponseErrorValidation<ManterClienteResponse>(mensagens);


            if (clienteRepository.ConsistirAsync(request.IdCliente, request.CodInterno, request.CpfOuCnpj, request.Login).Result)
                return GetResponseErrorValidation<ManterClienteResponse>("", "CNPJ ou login já existente");

            return new ManterClienteResponse();
        }

        public ListarClienteResponse Validar(string codInterno, string nomeOuRazao, int ? codFreteTipo, string cpfOuCnpj, int? codClienteStatus = null)
        {
            return new ListarClienteResponse();
        }

        private void ValidarEndereco(EnderecoDto endereco, List<MensagemSistema> mensagens, string tipoEndereco)
        {
            if (endereco == null) return;

            if (string.IsNullOrEmpty(endereco?.Endereco))
                mensagens.Add(new MensagemSistema() { Campo = tipoEndereco + "_Endereco", Mensagem = "Preencha o campo Endereço" });

            if (string.IsNullOrEmpty(endereco?.Bairro))
                mensagens.Add(new MensagemSistema() { Campo = tipoEndereco + "_Bairro", Mensagem = "Preencha o campo Bairro" });

            if (string.IsNullOrEmpty(endereco?.Cep))
                mensagens.Add(new MensagemSistema() { Campo = tipoEndereco + "_Cep", Mensagem = "Preencha o campo CEP" });
            
            if (string.IsNullOrEmpty(endereco?.Cidade.Titulo) || endereco?.Cidade?.CodCidade == 0)
                mensagens.Add(new MensagemSistema() { Campo = tipoEndereco + "_Estado_Cidade_DescCidade", Mensagem = "Selecione a cidade" });

            if (string.IsNullOrEmpty(endereco?.Cidade?.Estado?.UF) || endereco?.Cidade?.Estado?.CodEstado== 0)
                mensagens.Add(new MensagemSistema() { Campo = tipoEndereco + "_Estado_CodEstado", Mensagem = "Selecione o estado" });
            
        }

        public AutenticarClienteResponse Validar(AutenticarClienteRequest request)
        {
            if (request == null) return GetRequestIsNullResponse<AutenticarClienteResponse>();
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
                return GetResponseErrorValidation<AutenticarClienteResponse>(mensagens);

            return new AutenticarClienteResponse();
        }


    }
}
