using StoreManager.DTO;

public interface IEmployeeQueryService
{
    Task<Employee?> GetEmployeeByIdAsync(int id);
}