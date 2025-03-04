using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;

namespace StoreManager.Tests.RepositoryTests
{
    [Collection("Database Tests")]
    public class EmployeeRepositoryTest : RepositoryTestBase
    {
        public EmployeeRepositoryTest(IUnitOfWork unitOfWork, DatabaseFixture fixture) : base(unitOfWork, fixture) { }

        [Fact]
        public async Task Insert()
        {
            Employee employee = new Employee() { FirstName = "Inserted", LastName = "Employeer" };
            int id = (int)await _unitOfWork.EmployeeRepository.InsertAsync(employee);
            Assert.True(id > 0);
        }

        [Fact]
        public async Task Update()
        {
            Employee? employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(1);

            Assert.NotNull(employee);

            employee.FirstName = $"new {employee.FirstName}";
            employee.LastName = $"new {employee.LastName}";

            await _unitOfWork.EmployeeRepository.UpdateAsync(employee);
            Employee updatedEmployee = await _unitOfWork.EmployeeRepository.GetByIdAsync(employee.Id);

            Assert.True(updatedEmployee!.FirstName == employee.FirstName && updatedEmployee.LastName == employee.LastName);
        }

        [Fact]
        public async Task Delete()
        {
            Employee? employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(2);
            Assert.NotNull(employee);

            await _unitOfWork.EmployeeRepository.DeleteAsync(employee.Id);

            Assert.Null(await _unitOfWork.EmployeeRepository.GetByIdAsync(2));
        }

        [Fact]
        public async Task Insert_NullEmployee_ShouldFail()
        {
            Employee? employee = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _unitOfWork.EmployeeRepository.InsertAsync(employee));
        }

        [Fact]
        public async Task Update_NonExistentEmployee_ShouldFail()
        {
            Employee employee = new Employee { Id = 999, FirstName = "Non-Existent", LastName = "Employee" };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _unitOfWork.EmployeeRepository.UpdateAsync(employee));
        }

        [Fact]
        public async Task Delete_NonExistentEmployee_ShouldFail()
        {
            int nonExistentEmployeeId = 999;

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _unitOfWork.EmployeeRepository.DeleteAsync(nonExistentEmployeeId));
        }
    }
}