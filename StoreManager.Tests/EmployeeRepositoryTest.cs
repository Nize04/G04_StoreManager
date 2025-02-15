using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;

namespace StoreManager.Tests
{
    [Collection("Database Tests")]
    public class EmployeeRepositoryTest : TestBase
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
    }
}