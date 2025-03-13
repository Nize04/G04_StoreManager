using Microsoft.Extensions.Logging;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;

namespace StoreManager.Services
{
    public class EmployeeCommandService : IEmployeeCommandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EmployeeCommandService> _logger;

        public EmployeeCommandService(IUnitOfWork unitOfWork, ILogger<EmployeeCommandService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> AddEmployeeAsync(Employee employee)
        {
            _logger.LogInformation("adding employee in db: {@Employee}", employee);

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
        }
    }
}
