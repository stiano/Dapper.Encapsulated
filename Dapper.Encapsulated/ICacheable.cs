using System;

namespace Dapper.Encapsulated
{
   /// <summary>
   /// Attach to ISqlQuerys, and specify reasonable defaults
   /// </summary>
   public interface ICacheable
   {
      string CacheKey { get; }
      TimeSpan CacheLifetime { get; }
   }
}
