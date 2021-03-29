namespace Dapper.Encapsulated
{
    public interface IDapperCache
    {
        bool TryGetFromCache(ICacheable query, out object o);
        void TrySetInCache(ICacheable query, object result);
    }

    internal class DapperNoopCache : IDapperCache
    {
        private static readonly object empty = new object();

        public bool TryGetFromCache(ICacheable query, out object o)
        {
            o = empty;
            return false;
        }

        public void TrySetInCache(ICacheable query, object result)
        {
        }
    }
}