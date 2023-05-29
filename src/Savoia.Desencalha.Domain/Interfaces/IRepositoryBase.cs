using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Domain.Interfaces
{
    public interface IRepositoryBase<T>
    {

        Task<T> GetListAsync<E>(E pk);

        Task<T> CreateAsync(T model);

        Task<long> RemoveAsync<E>(E pk);

    }
}
