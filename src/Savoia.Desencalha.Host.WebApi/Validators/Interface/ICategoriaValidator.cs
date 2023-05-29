using Savoia.Desencalha.Host.WebApi.Messages.Categoria;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Host.WebApi.Validators
{
    public interface ICategoriaValidator
    {
        ManterCategoriaResponse Validar(ManterCategoriaRequest request);

        ListarCategoriaResponse Validar(string codInterno, string titulo, bool ? ativo);

        ExcluirCategoriaResponse Validar(string idCategoria);

    }
}
