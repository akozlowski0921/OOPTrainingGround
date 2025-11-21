using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SpecyfikaDotNet.AsyncAwait.Bad2
{
    // ❌ BAD: Task.Run abuse i async over sync

    // BŁĄD 1: Niepotrzebne Task.Run dla I/O operations
    public class BadFileService
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<string> DownloadDataAsync(string url)
        {
            // ❌ Task.Run dla I/O operation - niepotrzebne
            return await Task.Run(async () =>
            {
                return await _httpClient.GetStringAsync(url);
                // Tworzy dodatkowy wątek bez potrzeby - I/O jest już async
            });
        }

        // BŁĄD 2: Async wrapper dla synchronicznej operacji
        public async Task<int> CalculateAsync(int a, int b)
        {
            // ❌ Async over sync - niepotrzebny overhead
            return await Task.Run(() => a + b);
            // Prosta operacja CPU nie potrzebuje Task.Run
        }
    }

    // BŁĄD 3: Mieszanie sync i async - performance hit
    public class BadDataProcessor
    {
        public void ProcessDataSync(List<string> urls)
        {
            var httpClient = new HttpClient();
            
            foreach (var url in urls)
            {
                // ❌ Synchroniczne czekanie na async operację
                var result = httpClient.GetStringAsync(url).Result;
                // GetAwaiter().GetResult() też jest problematyczne
                Console.WriteLine(result);
            }
        }

        // BŁĄD 4: GetAwaiter().GetResult() zamiast await
        public string FetchDataSync(string url)
        {
            var httpClient = new HttpClient();
            // ❌ GetAwaiter().GetResult() - może prowadzić do deadlocka
            return httpClient.GetStringAsync(url).GetAwaiter().GetResult();
        }
    }

    // BŁĄD 5: Task.WaitAll vs await Task.WhenAll
    public class BadParallelProcessor
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public void ProcessUrlsSync(List<string> urls)
        {
            var tasks = new List<Task<string>>();

            foreach (var url in urls)
            {
                tasks.Add(_httpClient.GetStringAsync(url));
            }

            // ❌ Task.WaitAll - synchroniczne blokowanie
            Task.WaitAll(tasks.ToArray());
            // Powinno być: await Task.WhenAll(tasks)
        }
    }

    // BŁĄD 6: Async void event handler bez try-catch
    public class BadEventHandler
    {
        public event EventHandler DataProcessed;

        public async void OnDataReceived(object sender, EventArgs e)
        {
            // ❌ Brak try-catch w async void
            await Task.Delay(1000);
            throw new InvalidOperationException("Crash!");
            // Wyjątek crashuje aplikację - nie można go złapać
        }

        public void TriggerEvent()
        {
            DataProcessed?.Invoke(this, EventArgs.Empty);
        }
    }

    // BŁĄD 7: Task.Factory.StartNew zamiast Task.Run
    public class BadTaskFactory
    {
        public void StartBackgroundWork()
        {
            // ❌ Task.Factory.StartNew - niebezpieczny dla async delegates
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(1000);
                // Zwraca Task<Task> zamiast Task - łatwo o błędy
            });

            // Poprawnie: Task.Run(async () => { await Task.Delay(1000); });
        }
    }

    // BŁĄD 8: Closure w async/await - captured variables
    public class BadClosureExample
    {
        public async Task ProcessItemsAsync()
        {
            var tasks = new List<Task>();

            for (int i = 0; i < 10; i++)
            {
                // ❌ Closure problem - i jest captured by reference
                tasks.Add(Task.Run(async () =>
                {
                    await Task.Delay(100);
                    Console.WriteLine(i); // Wypisze 10 dla wszystkich!
                }));
            }

            await Task.WhenAll(tasks);
        }
    }
}
