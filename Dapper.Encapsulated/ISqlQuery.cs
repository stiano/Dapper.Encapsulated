using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dapper.Encapsulated
{
   /// <summary>
   /// Override in order to encapsulate a sql query that returns a type T.
   /// </summary>
   public interface ISqlQuery<TResult>
   {
      /// <summary>
      /// Gets the sql for the query
      /// </summary>
      string Sql { get; }

      /// <summary>
      /// Gets the arguments as an anonymous object/DynamicParameters used by Dapper
      /// </summary>
      object Arguments { get; }
   }

   /// <summary>
   /// Override in order to encapsulate a sql query that returns a type T, and executes a mapping before returning.
   /// </summary>
   public interface ISqlQueryMap<in TFirst, in TSecond, TResult> : ISqlQuery<TResult>
   {
      Func<TFirst, TSecond, TResult> MapFunc { get; }
   }

   /// <summary>
   /// Override in order to encapsulate a sql query that returns a type T, and executes a mapping before returning.
   /// </summary>
   public interface ISqlQueryMap<in TFirst, in TSecond, in TThird, TResult> : ISqlQuery<TResult>
   {
      Func<TFirst, TSecond, TThird, TResult> MapFunc { get; }
   }

   /// <summary>
   /// Override in order to query with multiple result sets.
   /// </summary>
   public interface ISqlQueryMultiple<TResult> : ISqlQuery<TResult>
   {
      /// <summary>
      /// Used to map result sets to different result items.
      /// </summary>
      Task<TResult> Map(SqlMapper.GridReader reader, CancellationToken cancellationToken);
   }
}
