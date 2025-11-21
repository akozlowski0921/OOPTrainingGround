using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SpecyfikaDotNet.Cancellation
{
    // ❌ BAD: Nieprawidłowe użycie CancellationToken

    // BŁĄD 1: Ignorowanie CancellationToken
    public class BadDataService
    {
        private readonly HttpClient _httpClient = new();

        public async Task<string> FetchDataAsync(string url, CancellationToken cancellationToken)
        {
            // ❌ Otrzymujemy token, ale go nie używamy
            await Task.Delay(5000); // Nie przekazujemy tokena
            return await _httpClient.GetStringAsync(url); // Nie przekazujemy tokena
        }

        // BŁĄD 2: Brak propagacji CancellationToken
        public async Task ProcessMultipleAsync(CancellationToken cancellationToken)
        {
            await StepOneAsync(); // ❌ Nie przekazujemy tokena
            await StepTwoAsync(); // ❌ Nie przekazujemy tokena
            await StepThreeAsync(); // ❌ Nie przekazujemy tokena
        }

        private Task StepOneAsync() => Task.Delay(1000);
        private Task StepTwoAsync() => Task.Delay(1000);
        private Task StepThreeAsync() => Task.Delay(1000);
    }

    // BŁĄD 3: Manual cancellation checking - nieefektywne
    public class BadManualCancellation
    {
        public async Task ProcessItemsAsync(int[] items, CancellationToken cancellationToken)
        {
            foreach (var item in items)
            {
                // ❌ Manual checking - boilerplate
                if (cancellationToken.IsCancellationRequested)
                    return; // Brak OperationCanceledException

                await ProcessItemAsync(item);
                await Task.Delay(100);
            }
        }

        private Task ProcessItemAsync(int item)
        {
            Console.WriteLine($"Processing {item}");
            return Task.CompletedTask;
        }
    }

    // BŁĄD 4: Catching i ignorowanie OperationCanceledException
    public class BadExceptionHandling
    {
        public async Task<string> FetchWithRetryAsync(string url, CancellationToken cancellationToken)
        {
            var httpClient = new HttpClient();
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    return await httpClient.GetStringAsync(url);
                }
                catch (OperationCanceledException)
                {
                    // ❌ Ignorowanie cancellation - kontynuujemy retry
                    Console.WriteLine("Retry...");
                    await Task.Delay(1000);
                }
            }
            return string.Empty;
        }
    }

    // BŁĄD 5: Brak użycia CancellationTokenSource.CreateLinkedTokenSource
    public class BadTimeoutImplementation
    {
        public async Task<string> FetchWithTimeoutAsync(string url)
        {
            var httpClient = new HttpClient();
            
            // ❌ Tworzenie osobnych tokenów zamiast linkowania
            var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var result = await httpClient.GetStringAsync(url);
            
            timeoutCts.Dispose();
            return result;
            // Nie można anulować z zewnątrz + timeout jednocześnie
        }
    }

    // BŁĄD 6: Brak dispose CancellationTokenSource
    public class BadResourceManagement
    {
        public void StartOperation()
        {
            var cts = new CancellationTokenSource();
            // ❌ Brak dispose - memory leak
            _ = ProcessAsync(cts.Token);
        }

        private async Task ProcessAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(1000, cancellationToken);
        }
    }

    // BŁĄD 7: Nadużywanie CancellationToken.None
    public class BadDefaultTokenUsage
    {
        public async Task<string> FetchDataAsync(string url)
        {
            var httpClient = new HttpClient();
            
            // ❌ Używanie CancellationToken.None - nie można anulować
            return await httpClient.GetStringAsync(url, CancellationToken.None);
        }

        public Task ProcessAsync()
        {
            // ❌ Metoda nie przyjmuje CancellationToken
            return LongRunningOperationAsync();
        }

        private async Task LongRunningOperationAsync()
        {
            for (int i = 0; i < 100; i++)
            {
                await Task.Delay(100); // Nie można anulować
            }
        }
    }

    // BŁĄD 8: Synchroniczne blokowanie z CancellationToken
    public class BadSyncBlocking
    {
        public void ProcessSync(CancellationToken cancellationToken)
        {
            var httpClient = new HttpClient();
            
            // ❌ Sync over async z CancellationToken
            var task = httpClient.GetStringAsync("https://api.example.com", cancellationToken);
            var result = task.Result; // Deadlock risk, token nie działa prawidłowo
        }
    }
}
