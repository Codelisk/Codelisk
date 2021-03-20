using Codelisk.Database.Daos.Interfaces;
using Codelisk.Database.Services.Interfaces;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Codelisk.Database.Daos
{
    public abstract class BaseDao<TEntity> : IDao<TEntity> where TEntity : class, new()
    {
        #region Fields

        private readonly IDbContext _dbContext;

        #endregion

        #region Constructors

        protected BaseDao(IDbContext context) => _dbContext = context;

        #endregion

        //ReaderWriterLockSlim insertSlim = new ReaderWriterLockSlim();
        #region IRepository

        public virtual async Task<int> InsertAsync(TEntity entity)
        {
            return await _dbContext.InsertOrReplaceAsync<TEntity>(entity).ConfigureAwait(false);
        }
        public virtual async Task<int> InsertOrReplaceAsync(TEntity entity)
        {
            return await _dbContext.InsertOrReplaceAsync<TEntity>(entity).ConfigureAwait(false);
        }
        public virtual async Task<int> InsertAllAsync(IEnumerable<TEntity> objects)
        {
            return await _dbContext.InsertAllAsync<TEntity>(objects).ConfigureAwait(false);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync() =>
            await _dbContext.GetCollectionAsync<TEntity>().ConfigureAwait(false);
        public virtual async Task<CreateTableResult> DropAndCreateTableAsync()
        {
            return await _dbContext.DropAndCreateTableAsync<TEntity>().ConfigureAwait(false);
        }
        public virtual async Task<TEntity> FirstOrDefaultAsync()
        {
            try
            {
                return await _dbContext.FirstOrDefaultAsync<TEntity>().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate) =>
            (await WithCollection()).Count();

        public async Task<List<TEntity>> QueryAsync(string query, params object[] args)
        {
            return await _dbContext.QueryAsync<TEntity>(query, args).ConfigureAwait(false);
        }
        public async Task<int> DeleteAsync(object primaryKey)
        {
            return await _dbContext.DeleteAsync<TEntity>(primaryKey).ConfigureAwait(false);
        }
        public async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predExpr)
        {
            var connection = await _dbContext.GetConnectionAsync<TEntity>().ConfigureAwait(false);
            var all = connection.Table<TEntity>();
            return await all.DeleteAsync(predExpr);
        }
        public async Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            return await _dbContext.GetConnectionAsync<TEntity>().ConfigureAwait(false);
        }
        #endregion

        #region Protected Methods

        protected async Task<List<TEntity>> WithCollection() =>
            await _dbContext.GetCollectionAsync<TEntity>().ConfigureAwait(false);


        #endregion
    }
}