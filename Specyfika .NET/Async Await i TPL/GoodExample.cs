using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace SpecyfikaDotNet.AsyncAwait
{
    // ✅ GOOD: Prawidłowe użycie async/await
    public class GoodAsyncService
    {
        private readonly HttpClient _httpClient = new HttpClient();

        // ✅ async Task - wyjątki można obsłużyć
        public async Task FetchDataAsync(string url)
        {
            try
            {
                var result = await _httpClient.GetStringAsync(url);
                Console.WriteLine(result);
            }
            catch (HttpRequestException ex)
            {
                // Wyjątek jest prawidłowo przechwytywany
                Console.WriteLine($"Błąd pobierania danych: {ex.Message}");
                throw;
            }
        }

        // ✅ Async all the way - unikanie deadlocków
        public async Task<string> GetDataAsync(string url)
        {
            // Używamy await zamiast .Result
            var result = await _httpClient.GetStringAsync(url);
            return result;
        }

        // ✅ ConfigureAwait(false) w kodzie biblioteki
        public async Task<string> LibraryMethodAsync(string url)
        {
            // ConfigureAwait(false) zapobiega przełączaniu kontekstu
            var result = await _httpClient.GetStringAsync(url).ConfigureAwait(false);
            return result.ToUpper();
        }

        // ✅ Równoległe wykonywanie niezależnych operacji
        public async Task<List<string>> FetchMultipleUrlsAsync(List<string> urls)
        {
            // Task.WhenAll wykonuje wszystkie zadania równolegle
            var tasks = urls.Select(url => _httpClient.GetStringAsync(url));
            var results = await Task.WhenAll(tasks);
            return results.ToList();
        }

        // ✅ Synchroniczna metoda dla synchronicznych operacji
        public int Calculate(int a, int b)
        {
            // Brak async/await dla czystych obliczeń
            return a + b;
        }

        // ✅ Prawidłowe łapanie wyjątków z await
        public async Task<string> FetchWithAwaitAsync(string url)
        {
            try
            {
                // await pozwala na prawidłowe łapanie wyjątków
                return await _httpClient.GetStringAsync(url);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Błąd HTTP: {ex.Message}");
                return string.Empty;
            }
        }

        // ✅ Kontrolowane zadania w tle z obsługą błędów
        public async Task StartBackgroundWorkAsync(string url)
        {
            try
            {
                await ProcessDataAsync(url);
            }
            catch (Exception ex)
            {
                // Logowanie błędów z background task
                Console.WriteLine($"Background task failed: {ex.Message}");
                // Można rzucić ponownie lub obsłużyć
            }
        }

        private async Task ProcessDataAsync(string url)
        {
            var data = await _httpClient.GetStringAsync(url).ConfigureAwait(false);
            await Task.Delay(1000).ConfigureAwait(false);
        }
    }

    // ✅ TPL - Task Parallel Library do operacji równoległych
    public class ParallelProcessingService
    {
        // Równoległe przetwarzanie CPU-bound operacji
        public void ProcessItemsInParallel(List<int> items)
        {
            // Parallel.ForEach dla operacji CPU-bound
            Parallel.ForEach(items, item =>
            {
                // Intensywne obliczenia CPU
                var result = PerformHeavyCalculation(item);
                Console.WriteLine($"Item {item}: {result}");
            });
        }

        // Task.WhenAll dla wielu asynchronicznych operacji
        public async Task<List<int>> ProcessItemsAsync(List<int> items)
        {
            var tasks = items.Select(item => ProcessSingleItemAsync(item));
            var results = await Task.WhenAll(tasks);
            return results.ToList();
        }

        // Task.WhenAny dla race condition
        public async Task<string> GetFastestResponseAsync(List<string> urls)
        {
            var httpClient = new HttpClient();
            var tasks = urls.Select(url => httpClient.GetStringAsync(url));
            
            // Zwraca pierwszą zakończoną operację
            var completedTask = await Task.WhenAny(tasks);
            return await completedTask;
        }

        private int PerformHeavyCalculation(int value)
        {
            // Symulacja obliczeń CPU-bound
            return value * value;
        }

        private async Task<int> ProcessSingleItemAsync(int item)
        {
            await Task.Delay(100); // Symulacja I/O
            return item * 2;
        }
    }

    // ✅ Prawidłowe użycie ValueTask dla hot path
    public class ValueTaskExample
    {
        private readonly Dictionary<int, string> _cache = new();

        // ValueTask dla operacji, które często są synchroniczne
        public ValueTask<string> GetValueAsync(int key)
        {
            // Jeśli wartość jest w cache, zwracamy synchronicznie
            if (_cache.TryGetValue(key, out var value))
            {
                return new ValueTask<string>(value);
            }

            // Jeśli nie, wykonujemy asynchronicznie
            return new ValueTask<string>(FetchFromDatabaseAsync(key));
        }

        private async Task<string> FetchFromDatabaseAsync(int key)
        {
            await Task.Delay(100); // Symulacja DB call
            var value = $"Value_{key}";
            _cache[key] = value;
            return value;
        }
    }
}
