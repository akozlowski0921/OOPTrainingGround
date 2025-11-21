using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace SpecyfikaDotNet.Cancellation
{
    // ✅ GOOD: Prawidłowe użycie CancellationToken

    // ✅ Zawsze przyjmuj i propaguj CancellationToken
    public class GoodDataService
    {
        private readonly HttpClient _httpClient = new();

        public async Task<string> FetchDataAsync(string url, CancellationToken cancellationToken = default)
        {
            // ✅ Przekazujemy token do wszystkich operacji
            await Task.Delay(5000, cancellationToken);
            return await _httpClient.GetStringAsync(url, cancellationToken);
        }

        // ✅ Propagacja tokena przez cały call stack
        public async Task ProcessMultipleAsync(CancellationToken cancellationToken = default)
        {
            await StepOneAsync(cancellationToken);
            await StepTwoAsync(cancellationToken);
            await StepThreeAsync(cancellationToken);
        }

        private Task StepOneAsync(CancellationToken cancellationToken) => 
            Task.Delay(1000, cancellationToken);
        
        private Task StepTwoAsync(CancellationToken cancellationToken) => 
            Task.Delay(1000, cancellationToken);
        
        private Task StepThreeAsync(CancellationToken cancellationToken) => 
            Task.Delay(1000, cancellationToken);
    }

    // ✅ ThrowIfCancellationRequested zamiast manual checking
    public class GoodCancellationChecking
    {
        public async Task ProcessItemsAsync(int[] items, CancellationToken cancellationToken = default)
        {
            foreach (var item in items)
            {
                // ✅ ThrowIfCancellationRequested - rzuca OperationCanceledException
                cancellationToken.ThrowIfCancellationRequested();

                await ProcessItemAsync(item, cancellationToken);
                await Task.Delay(100, cancellationToken);
            }
        }

        private Task ProcessItemAsync(int item, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Processing {item}");
            return Task.CompletedTask;
        }

        // ✅ Graceful cancellation dla cleanup
        public async Task ProcessWithCleanupAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await ProcessItemsAsync(new[] { 1, 2, 3 }, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // ✅ Cleanup przed propagacją
                Console.WriteLine("Operation cancelled, cleaning up...");
                await CleanupAsync();
                throw; // Re-throw dla informowania wywołującego
            }
        }

        private Task CleanupAsync()
        {
            Console.WriteLine("Cleanup completed");
            return Task.CompletedTask;
        }
    }

    // ✅ Prawidłowa obsługa OperationCanceledException
    public class GoodExceptionHandling
    {
        public async Task<string> FetchWithRetryAsync(string url, CancellationToken cancellationToken = default)
        {
            var httpClient = new HttpClient();
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    return await httpClient.GetStringAsync(url, cancellationToken);
                }
                catch (HttpRequestException ex) when (i < 2)
                {
                    // Retry tylko dla HTTP errors, nie dla cancellation
                    Console.WriteLine($"HTTP error, retry {i + 1}: {ex.Message}");
                    await Task.Delay(1000, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // ✅ Propaguj cancellation - NIE retry
                    Console.WriteLine("Operation cancelled");
                    throw;
                }
            }
            return string.Empty;
        }
    }

    // ✅ Linked CancellationToken dla timeout + external cancellation
    public class GoodTimeoutImplementation
    {
        public async Task<string> FetchWithTimeoutAsync(
            string url, 
            CancellationToken cancellationToken = default)
        {
            var httpClient = new HttpClient();
            
            // ✅ Linkowanie tokenów - timeout + external cancellation
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken, 
                timeoutCts.Token);
            
            try
            {
                return await httpClient.GetStringAsync(url, linkedCts.Token);
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
            {
                throw new TimeoutException("Request timed out after 5 seconds");
            }
        }

        // ✅ Multiple linked tokens
        public async Task ProcessWithMultipleTokensAsync(
            CancellationToken userToken = default,
            CancellationToken systemToken = default)
        {
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                userToken,
                systemToken,
                timeoutCts.Token);

            await LongRunningOperationAsync(linkedCts.Token);
        }

        private async Task LongRunningOperationAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(1000, cancellationToken);
        }
    }

    // ✅ Proper disposal of CancellationTokenSource
    public class GoodResourceManagement
    {
        private CancellationTokenSource? _cts;

        public void StartOperation()
        {
            // Dispose poprzedniego
            _cts?.Cancel();
            _cts?.Dispose();

            // ✅ Nowy CTS
            _cts = new CancellationTokenSource();
            _ = ProcessAsync(_cts.Token);
        }

        public void StopOperation()
        {
            // ✅ Cancel i Dispose
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        private async Task ProcessAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, cancellationToken);
                    Console.WriteLine("Working...");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation cancelled");
            }
        }
    }

    // ✅ CancellationToken jako opcjonalny parametr z default
    public class GoodApiDesign
    {
        public async Task<string> FetchDataAsync(
            string url, 
            CancellationToken cancellationToken = default)
        {
            var httpClient = new HttpClient();
            
            // ✅ cancellationToken = default pozwala na wywołanie bez parametru
            return await httpClient.GetStringAsync(url, cancellationToken);
        }

        // Można wywołać:
        // await FetchDataAsync("url"); // Użyje default (CancellationToken.None)
        // await FetchDataAsync("url", cts.Token); // Użyje przekazanego tokena
    }

    // ✅ Cooperative cancellation w pętlach
    public class GoodLoopCancellation
    {
        public async Task ProcessInfiniteLoopAsync(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessBatchAsync(cancellationToken);
                    await Task.Delay(1000, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Loop cancelled gracefully");
                    break;
                }
            }
        }

        private Task ProcessBatchAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Console.WriteLine("Processing batch");
            return Task.CompletedTask;
        }
    }

    // ✅ Callback registration
    public class GoodCallbackUsage
    {
        public async Task ProcessWithCallbackAsync(CancellationToken cancellationToken = default)
        {
            // ✅ Rejestracja callback dla cancellation
            using var registration = cancellationToken.Register(() =>
            {
                Console.WriteLine("Cancellation requested - cleanup");
            });

            await Task.Delay(5000, cancellationToken);
        }

        // ✅ Async callback (.NET 6+)
        public async Task ProcessWithAsyncCallbackAsync(CancellationToken cancellationToken = default)
        {
            await using var registration = cancellationToken.Register(async () =>
            {
                await CleanupAsync();
            });

            await Task.Delay(5000, cancellationToken);
        }

        private Task CleanupAsync()
        {
            Console.WriteLine("Async cleanup");
            return Task.CompletedTask;
        }
    }

    // ✅ CancellationToken w Parallel.ForEach
    public class GoodParallelCancellation
    {
        public void ProcessItemsInParallel(int[] items, CancellationToken cancellationToken = default)
        {
            var options = new ParallelOptions
            {
                CancellationToken = cancellationToken,
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };

            try
            {
                Parallel.ForEach(items, options, item =>
                {
                    ProcessItem(item);
                    cancellationToken.ThrowIfCancellationRequested();
                });
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Parallel operation cancelled");
            }
        }

        private void ProcessItem(int item)
        {
            Console.WriteLine($"Processing {item}");
        }
    }
}
