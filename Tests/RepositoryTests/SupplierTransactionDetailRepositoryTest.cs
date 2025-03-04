using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;

namespace StoreManager.Tests.RepositoryTests
{
    [Collection("Database Tests")]
    public class SupplierTransactionDetailRepositoryTest : RepositoryTestBase
    {
        public SupplierTransactionDetailRepositoryTest(IUnitOfWork unitOfWork, DatabaseFixture fixture) : base(unitOfWork, fixture)
        {
        }

        [Fact]
        public async Task Insert()
        {
            SupplierTransactionDetail supplierTransactionDetail = new SupplierTransactionDetail()
            {
                SupplierTransactionId = 2,
                ProductId = 4,
                Price = 100,
                Quantity = 10,
                ProductionDate = DateTime.UtcNow
            };
            int id = (int)await _unitOfWork.SupplierTransactionDetailRepository.InsertAsync(supplierTransactionDetail);
            Assert.True(id > 0);
        }

        [Fact]
        public async Task Update()
        {
            SupplierTransactionDetail? supplierTransactionDetail = await _unitOfWork.SupplierTransactionDetailRepository.GetByIdAsync(1, 3);

            Assert.NotNull(supplierTransactionDetail);

            supplierTransactionDetail.Quantity += 5;

            await _unitOfWork.SupplierTransactionDetailRepository.UpdateAsync(supplierTransactionDetail);
            SupplierTransactionDetail updatedSupplierTransactionDetail = await _unitOfWork.SupplierTransactionDetailRepository.GetByIdAsync(supplierTransactionDetail.SupplierTransactionId, supplierTransactionDetail.ProductId);

            Assert.True(updatedSupplierTransactionDetail!.Quantity == supplierTransactionDetail.Quantity);
        }

        [Fact]
        public async Task Delete()
        {
            SupplierTransactionDetail? supplierTransactionDetail = await _unitOfWork.SupplierTransactionDetailRepository.GetByIdAsync(1, 1);

            Assert.NotNull(supplierTransactionDetail);

            await _unitOfWork.SupplierTransactionDetailRepository.DeleteAsync(supplierTransactionDetail.SupplierTransactionId, supplierTransactionDetail.ProductId);

            Assert.Null(await _unitOfWork.SupplierTransactionDetailRepository.GetByIdAsync(supplierTransactionDetail.SupplierTransactionId, supplierTransactionDetail.ProductId));
        }

        [Fact]
        public async Task Insert_NullSupplierTransactionDetail_ShouldFail()
        {
            SupplierTransactionDetail? supplierTransactionDetail = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _unitOfWork.SupplierTransactionDetailRepository.InsertAsync(supplierTransactionDetail));
        }

        [Fact]
        public async Task Update_NullSupplierTransactionDetail_ShouldFail()
        {
            SupplierTransactionDetail? supplierTransactionDetail = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _unitOfWork.SupplierTransactionDetailRepository.UpdateAsync(supplierTransactionDetail));
        }

        [Fact]
        public async Task Delete_InvalidId_ShouldFail()
        {
            int invalidSupplierTransactionId = 999;
            int invalidProductId = 999;

            await Assert.ThrowsAsync<ArgumentException>(() => _unitOfWork.SupplierTransactionDetailRepository.DeleteAsync(invalidSupplierTransactionId, invalidProductId));
        }
    }
}