using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;

namespace StoreManager.Tests.RepositoryTests
{
    [Collection("Database Tests")]
    public class CustomerRepositoryTest : RepositoryTestBase
    {
        public CustomerRepositoryTest(IUnitOfWork unitOfWork, DatabaseFixture fixture) : base(unitOfWork, fixture) { }

        [Fact]
        public async Task Insert()
        {
            Customer customer = new Customer() { FullName = "Inserted Customer", PhoneNumber = "5551122334" };
            int id = (int)await _unitOfWork.CustomerRepository.InsertAsync(customer);
            Assert.True(id > 0);
        }

        [Fact]
        public async Task Update()
        {
            Customer? customer = await _unitOfWork.CustomerRepository.GetByIdAsync(1);

            Assert.NotNull(customer);

            customer.FullName = $"new {customer.FullName}";

            await _unitOfWork.CustomerRepository.UpdateAsync(customer);
            Customer updatedCustomer = await _unitOfWork.CustomerRepository.GetByIdAsync(customer.Id);

            Assert.True(updatedCustomer!.FullName == customer.FullName);
        }

        [Fact]
        public async Task Delete()
        {
            Customer? customer = await _unitOfWork.CustomerRepository.GetByIdAsync(2);
            Assert.NotNull(customer);

            await _unitOfWork.CustomerRepository.DeleteAsync(customer.Id);

            Assert.Null(await _unitOfWork.CustomerRepository.GetByIdAsync(2));
        }

        [Fact]
        public async Task Insert_NullCustomer_ShouldFail()
        {
            Customer? customer = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _unitOfWork.CustomerRepository.InsertAsync(customer));
        }

        [Fact]
        public async Task Update_NullCustomer_ShouldFail()
        {
            Customer? customer = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _unitOfWork.CustomerRepository.UpdateAsync(customer));
        }

        [Fact]
        public async Task Delete_InvalidId_ShouldFail()
        {
            int invalidCustomerId = 999;

            await Assert.ThrowsAsync<ArgumentException>(() => _unitOfWork.CustomerRepository.DeleteAsync(invalidCustomerId));
        }
    }
}