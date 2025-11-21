using System;
using System.Threading;
using System.Threading.Tasks;

namespace SpecyfikaDotNet.Cancellation.Bad2
{
    // ❌ BAD: CancellationToken anti-patterns część 2

    // BŁĄD 1: Przekazywanie CancellationToken.None wszędzie
    public class BadTokenUsage
    {
        public async Task ProcessAsync()
        {
            // ❌ Używa CancellationToken.None - nie można anulować
            await LongOperationAsync(CancellationToken.None);
        }

        private async Task LongOperationAsync(CancellationToken ct)
        {
            for (int i = 0; i < 100; i++)
            {
                await Task.Delay(100, ct);
            }
        }
    }

    // BŁĄD 2: Tworzenie zbyt wielu CancellationTokenSource
    public class BadTokenSourceCreation
    {
        public async Task ProcessItemsAsync(int[] items)
        {
            foreach (var item in items)
            {
                // ❌ Nowy CTS dla każdego item
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                await ProcessItemAsync(item, cts.Token);
            }
        }

        private Task ProcessItemAsync(int item, CancellationToken ct) =>
            Task.Delay(100, ct);
    }

    // BŁĄD 3: Nie używanie Register dla cleanup
    public class BadCleanupHandling
    {
        public async Task ProcessWithResourceAsync(CancellationToken ct)
        {
            var resource = new DisposableResource();
            
            // ❌ Brak cleanup przy cancellation
            await Task.Delay(5000, ct);
            
            resource.Dispose(); // Nie wykona się przy cancellation!
        }
    }

    class DisposableResource : IDisposable
    {
        public void Dispose() => Console.WriteLine("Disposed");
    }
}
