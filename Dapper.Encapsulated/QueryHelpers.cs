using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Dapper.Encapsulated
{
   internal static class EntityMetadataCache
   {
      private static readonly ConcurrentDictionary<Type, string> EntityToTableMappings = new ConcurrentDictionary<Type, string>();

      internal static string GetTableName(Type type)
      {
         return EntityToTableMappings.GetOrAdd(type, t =>
         {
            var tableAttribute = t.GetCustomAttribute<TableAttribute>();

            if (tableAttribute == null)
               throw new ArgumentOutOfRangeException(
                  $"GetByOidOrDefault: only types with [{nameof(TableAttribute)}] may be used by this method.");

            return tableAttribute.Name;
         });
      }
   }

   internal class GetByColumnExpressionQuery<T> : ISqlQuery<T>
   {
      private readonly string table;
      private readonly string column;
      private readonly object value;

      public GetByColumnExpressionQuery(Expression<Func<T, object?>> expression, object value)
      {
         table = EntityMetadataCache.GetTableName(typeof(T));

         var memberName = ExpressionUtils.GetMemberName(expression);
         var propertyInfo = typeof(T).GetProperty(memberName);
         var columnAttribute = propertyInfo!.GetCustomAttribute<ColumnAttribute>()!;

         this.column = columnAttribute.Name;
         this.value = value;
      }

      // top 1000 guard.
      public string Sql => $@"
         select top 1000 t.* 
         from {table} t 
         where t.{column} = @value
         ";

      public object Arguments => new
      {
         value = value,
      };
   }

   internal class GetAllQuery<T> : ISqlQuery<T>, INonBufferedQuery
   {
       private readonly int? maxRows;
       private readonly string table;

      public GetAllQuery(int? maxRows = null)
      {
          this.maxRows = maxRows;
          table = EntityMetadataCache.GetTableName(typeof(T));
      }

      // No guard - consider may be...
      public string Sql
      {
          get
          {
              var top = maxRows == null ? string.Empty : $"top {maxRows}";

              return $@"
         select {top} 
            t.* 
         from {table} t";
          }
      }

      public object Arguments => new
      {
      };
   }

   internal static class ExpressionUtils
   {
      public static string GetMemberName(Expression expression)
      {
         if (expression == null)
            throw new ArgumentNullException(nameof(expression));

         if (expression is MemberExpression memberExpression)
         {
            // Reference type property or field
            return memberExpression.Member.Name;
         }

         if (expression is MethodCallExpression methodCallExpression)
         {
            // Reference type method
            return methodCallExpression.Method.Name;
         }
         
         if (expression is LambdaExpression lambdaExpression)
         {
            return GetMemberName(lambdaExpression.Body);
         }

         if (expression is UnaryExpression unaryExpression)
         {
            // Property, field of method returning value type
            return GetMemberName(unaryExpression);
         }

         throw new ArgumentOutOfRangeException(nameof(expression));
      }

      private static string GetMemberName(UnaryExpression unaryExpression)
      {
         var memberInfo = ((MemberExpression) unaryExpression.Operand).Member;
         return memberInfo.Name;
      }
   }
}
