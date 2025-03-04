using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;

namespace StoreManager.Tests.RepositoryTests
{
    [Collection("Database Tests")]
    public class SupplierRepositoryTest : RepositoryTestBase
    {
        public SupplierRepositoryTest(IUnitOfWork unitOfWork, DatabaseFixture fixture) : base(unitOfWork, fixture)
        {
        }

        [Fact]
        public async Task Insert()
        {
            Supplier supplier = new Supplier() { CompanyName = "New Supplier", Country = "USA", City = "New York" };
            int id = (int)await _unitOfWork.SupplierRepository.InsertAsync(supplier);
            Assert.True(id > 0);
        }

        [Fact]
        public async Task Update()
        {
            Supplier? supplier = await _unitOfWork.SupplierRepository.GetByIdAsync(1);

            Assert.NotNull(supplier);

            supplier.CompanyName = $"Updated {supplier.CompanyName}";

            await _unitOfWork.SupplierRepository.UpdateAsync(supplier);
            Supplier updatedSupplier = await _unitOfWork.SupplierRepository.GetByIdAsync(supplier.Id);

            Assert.True(updatedSupplier!.CompanyName == supplier.CompanyName);
        }

        [Fact]
        public async Task Delete()
        {
            Supplier? supplier = await _unitOfWork.SupplierRepository.GetByIdAsync(2);

            Assert.NotNull(supplier);

            await _unitOfWork.SupplierRepository.DeleteAsync(supplier.Id);

            Assert.Null(await _unitOfWork.SupplierRepository.GetByIdAsync(supplier.Id));
        }

        [Fact]
        public async Task Insert_NullSupplier_ShouldFail()
        {
            Supplier? supplier = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _unitOfWork.SupplierRepository.InsertAsync(supplier));
        }

        [Fact]
        public async Task Update_NullSupplier_ShouldFail()
        {
            Supplier? supplier = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _unitOfWork.SupplierRepository.UpdateAsync(supplier));
        }

        [Fact]
        public async Task Delete_InvalidId_ShouldFail()
        {
            int invalidSupplierId = 999;

            await Assert.ThrowsAsync<ArgumentException>(() => _unitOfWork.SupplierRepository.DeleteAsync(invalidSupplierId));
        }
    }
}