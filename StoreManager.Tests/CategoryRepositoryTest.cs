using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;

namespace StoreManager.Tests
{
    [Collection("Database Tests")]
    public class CategoryRepositoryTest : TestBase
    {
        public CategoryRepositoryTest(IUnitOfWork unitOfWork, DatabaseFixture fixture) : base(unitOfWork, fixture) { }

        [Fact]
        public async Task Insert()
        {
            Category category = new Category() { Name = "Inserted Category" };
            int id = (int)await _unitOfWork.CategoryRepository.InsertAsync(category);
            Assert.True(id > 0);
        }

        [Fact]
        public async Task Update()
        {
            Category? category = await _unitOfWork.CategoryRepository.GetByIdAsync(1);

            Assert.NotNull(category);

            category.Name = $"New {category.Name}";

            await _unitOfWork.CategoryRepository.UpdateAsync(category);
            Category updatedCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(category.Id);

            Assert.True(updatedCategory.Name == category.Name);
        }

        [Fact]
        public async Task Delete()
        {
            Category? category = await _unitOfWork.CategoryRepository.GetByIdAsync(2);

            Assert.NotNull(category);

            await _unitOfWork.CategoryRepository.DeleteAsync(category.Id);

            Assert.Null(await _unitOfWork.CategoryRepository.GetByIdAsync(category.Id));
        }
    }
}