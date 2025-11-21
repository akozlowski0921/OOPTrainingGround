using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SpecyfikaDotNet.Channels
{
    // ❌ BAD: Nieefektywne użycie ConcurrentQueue bez sygnalizacji
    public class BadConcurrentQueueUsage
    {
        private readonly ConcurrentQueue<int> _queue = new();
        private bool _isCompleted = false;

        public async Task ProducerAsync()
        {
            for (int i = 0; i < 1000; i++)
            {
                _queue.Enqueue(i);
                await Task.Delay(1); // Symulacja pracy
            }
            _isCompleted = true;
        }

        public async Task ConsumerAsync()
        {
            // Busy waiting - marnuje CPU
            while (!_isCompleted || !_queue.IsEmpty)
            {
                if (_queue.TryDequeue(out int item))
                {
                    await ProcessAsync(item);
                }
                else
                {
                    // Czekanie przez polling - nieefektywne
                    await Task.Delay(10);
                }
            }
        }

        private async Task ProcessAsync(int item)
        {
            await Task.Delay(5);
        }
    }

    // ❌ BAD: BlockingCollection z synchronicznym blokowaniem
    public class BadBlockingCollectionUsage
    {
        private readonly BlockingCollection<int> _collection = new(boundedCapacity: 100);

        public void ProducerSync()
        {
            for (int i = 0; i < 1000; i++)
            {
                // Synchroniczne blokowanie - blokuje wątek
                _collection.Add(i);
                Thread.Sleep(1);
            }
            _collection.CompleteAdding();
        }

        public void ConsumerSync()
        {
            // GetConsumingEnumerable blokuje wątek
            foreach (var item in _collection.GetConsumingEnumerable())
            {
                // Synchroniczne przetwarzanie
                Thread.Sleep(5);
                Console.WriteLine($"Processed: {item}");
            }
        }
    }

    // ❌ BAD: Ręczne zarządzanie SemaphoreSlim jako kolejka
    public class BadSemaphoreQueueUsage
    {
        private readonly Queue<int> _queue = new();
        private readonly SemaphoreSlim _semaphore = new(0);
        private readonly object _lock = new();

        public async Task ProducerAsync()
        {
            for (int i = 0; i < 100; i++)
            {
                lock (_lock)
                {
                    _queue.Enqueue(i);
                }
                _semaphore.Release(); // Sygnalizuj nowy element
                await Task.Delay(10);
            }
        }

        public async Task ConsumerAsync()
        {
            for (int i = 0; i < 100; i++)
            {
                await _semaphore.WaitAsync(); // Czekaj na element

                int item;
                lock (_lock)
                {
                    item = _queue.Dequeue();
                }

                await ProcessAsync(item);
            }
        }

        private async Task ProcessAsync(int item)
        {
            await Task.Delay(5);
        }
    }

    public class BadQueueComparison
    {
        public static async Task Main()
        {
            Console.WriteLine("=== Bad Examples: Queue alternatives ===\n");

            // ConcurrentQueue - busy waiting
            Console.WriteLine("1. ConcurrentQueue with busy waiting:");
            var bad1 = new BadConcurrentQueueUsage();
            var p1 = bad1.ProducerAsync();
            var c1 = bad1.ConsumerAsync();
            await Task.WhenAll(p1, c1);

            // BlockingCollection - synchroniczne blokowanie wątków
            Console.WriteLine("\n2. BlockingCollection (sync blocking):");
            var bad2 = new BadBlockingCollectionUsage();
            var p2 = Task.Run(() => bad2.ProducerSync());
            var c2 = Task.Run(() => bad2.ConsumerSync());
            await Task.WhenAll(p2, c2);

            // Problemy:
            // - ConcurrentQueue: brak async, busy waiting
            // - BlockingCollection: blokuje wątki, nie async/await
            // - Ręczny SemaphoreSlim: skomplikowany, podatny na błędy
            // - Wszystkie: trudne w debugowaniu i utrzymaniu
        }
    }
}
