using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Dapper.Encapsulated
{
    public interface IDapperConnection : IDisposable
    {
        DbConnection GetUnderlyingConnection();
        Task<T> QueryFirstAsync<T>(ISqlQuery<T> query);
        Task<T?> QueryFirstOrDefaultAsync<T>(ISqlQuery<T> query) where T : class;
        Task<T> QuerySingleAsync<T>(ISqlQuery<T> query);
        Task<T?> QuerySingleOrDefaultAsync<T>(ISqlQuery<T> query) where T : class;
        Task<IEnumerable<T>> QueryAsync<T>(ISqlQuery<T> query, CancellationToken cancellationToken);
        Task<IEnumerable<TResult>> QueryAsync<TFirst, TSecond, TResult>(ISqlQueryMap<TFirst, TSecond, TResult> query);
        Task<T> QueryMultipleAsync<T>(ISqlQueryMultiple<T> query, CancellationToken cancellationToken);
        Task<Stream?> QueryOpenDbStreamAsync(ISqlQuery<Stream?> query, CancellationToken cancellationToken);
        Task<int> ExecuteAsync(ISqlQuery<int> query, CancellationToken cancellationToken);
        Task ExecuteStoredProcAsync(string scheme, string storedProcedureName, object? arguments);
        Task<IEnumerable<T>> ExecuteStoredProcAsync<T>(string scheme, string storedProcedureName, object? arguments);

        Task<IEnumerable<T>> QueryByAsync<T>(Expression<Func<T, object?>> expression, object value,
            CancellationToken cancellationToken) where T : class;

        Task<IEnumerable<T>> GetAllAsync<T>(CancellationToken cancellationToken) where T : class;
    }

    public class DapperConnection : IDapperConnection
    {
        private List<IDisposable>? disposables = null;

        private DbConnection connection;
        private IDapperCache cache;
        private int? commandTimeout;

        public DbConnection GetUnderlyingConnection()
        {
            return connection;
        }

        internal void SetDependencies(DbConnection connection, IDapperCache cache, int? commandTimeout)
        {
            this.connection = connection;
            this.cache = cache;
            this.commandTimeout = commandTimeout;
        }

        protected virtual bool TryGetFromCache<T>(ISqlQuery<T> query, out object result)
        {
            if (query is ICacheable cacheable)
            {
                return cache.TryGetFromCache(cacheable, out result);
            }

            result = null!;
            return false;
        }

        protected virtual void TrySetInCache<T>(ISqlQuery<T> query, object? result)
        {
            if (result == null)
            {
                return;
            }

            if (query is ICacheable cacheable)
            {
                cache.TrySetInCache(cacheable, result);
            }
        }


        /// <summary>
        /// Executes a single row query, returning the data typed as per T
        /// </summary>
        public async Task<T> QueryFirstAsync<T>(ISqlQuery<T> query)
        {
            if (TryGetFromCache(query, out object cachedResult))
                return (T) cachedResult;

            DapperAttributeMapping.EnsureAssemblyIsAdded<T>();

            var stopwatch = Stopwatch.StartNew();

            var result = await connection.QueryFirstAsync<T>(query.Sql,
                query.Arguments,
                transaction: null, /* Set from outside if used */
                commandTimeout: QueryUtilities.GetCommandTimeout(query, commandTimeout),
                commandType: QueryUtilities.GetCommandType(query));

            TrySetInCache(query, result);

            ReportOnQueryExecuted(query, connection, stopwatch);

            return result;
        }


        /// <summary>
        /// Executes a single row query, returning the data typed as per T
        /// </summary>
        public async Task<T?> QueryFirstOrDefaultAsync<T>(ISqlQuery<T> query)
            where T : class
        {
            if (TryGetFromCache(query, out object cachedResult))
                return (T) cachedResult;

            DapperAttributeMapping.EnsureAssemblyIsAdded<T>();

            var stopwatch = Stopwatch.StartNew();

            var result = await connection.QueryFirstOrDefaultAsync<T>(query.Sql,
                query.Arguments,
                transaction: null, /* Set from outside if used */
                commandTimeout: QueryUtilities.GetCommandTimeout(query, commandTimeout),
                commandType: QueryUtilities.GetCommandType(query));

            TrySetInCache(query, result);

            ReportOnQueryExecuted(query, connection, stopwatch);

            return result;
        }

        /// <summary>
        /// Executes a single row query, returning the data typed as per T
        /// </summary>
        public async Task<T> QuerySingleAsync<T>(ISqlQuery<T> query)
        {
            if (TryGetFromCache(query, out object cachedResult))
                return (T) cachedResult;

            DapperAttributeMapping.EnsureAssemblyIsAdded<T>();

            var stopwatch = Stopwatch.StartNew();

            var result = await connection.QuerySingleAsync<T>(query.Sql,
                query.Arguments,
                transaction: null, /* Set from outside if used */
                commandTimeout: QueryUtilities.GetCommandTimeout(query, commandTimeout),
                commandType: QueryUtilities.GetCommandType(query));

            TrySetInCache(query, result);

            ReportOnQueryExecuted(query, connection, stopwatch);

            return result;
        }

        /// <summary>
        /// Executes a single row query, returning the data typed as per T
        /// </summary>
        public async Task<T?> QuerySingleOrDefaultAsync<T>(ISqlQuery<T> query)
            where T : class
        {
            if (TryGetFromCache(query, out object cachedResult))
                return (T) cachedResult;

            DapperAttributeMapping.EnsureAssemblyIsAdded<T>();

            var stopwatch = Stopwatch.StartNew();

            var result = await connection.QuerySingleOrDefaultAsync<T>(query.Sql,
                query.Arguments,
                transaction: null, /* Set from outside if used */
                commandTimeout: QueryUtilities.GetCommandTimeout(query, commandTimeout),
                commandType: QueryUtilities.GetCommandType(query));

            TrySetInCache(query, result);

            ReportOnQueryExecuted(query, connection, stopwatch);

            return result;
        }

        /// <summary>
        /// Executes a query, returning the data typed as per T
        /// </summary>
        public async Task<IEnumerable<T>> QueryAsync<T>(ISqlQuery<T> query,
            CancellationToken cancellationToken)
        {
            if (TryGetFromCache(query, out object cachedResult))
                return (IEnumerable<T>) cachedResult;

            DapperAttributeMapping.EnsureAssemblyIsAdded<T>();

            var stopwatch = Stopwatch.StartNew();

            var result = await connection.QueryAsync<T>(new CommandDefinition(
                query.Sql,
                query.Arguments,
                commandType: QueryUtilities.GetCommandType(query),
                flags: QueryUtilities.GetIsBufferedFlag(query),
                cancellationToken: cancellationToken));

            TrySetInCache(query, result);

            ReportOnQueryExecuted(query, connection, stopwatch);

            return result;
        }

        /// <summary>
        /// Executes a query, returning the data typed as per T
        /// </summary>
        public async Task<IEnumerable<TResult>> QueryAsync<TFirst, TSecond, TResult>(
            ISqlQueryMap<TFirst, TSecond, TResult> query)
        {
            if (TryGetFromCache(query, out object cachedResult))
                return (IEnumerable<TResult>) cachedResult;

            DapperAttributeMapping.EnsureAssemblyIsAdded<TResult>();

            var stopwatch = Stopwatch.StartNew();

            var result = await connection.QueryAsync(
                query.Sql,
                query.MapFunc,
                query.Arguments,
                transaction: null, /* Set from outside if used */
                commandTimeout: QueryUtilities.GetCommandTimeout(query, commandTimeout),
                commandType: QueryUtilities.GetCommandType(query),
                buffered: QueryUtilities.GetIsBuffered(query));

            TrySetInCache(query, result);

            ReportOnQueryExecuted(query, connection, stopwatch);

            return result;
        }

        /// <summary>
        /// Executes a query, returning the data typed as per T
        /// </summary>
        public async Task<IEnumerable<TResult>> QueryAsync<TFirst, TSecond, TThird, TResult>(
            ISqlQueryMap<TFirst, TSecond, TThird, TResult> query)
        {
            if (TryGetFromCache(query, out object cachedResult))
                return (IEnumerable<TResult>) cachedResult;

            DapperAttributeMapping.EnsureAssemblyIsAdded<TResult>();

            var stopwatch = Stopwatch.StartNew();

            var result = await connection.QueryAsync(
                query.Sql,
                query.MapFunc,
                query.Arguments,
                transaction: null, /* Set from outside if used */
                commandTimeout: QueryUtilities.GetCommandTimeout(query, commandTimeout),
                commandType: QueryUtilities.GetCommandType(query),
                buffered: QueryUtilities.GetIsBuffered(query));

            TrySetInCache(query, result);

            ReportOnQueryExecuted(query, connection, stopwatch);

            return result;
        }

        /// <summary>
        /// Executes a command that returns multiple result sets.
        /// </summary>
        public async Task<T> QueryMultipleAsync<T>(ISqlQueryMultiple<T> query, CancellationToken cancellationToken)
        {
            T result;

            if (TryGetFromCache(query, out object cachedResult))
                return (T)cachedResult;

            DapperAttributeMapping.EnsureAssemblyIsAdded<T>();

            var stopwatch = Stopwatch.StartNew();

            using (var gridReader = await connection.QueryMultipleAsync(new CommandDefinition(query.Sql,
                parameters: query.Arguments,
                commandTimeout: QueryUtilities.GetCommandTimeout(query, commandTimeout),
                commandType: QueryUtilities.GetCommandType(query),
                cancellationToken: cancellationToken)))
            {
                result = await query.Map(gridReader, cancellationToken);
            }

            TrySetInCache(query, result);

            ReportOnQueryExecuted(query, connection, stopwatch);

            return result;
        }

        /// <summary>
        /// Executes a command that returns a database stream object from index 0 of the data reader.
        /// See: // https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sqlclient-streaming-support

        /// NOTE: You must call this on a Dapper connection created anew each time using this operation since can only 
        ///       perform one such operation on a connection a the time.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="disposables">Need to inject an instance of this that is made by the container, so resources are disposed properly.</param>
        /// <param name="cancellationToken"></param>
        public async Task<Stream?> QueryOpenDbStreamAsync(ISqlQuery<Stream?> query, CancellationToken cancellationToken)
        {
            if (query is ICacheable)
                throw new Exception(
                    $"Should not cache SQL streams (browser responsibility). Please remove '{nameof(ICacheable)}' attribute from '{query.GetType().Name}'.");

            var stopwatch = Stopwatch.StartNew();

            //var command = new SqlCommand(query.Sql, (SqlConnection)Connection);
            var command = connection.CreateCommand();
            command.Connection = connection;
            command.CommandText = query.Sql;

            AddForDisposal(command);

            // Add parameters
            foreach (var property in query.Arguments.GetType().GetProperties())
            {
                var parameterName = property.Name;
                var value = property.GetValue(query.Arguments);

                var parameter = command.CreateParameter();
                parameter.ParameterName = parameterName;
                parameter.Value = value;

                command.Parameters.Add(parameter);
            }

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync(cancellationToken);

            // The reader needs to be executed with the SequentialAccess behavior to enable network streaming
            // Otherwise ReadAsync will buffer the entire BLOB into memory which can cause scalability issues or even OutOfMemoryExceptions
            var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken);
            AddForDisposal(reader);

            if (!await reader.ReadAsync(cancellationToken))
                return null;

            if (await reader.IsDBNullAsync(0, cancellationToken))
                return null;

            var stream = reader.GetStream(0);
            AddForDisposal(stream);

            ReportOnQueryExecuted(query, connection, stopwatch);

            return stream;
        }

        private void AddForDisposal(IDisposable disposable)
        {
            disposables ??= new List<IDisposable>();
            disposables.Add(disposable);
        }

        /// <summary>
        /// Executes a statement asynchronously.
        /// </summary>
        public Task<int> ExecuteAsync(ISqlQuery<int> query, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var result = connection.ExecuteAsync(new CommandDefinition(query.Sql,
                parameters: query.Arguments,
                commandTimeout: QueryUtilities.GetCommandTimeout(query, commandTimeout),
                commandType: QueryUtilities.GetCommandType(query),
                cancellationToken: cancellationToken));

            ReportOnQueryExecuted(query, connection, stopwatch);

            return result;
        }

        /// <summary>
        /// Executes a Stored Procedure without a return type.
        /// Arguments example: new { paramName = paramValue }
        /// </summary>
        public async Task ExecuteStoredProcAsync(
            string scheme,
            string storedProcedureName,
            object? arguments)
        {
            await connection.QueryAsync(
                $"{scheme}.{storedProcedureName}",
                arguments,
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Executes a Stored Procedure with a return type.
        /// Arguments example: new { paramName = paramValue }
        /// </summary>
        public async Task<IEnumerable<T>> ExecuteStoredProcAsync<T>(
            string scheme,
            string storedProcedureName,
            object? arguments)
        {
            DapperAttributeMapping.EnsureAssemblyIsAdded<T>();

            var result = await connection.QueryAsync<T>(
                $"{scheme}.{storedProcedureName}",
                arguments,
                commandType: CommandType.StoredProcedure);

            return result;
        }




        public async Task<IEnumerable<T>> QueryByAsync<T>(Expression<Func<T, object?>> expression, object value,
            CancellationToken cancellationToken)
            where T : class
        {
            if (value == null)
                return Enumerable.Empty<T>();

            DapperAttributeMapping.EnsureAssemblyIsAdded<T>();

            var query = new GetByColumnExpressionQuery<T>(expression, value);

            var stopwatch = Stopwatch.StartNew();

            var item = await connection.QueryAsync<T>(new CommandDefinition(
                commandText: query.Sql,
                commandType: CommandType.Text,
                parameters: query.Arguments,
                commandTimeout: commandTimeout,
                cancellationToken: cancellationToken));

            ReportOnQueryExecuted(query, connection, stopwatch);

            return item;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(CancellationToken cancellationToken)
            where T : class
        {
            DapperAttributeMapping.EnsureAssemblyIsAdded<T>();

            var query = new GetAllQuery<T>();

            var stopwatch = Stopwatch.StartNew();

            var item = await connection.QueryAsync<T>(new CommandDefinition(
                commandText: query.Sql,
                commandType: CommandType.Text,
                parameters: query.Arguments,
                commandTimeout: commandTimeout,
                cancellationToken: cancellationToken));

            ReportOnQueryExecuted(query, connection, stopwatch);

            return item;
        }

        void IDisposable.Dispose()
        {
            if (disposables != null)
            {
                foreach (var disposable in disposables)
                    disposable.Dispose();
            }

            connection?.Dispose();
        }

        private static void ReportOnQueryExecuted<T>(ISqlQuery<T> query, DbConnection connection, Stopwatch stopwatch)
        {
            stopwatch.Stop();

            //DapperEncapsulatedConfiguration.OnQueryExecuted?.Invoke(new QueryExecutedContext
            //{
            //   QueryType = query.GetType(),
            //   ReturnType = typeof(T),
            //   Sql = query.Sql,
            //   Arguments = query.Arguments,
            //   Database = connection.Database,
            //   Duration = stopwatch.Elapsed,
            //});
        }
    }
}