using Savoia.Desencalha.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Domain.Repositories
{
    public interface IUsuarioRepository
    {
        Task<UsuarioModel> ManterAsync(UsuarioModel model);

        Task<List<UsuarioModel>> ListarAsync(int? paginaAtual = null, int? numeroRegistros = null);

        Task<UsuarioModel> ObterAsync(string id);

        Task<UsuarioModel> ObterPorLoginAsync(string id);

        Task<long> ExcluirAsync(string id);

        Task<UsuarioModel> ObterAsync(string login, string senha);
    }
}
