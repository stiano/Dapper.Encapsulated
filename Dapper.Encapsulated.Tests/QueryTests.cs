using System;
using System.Threading;
using System.Threading.Tasks;
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
                    DbConnectionFactory = _ => new SqlConnection("ConnectionString"),
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

    public class QueryTests : BaseQueryTest
    {
        [Test]
        [Explicit]
        public async Task GetUsers()
        {
            var users = await connection.QueryAsync(new GetUsersQuery("joh"), CancellationToken.None);
        }
    }

    public class GetUsersQuery : ISqlQuery<GetUsersQuery.User>
    {
        private readonly string filter;

        public GetUsersQuery(string filter)
        {
            this.filter = filter;
        }

        public string Sql => @"
select 
    u.id, u.name 
from User 
where 
    u.signature like @filter
";
        public object Arguments => new
        {
            filter
        };

        public class User
        {
            [Column("id")]
            public string Id { get; set; }

            [Column("name")]
            public string Name { get; set; }
        }
    }
}