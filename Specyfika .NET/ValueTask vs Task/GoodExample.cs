using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SpecyfikaDotNet.AsyncPatterns
{
    // ✅ GOOD: Używanie ValueTask dla high-performance scenariuszy
    public class GoodHighPerformanceApi
    {
        private readonly Dictionary<string, string> _cache = new();

        // ValueTask - zero alokacji gdy zwraca z cache
        public ValueTask<string> GetDataAsync(string key)
        {
            if (_cache.TryGetValue(key, out string cachedValue))
            {
                // ValueTask opakowuje wartość bez alokacji heap
                return new ValueTask<string>(cachedValue);
            }

            // Dla prawdziwej async operacji używamy pomocniczej metody
            return GetDataSlowPathAsync(key);
        }

        private async ValueTask<string> GetDataSlowPathAsync(string key)
        {
            // Symulacja async operacji
            await Task.Delay(100);
            string value = $"Data for {key}";
            _cache[key] = value;
            return value;
        }

        // ValueTask dla operacji, która jest zazwyczaj synchroniczna
        public ValueTask<int> CalculateAsync(int a, int b)
        {
            // Proste obliczenie - zero alokacji
            return new ValueTask<int>(a + b);
        }

        // ValueTask dla walidacji - hot path bez alokacji
        public ValueTask<bool> ValidateAsync(string input)
        {
            // Fast path - synchroniczne sprawdzenia
            if (string.IsNullOrEmpty(input))
                return new ValueTask<bool>(false);

            if (input.Length < 3)
                return new ValueTask<bool>(false);

            // Slow path - async operacja tylko gdy potrzebna
            if (input.Contains("special"))
            {
                return ValidateSpecialAsync(input);
            }

            return new ValueTask<bool>(true);
        }

        private async ValueTask<bool> ValidateSpecialAsync(string input)
        {
            await Task.Delay(10); // Symulacja zewnętrznego API
            return true;
        }

        // ValueTask dla operacji I/O z możliwością synchronicznego completion
        public ValueTask<string> ReadConfigAsync(string configKey)
        {
            // Jeśli możemy zwrócić od razu, unikamy alokacji
            if (configKey == "default")
            {
                return new ValueTask<string>("default_config");
            }

            // W przeciwnym razie async
            return ReadConfigSlowPathAsync(configKey);
        }

        private async ValueTask<string> ReadConfigSlowPathAsync(string configKey)
        {
            await Task.Delay(50); // Symulacja odczytu z pliku/DB
            return $"config_{configKey}";
        }
    }

    public class GoodUsageExample
    {
        public static async Task Main()
        {
            var api = new GoodHighPerformanceApi();

            Console.WriteLine("=== High-performance operations with ValueTask ===");

            // Zero alokacji dla cache hits
            for (int i = 0; i < 100_000; i++)
            {
                // ValueTask - brak alokacji heap dla cached values
                string data = await api.GetDataAsync("key1");
                
                // ValueTask - brak alokacji dla synchronicznych operacji
                int result = await api.CalculateAsync(i, i + 1);
                
                // ValueTask - brak alokacji dla fast path
                bool isValid = await api.ValidateAsync("test");
            }

            Console.WriteLine("Operations completed with minimal allocations");

            // Demonstracja różnych ścieżek
            Console.WriteLine("\n=== Demonstrating different paths ===");
            
            // Cache miss - pierwsza alokacja Task
            string newData = await api.GetDataAsync("new_key");
            Console.WriteLine($"New data: {newData}");

            // Cache hit - zero alokacji
            string cachedData = await api.GetDataAsync("new_key");
            Console.WriteLine($"Cached data: {cachedData}");

            // Synchroniczne obliczenie - zero alokacji
            int calc = await api.CalculateAsync(10, 20);
            Console.WriteLine($"Calculation: {calc}");

            // Fast path validation - zero alokacji
            bool valid1 = await api.ValidateAsync("abc");
            Console.WriteLine($"Validation 1: {valid1}");

            // Slow path validation - alokuje Task
            bool valid2 = await api.ValidateAsync("special_case");
            Console.WriteLine($"Validation 2: {valid2}");
        }
    }
}
