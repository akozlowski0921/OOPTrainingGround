using System;
using System.Threading;
using System.Threading.Tasks;

namespace SpecyfikaDotNet.Cancellation.Good3
{
    // ✅ GOOD: Advanced cancellation patterns

    // ✅ Prawidłowy loop z cancellation
    public class GoodLoopCancellation
    {
        public async Task ProcessLoopAsync(CancellationToken ct = default)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(100, ct);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Loop cancelled gracefully");
                    break;
                }
            }
        }
    }

    // ✅ Periodic cancellation checking w sync code
    public class GoodSyncCancellation
    {
        public void ProcessSync(CancellationToken ct = default)
        {
            for (int i = 0; i < 1000000; i++)
            {
                if (i % 100 == 0) // Check co 100 iteracji
                    ct.ThrowIfCancellationRequested();
                
                HeavyOperation();
            }
        }

        private void HeavyOperation() => Thread.Sleep(10);
    }

    // ✅ Rozróżnienie timeout vs cancellation
    public class GoodTimeoutHandling
    {
        public async Task<string> FetchWithTimeoutAsync(
            string url,
            CancellationToken ct = default)
        {
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                ct, timeoutCts.Token);
            
            try
            {
                return await FetchDataAsync(url, linkedCts.Token);
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
            {
                throw new TimeoutException("Request timed out");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("User cancelled");
                throw;
            }
        }

        private async Task<string> FetchDataAsync(string url, CancellationToken ct)
        {
            await Task.Delay(10000, ct);
            return "data";
        }
    }
}
