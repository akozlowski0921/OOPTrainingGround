using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SpecyfikaDotNet.Channels
{
    // ❌ BAD: Używanie Queue z lock - nieefektywne i podatne na błędy
    public class BadProducerConsumer
    {
        private readonly Queue<int> _queue = new();
        private readonly object _lock = new();
        private bool _isCompleted = false;

        public async Task ProduceAsync()
        {
            for (int i = 0; i < 100; i++)
            {
                // Ręczne lockowanie - ryzyko deadlock
                lock (_lock)
                {
                    _queue.Enqueue(i);
                    Monitor.Pulse(_lock); // Ręczne sygnalizowanie
                }

                await Task.Delay(10); // Symulacja pracy
            }

            lock (_lock)
            {
                _isCompleted = true;
                Monitor.PulseAll(_lock);
            }
        }

        public async Task ConsumeAsync()
        {
            while (true)
            {
                int item;
                bool hasItem = false;

                lock (_lock)
                {
                    // Oczekiwanie na dane - blokuje wątek!
                    while (_queue.Count == 0 && !_isCompleted)
                    {
                        Monitor.Wait(_lock); // Blokujące oczekiwanie
                    }

                    if (_queue.Count > 0)
                    {
                        item = _queue.Dequeue();
                        hasItem = true;
                    }
                    else if (_isCompleted)
                    {
                        break;
                    }
                }

                if (hasItem)
                {
                    await ProcessItemAsync(item);
                }
            }
        }

        private async Task ProcessItemAsync(int item)
        {
            await Task.Delay(5);
            Console.WriteLine($"Processed: {item}");
        }
    }

    // ❌ BAD: Brak obsługi wielu producentów i konsumentów
    public class BadMultipleProducersConsumers
    {
        private readonly List<int> _sharedList = new();
        private readonly object _lock = new();

        public async Task ProducerAsync(int producerId)
        {
            for (int i = 0; i < 10; i++)
            {
                lock (_lock)
                {
                    _sharedList.Add(i * 100 + producerId);
                }
                await Task.Delay(10);
            }
        }

        public async Task ConsumerAsync()
        {
            // Problem: nie ma dobrego sposobu na czekanie na nowe elementy
            while (true)
            {
                int? item = null;

                lock (_lock)
                {
                    if (_sharedList.Count > 0)
                    {
                        item = _sharedList[0];
                        _sharedList.RemoveAt(0); // Nieefektywne RemoveAt(0)
                    }
                }

                if (item.HasValue)
                {
                    Console.WriteLine($"Consumed: {item}");
                }
                else
                {
                    // Busy waiting - marnuje CPU
                    await Task.Delay(10);
                }
            }
        }
    }

    public class BadChannelUsage
    {
        public static async Task Main()
        {
            Console.WriteLine("=== Bad Example: Manual synchronization ===");

            var bad = new BadProducerConsumer();

            var producer = Task.Run(() => bad.ProduceAsync());
            var consumer = Task.Run(() => bad.ConsumeAsync());

            await Task.WhenAll(producer, consumer);

            // Problemy:
            // - Ręczne lockowanie - ryzyko deadlock
            // - Monitor.Wait blokuje wątek
            // - Trudne w utrzymaniu
            // - Podatne na race conditions
            // - Brak backpressure
            // - Nieefektywne busy waiting
        }
    }
}
