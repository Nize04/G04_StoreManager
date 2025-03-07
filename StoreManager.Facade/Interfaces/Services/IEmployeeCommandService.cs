using StoreManager.DTO;

public interface IEmployeeCommandService
{
    Task<int> AddEmployeeAsync(Employee employee);
}