using Savoia.Desencalha.Host.WebApi.Messages.Frete;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Host.WebApi.Validators
{
    public interface IFreteValidator
    {
        ListarFreteEstadoResponse Validar(string idFrete);

    }
}
