using Savoia.Desencalha.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Domain.Repositories
{
    public interface IFreteEstadoRepository
    {
        Task<FreteEstadoModel> ManterAsync(FreteEstadoModel model);

        Task<List<FreteEstadoModel>> ListarAsync(string idCliente);

    }
}
