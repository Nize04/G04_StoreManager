using StoreManager.Extensions;
using StoreManager.Facade.Interfaces.Repositories;
using System.Data;

namespace StoreManager.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _connection;
    private IDbTransaction? _transaction;

    private readonly Lazy<ICategoryRepository> _categoryRepository;
    private readonly Lazy<IProductRepository> _productRepository;
    private readonly Lazy<IAccountRepository> _accountRepository;
    private readonly Lazy<IAccountImageRepository> _accountImageRepository;
    private readonly Lazy<IEmployeeRepository> _employeeRepository;
    private readonly Lazy<ICustomerRepository> _customerRepository;
    private readonly Lazy<ISupplierRepository> _supplierRepository;
    private readonly Lazy<ISupplierTransactionRepository> _supplierTransactionRepository;
    private readonly Lazy<ISupplierTransactionDetailRepository> _supplierTransactionDetailRepository;
    private readonly Lazy<IOrderDetailRepository> _orderDetailRepository;
    private readonly Lazy<IOrderRepository> _orderRepository;
    private readonly Lazy<IProductQuantityRepository> _productQuantityRepository;
    private readonly Lazy<IRoleRepository> _roleRepository;
    private readonly Lazy<IAccountRoleRepository> _accountRoleRepository;
    private readonly Lazy<ITokenRepository> _tokenRepository;

    public UnitOfWork(IDbConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));

        _categoryRepository = new Lazy<ICategoryRepository>(() => new CategoryRepository(connection, _transaction));
        _productRepository = new Lazy<IProductRepository>(() => new ProductRepository(connection, _transaction));
        _accountRepository = new Lazy<IAccountRepository>(() => new AccountRepository(connection, _transaction));
        _accountImageRepository = new Lazy<IAccountImageRepository>(() => new AccountImageRepository(connection, _transaction));
        _employeeRepository = new Lazy<IEmployeeRepository>(() => new Employeerepository(connection, _transaction));
        _customerRepository = new Lazy<ICustomerRepository>(() => new CustomerRepository(connection, _transaction));
        _supplierRepository = new Lazy<ISupplierRepository>(() => new SupplierRepository(connection, _transaction));
        _supplierTransactionRepository = new Lazy<ISupplierTransactionRepository>(() => new SupplierTransactionRepository(connection, _transaction));
        _supplierTransactionDetailRepository = new Lazy<ISupplierTransactionDetailRepository>(() => new SupplierTransactionDetailRepsitory(connection, _transaction));
        _orderDetailRepository = new Lazy<IOrderDetailRepository>(() => new OrderDetailRepository(connection, _transaction));
        _orderRepository = new Lazy<IOrderRepository>(() => new OrderRepository(connection, _transaction));
        _productQuantityRepository = new Lazy<IProductQuantityRepository>(() => new ProductsQuantityRepository(connection, _transaction));
        _roleRepository = new Lazy<IRoleRepository>(() => new RoleRepository(connection, _transaction));
        _accountRoleRepository = new Lazy<IAccountRoleRepository>(() => new AccountRoleRepository(connection, _transaction));
        _tokenRepository = new Lazy<ITokenRepository>(() => new TokenRepository(connection, _transaction));
    }

    public ICategoryRepository CategoryRepository => _categoryRepository.Value;

    public IProductRepository ProductRepository => _productRepository.Value;

    public IAccountRepository AccountRepository => _accountRepository.Value;

    public IAccountImageRepository AccountImageRepository => _accountImageRepository.Value;

    public IEmployeeRepository EmployeeRepository => _employeeRepository.Value;

    public ICustomerRepository CustomerRepository => _customerRepository.Value;

    public ISupplierRepository SupplierRepository => _supplierRepository.Value;

    public ISupplierTransactionRepository SupplierTransactionRepository => _supplierTransactionRepository.Value;

    public ISupplierTransactionDetailRepository SupplierTransactionDetailRepository => _supplierTransactionDetailRepository.Value;

    public IOrderDetailRepository OrderDetailRepository => _orderDetailRepository.Value;

    public IOrderRepository OrderRepository => _orderRepository.Value;

    public IProductQuantityRepository ProductQuantityRepository => _productQuantityRepository.Value;

    public IRoleRepository RoleRepository => _roleRepository.Value;

    public IAccountRoleRepository AccountRoleRepository => _accountRoleRepository.Value;

    public ITokenRepository TokenRepository => _tokenRepository.Value;

    public async Task OpenConnectionAsync()
    {
        if (_connection.State != ConnectionState.Open)
        {
            await _connection.OpenAsync();
        }
    }

    public async Task CloseConnectionAsync()
    {
        if (_connection.State != ConnectionState.Closed)
        {
            await _connection.CloseAsync();
        }
    }

    public async Task BeginTransactionAsync()
    {
        if (_transaction != null) throw new InvalidOperationException("transaction is already active");
        _transaction = await _connection.BeginTransactionAsync();
    }

    public async Task CommitAsync() => await HandleTranaction(async transaction => await transaction.CommitAsync());

    public async Task RollBackAsync() => await HandleTranaction(async transaction => await transaction.RollBackAsync());

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            _transaction?.Dispose();
            _connection.Dispose();
        }
    }

    private async Task HandleTranaction(Func<IDbTransaction, Task> handle)
    {
        if (_connection.State != ConnectionState.Open) throw new InvalidOperationException("connectio is close");
        if (_transaction == null) throw new InvalidOperationException("transaction is not active");
        await handle(_transaction);
    }

    ~UnitOfWork() => Dispose(false);
}