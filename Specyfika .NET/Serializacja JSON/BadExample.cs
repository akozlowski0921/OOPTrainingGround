using System;
using System.Text.Json;
using Newtonsoft.Json;

namespace SpecyfikaDotNet.Serialization
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // BŁĄD 1: Publiczne pole zamiast property - System.Text.Json nie serializuje pól
        public string InternalNote;
    }

    public class Product
    {
        // BŁĄD 2: Camel case w C# - niezgodność z JavaScript conventions
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
    }

    // ❌ BAD: Problemy z serializacją
    public class BadSerializationService
    {
        // BŁĄD 3: Mieszanie System.Text.Json i Newtonsoft.Json
        public string SerializeUser(User user)
        {
            // Czasem używamy System.Text.Json
            return System.Text.Json.JsonSerializer.Serialize(user);
        }

        public User DeserializeUser(string json)
        {
            // Czasem Newtonsoft.Json - niespójność
            return JsonConvert.DeserializeObject<User>(json);
        }

        // BŁĄD 4: Brak obsługi null i błędów deserializacji
        public Product DeserializeProduct(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<Product>(json);
            // Co jeśli json jest null lub nieprawidłowy? Wyjątek!
        }

        // BŁĄD 5: Tworzenie nowej instancji JsonSerializerOptions przy każdym wywołaniu
        public string SerializeWithOptions(object obj)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            return System.Text.Json.JsonSerializer.Serialize(obj, options);
            // Performance hit - options powinny być singleton
        }

        // BŁĄD 6: Brak kontroli nad cyklicznymi referencjami
        public string SerializeWithReferences(Order order)
        {
            // Order -> Customer -> Orders -> Order -> ... infinite loop
            return System.Text.Json.JsonSerializer.Serialize(order);
            // JsonException: A possible object cycle was detected
        }
    }

    // BŁĄD 7: Brak custom convertera dla specjalnych typów
    public class Temperature
    {
        public double Celsius { get; set; }
        
        public double Fahrenheit => Celsius * 9 / 5 + 32;
    }

    public class WeatherData
    {
        public Temperature Temp { get; set; }
        // Serializuje jako: { "temp": { "celsius": 20, "fahrenheit": 68 } }
        // Ale chcemy: { "tempCelsius": 20 } - zwięźlej
    }

    // BŁĄD 8: Wrażliwe dane w serializacji
    public class UserCredentials
    {
        public string Username { get; set; }
        
        // Hasło nie powinno być serializowane!
        public string Password { get; set; }
        
        public string Email { get; set; }
    }

    // Pomocnicze klasy dla błędu #6
    public class Order
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Order> Orders { get; set; } // Cykliczna referencja
    }
}
