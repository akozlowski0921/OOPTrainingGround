using System;
using System.Text.Json;
using System.Collections.Generic;

namespace SpecyfikaDotNet.Serialization.Bad3
{
    // ❌ BAD: Performance i memory issues

    // BŁĄD 1: Serializacja w pętli - alokacje
    public class BadSerializationInLoop
    {
        public void ProcessItems(List<object> items)
        {
            foreach (var item in items)
            {
                // ❌ Tworzy nowy options przy każdej iteracji
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                
                var json = JsonSerializer.Serialize(item, options);
                Console.WriteLine(json);
            }
        }
    }

    // BŁĄD 2: Duże obiekty bez streaming
    public class BadLargeObjectHandling
    {
        public string SerializeLargeObject(List<DataItem> items)
        {
            // ❌ Cały JSON w pamięci - może być gigabajty
            return JsonSerializer.Serialize(items);
        }
    }

    public class DataItem
    {
        public int Id { get; set; }
        public string LargeData { get; set; } // Może być MB
    }

    // BŁĄD 3: Brak reuse JsonSerializerOptions
    public class BadOptionsCreation
    {
        public string SerializeUser(object user)
        {
            // ❌ Nowa instancja za każdym razem
            return JsonSerializer.Serialize(user, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }

    // BŁĄD 4: Synchroniczna serializacja do stream
    public class BadSyncStreamSerialization
    {
        public void SaveToFile(object data, string path)
        {
            using var stream = System.IO.File.Create(path);
            
            // ❌ Synchroniczne - blokuje I/O
            JsonSerializer.Serialize(stream, data);
        }
    }

    // BŁĄD 5: Exception handling w deserializacji
    public class BadExceptionHandling
    {
        public object Deserialize(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<object>(json);
            }
            catch (JsonException)
            {
                // ❌ Zwraca null zamiast rzucić lub obsłużyć
                return null;
            }
        }
    }

    // BŁĄD 6: Niepotrzebne property getters/setters w DTO
    public class BadDtoWithLogic
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        // ❌ Logic w DTO - może powodować problemy przy serializacji
        public string FullName => $"{FirstName} {LastName}";
        
        private string _computed;
        public string Computed
        {
            get
            {
                // ❌ Side effects w getter
                _computed = DateTime.Now.ToString();
                return _computed;
            }
        }
    }

    // BŁĄD 7: Brak use JsonDocument dla read-only scenarios
    public class BadJsonParsing
    {
        public string GetProperty(string json, string propertyName)
        {
            // ❌ Pełna deserializacja gdy potrzebujemy tylko jednej property
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return dict?[propertyName]?.ToString();
        }
    }
}
