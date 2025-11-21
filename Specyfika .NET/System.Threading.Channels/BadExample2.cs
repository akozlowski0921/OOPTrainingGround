using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SpecyfikaDotNet.Channels
{
    // ❌ BAD: Brak backpressure - producer może zalewać consumer
    public class BadNoBackpressure
    {
        private readonly Queue<string> _queue = new();
        private readonly object _lock = new();

        public async Task FastProducerAsync()
        {
            // Producer generuje dane szybciej niż consumer je przetwarza
            for (int i = 0; i < 100000; i++)
            {
                lock (_lock)
                {
                    // Brak limitu - kolejka rośnie w nieskończoność
                    _queue.Enqueue($"Message {i}");
                }

                // Bardzo szybka produkcja - bez delay
                if (i % 1000 == 0)
                {
                    Console.WriteLine($"Queue size: {_queue.Count}");
                }
            }

            Console.WriteLine($"Final queue size: {_queue.Count}");
        }

        public async Task SlowConsumerAsync()
        {
            while (true)
            {
                string item = null;

                lock (_lock)
                {
                    if (_queue.Count > 0)
                    {
                        item = _queue.Dequeue();
                    }
                }

                if (item != null)
                {
                    // Wolne przetwarzanie
                    await Task.Delay(10);
                    // Console.WriteLine($"Processed: {item}");
                }
                else
                {
                    await Task.Delay(10);
                }
            }
        }
    }

    // ❌ BAD: Brak throttling - zbyt wiele równoczesnych operacji
    public class BadNoThrottling
    {
        public async Task ProcessManyItemsAsync(List<string> items)
        {
            // Uruchomienie wszystkich tasków naraz - może przytłoczyć system
            var tasks = new List<Task>();

            foreach (var item in items)
            {
                // Brak limitu równoczesnych operacji
                tasks.Add(ProcessItemAsync(item));
            }

            // Jeśli mamy 10000 items, uruchomi się 10000 tasków naraz!
            await Task.WhenAll(tasks);
        }

        private async Task ProcessItemAsync(string item)
        {
            // Symulacja kosztownej operacji (np. HTTP request)
            await Task.Delay(100);
            Console.WriteLine($"Processed: {item}");
        }
    }

    public class BadBackpressureUsage
    {
        public static async Task Main()
        {
            Console.WriteLine("=== Bad Example: No backpressure ===");

            var bad = new BadNoBackpressure();

            var producer = bad.FastProducerAsync();
            var consumer = bad.SlowConsumerAsync();

            // Producer szybko zapełnia pamięć
            // Consumer nie nadąża
            // Brak mechanizmu backpressure - producer nie zwalnia

            // Po pewnym czasie: OutOfMemoryException lub bardzo wysoka latencja

            // W praktycznym scenariuszu:
            var items = new List<string>();
            for (int i = 0; i < 10000; i++)
            {
                items.Add($"Item {i}");
            }

            var noThrottle = new BadNoThrottling();
            
            // To uruchomi 10000 tasków jednocześnie - źle!
            // await noThrottle.ProcessManyItemsAsync(items);
            
            Console.WriteLine("No backpressure demo completed");
        }
    }
}
