using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;

namespace StoreManager.Tests.RepositoryTests
{
    [Collection("Database Tests")]
    public class AccountRepositoryTest : RepositoryTestBase
    {
        public AccountRepositoryTest(IUnitOfWork unitOfWork, DatabaseFixture fixture) : base(unitOfWork, fixture)
        {
        }

        [Fact]
        public async Task Insert()
        {
            Account account = new Account() {Id = 4, Email = "inserteduser@example.com", Password = "password123" };
            await _unitOfWork.AccountRepository.InsertAsync(account);
            Assert.NotNull(_unitOfWork.AccountRepository.GetByIdAsync(account.Id));
        }

        [Fact]
        public async Task Update()
        {
            Account? account = await _unitOfWork.AccountRepository.GetByIdAsync(1);

            Assert.NotNull(account);

            account.Email = $"updated_{account.Email}";

            await _unitOfWork.AccountRepository.UpdateAsync(account);
            Account updatedAccount = await _unitOfWork.AccountRepository.GetByIdAsync(account.Id);

            Assert.True(updatedAccount.Email == account.Email);
        }

        [Fact]
        public async Task Delete()
        {
            Account? account = await _unitOfWork.AccountRepository.GetByIdAsync(2);

            Assert.NotNull(account);

            await _unitOfWork.AccountRepository.DeleteAsync(account.Id);

            Assert.Null(await _unitOfWork.AccountRepository.GetByIdAsync(account.Id));
        }

        [Fact]
        public async Task Authenticate_ValidCredentials_ShouldSucceed()
        {
            bool isAuthenticated = await _unitOfWork.AccountRepository.AuthenticateAsync("existed@gmail.com", "password123");
            Assert.True(isAuthenticated);
        }

        [Fact]
        public async Task Insert_NullAccount_ShouldFail()
        {
            Account account = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _unitOfWork.AccountRepository.InsertAsync(account));
        }

        [Fact]
        public async Task Update_NullAccount_ShouldFail()
        {
            Account account = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _unitOfWork.AccountRepository.UpdateAsync(account));
        }

        [Fact]
        public async Task Delete_InvalidId_ShouldFail()
        {
            int invalidAccountId = 999;

            await Assert.ThrowsAsync<ArgumentException>(() => _unitOfWork.AccountRepository.DeleteAsync(invalidAccountId));
        }

        [Fact]
        public async Task Authenticate_InvalidCredentials_ShouldFail()
        {
            bool isAuthenticated = await _unitOfWork.AccountRepository.AuthenticateAsync("nonexistentuser@example.com", "wrongpassword");
            Assert.False(isAuthenticated);
        }
    }
}