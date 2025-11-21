using System;
using System.Threading;
using System.Threading.Tasks;

namespace SpecyfikaDotNet.AsyncAwait.Good3
{
    // ✅ GOOD: ConfigureAwait i context management

    // ✅ ConfigureAwait(false) w library code
    public class GoodLibraryCode
    {
        public async Task<string> ProcessDataAsync()
        {
            // ✅ ConfigureAwait(false) - nie potrzebujemy kontekstu
            await Task.Delay(100).ConfigureAwait(false);
            
            var result = "processed";
            
            await Task.Delay(100).ConfigureAwait(false);
            
            return result.ToUpper();
        }
    }

    // ✅ Eliding async/await w chain
    public class GoodAsyncChain
    {
        // ✅ Zwraca Task bezpośrednio
        public Task<int> MethodAAsync() => MethodBAsync();
        
        public Task<int> MethodBAsync() => MethodCAsync();

        public async Task<int> MethodCAsync()
        {
            await Task.Delay(100);
            return 42;
        }
    }

    // ✅ Task.Run dla długich sync operations
    public class GoodLongSyncInAsync
    {
        public async Task<string> ProcessAsync(string data)
        {
            // ✅ Task.Run dla CPU-bound work
            var result = await Task.Run(() =>
            {
                for (int i = 0; i < 1000000; i++)
                {
                    data += i.ToString();
                }
                return data;
            });

            await Task.Delay(100);
            return result;
        }
    }

    // ✅ Factory method zamiast async constructor
    public class GoodAsyncFactory
    {
        private string _data;

        private GoodAsyncFactory() { }

        // ✅ Static factory method
        public static async Task<GoodAsyncFactory> CreateAsync()
        {
            var instance = new GoodAsyncFactory();
            await instance.InitializeAsync();
            return instance;
        }

        private async Task InitializeAsync()
        {
            await Task.Delay(1000);
            _data = "initialized";
        }
    }

    // ✅ Lazy async initialization
    public class GoodLazyInit
    {
        private readonly Lazy<Task<string>> _dataLoader;

        public GoodLazyInit()
        {
            _dataLoader = new Lazy<Task<string>>(async () =>
            {
                await Task.Delay(1000);
                return "initialized";
            });
        }

        public Task<string> GetDataAsync() => _dataLoader.Value;
    }

    // ✅ Proper TaskCompletionSource usage
    public class GoodTaskCompletionSource
    {
        public Task<string> GetDataAsync()
        {
            var tcs = new TaskCompletionSource<string>();

            Task.Run(() =>
            {
                try
                {
                    var result = ExpensiveOperation();
                    // ✅ TrySetResult - bezpieczne
                    tcs.TrySetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            });

            return tcs.Task;
        }

        private string ExpensiveOperation() => "result";
    }

    // ✅ Async lambda z error handling
    public class GoodAsyncLambda
    {
        public void StartProcessing()
        {
            var timer = new System.Timers.Timer(1000);
            
            // ✅ Wrapper z try-catch
            timer.Elapsed += async (sender, e) =>
            {
                try
                {
                    await ProcessAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            };
            
            timer.Start();
        }

        private async Task ProcessAsync()
        {
            await Task.Delay(500);
        }
    }

    // ✅ Proper task continuation
    public class GoodTaskContinuation
    {
        public async Task ProcessAsync()
        {
            var result = await Task.Run(() =>
            {
                Thread.Sleep(1000);
                return 42;
            });

            // ✅ Async/await zamiast ContinueWith
            Console.WriteLine(result);
        }

        // Jeśli musisz użyć ContinueWith
        public Task ProcessWithContinuationAsync()
        {
            var task = Task.Run(() =>
            {
                Thread.Sleep(1000);
                return 42;
            });

            // ✅ Bez ExecuteSynchronously
            return task.ContinueWith(t =>
            {
                Console.WriteLine(t.Result);
            }, TaskScheduler.Default);
        }
    }

    // ✅ ValueTask dla często synchronicznych operations
    public class GoodValueTask
    {
        private string _cachedData = "cached";

        // ✅ ValueTask - optymalizacja dla hot path
        public ValueTask<string> GetDataAsync()
        {
            if (_cachedData != null)
            {
                return new ValueTask<string>(_cachedData);
            }

            return new ValueTask<string>(FetchDataAsync());
        }

        private async Task<string> FetchDataAsync()
        {
            await Task.Delay(100);
            return "fetched";
        }
    }

    // ✅ Async/await best practices summary
    public class AsyncBestPractices
    {
        // DO:
        // ✅ async Task (nie async void)
        // ✅ ConfigureAwait(false) w libraries
        // ✅ await Task.WhenAll dla parallelism
        // ✅ ValueTask dla hot path z częstym sync completion
        // ✅ CancellationToken dla long-running operations
        
        // DON'T:
        // ❌ .Result lub .Wait()
        // ❌ async void (chyba że event handler)
        // ❌ Task.Run dla I/O operations
        // ❌ Sync over async
    }
}
