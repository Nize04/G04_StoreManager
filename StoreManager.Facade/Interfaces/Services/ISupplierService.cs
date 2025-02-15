
using StoreManager.DTO;

namespace StoreManager.Facade.Interfaces.Services
{
    public interface ISupplierService
    {
        Task<int> AddSupplierAsync(Supplier supplier);
        Task<IEnumerable<Supplier>> GetSuppliersByCountryAsync(string country);
        Task<Supplier?> GetSupplierById(int id);
    }
}
