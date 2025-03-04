using System.Data;
using System.Text.Json;

namespace StoreManager.Tests
{
    public class DatabaseFixture : IDisposable
    {
        private readonly IDbConnection _database;

        private readonly string _deleteTypesCommand;
        private readonly string[]? _toCleanTypes;
        private readonly string[] typeWithoutIdentityId;
        private readonly string _sqlCommandsFilePath;

        public DatabaseFixture(IDbConnection connection)
        {
            _database = connection ?? throw new ArgumentNullException(nameof(connection));
            _toCleanTypes = new[] {"SupplierTransactionDetails","SupplierTransactions","Accounts","OrderDetails","Orders","Employees","ProductsQuantity","PriceHistory",
                "Products","Categories","Suppliers","Customers"};
            typeWithoutIdentityId = new[] { "ProductsQuantity", "Accounts", "OrderDetails", "SupplierTransactionDetails" };
            _deleteTypesCommand = GetDeleteTypesCommand(_toCleanTypes);

            _sqlCommandsFilePath = Environment.GetEnvironmentVariable("SQL_COMMANDS_FOR_DBFIXTURE_FILE_PATH")
                ?? throw new InvalidOperationException("Environment variable 'SQL_COMMANDS_FOR_DBFIXTURE_FILE_PATH' is not set.");
            SetUp();
        }

        public void Dispose()
        {
            var command = _database.CreateCommand();
            _database.Open();

            if (_deleteTypesCommand != string.Empty)
            {
                command.CommandText = _deleteTypesCommand;
                command.ExecuteNonQuery();
            }
            _database.Close();
            _database.Dispose();
        }

        private void SetUp()
        {
            _database.Open();
            var deleteCommand = _database.CreateCommand();
            deleteCommand.CommandText = _deleteTypesCommand;
            deleteCommand.ExecuteNonQuery();

            string jsonAddress = _sqlCommandsFilePath;

            if (File.Exists(jsonAddress))
            {
                string jsonContent = File.ReadAllText(jsonAddress);
                var sqlCommands = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(jsonContent);

                using var transaction = _database.BeginTransaction();

                try
                {
                    foreach (var commandText in sqlCommands.Values.SelectMany(list => list))
                    {
                        var command = _database.CreateCommand();
                        command.CommandText = commandText;
                        command.Transaction = transaction;
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    _database.Close();
                }
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