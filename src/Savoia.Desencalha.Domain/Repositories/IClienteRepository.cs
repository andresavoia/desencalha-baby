using Savoia.Desencalha.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Domain.Repositories
{
    public interface IClienteRepository
    {
        Task<ClienteModel> ManterAsync(ClienteModel model);

        Task<ClienteModel> ObterPorLoginAsync(string login);

        Task<List<ClienteModel>> ListarAsync(string codInterno, string nomeOuRazao, int ?codFreteTipo, string cpfOuCnpj, int? codClienteStatus = null , int? paginaAtual = null, int? numeroRegistros = null);

        Task<long> ObterTotalStatusAsync(int codClienteStatus);

        Task<ClienteModel> ObterAsync(string id);
         
        Task<long> ExcluirAsync(string id);

        Task<bool> ConsistirAsync(string idCliente, string codInterno, string cpfOuCnpj, string login);

        Task<bool> ConsistirAsync(string idRamoAtividade);
    }
}
