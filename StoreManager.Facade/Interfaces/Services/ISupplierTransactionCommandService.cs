using StoreManager.DTO;

public interface ISupplierTransactionCommandService
{
    Task CreateTransactionAsync(SupplierTransaction supplierTransaction, IEnumerable<SupplierTransactionDetail> supplierTransactionDetails);
}