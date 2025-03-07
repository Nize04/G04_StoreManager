using Microsoft.Extensions.Logging;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using StoreManager.Facade.Interfaces.Services;

namespace StoreManager.Services
{
    public class EmployeeQueryService : IEmployeeQueryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EmployeeQueryService> _logger;

        public EmployeeQueryService(IUnitOfWork unitOfWork, ILogger<EmployeeQueryService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
