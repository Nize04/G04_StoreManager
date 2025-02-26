using StoreManager.DTO;

namespace StoreManager.Facade.Interfaces.Services
{
    public interface ISupplierTransactionService
    {
        Task CreateTransactionAsync(SupplierTransaction supplierTransaction, IEnumerable<SupplierTransactionDetail> supplierTransactionDetails);
        Task<IEnumerable<SupplierTransactionDetail>?> GetTransactionDetailsAsync(int supplierTransactionId);
        Task<SupplierTransaction?> GetTransactionByIdAsync(int id);
    }
}