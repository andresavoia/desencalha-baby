using Savoia.Desencalha.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Domain.Repositories
{
    public interface ICategoriaRepository
    {
        Task<CategoriaModel> ManterAsync(CategoriaModel model);

        Task<List<CategoriaModel>> ListarAsync(string idCategoria, string codInterno, string titulo, bool? ativo = null, int ? paginaAtual = null, int? numeroRegistros = null);

        Task<CategoriaModel> ObterAsync(string id);

        Task<long> ExcluirAsync(string id);

        Task<bool> ConsistirAsync(string idCategoria, string codInterno, string titulo);
    }
}
