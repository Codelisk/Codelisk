using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Codelisk.Database.Daos.Interfaces
{
    public interface IDao<TEntity> where TEntity : class, new()
    {
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
        Task<int> DeleteAsync(object primaryKey);
        Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predExpr);
        Task<CreateTableResult> DropAndCreateTableAsync();
        Task<TEntity> FirstOrDefaultAsync();
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<int> InsertAllAsync(IEnumerable<TEntity> objects);
        Task<int> InsertAsync(TEntity entity);
        Task<int> InsertOrReplaceAsync(TEntity entity);
        Task<List<TEntity>> QueryAsync(string query, params object[] args);
    }
}
