using LinqToDB.Data.DbCommandProcessor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace NorthwindDataAccess
{
    public class NorthwindLinq2DbCommandProcessor : IDbCommandProcessor
    {
        private readonly ILogger<NorthwindLinq2DbCommandProcessor> logger;

        public NorthwindLinq2DbCommandProcessor(ILogger<NorthwindLinq2DbCommandProcessor> logger)
        {
            this.logger = logger;
        }

        public int ExecuteNonQuery(DbCommand command)
        {
            return command.ExecuteNonQuery();
        }

        public Task<int> ExecuteNonQueryAsync(DbCommand command, CancellationToken cancellationToken)
        {
            return command.ExecuteNonQueryAsync();
        }

        public DbDataReader ExecuteReader(DbCommand command, CommandBehavior commandBehavior)
        {
            return command.ExecuteReader();
        }

        public Task<DbDataReader> ExecuteReaderAsync(DbCommand command, CommandBehavior commandBehavior, CancellationToken cancellationToken)
        {
            return command.ExecuteReaderAsync();
        }

        public object ExecuteScalar(DbCommand command)
        {
            return command.ExecuteScalar();
        }

        public Task<object> ExecuteScalarAsync(DbCommand command, CancellationToken cancellationToken)
        {
            return command.ExecuteScalarAsync();
        }

        public static void RegisterProcessor(IServiceProvider container)
        {
            NorthwindLinq2DbCommandProcessor processor = 
                container.GetService<NorthwindLinq2DbCommandProcessor>();

            DbCommandProcessorExtensions.Instance = processor;
        }
    }
}
