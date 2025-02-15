using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;

namespace StoreManager.Tests
{
    [Collection("Database Tests")]
    public class ProductRepositoryTests : TestBase
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

            _unitOfWork.ProductRepository.DeleteAsync(product.Id);
            Assert.Null(_unitOfWork.ProductRepository.GetByIdAsync(1));
        }
    }
}