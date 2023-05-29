using Savoia.Desencalha.Host.WebApi.Messages.RamoAtividade;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Host.WebApi.Validators
{
    public interface IRamoAtividadeValidator
    {
        ManterRamoAtividadeResponse Validar(ManterRamoAtividadeRequest request);

        ListarRamoAtividadeResponse Validar(string titulo, bool ? ativo);

        ExcluirRamoAtividadeResponse Validar(string idRamoAtividade);

    }
}
