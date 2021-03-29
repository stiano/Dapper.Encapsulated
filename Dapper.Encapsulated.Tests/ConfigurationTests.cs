using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;

namespace Dapper.Encapsulated.Tests
{
    public class ConfigurationTests
    {
        public ConfigurationTests()
        {
            var services = new ServiceCollection();

            services.RegisterDapperEncapsulated(builder =>
            {
                builder.UseCache<DapperMemoryCacheProvider>();
                
                builder.Add<UsersDbConnection>(new DbConnectionOptions
                {
                    DbConnectionFactory = _ => new SqlConnection("UsersDbConnection"),
                });
                
                builder.Add<InventoryDbConnection>(new DbConnectionOptions
                {
                    DbConnectionFactory = _ => new SqlConnection("InventoryDbConnection"),
                });
            });
            

            using var serviceProvider = services.BuildServiceProvider();
            using var connection = serviceProvider.GetRequiredService<UsersDbConnection>();
        }
    }
}
