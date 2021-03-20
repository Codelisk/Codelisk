using Codelisk.Database.Services.Interfaces;
using Prism.Magician;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codelisk.Database.Base
{
    [RegisterSingleton(typeof(IDbContext))]
    public abstract class SQLiteDatabase : BaseDatabase, IDbContext
    {
        public SQLiteDatabase() : base(new Xamarin.Essentials.Implementation.FileSystemImplementation())
        {
        }
        #region Helper
        public bool DeleteDatabaseByPath()
        {
            try
            {
                System.IO.File.Delete(DatabaseConnection.DatabasePath);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public async Task<List<TEntity>> GetCollectionAsync<TEntity>() where TEntity : class, new()
        {
            var databaseConnection = await GetDatabaseConnection<TEntity>().ConfigureAwait(false);
            await CreateTableAsync<TEntity>().ConfigureAwait(false);
            var result = await AttemptAndRetry(async () => databaseConnection.Table<TEntity>()).ConfigureAwait(false);
            return await result.ToListAsync();
        }
        public async Task<CreateTableResult> DropAndCreateTableAsync<TEntity>() where TEntity : class, new()
        {
            var databaseConnection = await GetDatabaseConnection<TEntity>().ConfigureAwait(false);
            await AttemptAndRetry(() => databaseConnection.DropTableAsync<TEntity>()).ConfigureAwait(false);

            var result = await CreateTableAsync<TEntity>().ConfigureAwait(false);
            return result;
        }
        private async Task<CreateTableResult> CreateTableAsync<TEntity>() where TEntity : class, new()
        {
            var databaseConnection = await GetDatabaseConnection<TEntity>().ConfigureAwait(false);
            return await AttemptAndRetry(() => databaseConnection.CreateTableAsync<TEntity>()).ConfigureAwait(false);
        }
        public async Task<int> InsertOrReplaceAsync<TEntity>(object obj)
        {
            var databaseConnection = await GetDatabaseConnection<TEntity>().ConfigureAwait(false);
            return await AttemptAndRetry<int>(async () => await databaseConnection.InsertOrReplaceAsync(obj)).ConfigureAwait(false);
        }
        public async Task<int> InsertAsync<TEntity>(object obj)
        {
            var databaseConnection = await GetDatabaseConnection<TEntity>().ConfigureAwait(false);
            return await AttemptAndRetry<int>(async () => await databaseConnection.InsertAsync(obj)).ConfigureAwait(false);
        }
        public async Task<int> InsertAllAsync<TEntity>(IEnumerable<TEntity> objs) where TEntity : class, new()
        {
            var databaseConnection = await GetDatabaseConnection<TEntity>().ConfigureAwait(false);
            await CreateTableAsync<TEntity>().ConfigureAwait(false);
            return await AttemptAndRetry<int>(async () => await databaseConnection.InsertAllAsync(objs)).ConfigureAwait(false);
        }
        public async Task<TEntity> FirstOrDefaultAsync<TEntity>() where TEntity : class, new()
        {
            try
            {
                var coll = await GetCollectionAsync<TEntity>().ConfigureAwait(false);
                var result = coll.FirstOrDefault();
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public async Task<List<TEntity>> QueryAsync<TEntity>(string query, params object[] args) where TEntity : class, new()
        {
            var databaseConnection = await GetDatabaseConnection<TEntity>().ConfigureAwait(false);
            return await AttemptAndRetry<List<TEntity>>(async () => await databaseConnection.QueryAsync<TEntity>(query, args)).ConfigureAwait(false);
        }
        public async Task<int> DeleteAsync<TEntity>(object primaryKey)
        {
            var databaseConnection = await GetDatabaseConnection<TEntity>().ConfigureAwait(false);
            return await AttemptAndRetry<int>(async () => await databaseConnection.DeleteAsync<int>(primaryKey)).ConfigureAwait(false);
        }
        public async ValueTask<SQLiteAsyncConnection> GetConnectionAsync<TEntity>()
        {
            return await GetDatabaseConnection<TEntity>().ConfigureAwait(false);
        }
        public async override Task<int> DeleteAllData<TEntity>()
        {
            var databaseConnection = await GetDatabaseConnection<TEntity>().ConfigureAwait(false);
            return await AttemptAndRetry(() => databaseConnection.DeleteAllAsync<TEntity>()).ConfigureAwait(false);
        }

        public void ClearAllTables()
        {
            this.DropAllTables();
            CreateAllTables();
        }
        #endregion

        //var connection = this.DatabaseConnection.GetConnection();
        //connection.DropTable<Abo>();
        public abstract void DropAllTables();

        //var connection = this.DatabaseConnection.GetConnection();
        //connection.CreateTable<Abo>();
        public abstract bool CreateAllTables();
    }
}