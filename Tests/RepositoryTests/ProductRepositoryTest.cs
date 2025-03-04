using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;

namespace StoreManager.Tests.RepositoryTests
{
    [Collection("Database Tests")]
    public class ProductRepositoryTests : RepositoryTestBase
    {
        public ProductRepositoryTests(IUnitOfWork unitOfWork, DatabaseFixture fixture) : base(unitOfWork, fixture)
        {

        }

        [Fact]
        public async Task Insert()
        {
            Assert.True((int)await _unitOfWork.ProductRepository.InsertAsync
                (new Product() { Name = "Iphone 16 Pro", Price = 1100, CategoryId = 1, SupplierId = 1 }) > 0);
        }

        [Fact]
        public async Task Update()
        {
            Product? product = await _unitOfWork.ProductRepository.GetByIdAsync(2);

            Assert.NotNull(product);
            product.Name = "updated" + product.Name;
            await _unitOfWork.ProductRepository.UpdateAsync(product);

            Product? updatedProduct = await _unitOfWork.ProductRepository.GetByIdAsync(product.Id);

            Assert.True(updatedProduct.Name == product.Name);
        }

        [Fact]
        public async Task Delete()
        {
            Product? product = await _unitOfWork.ProductRepository.GetByIdAsync(1);
            Assert.NotNull(product);

            await _unitOfWork.ProductRepository.DeleteAsync(product.Id);
            Assert.Null(await _unitOfWork.ProductRepository.GetByIdAsync(product.Id));
        }

        [Fact]
        public async Task Insert_InvalidPrice_ShouldFail()
        {

            Product product = new Product { Name = "Invalid Product", Price = -10 };

            await Assert.ThrowsAsync<ArgumentException>(() => _unitOfWork.ProductRepository.InsertAsync(product));
        }

        [Fact]
        public async Task Update_NullProduct_ShouldFail()
        {
            Product? product = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _unitOfWork.ProductRepository.UpdateAsync(product));
        }

        [Fact]
        public async Task Delete_InvalidId_ShouldFail()
        {
            int invalidProductId = 999;

            await Assert.ThrowsAsync<ArgumentException>(() => _unitOfWork.ProductRepository.DeleteAsync(invalidProductId));
        }

    }
}