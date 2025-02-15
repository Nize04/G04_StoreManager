
using StoreManager.DTO.Interfaces;
using System.Linq.Expressions;

namespace StoreManager.Facade.Interfaces.Repositories
{
    public interface IRepository<T>
    {
        Task<T?> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate);
        Task<object> InsertAsync(T item);
        Task UpdateAsync(T item);
        Task DeleteAsync(object id);
    }
}