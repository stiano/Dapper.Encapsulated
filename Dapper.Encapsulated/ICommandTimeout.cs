namespace Dapper.Encapsulated
{
   /// <summary>
   /// Specifies a custom timeout value for the given ISqlQuery.
   /// </summary>
   public interface ICommandTimeout
   {
      /// <summary>
      /// Gets or sets a timeout that will overrule global settings.
      /// </summary>
      int TimeoutSeconds { get; set; }
   }
}
