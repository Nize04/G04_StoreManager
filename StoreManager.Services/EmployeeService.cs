using Microsoft.Extensions.Logging;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using StoreManager.Facade.Interfaces.Services;

namespace StoreManager.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(IUnitOfWork unitOfWork, ILogger<EmployeeService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> AddEmployeeAsync(Employee employee)
        {
            _logger.LogInformation("adding employee in db: {@Employee}", employee);

            await _unitOfWork.OpenConnectionAsync();
            try
            {
                int employeeId = (int)await _unitOfWork.EmployeeRepository.InsertAsync(employee);
                _logger.LogInformation("Employee added successfully with EmployeeId: {EmployeeId}", employeeId);

                return employeeId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding employee {@Employee}", employee);
                throw;
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            _logger.LogInformation("Attempting to retrieve employee with ID: {EmployeeId}", id);

            await _unitOfWork.OpenConnectionAsync();
            try
            {
                var employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(id);

                if (employee == null)
                {
                    _logger.LogWarning("No employee found with ID: {EmployeeId}", id);
                    return null;
                }

                _logger.LogInformation("Retrieved Employee with ID: {EmployeeId}", id);
                return employee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving EmployeeId: {EmployeeId}", id);
                throw;
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }
    }
}