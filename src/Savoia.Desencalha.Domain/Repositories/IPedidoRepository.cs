using Savoia.Desencalha.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Domain.Repositories
{
    public interface IPedidoRepository
    {
        Task<PedidoModel> ManterAsync(PedidoModel model, bool? criarPedido);

        Task<int?> ObterPedidoUltimoAsync();

        Task<long> ObterTotalStatusAsync(int codPedidoStatus);

        Task RemoverCarrinhoAsync(string idCliente);

        Task RemoverCarrinhoPorTokenAsync(string idCliente);

        Task<List<PedidoModel>> ListarAsync(int? idPedido = null, int? codPedidoStatus = null, string cliente = null, DateTime? dataCadastroInicial = null, DateTime? dataCadastroFinal = null, string idProduto = null, string idCliente = null, string cnpjCliente = null, int? paginaAtual = null, int? numeroRegistros = null, string ordenacao = null, bool ordenacaoAsc = true);

        Task<PedidoModel> ObterAsync(int idPedido);
        
        Task<bool> ConsistirAsync(int idPedido);

        Task<CarrinhoModel> ObterCarrinhoAsync(string idCliente);

        Task<bool> ManterCarrinhoAsync(CarrinhoModel model, bool? carrinhoPorToken =false);

        Task<CarrinhoModel> ObterCarrinhoPorTokenAsync(string token);

        Task<bool> AlterarValidadeCarrinhoAsync(string token);

        Task<PedidoVendedorModel> ManterPedidoVendedorAsync(PedidoVendedorModel model, bool? criarPedido);

        Task<List<PedidoVendedorModel>> ListarPedidoVendedorAsync(string idCliente, int? idPedido = null, int? codPedidoVendedorStatus = null, int? paginaAtual = null, int? numeroRegistros = null, string ordenacao = null, bool ordenacaoAsc = true, bool? isVendedor = null);

        Task<List<PedidoModel>> ListarInformacaoPedidoBaseAsync(List<int> idsPedido, int? paginaAtual = null, int? numeroRegistros = null, string ordenacao = null, bool ordenacaoAsc = true, bool? isVendedor = null, string idClienteComprador = null);

        Task<List<PedidoVendedorModel>> ObterPedidoVendedorAsync(string idCliente, int idPedido, bool? IsVendedor = null);

    }
}
