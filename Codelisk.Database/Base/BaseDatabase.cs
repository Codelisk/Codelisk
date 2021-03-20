using Prism.Magician;
using AsyncAwaitBestPractices;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codelisk.Database.Services.Interfaces;
using System.IO;
using Polly;
using Xamarin.Essentials.Interfaces;

namespace Codelisk.Database.Base
{
    public abstract class BaseDatabase
    {
        protected BaseDatabase(IFileSystem fileSystem)
        {
            DatabasePath = Path.Combine(fileSystem.AppDataDirectory, "sybosDB.db3");
            DatabaseConnection = new SQLiteAsyncConnection(DatabasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
        }

        public string DatabasePath { get; }
        public SQLiteAsyncConnection DatabaseConnection { get; }

        public abstract Task<int> DeleteAllData<TEntity>();

        protected static Task<T> AttemptAndRetry<T>(Func<Task<T>> action, int numRetries = 12)
        {
            return Policy.Handle<SQLiteException>().WaitAndRetryAsync(numRetries, pollyRetryAttempt).ExecuteAsync(action);

            static TimeSpan pollyRetryAttempt(int attemptNumber) => TimeSpan.FromMilliseconds(Math.Pow(2, attemptNumber));
        }


        protected async ValueTask<SQLiteAsyncConnection> GetDatabaseConnection<T>()
        {
            if (!DatabaseConnection.TableMappings.Any(x => x.MappedType == typeof(T)))
            {
                await DatabaseConnection.EnableWriteAheadLoggingAsync().ConfigureAwait(false);

                try
                {
                    await DatabaseConnection.CreateTablesAsync(CreateFlags.None, typeof(T)).ConfigureAwait(false);
                }
                catch (SQLiteException e) when (e.Message.IndexOf("PRIMARY KEY", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    await DatabaseConnection.DropTableAsync(DatabaseConnection.TableMappings.First(x => x.MappedType == typeof(T)));
                    await DatabaseConnection.CreateTablesAsync(CreateFlags.None, typeof(T)).ConfigureAwait(false);
                }
            }

            return DatabaseConnection;
        }
    }
}