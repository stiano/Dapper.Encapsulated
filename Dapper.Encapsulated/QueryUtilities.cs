using System.Data;
// ReSharper disable SuspiciousTypeConversion.Global

namespace Dapper.Encapsulated
{
   internal static class QueryUtilities
   {
      internal static int? GetCommandTimeout<T>(ISqlQuery<T> query, int? fallback)
      {
         if (query is ICommandTimeout q)
         {
            return q.TimeoutSeconds;
         }

         return fallback;
      }

      internal static int? GetCommandTimeout<T>(ISqlQueryMultiple<T> query, int? fallback)
      {
         if (query is ICommandTimeout q)
         {
            return q.TimeoutSeconds;
         }

         return fallback;
      }

      internal static CommandType GetCommandType<T>(ISqlQuery<T> query)
      {
         const string separatorToken = " ";

         return query.Sql.Trim().Contains(separatorToken)
            ? CommandType.Text
            : CommandType.StoredProcedure;
      }

      internal static CommandType GetCommandType<T>(ISqlQueryMultiple<T> query)
      {
         const string separatorToken = " ";

         return query.Sql.Trim().Contains(separatorToken)
            ? CommandType.Text
            : CommandType.StoredProcedure;
      }

      internal static bool GetIsBuffered<T>(ISqlQuery<T> query)
      {
         return !(query is INonBufferedQuery);
      }

      internal static CommandFlags GetIsBufferedFlag<T>(ISqlQuery<T> query)
      {
         return query is INonBufferedQuery
            ? CommandFlags.None
            : CommandFlags.Buffered;
      }
   }
}
