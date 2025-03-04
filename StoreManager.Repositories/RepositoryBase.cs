using Dapper;
using StoreManager.Extensions;
using StoreManager.Facade.Interfaces.Repositories;
using System.Data;
using System.Linq.Expressions;

namespace StoreManager.Repositories
{
    internal abstract class RepositoryBase<T> : IRepository<T>
    {
        protected IDbConnection _connection;
        protected IDbTransaction? _transaction;
        private readonly string _pluralTypeName;
        private readonly string _typeName;
        protected string[]? _manyToManyKeys;
        protected readonly string[] _unwantedPropertiesForInsert;
        protected readonly string[] _unwantedPropertiesForUpdate;

        protected RepositoryBase(IDbConnection connection, IDbTransaction? transaction)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _typeName = typeof(T).Name;
            _transaction = transaction;
            _pluralTypeName = _typeName.Pluralize();
            _manyToManyKeys = null;
            _unwantedPropertiesForInsert = UnwantedPropertiesForInsert;
            _unwantedPropertiesForUpdate = UnwantedPropertiesForUpdate;
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate) =>
            await _connection.QueryAsync<T>($"SELECT * FROM {_pluralTypeName} WHERE {predicate.GetWhereDbQuery()}");

        public async Task<T?> GetByIdAsync(params object[] keys)
        {
            var keyParameters = new DynamicParameters();

            if (_manyToManyKeys != null && _manyToManyKeys.Length == keys.Length)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    keyParameters.Add($"@{_manyToManyKeys[i]}", keys[i]);
                }
            }
            else keyParameters.Add("Id", keys[0]);

            return await _connection.QueryFirstOrDefaultAsync<T>($"sp_Get{_typeName}", keyParameters, commandType: CommandType.StoredProcedure, transaction: _transaction);
        }

        public async Task DeleteAsync(params object[] keys)
        {
            var keyParameters = new DynamicParameters();

            if (_manyToManyKeys != null && _manyToManyKeys.Length == keys.Length)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    keyParameters.Add($"@{_manyToManyKeys[i]}", keys[i]);
                }
            }
            else keyParameters.Add("Id", keys[0]);

            await _connection.ExecuteAsync($"sp_Delete{_typeName}", keyParameters, commandType: CommandType.StoredProcedure, transaction: _transaction);
        }

        public async virtual Task<object> InsertAsync(T item)
        {
            string sqlCommand = $"sp_Insert{_typeName}";
            DynamicParameters parameters = new DynamicParameters(GetParameters(item, _unwantedPropertiesForInsert));
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _connection.ExecuteAsync(sqlCommand, parameters, commandType: CommandType.StoredProcedure, transaction: _transaction);

            return parameters.Get<int>("@Id");
        }

        public async virtual Task UpdateAsync(T item)
        {
            string sqlCommand = $"sp_Update{_typeName}";
            DynamicParameters parameters = new DynamicParameters(
                GetParameters(item, _unwantedPropertiesForUpdate));
            await _connection.ExecuteAsync(
                sqlCommand,
                parameters,
                commandType: CommandType.StoredProcedure,
                transaction: _transaction
            );
        }


        protected virtual string[] UnwantedPropertiesForInsert => new[] { "Id", "CreateDate", "UpdateDate", "IsActive" };
        protected virtual string[] UnwantedPropertiesForUpdate => new[] { "CreateDate", "UpdateDate", "IsActive" };

        protected IEnumerable<KeyValuePair<string, object>> GetParameters(T dto, params string[] ignoredParameters)
        {
            Dictionary<string, object> parameters = new();
            foreach (var prop in dto!.GetType().GetProperties())
            {
                if (!ignoredParameters.Contains(prop.Name))
                {
                    parameters.Add(prop.Name, prop.GetValue(dto)!);
                }
            }

            return parameters;
        }
    }
}