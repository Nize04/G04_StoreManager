using StoreManager.DTO;

public interface ISupplierQueryService
{
    Task<Supplier?> GetSupplierById(int id);
    Task<IEnumerable<Supplier>> GetSuppliersByCountryAsync(string country);
}