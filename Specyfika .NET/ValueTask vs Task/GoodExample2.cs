using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace SpecyfikaDotNet.AsyncPatterns
{
    // ✅ GOOD: Prawidłowe użycie ValueTask z cache pattern
    public class GoodValueTaskCaching
    {
        // Cache przechowuje Task, nie ValueTask
        private readonly ConcurrentDictionary<string, Task<string>> _cache = new();

        // ValueTask jako warstwa optymalizacji
        public ValueTask<string> GetDataAsync(string key)
        {
            // Sprawdź cache
            if (_cache.TryGetValue(key, out Task<string> cachedTask))
            {
                // Jeśli Task jest już ukończony, zwróć ValueTask z rezultatem (zero alokacji)
                if (cachedTask.IsCompletedSuccessfully)
                {
                    return new ValueTask<string>(cachedTask.Result);
                }

                // Jeśli Task jeszcze się wykonuje, zwróć ValueTask opakowujący Task
                return new ValueTask<string>(cachedTask);
            }

            // Brak w cache - pobierz i zapisz
            return GetDataSlowPathAsync(key);
        }

        private async ValueTask<string> GetDataSlowPathAsync(string key)
        {
            // Utwórz Task dla cache
            var task = FetchDataAsync(key);
            
            // Zapisz Task w cache (nie ValueTask!)
            _cache[key] = task;

            return await task;
        }

        private async Task<string> FetchDataAsync(string key)
        {
            await Task.Delay(100); // Symulacja I/O
            return $"Data for {key}";
        }

        // Prawidłowe użycie - ValueTask jest używany tylko raz
        public async ValueTask<int> ProcessAsync(int value)
        {
            ValueTask<int> task = ComputeAsync(value);
            
            // Await dokładnie raz
            int result = await task;
            
            return result * 2;
        }

        private ValueTask<int> ComputeAsync(int value)
        {
            // Fast path - synchroniczny rezultat
            if (value < 10)
            {
                return new ValueTask<int>(value * value);
            }

            // Slow path - async operacja
            return ComputeSlowPathAsync(value);
        }

        private async ValueTask<int> ComputeSlowPathAsync(int value)
        {
            await Task.Delay(10);
            return value * value;
        }

        // Gdy potrzebujemy Task (np. Task.WhenAll), konwertujemy świadomie
        public async Task<string[]> GetMultipleDataAsync(params string[] keys)
        {
            // Konwertuj ValueTask do Task gdy naprawdę potrzeba
            Task<string>[] tasks = new Task<string>[keys.Length];
            
            for (int i = 0; i < keys.Length; i++)
            {
                // AsTask() tylko gdy potrzebujemy Task API
                tasks[i] = GetDataAsync(keys[i]).AsTask();
            }

            return await Task.WhenAll(tasks);
        }

        // ConfigureAwait z ValueTask
        public async ValueTask<string> GetDataWithConfigureAwait(string key)
        {
            // ValueTask wspiera ConfigureAwait
            return await GetDataAsync(key).ConfigureAwait(false);
        }

        // Invalidate cache
        public void InvalidateCache(string key)
        {
            _cache.TryRemove(key, out _);
        }
    }

    public class GoodValueTaskUsageExample
    {
        public static async Task Main()
        {
            var cache = new GoodValueTaskCaching();

            Console.WriteLine("=== Proper ValueTask usage with caching ===");

            // Pierwsze wywołanie - miss, tworzy Task
            Console.WriteLine("First call (cache miss):");
            string result1 = await cache.GetDataAsync("key1");
            Console.WriteLine($"Result: {result1}");

            // Drugie wywołanie - hit, zero alokacji (Task.Result)
            Console.WriteLine("\nSecond call (cache hit, completed Task):");
            string result2 = await cache.GetDataAsync("key1");
            Console.WriteLine($"Result: {result2}");

            // Trzecie wywołanie - nadal zero alokacji
            Console.WriteLine("\nThird call (cache hit, completed Task):");
            string result3 = await cache.GetDataAsync("key1");
            Console.WriteLine($"Result: {result3}");

            // Processing z ValueTask
            Console.WriteLine("\n=== Processing ===");
            int processed1 = await cache.ProcessAsync(5);  // Fast path
            int processed2 = await cache.ProcessAsync(15); // Slow path
            Console.WriteLine($"Processed: {processed1}, {processed2}");

            // Pobieranie wielu wartości
            Console.WriteLine("\n=== Multiple data fetch ===");
            string[] multiple = await cache.GetMultipleDataAsync("key2", "key3", "key4");
            foreach (var item in multiple)
            {
                Console.WriteLine($"- {item}");
            }

            // ConfigureAwait
            Console.WriteLine("\n=== With ConfigureAwait ===");
            string result4 = await cache.GetDataWithConfigureAwait("key5");
            Console.WriteLine($"Result: {result4}");

            // Invalidate cache
            cache.InvalidateCache("key1");
            Console.WriteLine("\n=== After cache invalidation ===");
            string result5 = await cache.GetDataAsync("key1");
            Console.WriteLine($"Result: {result5}");
        }
    }
}
