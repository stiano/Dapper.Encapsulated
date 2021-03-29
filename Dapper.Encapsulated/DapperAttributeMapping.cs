using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Dapper.Encapsulated
{
    public static class DapperAttributeMapping
   {
      private static readonly ConcurrentDictionary<Assembly, bool> Assemblies = new ConcurrentDictionary<Assembly, bool>();

      public static IEnumerable<string> RegisteredAssemblies
      {
         get { return Assemblies.Keys.Select(x => x.FullName!).ToArray(); }
      }

      static DapperAttributeMapping()
      {
         SqlMapper.AddTypeHandler(typeof(IEqualityComparer<string>), new EqualityComparerHandler());
      }

      public static void EnsureAssemblyIsAdded<T>()
      {
         var returnType = typeof(T);

         if (returnType.IsPrimitive || returnType == typeof(string))
            return;

         Assemblies.GetOrAdd(returnType.Assembly, x =>
         {
            var types = from type in x.GetTypes()
               where type.IsClass
                     && !type.IsAbstract
                     && (type.GetProperties().SelectMany(p => p.GetCustomAttributes<ColumnAttribute>()).Any()
                     || type.GetProperties().SelectMany(p => p.GetCustomAttributes<IgnorePropertyAttribute>()).Any())
               select type;

            types.ToList().ForEach(type =>
            {
               try
               {
                  var genericType = typeof(ColumnAttributeTypeMapper<>).MakeGenericType(type);
                  var mapper = Activator.CreateInstance(genericType) as SqlMapper.ITypeMap;

                  SqlMapper.SetTypeMap(type, mapper);
               }
               catch (Exception)
               {
                  if (Debugger.IsAttached)
                     Debugger.Break();
                  throw;
               }
            });

            return true;
         });
      }
   }

   internal class ColumnAttributeTypeMapper<T> : FallBackTypeMapper
   {
      public ColumnAttributeTypeMapper()
         : base(new SqlMapper.ITypeMap[]
         {
            new CustomPropertyTypeMap(typeof(T),
               (type, columnName) =>
                  type.GetProperties().FirstOrDefault(prop =>
                     {
                        var attributes = prop.GetCustomAttributes(false);
                        var isIgnored = attributes
                           .OfType<IgnorePropertyAttribute>()
                           .Any();

                        if (isIgnored)
                           return false;
                        
                        return attributes
                           .OfType<ColumnAttribute>()
                           .Any(attribute => attribute.Name?.Trim() == columnName);
                     }
                  )
            ),
            new DefaultTypeMap(typeof(T))
         })
      {
      }
   }

   internal class FallBackTypeMapper : SqlMapper.ITypeMap
   {
      private readonly IEnumerable<SqlMapper.ITypeMap> mappers;

      public FallBackTypeMapper(IEnumerable<SqlMapper.ITypeMap> mappers)
      {
         this.mappers = mappers;
      }

      public ConstructorInfo? FindConstructor(string[] names, Type[] types)
      {
         foreach (var mapper in mappers)
         {
            try
            {
               var result = mapper.FindConstructor(names, types);
               if (result == null) 
                  continue;

               return result;
            }
            catch (NotImplementedException)
            {
               // the CustomPropertyTypeMap only supports a no-args
               // constructor and throws a not implemented exception.
               // to work around that, catch and ignore.
            }
         }

         return null;
      }

      public ConstructorInfo FindExplicitConstructor()
      {
         return mappers
            .Select(m => m.FindExplicitConstructor())
            .FirstOrDefault(result => result != null);
      }

      public SqlMapper.IMemberMap? GetConstructorParameter(ConstructorInfo constructor, string columnName)
      {
         foreach (var mapper in mappers)
         {
            try
            {
               var result = mapper.GetConstructorParameter(constructor, columnName);
               if (result == null) 
                  continue;
               
               return result;
            }
            catch (NotImplementedException)
            {
               // the CustomPropertyTypeMap only supports a no-args
               // constructor and throws a not implemented exception.
               // to work around that, catch and ignore.
            }
         }

         return null;
      }

      public SqlMapper.IMemberMap? GetMember(string columnName)
      {
         foreach (var mapper in mappers)
         {
            try
            {
               var result = mapper.GetMember(columnName);
               if (result == null) 
                  continue;

               return result;
            }
            catch (NotImplementedException)
            {
               // the CustomPropertyTypeMap only supports a no-args
               // constructor and throws a not implemented exception.
               // to work around that, catch and ignore.
            }
         }

         return null;
      }
   }

   internal class EqualityComparerHandler : SqlMapper.ITypeHandler
   {
      public void SetValue(IDbDataParameter parameter, object value)
      {
         throw new NotImplementedException();
      }

      public object Parse(Type destinationType, object value)
      {
         throw new NotImplementedException();
      }
   }
}
