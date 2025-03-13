using Microsoft.Extensions.Logging;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using StoreManager.Facade.Interfaces.Services;

namespace StoreManager.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RoleService> _logger;

        public RoleService(IUnitOfWork unitOfWork, ILogger<RoleService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> AddRoleAsync(Role role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            try
            {
                _logger.LogInformation("Adding a new role: {RoleName}", role.RoleName);

                int roleId = (int)await _unitOfWork.RoleRepository.InsertAsync(role);
                _logger.LogInformation("Role added successfully with ID: {RoleId}", roleId);

                return roleId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the role: {RoleName}", role.RoleName);
                throw;
            }
        }

        public async Task AssignRoleToAccountAsync(AccountRole accountRole)
        {
            if (accountRole == null)
                throw new ArgumentNullException(nameof(accountRole));

            try
            {
                _logger.LogInformation("Assigning RoleId: {RoleId} to AccountId: {AccountId}", accountRole.RoleId, accountRole.AccountId);

                await _unitOfWork.AccountRoleRepository.InsertAsync(accountRole);
                _logger.LogInformation("RoleId: {RoleId} successfully assigned to AccountId: {AccountId}.", accountRole.RoleId, accountRole.AccountId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while assigning RoleId: {RoleId} to AccountId: {AccountId}.", accountRole.RoleId, accountRole.AccountId);
                throw;
            }
        }

        public async Task<IEnumerable<Role>> GetRolesByAccountIdAsync(int accountId)
        {
            try
            {
                _logger.LogInformation("Retrieving roles for AccountId: {AccountId}", accountId);

                var roles = await _unitOfWork.AccountRoleRepository.GetRolesByAccountId(accountId);

                if (!roles.Any())
                {
                    _logger.LogWarning("No roles found for AccountId: {AccountId}", accountId);
                }
                else
                {
                    _logger.LogInformation("Roles retrieved successfully for AccountId: {AccountId}", accountId);
                }

                return roles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving roles for AccountId: {AccountId}", accountId);
                throw;
            }
        }
    }
}
