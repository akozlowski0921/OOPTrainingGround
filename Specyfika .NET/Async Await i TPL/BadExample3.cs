using System;
using System.Threading;
using System.Threading.Tasks;

namespace SpecyfikaDotNet.AsyncAwait.Bad3
{
    // ❌ BAD: Context switching i SynchronizationContext issues

    // BŁĄD 1: Niepotrzebne przełączanie kontekstu
    public class BadContextSwitching
    {
        public async Task<string> ProcessDataAsync()
        {
            // W library code - niepotrzebnie wracamy na caller context
            await Task.Delay(100); // Bez ConfigureAwait(false)
            
            // Operacje nie wymagające kontekstu
            var result = "processed";
            
            await Task.Delay(100); // Znowu przełączenie kontekstu
            
            return result.ToUpper();
        }
    }

    // BŁĄD 2: async/await dla każdej operacji - overhead
    public class BadAsyncChain
    {
        public async Task<int> MethodAAsync()
        {
            return await MethodBAsync(); // ❌ Niepotrzebny await
        }

        public async Task<int> MethodBAsync()
        {
            return await MethodCAsync(); // ❌ Niepotrzebny await
        }

        public async Task<int> MethodCAsync()
        {
            await Task.Delay(100);
            return 42;
        }

        // Lepiej: zwrócić Task bezpośrednio bez await
        // public Task<int> MethodAAsync() => MethodBAsync();
    }

    // BŁĄD 3: Długie synchroniczne operacje w async method
    public class BadLongSyncInAsync
    {
        public async Task<string> ProcessAsync(string data)
        {
            // ❌ Długa synchroniczna operacja blokuje wątek
            for (int i = 0; i < 1000000; i++)
            {
                data += i.ToString(); // Bardzo wolne
            }

            await Task.Delay(100);
            return data;
            
            // Powinno być: await Task.Run(() => { /* sync work */ });
        }
    }

    // BŁĄD 4: Async methods w konstruktorze
    public class BadAsyncConstructor
    {
        private string _data;

        public BadAsyncConstructor()
        {
            // ❌ Nie można użyć await w konstruktorze
            InitializeAsync().Wait(); // Deadlock risk!
            
            // Alternatywa: async factory method lub lazy initialization
        }

        private async Task InitializeAsync()
        {
            await Task.Delay(1000);
            _data = "initialized";
        }
    }

    // BŁĄD 5: Niepoprawne użycie TaskCompletionSource
    public class BadTaskCompletionSource
    {
        public Task<string> GetDataAsync()
        {
            var tcs = new TaskCompletionSource<string>();

            Task.Run(() =>
            {
                try
                {
                    var result = ExpensiveOperation();
                    // ❌ Może być wywołane na innym wątku
                    tcs.SetResult(result);
                    // Powinno być: TrySetResult dla bezpieczeństwa
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            return tcs.Task;
        }

        private string ExpensiveOperation() => "result";
    }

    // BŁĄD 6: Async lambda bez error handling
    public class BadAsyncLambda
    {
        public void StartProcessing()
        {
            var timer = new System.Timers.Timer(1000);
            
            // ❌ Async lambda w event - exceptions nie są obsługiwane
            timer.Elapsed += async (sender, e) =>
            {
                await Task.Delay(500);
                throw new Exception("Error!"); // Unobserved exception
            };
            
            timer.Start();
        }
    }

    // BŁĄD 7: Task continuation z ExecuteSynchronously
    public class BadTaskContinuation
    {
        public Task ProcessAsync()
        {
            var task = Task.Run(() =>
            {
                Thread.Sleep(1000);
                return 42;
            });

            // ❌ ExecuteSynchronously może blokować calling thread
            return task.ContinueWith(t =>
            {
                Console.WriteLine(t.Result);
            }, TaskContinuationOptions.ExecuteSynchronously);
        }
    }

    // BŁĄD 8: Async state machine overhead dla prostych case'ów
    public class BadAsyncOverhead
    {
        private string _cachedData = "cached";

        public async Task<string> GetDataAsync()
        {
            // ❌ Async overhead gdy często zwracamy cached value
            if (_cachedData != null)
            {
                return _cachedData; // Zwraca Task z overhead
            }

            await Task.Delay(100);
            return "fetched";
            
            // Lepiej użyć ValueTask<T> lub Task.FromResult
        }
    }
}
