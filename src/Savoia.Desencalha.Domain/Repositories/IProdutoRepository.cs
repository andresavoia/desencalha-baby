using Savoia.Desencalha.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Domain.Repositories
{
    public interface IProdutoRepository
    {
        Task<ProdutoModel> ManterAsync(ProdutoModel model);

        Task AtualizarEstoqueAsync(string idProduto, int qtBaixar);

        Task<List<ProdutoModel>> ListarAsync(string codInterno, string titulo, string idCategoria = null,
                                            bool? ativo = null, bool? promocao = null, bool? lancamento = null,
                                            string ordenacao = null, int? paginaAtual = null, int? numeroRegistros = null, 
                                            string? IdCliente = null, bool? webAppAdmin = null, List<string> categorias = null);
        Task<List<ProdutoModel>> ListarPorRamoAtividadeAsync(string idRamoAtividade, string ordenacao = null, int? paginaAtual = null, int? numeroRegistros = null,
                                                         string? IdCliente = null);

        Task<List<ProdutoModel>> ListarAsync(List<string> ids);

        Task<ProdutoModel> ObterAsync(string id);

        Task<long> ExcluirAsync(string id);

        Task<bool> ConsistirAsync(string idProduto, string idCategoria, string codInterno, string titulo, string idCliente);

        Task<List<ProdutoModel>> ListarAtivosAsync(string titulo, string idCategoria = null, List<string> categorias = null,
                                                   string? IdCliente = null, string? idRamoAtividade = null, string? ordenacao = null, int? paginaAtual = null, int? numeroRegistros = null);

    }
}
