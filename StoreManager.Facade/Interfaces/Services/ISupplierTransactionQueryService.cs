using StoreManager.DTO;

public interface ISupplierTransactionQueryService
{
    Task<SupplierTransaction?> GetTransactionByIdAsync(int id);
    Task<IEnumerable<SupplierTransactionDetail>?> GetTransactionDetailsAsync(int supplierTransactionId);
}