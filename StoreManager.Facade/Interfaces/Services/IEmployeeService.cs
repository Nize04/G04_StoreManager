using StoreManager.DTO;

namespace StoreManager.Facade.Interfaces.Services
{
    public interface IEmployeeService
    {
        Task<int> AddEmployeeAsync(Employee employee);
        Task<Employee?> GetEmployeeByIdAsync(int id);
    }
}
