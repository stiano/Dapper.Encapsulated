# Dapper.Encapsulated

A thin overlay over Dapper for easy management of both simple and complex queries. Each query is bound to its own object, and can easily be reused by several parts of the application.

This library is more focused on reading than writing.

## Features
- Dependency injection through plain Microsoft.Extensions.DependencyInjection
- Attribute mapping
- Caching 
- Custom timeouts
- Streaming data
- Custom timeouts

## Setup

```csharp

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

```

## Example

### Query

```csharp
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
```

### Usage
Inject the db-connection registered above, and use one of the provided methods for querying..
```csharp
    var users = await usersDbConnection.QueryAsync(new GetUsersQuery(filter: "tom"), cancellationToken);
```