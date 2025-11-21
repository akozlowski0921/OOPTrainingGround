using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpecyfikaDotNet.Serialization.Good3
{
    // ✅ GOOD: Performance optimization

    // ✅ Reuse JsonSerializerOptions
    public static class JsonOptions
    {
        public static readonly JsonSerializerOptions Default = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false // ✅ Compact dla performance
        };
    }

    // ✅ Single options instance
    public class GoodSerializationPerformance
    {
        public void ProcessItems(List<object> items)
        {
            foreach (var item in items)
            {
                var json = JsonSerializer.Serialize(item, JsonOptions.Default);
                Console.WriteLine(json);
            }
        }
    }

    // ✅ Streaming dla dużych obiektów
    public class GoodLargeObjectHandling
    {
        public async Task SerializeLargeObjectAsync(List<DataItem> items, Stream stream)
        {
            await JsonSerializer.SerializeAsync(stream, items, JsonOptions.Default);
        }
    }

    public class DataItem
    {
        public int Id { get; set; }
        public string LargeData { get; set; }
    }

    // ✅ Async stream serialization
    public class GoodAsyncStreamSerialization
    {
        public async Task SaveToFileAsync(object data, string path)
        {
            using var stream = File.Create(path);
            await JsonSerializer.SerializeAsync(stream, data, JsonOptions.Default);
        }
    }

    // ✅ Proper exception handling
    public class GoodExceptionHandling
    {
        public T Deserialize<T>(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json, JsonOptions.Default);
            }
            catch (JsonException ex)
            {
                // ✅ Log i re-throw lub zwróć default
                Console.WriteLine($"Deserialization error: {ex.Message}");
                throw;
            }
        }
    }

    // ✅ Clean DTO bez logic
    public class GoodDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        // ✅ [JsonIgnore] dla computed properties
        [JsonIgnore]
        public string FullName => $"{FirstName} {LastName}";
    }

    // ✅ JsonDocument dla read-only scenarios
    public class GoodJsonParsing
    {
        public string GetProperty(string json, string propertyName)
        {
            using var doc = JsonDocument.Parse(json);
            
            if (doc.RootElement.TryGetProperty(propertyName, out var property))
            {
                return property.GetString();
            }
            
            return null;
        }

        // ✅ Partial deserialization
        public List<string> GetIds(string json)
        {
            var ids = new List<string>();
            
            using var doc = JsonDocument.Parse(json);
            
            if (doc.RootElement.TryGetProperty("items", out var items))
            {
                foreach (var item in items.EnumerateArray())
                {
                    if (item.TryGetProperty("id", out var id))
                    {
                        ids.Add(id.GetString());
                    }
                }
            }
            
            return ids;
        }
    }

    // ✅ Source generators (.NET 6+)
    [JsonSourceGenerationOptions(WriteIndented = false)]
    [JsonSerializable(typeof(UserDto))]
    [JsonSerializable(typeof(List<UserDto>))]
    public partial class AppJsonContext : JsonSerializerContext
    {
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class GoodSourceGeneratorUsage
    {
        public string Serialize(UserDto user)
        {
            // ✅ Source generator - AOT friendly, faster
            return JsonSerializer.Serialize(user, AppJsonContext.Default.UserDto);
        }

        public UserDto Deserialize(string json)
        {
            return JsonSerializer.Deserialize(json, AppJsonContext.Default.UserDto);
        }
    }
}
