using Savoia.Desencalha.Host.WebApi.Messages;
using Savoia.Desencalha.Host.WebApi.Messages.Cliente;
using Savoia.Desencalha.Host.WebApi.Util;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Host.WebApi.Validators
{
    public interface IClienteValidator
    {
        CriarClienteResponse Validar(CriarClienteRequest request);

        BaseResponse Validar(AlterarSenhaRequest request);
        
        BaseResponse Validar(EnviarTokenSenhaRequest request);

        BaseResponse Validar(EnviarFaleConoscoRequest request);

        AutenticarClienteResponse Validar(AutenticarClienteRequest request);

        ManterClienteResponse Validar(ManterClienteRequest request, ConstantesWebUtil.ClienteAtualizacaoTipo tipoAtualizacao);

        ListarClienteResponse Validar(string codInterno, string nomeOuRazao, int ? codFreteTipo, string cpfOuCnpj, int? codClienteStatus = null);
    }
}
