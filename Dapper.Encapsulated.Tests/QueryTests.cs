using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Dapper.Encapsulated.Tests
{
    [Explicit]
    public class QueryTests : BaseQueryTest
    {
        [Test]
        public async Task GetUsers()
        {
            var users = await connection.QueryAsync(new GetUsersQuery("t%"), CancellationToken.None);
            users.ToConsole();
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
from [User] u
where 
    u.name like @filter
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