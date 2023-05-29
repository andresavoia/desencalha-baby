using Savoia.Desencalha.Host.WebApi.Messages.Produto;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Host.WebApi.Validators
{
    public interface IProdutoValidator
    {
        ManterProdutoResponse Validar(ManterProdutoRequest request);

        ManterProdutoPrecoResponse Validar(ManterProdutoPrecoRequest request);

        ListarProdutoResponse Validar(string codInterno, string titulo, string idCategoria = null,
                                     bool? ativo = null, bool? promocao = null, bool? lancamento = null,
                                     string ? idCliente = null);

        ListarProdutoResponse Validar(string titulo, string idCategoria = null,
                                        string? idCliente = null);
    }
}
