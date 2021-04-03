using System;
using System.Text.Json;

namespace Dapper.Encapsulated.Tests
{
    public static class TestExtensions
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
        };

        public static void ToConsole(this object? o)
        {
            if (o is null)
                return;

            var json = ToJson(o);
            Console.WriteLine(json);
        }
        
        public static string ToJson(this object? o)
        {
            if (o is null)
                return string.Empty;

            return JsonSerializer.Serialize(o, JsonSerializerOptions);
        }
    }
}