using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SpecyfikaDotNet.AsyncPatterns
{
    // ❌ BAD: Używanie Task wszędzie, nawet gdy ValueTask byłby lepszy
    public class BadHighPerformanceApi
    {
        private readonly Dictionary<string, string> _cache = new();

        // Problem: Task alokuje za każdym razem, nawet gdy dane są w cache
        public async Task<string> GetDataAsync(string key)
        {
            if (_cache.TryGetValue(key, out string cachedValue))
            {
                // Niepotrzebna alokacja Task dla synchronicznego rezultatu
                return cachedValue; // Kompilator tworzy Task.FromResult
            }

            // Symulacja async operacji
            await Task.Delay(100);
            string value = $"Data for {key}";
            _cache[key] = value;
            return value;
        }

        // Problem: Zawsze zwraca Task, nawet dla prostych operacji
        public async Task<int> CalculateAsync(int a, int b)
        {
            // Synchroniczna operacja owinięta w async
            // Niepotrzebna alokacja Task
            return a + b;
        }

        // Problem: Hot path używa Task
        public async Task<bool> ValidateAsync(string input)
        {
            // Większość walidacji to szybkie sprawdzenia
            if (string.IsNullOrEmpty(input))
                return false;

            if (input.Length < 3)
                return false;

            // Tylko czasami potrzebna async operacja
            if (input.Contains("special"))
            {
                await Task.Delay(10); // Symulacja zewnętrznego API
                return true;
            }

            return true;
        }
    }

    public class BadUsageExample
    {
        public static async Task Main()
        {
            var api = new BadHighPerformanceApi();

            // Performance problem: setki tysięcy alokacji Task
            for (int i = 0; i < 100_000; i++)
            {
                // Każde wywołanie alokuje Task, nawet gdy dane są w cache
                await api.GetDataAsync("key1");
                await api.CalculateAsync(i, i + 1);
                await api.ValidateAsync("test");
            }

            // Memory pressure rośnie niepotrzebnie
            // GC musi zbierać wszystkie te Task obiekty
            // Latency może wzrosnąć przez GC pauses
        }
    }
}
