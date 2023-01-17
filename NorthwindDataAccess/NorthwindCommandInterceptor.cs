using LinqToDB.Common;
using LinqToDB.Interceptors;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace NorthwindDataAccess
{
    public class NorthwindCommandInterceptor : DbCommandInterceptor, ICommandInterceptor
    {
        private readonly ILogger<NorthwindCommandInterceptor> logger;

        public NorthwindCommandInterceptor(ILogger<NorthwindCommandInterceptor> logger)
        {
            this.logger = logger;
        }

        public override InterceptionResult<DbCommand> CommandCreating(CommandCorrelatedEventData eventData, InterceptionResult<DbCommand> result)
        {
            return base.CommandCreating(eventData, result);
        }

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, Microsoft.EntityFrameworkCore.Diagnostics.CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            return base.ReaderExecuting(command, eventData, result);
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, Microsoft.EntityFrameworkCore.Diagnostics.CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        public async override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, Microsoft.EntityFrameworkCore.Diagnostics.CommandEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var res = await base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);

            return res;
        }

        public override InterceptionResult<int> NonQueryExecuting(DbCommand command, Microsoft.EntityFrameworkCore.Diagnostics.CommandEventData eventData, InterceptionResult<int> result)
        {
            return base.NonQueryExecuting(command, eventData, result);
        }

        public async override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, Microsoft.EntityFrameworkCore.Diagnostics.CommandEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken = default)
        {
            var res = await base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
            return res;
        }

        public override InterceptionResult<object> ScalarExecuting(DbCommand command, Microsoft.EntityFrameworkCore.Diagnostics.CommandEventData eventData, InterceptionResult<object> result)
        {
            return base.ScalarExecuting(command, eventData, result);
        }

        public DbCommand CommandInitialized(LinqToDB.Interceptors.CommandEventData eventData, DbCommand command)
        {
            return command;
        }

        public Option<object> ExecuteScalar(LinqToDB.Interceptors.CommandEventData eventData, DbCommand command, Option<object> result)
        {
            return result;
        }

        public Task<Option<object>> ExecuteScalarAsync(LinqToDB.Interceptors.CommandEventData eventData, DbCommand command, Option<object> result, CancellationToken cancellationToken)
        {
            return Task.FromResult(result);
        }

        public Option<int> ExecuteNonQuery(LinqToDB.Interceptors.CommandEventData eventData, DbCommand command, Option<int> result)
        {
            return result;
        }

        public Task<Option<int>> ExecuteNonQueryAsync(LinqToDB.Interceptors.CommandEventData eventData, DbCommand command, Option<int> result, CancellationToken cancellationToken)
        {
            return Task.FromResult(result);
        }

        public Option<DbDataReader> ExecuteReader(LinqToDB.Interceptors.CommandEventData eventData, DbCommand command, CommandBehavior commandBehavior, Option<DbDataReader> result)
        {
            return result;
        }

        public Task<Option<DbDataReader>> ExecuteReaderAsync(LinqToDB.Interceptors.CommandEventData eventData, DbCommand command, CommandBehavior commandBehavior, Option<DbDataReader> result, CancellationToken cancellationToken)
        {
            return Task.FromResult(result);
        }

        public void AfterExecuteReader(LinqToDB.Interceptors.CommandEventData eventData, DbCommand command, CommandBehavior commandBehavior, DbDataReader dataReader)
        {
        }

        public void BeforeReaderDispose(LinqToDB.Interceptors.CommandEventData eventData, DbCommand command, DbDataReader dataReader)
        {
        }

        public Task BeforeReaderDisposeAsync(LinqToDB.Interceptors.CommandEventData eventData, DbCommand command, DbDataReader dataReader)
        {
            return Task.CompletedTask;
        }
    }
}
