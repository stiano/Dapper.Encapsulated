using System;
using System.Data;
using System.Data.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Dapper.Encapsulated
{
    public class DbConnectionOptions
    {
        public Func<IServiceProvider, DbConnection> DbConnectionFactory { get; set; } = null!;

        /// <summary>
        /// Default: IsolationLevel.ReadUncommitted
        /// </summary>
        public IsolationLevel TransactionIsolationLevel { get; set; } = IsolationLevel.ReadUncommitted;

        /// <summary>
        /// Seconds before timing out. Default: 30
        /// </summary>
        public int? CommandTimeout { get; set; } = 30;
    }

    public class QueryExecutedContext
    {
        public Type? QueryType { get; set; }
        public Type? ReturnType { get; set; }
        public string Database { get; set; } = string.Empty;
        public string Sql { get; set; } = string.Empty;
        public object? Arguments { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public class RegistrationBuilder
    {
        private static readonly IDapperCache nullCache = new DapperNoopCache();
        private readonly IServiceCollection services;

        public RegistrationBuilder(IServiceCollection services)
        {
            this.services = services;
        }

        internal Type? CacheType { get; private set; }

        public void UseCache<T>()
            where T: IDapperCache
        {
            services.AddTransient(typeof(IDapperCache), typeof(T));
        }

        public void Add<T>(DbConnectionOptions config)
            where T : DapperConnection, new()
        {
            services.AddScoped<T>(x =>
            {
                var underlyingConnection = config.DbConnectionFactory(x);
                var cache = x.GetService<IDapperCache>() ?? nullCache;

                var dapperConnection = new T();
                dapperConnection.SetDependencies(underlyingConnection, cache, config.CommandTimeout);

                return dapperConnection;
            });
        }
    }


    public static class ServiceCollectionExtensions
   {

       public static void RegisterDapperEncapsulated(this IServiceCollection services, Action<RegistrationBuilder> builder)
       {
           var registrationBuilder = new RegistrationBuilder(services);
           builder(registrationBuilder);
       }



      
   }
}
