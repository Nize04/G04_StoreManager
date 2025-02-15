using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using System.Data;

namespace StoreManager.Repositories
{
    internal class Employeerepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public Employeerepository(IDbConnection connection, IDbTransaction? dbTransaction) : base(connection, dbTransaction) { }
    }
}