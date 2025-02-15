namespace StoreManager.Facade.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        IAccountRepository AccountRepository { get; }
        IAccountImageRepository AccountImageRepository { get; }
        ICustomerRepository CustomerRepository { get; }
        IEmployeeRepository EmployeeRepository { get; }
        ISupplierRepository SupplierRepository { get; }
        ISupplierTransactionRepository SupplierTransactionRepository { get; }
        ISupplierTransactionDetailRepository SupplierTransactionDetailRepository { get; }
        IOrderDetailRepository OrderDetailRepository { get; }
        IOrderRepository OrderRepository { get; }
        IProductQuantityRepository ProductQuantityRepository { get; }
        IRoleRepository RoleRepository { get; }
        IAccountRoleRepository AccountRoleRepository { get;}
        ITokenRepository TokenRepository { get; }
        Task OpenConnectionAsync();
        Task CloseConnectionAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollBackAsync();
    }
}