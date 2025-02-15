using System.Data;

namespace StoreManager.Tests
{
    public class DatabaseFixture : IDisposable
    {
        private readonly IDbConnection _database;
        private string? DeleteTypesCommand;

        public string[]? toCleanTypes { get; set; }
        public string[] typeWithoutIdentityId { get; set; }
        public string[]? toSetUpTypes { get; set; }

        public DatabaseFixture(IDbConnection connection)
        {
            _database = connection ?? throw new ArgumentNullException(nameof(connection));
            toCleanTypes = new string[] {"Accounts","OrderDetails","Orders","Employees","ProductsQuantity","PriceHistory",
                "Products","Categories","Suppliers","Customers"};
            typeWithoutIdentityId = new string[] { "ProductsQuantity","Accounts","OrderDetails" };
            _database.Open();
            SetUp();
        }

        public void Dispose()
        {
            //var command = _database.CreateCommand();
            //if (DeleteTypesCommand == null) DeleteTypesCommand = GetDeleteTypesCommand(toCleanTypes!);
            //if (DeleteTypesCommand != string.Empty)
            //{
            //    command.CommandText = DeleteTypesCommand;
            //    command.ExecuteNonQuery();
            //}
            //_database.Dispose();
        }
        
        private void SetUp()
        {
            var deleteCommand = _database.CreateCommand();
            if (DeleteTypesCommand == null) DeleteTypesCommand = GetDeleteTypesCommand(toCleanTypes!);
            if (DeleteTypesCommand != string.Empty)
            {
                deleteCommand.CommandText = DeleteTypesCommand;
                deleteCommand.ExecuteNonQuery();
            }

            string textCommand = "INSERT INTO Categories (Name, IsActive) VALUES('SmartPhone', 1); " +
             "INSERT INTO Categories (Name, IsActive) VALUES('Leptop', 1); " +

             "INSERT INTO Suppliers (CompanyName, Country, City) VALUES('Apple', 'USA', 'Cupertino'); " +
             "INSERT INTO Suppliers (CompanyName, Country, City) VALUES('Microsoft', 'USA', 'Redmond'); " +

             "INSERT INTO Products (CategoryId, SupplierId, Name, Price) VALUES(1,1,'Iphone 15 pro', 749.99); " +
             "INSERT INTO Products (CategoryId, SupplierId, Name, Price) VALUES(2,2,'Surface Laptop 5', 999.99); " +

             "INSERT INTO Employees (FirstName, LastName) VALUES('Vaja', 'Maisuradze'); " +
             "INSERT INTO Employees (FirstName, LastName) VALUES('Aravin', 'Maisuradze'); " +

             "INSERT INTO Customers (FullName, PhoneNumber) VALUES('Shota Rustaveli', '55555555') " +
             "INSERT INTO Customers (FullName, PhoneNumber) VALUES('Iakob Gogebashvili', '55555511') " +

            "INSERT INTO PriceHistory (ProductId, Price) VALUES(1, 749.99); " +
            "INSERT INTO PriceHistory (ProductId, Price) VALUES(2, 999.99); " +

            "INSERT INTO ProductsQuantity (Id, Quantity) VALUES(1,17); " +
            "INSERT INTO ProductsQuantity (Id, Quantity) VALUES(2,12); " +

            "INSERT INTO Orders (EmployeeId, CustomerId, Description) VALUES(1,2,'Really nice customer lets give him some discount next time')" +
            "INSERT INTO Orders (EmployeeId, CustomerId, Description) VALUES(2,1,'Really bad customer lets give him bad price next time')";

            using var transaction = _database.BeginTransaction();

            try
            {
                var command = _database.CreateCommand();

                command.CommandText = textCommand;

                command.Transaction = transaction;
                command.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private string GetDeleteTypesCommand(params string[] typeNames)
        {
            string command = string.Empty;
            foreach (string typeName in typeNames)
            {
                if (typeWithoutIdentityId.Contains(typeName))
                {
                    command += $"Delete From {typeName};";
                }
                else command += $"Delete {typeName};  DBCC CHECKIDENT('{typeName}', RESEED, 0)" + " ";
            }

            return command;
        }
    }
}