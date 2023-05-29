using Savoia.Desencalha.Host.WebApi.Messages.Pedido;
using System;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Host.WebApi.Validators
{
    public interface IPedidoValidator
    {
        ManterPedidoResponse Validar(ManterPedidoRequest request);

        CriarPedidoResponse Validar(CriarPedidoRequest request, string idCliente);

        ListarPedidoResponse Validar(int ? idPedido = null ,int? codPedidoStatus = null, string cliente = null, DateTime? dataCadastroInicial = null, DateTime? dataCadastroFinal = null, string cnpjCliente = null);

        ObterPedidoResponse Validar(int id);

        ObterCarrinhoResponse ValidarCarrinho(string idCliente);

        ManterCarrinhoResponse ValidarCarrinhoManter(ManterCarrinhoRequest request);

        CriarCarrinhoResponse ValidarCarrinhoCriar(CriarCarrinhoRequest request);

        ExcluirAnexoResponse Validar(ExcluirAnexoRequest request);
    }
}
