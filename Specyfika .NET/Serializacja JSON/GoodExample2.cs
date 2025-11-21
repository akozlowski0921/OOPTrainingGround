using System;
using System.Collections.Generic;
using System.Text.Json;

namespace SpecyfikaDotNet.Serialization.Good2
{
    // ✅ GOOD: Proper naming i case handling

    // ✅ Spójne naming z JsonPropertyName
    public class GoodNamingDto
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }
        
        [JsonPropertyName("userName")]
        public string UserName { get; set; }
        
        [JsonPropertyName("emailAddress")]
        public string EmailAddress { get; set; }
    }

    // ✅ Global camelCase policy
    public static class SerializationOptions
    {
        public static readonly JsonSerializerOptions Default = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true, // ✅ Case-insensitive deserializacja
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    // ✅ Case-insensitive deserializacja
    public class GoodCaseSensitiveService
    {
        public UserDto Deserialize(string json)
        {
            return JsonSerializer.Deserialize<UserDto>(json, SerializationOptions.Default);
        }
    }

    public class UserDto
    {
        public int UserId { get; set; }
        public string Name { get; set; }
    }

    // ✅ Null handling opcje
    public class GoodNullHandling
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public string Serialize(object obj)
        {
            return JsonSerializer.Serialize(obj, Options);
        }
    }

    // ✅ Nested objects z kontrolą depth
    public class GoodNestedSerialization
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            MaxDepth = 64, // ✅ Limit depth
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        public class OrderDto
        {
            public int Id { get; set; }
            public CustomerDto Customer { get; set; }
            public List<ProductDto> Products { get; set; }
        }

        public class CustomerDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class ProductDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public string Serialize(OrderDto order)
        {
            return JsonSerializer.Serialize(order, Options);
        }
    }

    // ✅ DateTime z custom converter
    public class GoodDateTimeSerialization
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            // ISO 8601 format
            Converters = { new DateTimeConverter() }
        };

        public class EventDto
        {
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public string Serialize(EventDto evt)
        {
            return JsonSerializer.Serialize(evt, Options);
        }
    }

    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("O")); // ISO 8601
        }
    }

    // ✅ Decimal jako string dla precision
    public class GoodDecimalHandling
    {
        public class PriceDto
        {
            [JsonConverter(typeof(DecimalStringConverter))]
            public decimal Amount { get; set; }
        }
    }

    public class DecimalStringConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return decimal.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("F2"));
        }
    }

    // ✅ Validation po deserializacji
    public class GoodDeserializationValidation
    {
        public UserDto CreateUser(string json)
        {
            var user = JsonSerializer.Deserialize<UserDto>(json, SerializationOptions.Default);
            
            // ✅ Validation
            if (user == null || string.IsNullOrWhiteSpace(user.Name))
                throw new ArgumentException("Invalid user data");
                
            return user;
        }
    }

    // ✅ JSON comments support
    public class GoodJsonComments
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        public UserDto Deserialize(string jsonWithComments)
        {
            return JsonSerializer.Deserialize<UserDto>(jsonWithComments, Options);
        }
    }
}
