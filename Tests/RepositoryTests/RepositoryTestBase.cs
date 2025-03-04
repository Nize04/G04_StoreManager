using StoreManager.Facade.Interfaces.Repositories;

namespace StoreManager.Tests.RepositoryTests
{
    public class RepositoryTestBase : TestBase
    {
        protected readonly IUnitOfWork _unitOfWork;

        public RepositoryTestBase(IUnitOfWork unitOfWork, DatabaseFixture databaseFixture) : base(databaseFixture)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
    }
}