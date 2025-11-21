using System;
using System.Threading;
using System.Threading.Tasks;

namespace SpecyfikaDotNet.Cancellation.Good2
{
    // ✅ GOOD: Proper CancellationToken patterns

    // ✅ Przyjmuj token jako optional parameter
    public class GoodTokenUsage
    {
        public async Task ProcessAsync(CancellationToken ct = default)
        {
            await LongOperationAsync(ct);
        }

        private async Task LongOperationAsync(CancellationToken ct = default)
        {
            for (int i = 0; i < 100; i++)
            {
                ct.ThrowIfCancellationRequested();
                await Task.Delay(100, ct);
            }
        }
    }

    // ✅ Reuse CancellationTokenSource
    public class GoodTokenSourceUsage
    {
        private CancellationTokenSource _cts;

        public async Task ProcessItemsAsync(int[] items)
        {
            _cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            
            try
            {
                foreach (var item in items)
                {
                    await ProcessItemAsync(item, _cts.Token);
                }
            }
            finally
            {
                _cts?.Dispose();
            }
        }

        private Task ProcessItemAsync(int item, CancellationToken ct) =>
            Task.Delay(100, ct);

        public void Cancel() => _cts?.Cancel();
    }

    // ✅ Register dla cleanup
    public class GoodCleanupHandling
    {
        public async Task ProcessWithResourceAsync(CancellationToken ct = default)
        {
            var resource = new DisposableResource();
            
            // ✅ Cleanup przy cancellation
            using var registration = ct.Register(() => resource.Dispose());
            
            await Task.Delay(5000, ct);
            resource.Dispose();
        }
    }

    class DisposableResource : IDisposable
    {
        public void Dispose() => Console.WriteLine("Disposed");
    }
}
