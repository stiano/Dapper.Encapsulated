using Microsoft.Extensions.Caching.Memory;

namespace Dapper.Encapsulated.Tests
{
    public sealed class UsersDbConnection : DapperConnection
    {
    }

    public sealed class InventoryDbConnection : DapperConnection
    {
    }

    public sealed class DapperMemoryCacheProvider : IDapperCache
    {
        private readonly IMemoryCache memoryCache;

        public DapperMemoryCacheProvider(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        public bool TryGetFromCache(ICacheable query, out object o)
        {
            return memoryCache.TryGetValue(query.CacheKey, out o);
        }

        public void TrySetInCache(ICacheable query, object result)
        {
            memoryCache.Set(query.CacheKey, result, query.CacheLifetime);
        }
    }
}