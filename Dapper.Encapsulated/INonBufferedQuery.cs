namespace Dapper.Encapsulated
{
   /// <summary>
   /// Used by queries that returns a whole lot of rows, that can be processed sequentally from the db instead of
   /// loaded into memory all at once.
   ///
   /// Sets buffered: false on query / CommandFlags.None
   /// </summary>
   public interface INonBufferedQuery
   {
   }
}
