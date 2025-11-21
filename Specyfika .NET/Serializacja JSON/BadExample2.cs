using System;
using System.Collections.Generic;
using System.Text.Json;

namespace SpecyfikaDotNet.Serialization.Bad2
{
    // ❌ BAD: Problemy z naming conventions i case sensitivity

    // BŁĄD 1: Niespójne naming - mix PascalCase i camelCase
    public class BadNamingDto
    {
        public int UserId { get; set; }
        public string user_name { get; set; } // ❌ snake_case
        public string EmailAddress { get; set; }
        
        // Serializuje się niespójnie: {"UserId": 1, "user_name": "john", "EmailAddress": "..."}
    }

    // BŁĄD 2: Brak obsługi case insensitivity przy deserializacji
    public class BadCaseSensitiveService
    {
        public UserDto Deserialize(string json)
        {
            // ❌ Case-sensitive by default
            return JsonSerializer.Deserialize<UserDto>(json);
            // Fail if JSON has: {"userid": 1} zamiast {"UserId": 1}
        }
    }

    public class UserDto
    {
        public int UserId { get; set; }
        public string Name { get; set; }
    }

    // BŁĄD 3: Ignorowanie null values nieprawidłowo
    public class BadNullHandling
    {
        public string Serialize(object obj)
        {
            // ❌ Nie ignoruje null - duży JSON
            return JsonSerializer.Serialize(obj);
            // Zwraca: {"name": "John", "email": null, "phone": null, ...}
        }
    }

    // BŁĄD 4: Nested object bez proper handling
    public class BadNestedSerialization
    {
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
            // ❌ Brak kontroli nad depth - problemy z circular refs
            return JsonSerializer.Serialize(order);
        }
    }

    // BŁĄD 5: Serializacja DateTime bez format specification
    public class BadDateTimeSerialization
    {
        public class EventDto
        {
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public string Serialize(EventDto evt)
        {
            // ❌ Default DateTime format może być niespójny
            return JsonSerializer.Serialize(evt);
            // Format zależy od culture
        }
    }

    // BŁĄD 6: Decimal precision loss
    public class BadDecimalHandling
    {
        public class PriceDto
        {
            public decimal Amount { get; set; } // ❌ Może stracić precision
        }

        public string Serialize(PriceDto price)
        {
            return JsonSerializer.Serialize(price);
            // 19.99 może stać się 19.990000000000002
        }
    }

    // BŁĄD 7: Brak validation po deserializacji
    public class BadDeserializationValidation
    {
        public UserDto CreateUser(string json)
        {
            var user = JsonSerializer.Deserialize<UserDto>(json);
            
            // ❌ Brak validation - może być null lub nieprawidłowe dane
            return user; // Może zwrócić null!
        }
    }

    // BŁĄD 8: Comments w JSON nie są obsługiwane
    public class BadJsonComments
    {
        public UserDto Deserialize(string jsonWithComments)
        {
            // ❌ Fail na JSON z komentarzami
            // {"userId": 1, /* comment */ "name": "John"}
            return JsonSerializer.Deserialize<UserDto>(jsonWithComments);
        }
    }
}
