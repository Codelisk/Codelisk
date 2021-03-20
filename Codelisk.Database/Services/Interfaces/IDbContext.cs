using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codelisk.Database.Services.Interfaces
{
    public interface IDbContext
    {
        bool CreateAllTables();
        string DatabasePath { get; }
        Task<int> DeleteAsync<TEntity>(object primaryKey);
        bool DeleteDatabaseByPath();
        Task<CreateTableResult> DropAndCreateTableAsync<TEntity>() where TEntity : class, new();
        Task<TEntity> FirstOrDefaultAsync<TEntity>() where TEntity : class, new();
        Task<List<TEntity>> GetCollectionAsync<TEntity>() where TEntity : class, new();
        ValueTask<SQLiteAsyncConnection> GetConnectionAsync<TEntity>();
        Task<int> InsertAllAsync<TEntity>(IEnumerable<TEntity> objs) where TEntity : class, new();
        Task<int> InsertAsync<TEntity>(object obj);
        Task<int> InsertOrReplaceAsync<TEntity>(object obj);
        Task<List<TEntity>> QueryAsync<TEntity>(string query, params object[] args) where TEntity : class, new();
        void ClearAllTables();
    }
}
