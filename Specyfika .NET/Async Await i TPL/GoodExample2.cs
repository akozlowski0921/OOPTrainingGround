using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace SpecyfikaDotNet.AsyncAwait.Good2
{
    // ✅ GOOD: Proper async patterns

    // ✅ I/O operations są naturally async - brak Task.Run
    public class GoodFileService
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<string> DownloadDataAsync(string url)
        {
            // ✅ Bezpośredni await - I/O jest już async
            return await _httpClient.GetStringAsync(url);
            // Brak niepotrzebnego Task.Run
        }

        // ✅ Synchroniczna operacja pozostaje synchroniczna
        public int Calculate(int a, int b)
        {
            return a + b; // Brak async overhead dla prostych operacji
        }

        // ✅ Task.Run tylko dla CPU-bound work
        public async Task<int> CalculateHeavyAsync(int n)
        {
            return await Task.Run(() =>
            {
                // Intensywne obliczenia CPU
                int result = 0;
                for (int i = 0; i < n; i++)
                {
                    result += i * i;
                }
                return result;
            });
        }
    }

    // ✅ Async all the way - brak sync-over-async
    public class GoodDataProcessor
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task ProcessDataAsync(List<string> urls)
        {
            foreach (var url in urls)
            {
                // ✅ await zamiast .Result
                var result = await _httpClient.GetStringAsync(url);
                Console.WriteLine(result);
            }
        }

        // ✅ Async method dla async operations
        public async Task<string> FetchDataAsync(string url)
        {
            return await _httpClient.GetStringAsync(url);
        }
    }

    // ✅ await Task.WhenAll dla równoległości
    public class GoodParallelProcessor
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<List<string>> ProcessUrlsAsync(List<string> urls)
        {
            var tasks = urls.Select(url => _httpClient.GetStringAsync(url));

            // ✅ await Task.WhenAll - nie blokuje
            var results = await Task.WhenAll(tasks);
            return results.ToList();
        }
    }

    // ✅ Async event handler z proper error handling
    public class GoodEventHandler
    {
        public event Func<object, EventArgs, Task> DataProcessedAsync;

        // ✅ Async Task event handler (nie void)
        public async Task OnDataReceivedAsync(object sender, EventArgs e)
        {
            try
            {
                await Task.Delay(1000);
                await DataProcessedAsync?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                // ✅ Proper error handling
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
    }

    // ✅ Task.Run dla async delegates
    public class GoodTaskFactory
    {
        public Task StartBackgroundWorkAsync()
        {
            // ✅ Task.Run obsługuje async delegates prawidłowo
            return Task.Run(async () =>
            {
                await Task.Delay(1000);
                Console.WriteLine("Work completed");
            });
        }
    }

    // ✅ Proper variable capture w async
    public class GoodClosureExample
    {
        public async Task ProcessItemsAsync()
        {
            var tasks = new List<Task>();

            for (int i = 0; i < 10; i++)
            {
                // ✅ Capture local copy
                int localI = i;
                tasks.Add(Task.Run(async () =>
                {
                    await Task.Delay(100);
                    Console.WriteLine(localI); // Prawidłowa wartość
                }));
            }

            await Task.WhenAll(tasks);
        }

        // ✅ Alternatywa: LINQ eliminuje problem
        public async Task ProcessItemsLinqAsync()
        {
            var tasks = Enumerable.Range(0, 10)
                .Select(i => Task.Run(async () =>
                {
                    await Task.Delay(100);
                    Console.WriteLine(i);
                }));

            await Task.WhenAll(tasks);
        }
    }

    // ✅ Eliding async/await gdy nie potrzeba
    public class GoodAsyncEliding
    {
        private readonly HttpClient _httpClient = new HttpClient();

        // ✅ Zwraca Task bezpośrednio - brak overhead
        public Task<string> GetDataAsync(string url)
        {
            return _httpClient.GetStringAsync(url);
            // Brak async/await gdy nie ma dodatkowej logiki
        }

        // ✅ Używa async tylko gdy potrzeba
        public async Task<string> GetDataWithLoggingAsync(string url)
        {
            Console.WriteLine("Fetching...");
            var result = await _httpClient.GetStringAsync(url);
            Console.WriteLine("Done");
            return result;
        }
    }
}
