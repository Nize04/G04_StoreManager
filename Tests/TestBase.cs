namespace StoreManager.Tests
{
    [CollectionDefinition("Database Tests", DisableParallelization = true)]
    public abstract class TestBase : IClassFixture<DatabaseFixture>
    {
        protected TestBase(DatabaseFixture fixture) { }
    }
}