using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;

namespace StoreManager.Tests.RepositoryTests
{
    [Collection("Database Tests")]
    public class CategoryRepositoryTest : RepositoryTestBase
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

        [Fact]
        public async Task Insert_NullCategory_ShouldFail()
        {
            Category? category = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _unitOfWork.CategoryRepository.InsertAsync(category));
        }

        [Fact]
        public async Task Update_NonExistentCategory_ShouldFail()
        {
            Category category = new Category { Id = 999, Name = "Non-Existent Category" };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _unitOfWork.CategoryRepository.UpdateAsync(category));
        }

        [Fact]
        public async Task Delete_NonExistentCategory_ShouldFail()
        {
            int nonExistentCategoryId = 999;

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _unitOfWork.CategoryRepository.DeleteAsync(nonExistentCategoryId));
        }
    }
}