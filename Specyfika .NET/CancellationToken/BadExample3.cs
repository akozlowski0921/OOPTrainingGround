using System;
using System.Threading;
using System.Threading.Tasks;

namespace SpecyfikaDotNet.Cancellation.Bad3
{
    // ❌ BAD: Więcej problemów z CancellationToken

    // BŁĄD 1: Ignorowanie IsCancellationRequested w pętlach
    public class BadLoopCancellation
    {
        public async Task ProcessLoopAsync(CancellationToken ct)
        {
            while (true) // ❌ Infinite loop bez checking
            {
                await Task.Delay(100);
                // Nigdy nie sprawdza ct.IsCancellationRequested
            }
        }
    }

    // BŁĄD 2: Synchroniczne operacje bez cancellation support
    public class BadSyncCancellation
    {
        public void ProcessSync(CancellationToken ct)
        {
            for (int i = 0; i < 1000000; i++)
            {
                HeavyOperation();
                // ❌ Nie sprawdza cancellation
            }
        }

        private void HeavyOperation() => Thread.Sleep(10);
    }

    // BŁĄD 3: Mieszanie cancellation z timeout
    public class BadTimeoutMixing
    {
        public async Task<string> FetchWithTimeoutAsync(string url)
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            
            try
            {
                return await FetchDataAsync(url, cts.Token);
            }
            catch (OperationCanceledException)
            {
                // ❌ Nie wiadomo czy timeout czy user cancellation
                throw new Exception("Operation failed");
            }
        }

        private async Task<string> FetchDataAsync(string url, CancellationToken ct)
        {
            await Task.Delay(10000, ct);
            return "data";
        }
    }
}
