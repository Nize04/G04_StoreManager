using StoreManager.Facade.Interfaces.Repositories;

namespace StoreManager.Tests
{

    [CollectionDefinition("Database Tests", DisableParallelization = true)]
    public abstract class TestBase : IClassFixture<DatabaseFixture>
    {
        protected readonly DatabaseFixture _fixture;
        protected readonly IUnitOfWork _unitOfWork;

        public TestBase(IUnitOfWork unitOfWork, DatabaseFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
    }
}