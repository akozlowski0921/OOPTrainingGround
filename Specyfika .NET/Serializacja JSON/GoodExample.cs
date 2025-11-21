using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SpecyfikaDotNet.Serialization
{
    // ✅ Properties zamiast pól
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // JsonIgnore dla pól wewnętrznych
        [JsonIgnore]
        public string InternalNote { get; set; }
    }

    // ✅ Camel case naming dla zgodności z JavaScript
    public class ProductDto
    {
        [JsonPropertyName("id")]
        public int ProductId { get; set; }
        
        [JsonPropertyName("name")]
        public string ProductName { get; set; }
        
        [JsonPropertyName("price")]
        public decimal UnitPrice { get; set; }
    }

    // ✅ GOOD: Spójne i bezpieczne użycie System.Text.Json
    public class GoodSerializationService
    {
        // Singleton options - jedna instancja dla całej aplikacji
        private static readonly JsonSerializerOptions DefaultOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };

        // ✅ Spójne użycie System.Text.Json
        public string SerializeUser(UserDto user)
        {
            return JsonSerializer.Serialize(user, DefaultOptions);
        }

        public UserDto? DeserializeUser(string json)
        {
            return JsonSerializer.Deserialize<UserDto>(json, DefaultOptions);
        }

        // ✅ Obsługa null i błędów
        public ProductDto? DeserializeProductSafe(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            try
            {
                return JsonSerializer.Deserialize<ProductDto>(json, DefaultOptions);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Błąd deserializacji: {ex.Message}");
                return null;
            }
        }

        // ✅ Obsługa cyklicznych referencji
        public string SerializeWithReferences(OrderDto order)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
            
            return JsonSerializer.Serialize(order, options);
        }
    }

    // ✅ Custom converter dla specjalnych typów
    public class TemperatureDto
    {
        public double Celsius { get; set; }
    }

    public class TemperatureConverter : JsonConverter<TemperatureDto>
    {
        public override TemperatureDto Read(
            ref Utf8JsonReader reader, 
            Type typeToConvert, 
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.Number)
                throw new JsonException("Expected number for temperature");

            return new TemperatureDto { Celsius = reader.GetDouble() };
        }

        public override void Write(
            Utf8JsonWriter writer, 
            TemperatureDto value, 
            JsonSerializerOptions options)
        {
            // Serializuje tylko wartość Celsius jako liczbę
            writer.WriteNumberValue(value.Celsius);
        }
    }

    public class WeatherDataDto
    {
        [JsonConverter(typeof(TemperatureConverter))]
        public TemperatureDto Temp { get; set; }
        
        // Serializuje jako: { "temp": 20 } zamiast { "temp": { "celsius": 20 } }
    }

    // ✅ Wrażliwe dane - JsonIgnore
    public class UserCredentialsDto
    {
        public string Username { get; set; }
        
        // Hasło nigdy nie jest serializowane
        [JsonIgnore]
        public string Password { get; set; }
        
        public string Email { get; set; }
    }

    // ✅ Source Generator dla lepszej wydajności (C# 9+, .NET 6+)
    [JsonSerializable(typeof(UserDto))]
    [JsonSerializable(typeof(ProductDto))]
    [JsonSerializable(typeof(List<ProductDto>))]
    public partial class AppJsonContext : JsonSerializerContext
    {
    }

    public class SourceGeneratorExample
    {
        public string SerializeWithSourceGenerator(ProductDto product)
        {
            // Source generator - AOT compilation, lepsza wydajność
            return JsonSerializer.Serialize(product, AppJsonContext.Default.ProductDto);
        }

        public ProductDto? DeserializeWithSourceGenerator(string json)
        {
            return JsonSerializer.Deserialize(json, AppJsonContext.Default.ProductDto);
        }
    }

    // ✅ Enum handling
    public enum OrderStatus
    {
        Pending,
        Processing,
        Completed,
        Cancelled
    }

    public class OrderDto
    {
        public int Id { get; set; }
        
        // Serializuje jako string "Pending" zamiast 0
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OrderStatus Status { get; set; }
        
        public CustomerDto Customer { get; set; }
    }

    public class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        // Ignorujemy kolekcję dla uniknięcia cyklicznych referencji
        [JsonIgnore]
        public List<OrderDto> Orders { get; set; }
    }

    // ✅ Polymorphic serialization
    [JsonDerivedType(typeof(CreditCardPayment), "creditCard")]
    [JsonDerivedType(typeof(PayPalPayment), "paypal")]
    public abstract class Payment
    {
        public decimal Amount { get; set; }
    }

    public class CreditCardPayment : Payment
    {
        public string CardNumber { get; set; }
    }

    public class PayPalPayment : Payment
    {
        public string Email { get; set; }
    }

    public class PolymorphicExample
    {
        public string SerializePayment(Payment payment)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            
            // Serializuje z discriminatorem typu:
            // { "$type": "creditCard", "amount": 100, "cardNumber": "..." }
            return JsonSerializer.Serialize(payment, options);
        }
    }
}
