using buying_order_server.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace buying_order_server.Contracts
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetLastAsync();
        Task<T> GetByIdAsync(object id);
        Task<bool> CreateAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(object id);
        Task<bool> ExistAsync(object id);
    }
}
