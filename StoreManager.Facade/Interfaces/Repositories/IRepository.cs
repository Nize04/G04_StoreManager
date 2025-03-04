using System.Linq.Expressions;

namespace StoreManager.Facade.Interfaces.Repositories
{
    public interface IRepository<T>
    {
        Task<T?> GetByIdAsync(params object[] keys);
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate);
        Task<object> InsertAsync(T item);
        Task UpdateAsync(T item);
        Task DeleteAsync(params object[] keys);
    }
}