using StoreManager.DTO;

public interface ISupplierCommandService
{
    Task<int> AddSupplierAsync(Supplier supplier);
}