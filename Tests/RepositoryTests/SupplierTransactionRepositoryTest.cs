using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;

namespace StoreManager.Tests.RepositoryTests
{
    [Collection("Database Tests")]
    public class SupplierTransactionRepositoryTest : RepositoryTestBase
    {
        public SupplierTransactionRepositoryTest(IUnitOfWork unitOfWork, DatabaseFixture fixture) : base(unitOfWork, fixture)
        {
        }

        [Fact]
        public async Task Insert()
        {
            SupplierTransaction supplierTransaction = new SupplierTransaction()
            {
                SupplierId = 1,
                EmployeeId = 1,
                Description = "Shesrulebulia"
            };
            int id = (int)await _unitOfWork.SupplierTransactionRepository.InsertAsync(supplierTransaction);
            Assert.True(id > 0);
        }

        [Fact]
        public async Task Update()
        {
            SupplierTransaction? supplierTransaction = await _unitOfWork.SupplierTransactionRepository.GetByIdAsync(1);

            Assert.NotNull(supplierTransaction);

            supplierTransaction.Description = $"Updated {supplierTransaction.Description}";

            await _unitOfWork.SupplierTransactionRepository.UpdateAsync(supplierTransaction);
            SupplierTransaction updatedSupplierTransaction = await _unitOfWork.SupplierTransactionRepository.GetByIdAsync(supplierTransaction.Id);

            Assert.True(updatedSupplierTransaction!.Description == supplierTransaction.Description);
        }

        [Fact]
        public async Task Delete()
        {
            SupplierTransaction? supplierTransaction = await _unitOfWork.SupplierTransactionRepository.GetByIdAsync(2);

            Assert.NotNull(supplierTransaction);

            await _unitOfWork.SupplierTransactionRepository.DeleteAsync(supplierTransaction.Id);

            Assert.Null(await _unitOfWork.SupplierTransactionRepository.GetByIdAsync(supplierTransaction.Id));
        }

        [Fact]
        public async Task Insert_NullSupplierTransaction_ShouldFail()
        {
            SupplierTransaction? supplierTransaction = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _unitOfWork.SupplierTransactionRepository.InsertAsync(supplierTransaction));
        }

        [Fact]
        public async Task Update_NullSupplierTransaction_ShouldFail()
        {
            SupplierTransaction? supplierTransaction = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _unitOfWork.SupplierTransactionRepository.UpdateAsync(supplierTransaction));
        }

        [Fact]
        public async Task Delete_InvalidId_ShouldFail()
        {
            int invalidSupplierTransactionId = 999;

            await Assert.ThrowsAsync<ArgumentException>(() => _unitOfWork.SupplierTransactionRepository.DeleteAsync(invalidSupplierTransactionId));
        }
    }
}