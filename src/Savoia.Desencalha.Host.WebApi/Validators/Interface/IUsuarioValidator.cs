using Savoia.Desencalha.Host.WebApi.Messages;
using Savoia.Desencalha.Host.WebApi.Messages.Usuario;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Host.WebApi.Validators
{
    public interface IUsuarioValidator
    {
        ManterUsuarioResponse Validar(ManterUsuarioRequest request);

        AutenticarUsuarioResponse Validar(AutenticarUsuarioRequest request);

        BaseResponse Validar(AlterarSenhaUsuarioRequest request);
    }
}
