using Savoia.Desencalha.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Domain.Repositories
{
    public interface IRamoAtividadeRepository
    {
        Task<RamoAtividadeModel> ManterAsync(RamoAtividadeModel model);

        Task<List<RamoAtividadeModel>> ListarAsync(string idRamoAtividade, string titulo, bool? ativo = null, int ? paginaAtual = null, int? numeroRegistros = null);

        Task<RamoAtividadeModel> ObterAsync(string id);

        Task<long> ExcluirAsync(string id);

        Task<bool> ConsistirAsync(string idRamoAtividade, string titulo);
    }
}
