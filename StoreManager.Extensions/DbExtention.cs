using System.Data;

namespace StoreManager.Extensions
{
    public static class DbExtensions
    {
        public static Task OpenAsync(this IDbConnection connection)
        {
            return Task.Run(() => connection.Open());
        }
        public static Task CloseAsync(this IDbConnection connection)
        {
            return Task.Run(() => connection.Close());
        }
        public static async Task<IDbTransaction> BeginTransactionAsync(this IDbConnection connection)
        {
            return await Task.Run(() =>
            {
                // Begin the transaction synchronously on a background thread
                return connection.BeginTransaction();
            });
        }
        public static async Task RollBackAsync(this IDbTransaction transaction)
        {
            await Task.Run(() =>
            {
                // Rollback the transaction synchronously on a background thread
                transaction.Rollback();
            });
        }
        public static async Task CommitAsync(this IDbTransaction transaction)
        {
            await Task.Run(() =>
            {
                // Commit the transaction synchronously on a background thread
                transaction.Commit();
            });
        }
    }
}
