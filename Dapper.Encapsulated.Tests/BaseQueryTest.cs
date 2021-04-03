using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Dapper.Encapsulated.Tests
{
    public abstract class BaseQueryTest
    {
        protected UsersDbConnection connection;
        protected IServiceProvider serviceProvider;

        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();

            services.RegisterDapperEncapsulated(builder =>
            {
                builder.Add<UsersDbConnection>(new DbConnectionOptions
                {
                    DbConnectionFactory = _ => new SqlConnection("Server=localhost;Database=dapper.encapsulated.tests;user id=sa;pwd=Password123;"),
                });
            });

            serviceProvider = services.BuildServiceProvider();
            connection = serviceProvider.GetRequiredService<UsersDbConnection>();
        }

        [TearDown]
        public void TearDown()
        {
            (serviceProvider as IDisposable)?.Dispose();
        }
    }
}