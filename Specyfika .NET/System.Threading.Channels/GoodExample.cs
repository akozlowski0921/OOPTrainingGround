using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SpecyfikaDotNet.Channels
{
    // ✅ GOOD: Używanie System.Threading.Channels - efektywne i bezpieczne
    public class GoodProducerConsumer
    {
        private readonly Channel<int> _channel;

        public GoodProducerConsumer(int capacity = 100)
        {
            // UnboundedChannel - nieograniczona pojemność
            // Alternatywnie: BoundedChannel dla backpressure
            _channel = Channel.CreateUnbounded<int>(new UnboundedChannelOptions
            {
                SingleReader = false, // Wiele konsumentów
                SingleWriter = false  // Wiele producentów
            });
        }

        public async Task ProduceAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                for (int i = 0; i < 100; i++)
                {
                    // Async write - nie blokuje wątku
                    await _channel.Writer.WriteAsync(i, cancellationToken);
                    Console.WriteLine($"Produced: {i}");
                    await Task.Delay(10, cancellationToken);
                }
            }
            finally
            {
                // Sygnalizuje zakończenie - konsumenci wiedzą kiedy skończyć
                _channel.Writer.Complete();
            }
        }

        public async Task ConsumeAsync(int consumerId, CancellationToken cancellationToken = default)
        {
            try
            {
                // Async enumeration - czeka na dane bez blokowania
                await foreach (var item in _channel.Reader.ReadAllAsync(cancellationToken))
                {
                    await ProcessItemAsync(consumerId, item, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Consumer {consumerId} cancelled");
            }
        }

        private async Task ProcessItemAsync(int consumerId, int item, CancellationToken cancellationToken)
        {
            await Task.Delay(5, cancellationToken);
            Console.WriteLine($"Consumer {consumerId} processed: {item}");
        }

        // Alternatywna metoda konsumpcji z TryRead
        public async Task ConsumeWithTryReadAsync(int consumerId, CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // Non-blocking TryRead
                if (_channel.Reader.TryRead(out int item))
                {
                    await ProcessItemAsync(consumerId, item, cancellationToken);
                }
                else
                {
                    // Czekaj na dostępność danych
                    if (await _channel.Reader.WaitToReadAsync(cancellationToken))
                    {
                        continue;
                    }
                    else
                    {
                        // Channel completed
                        break;
                    }
                }
            }
        }
    }

    // ✅ GOOD: Bounded channel dla wielu producentów i konsumentów
    public class GoodMultipleProducersConsumers
    {
        private readonly Channel<WorkItem> _channel;

        public GoodMultipleProducersConsumers(int capacity)
        {
            // BoundedChannel - ograniczona pojemność dla backpressure
            _channel = Channel.CreateBounded<WorkItem>(new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait, // Wait gdy pełny
                SingleReader = false,
                SingleWriter = false
            });
        }

        public async Task ProducerAsync(int producerId, int itemCount, CancellationToken cancellationToken = default)
        {
            try
            {
                for (int i = 0; i < itemCount; i++)
                {
                    var item = new WorkItem
                    {
                        Id = i,
                        ProducerId = producerId,
                        Data = $"Data from producer {producerId}, item {i}"
                    };

                    // WriteAsync będzie czekać jeśli channel jest pełny (backpressure)
                    await _channel.Writer.WriteAsync(item, cancellationToken);
                    Console.WriteLine($"Producer {producerId} wrote item {i}");
                    
                    await Task.Delay(Random.Shared.Next(5, 15), cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Producer {producerId} cancelled");
            }
        }

        public async Task ConsumerAsync(int consumerId, CancellationToken cancellationToken = default)
        {
            try
            {
                await foreach (var item in _channel.Reader.ReadAllAsync(cancellationToken))
                {
                    Console.WriteLine($"Consumer {consumerId} processing: {item.Data}");
                    await Task.Delay(Random.Shared.Next(10, 30), cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Consumer {consumerId} cancelled");
            }

            Console.WriteLine($"Consumer {consumerId} completed");
        }

        public void CompleteProduction()
        {
            _channel.Writer.Complete();
        }
    }

    public class WorkItem
    {
        public int Id { get; set; }
        public int ProducerId { get; set; }
        public string Data { get; set; }
    }

    public class GoodChannelUsage
    {
        public static async Task Main()
        {
            Console.WriteLine("=== Good Example 1: Single producer, single consumer ===\n");
            await RunSingleProducerConsumer();

            Console.WriteLine("\n=== Good Example 2: Multiple producers, multiple consumers ===\n");
            await RunMultipleProducersConsumers();
        }

        private static async Task RunSingleProducerConsumer()
        {
            var pipeline = new GoodProducerConsumer();
            var cts = new CancellationTokenSource();

            var producer = pipeline.ProduceAsync(cts.Token);
            var consumer = pipeline.ConsumeAsync(1, cts.Token);

            await Task.WhenAll(producer, consumer);

            Console.WriteLine("Single producer-consumer completed");
        }

        private static async Task RunMultipleProducersConsumers()
        {
            const int producerCount = 3;
            const int consumerCount = 2;
            const int itemsPerProducer = 10;

            var pipeline = new GoodMultipleProducersConsumers(capacity: 10);
            var cts = new CancellationTokenSource();

            // Start producers
            var producers = new Task[producerCount];
            for (int i = 0; i < producerCount; i++)
            {
                int producerId = i;
                producers[i] = pipeline.ProducerAsync(producerId, itemsPerProducer, cts.Token);
            }

            // Start consumers
            var consumers = new Task[consumerCount];
            for (int i = 0; i < consumerCount; i++)
            {
                int consumerId = i;
                consumers[i] = pipeline.ConsumerAsync(consumerId, cts.Token);
            }

            // Wait for all producers to complete
            await Task.WhenAll(producers);
            
            // Signal completion to consumers
            pipeline.CompleteProduction();

            // Wait for all consumers to complete
            await Task.WhenAll(consumers);

            Console.WriteLine("\nMultiple producers-consumers completed");

            // Korzyści:
            // ✅ Thread-safe bez explicit locks
            // ✅ Async/await friendly
            // ✅ Backpressure support (bounded channels)
            // ✅ Completion signaling
            // ✅ Wysokowydajne
            // ✅ Łatwe w użyciu
        }
    }
}
