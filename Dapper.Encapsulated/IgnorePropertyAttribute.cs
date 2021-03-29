using System;

namespace Dapper.Encapsulated
{
   /// <summary>
   /// Needs to be added to all navigational properties so that
   /// Dapper don`t get confused.
   /// </summary>
   [AttributeUsage(AttributeTargets.Property)]
   public sealed class IgnorePropertyAttribute : Attribute
   {
   }
}
