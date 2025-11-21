using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SpecyfikaDotNet.Channels
{
    // ✅ GOOD: Bounded channel zapewnia backpressure
    public class GoodBackpressure
    {
        private readonly Channel<string> _channel;

        public GoodBackpressure(int capacity)
        {
            // BoundedChannel automatycznie zarządza backpressure
            _channel = Channel.CreateBounded<string>(new BoundedChannelOptions(capacity)
            {
                // Wait - producer czeka gdy channel jest pełny
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = false,
                SingleWriter = false
            });
        }

        public async Task FastProducerAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                for (int i = 0; i < 100000; i++)
                {
                    // WriteAsync będzie czekać gdy channel jest pełny (backpressure!)
                    await _channel.Writer.WriteAsync($"Message {i}", cancellationToken);

                    if (i % 1000 == 0)
                    {
                        Console.WriteLine($"Produced: {i} messages");
                    }
                }
            }
            finally
            {
                _channel.Writer.Complete();
            }
        }

        public async Task SlowConsumerAsync(CancellationToken cancellationToken = default)
        {
            int count = 0;

            await foreach (var item in _channel.Reader.ReadAllAsync(cancellationToken))
            {
                // Wolne przetwarzanie
                await Task.Delay(10, cancellationToken);
                count++;

                if (count % 100 == 0)
                {
                    Console.WriteLine($"Consumed: {count} messages");
                }
            }

            Console.WriteLine($"Total consumed: {count}");
        }
    }

    // ✅ GOOD: Throttling z Channels
    public class GoodThrottling
    {
        private readonly int _maxConcurrency;

        public GoodThrottling(int maxConcurrency)
        {
            _maxConcurrency = maxConcurrency;
        }

        public async Task ProcessManyItemsAsync(List<string> items, CancellationToken cancellationToken = default)
        {
            // Channel jako kolejka zadań z ograniczoną pojemnością
            var channel = Channel.CreateBounded<string>(new BoundedChannelOptions(_maxConcurrency)
            {
                FullMode = BoundedChannelFullMode.Wait
            });

            // Producer - dodaje items do channel
            var producer = Task.Run(async () =>
            {
                foreach (var item in items)
                {
                    await channel.Writer.WriteAsync(item, cancellationToken);
                }
                channel.Writer.Complete();
            }, cancellationToken);

            // Consumers - ograniczona liczba równoczesnych workerów
            var consumers = new Task[_maxConcurrency];
            for (int i = 0; i < _maxConcurrency; i++)
            {
                int workerId = i;
                consumers[i] = Task.Run(async () =>
                {
                    await foreach (var item in channel.Reader.ReadAllAsync(cancellationToken))
                    {
                        await ProcessItemAsync(workerId, item, cancellationToken);
                    }
                }, cancellationToken);
            }

            await producer;
            await Task.WhenAll(consumers);
        }

        private async Task ProcessItemAsync(int workerId, string item, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Worker {workerId} processing: {item}");
            await Task.Delay(100, cancellationToken); // Symulacja pracy
        }
    }

    // ✅ GOOD: Rate limiting z Channels
    public class GoodRateLimiting
    {
        private readonly Channel<string> _channel;
        private readonly int _itemsPerSecond;

        public GoodRateLimiting(int capacity, int itemsPerSecond)
        {
            _channel = Channel.CreateBounded<string>(capacity);
            _itemsPerSecond = itemsPerSecond;
        }

        public async Task ProduceWithRateLimitAsync(List<string> items, CancellationToken cancellationToken = default)
        {
            int delayMs = 1000 / _itemsPerSecond;

            try
            {
                foreach (var item in items)
                {
                    await _channel.Writer.WriteAsync(item, cancellationToken);
                    
                    // Rate limiting - max itemsPerSecond
                    await Task.Delay(delayMs, cancellationToken);
                }
            }
            finally
            {
                _channel.Writer.Complete();
            }
        }

        public async Task ConsumeAsync(CancellationToken cancellationToken = default)
        {
            await foreach (var item in _channel.Reader.ReadAllAsync(cancellationToken))
            {
                Console.WriteLine($"Processing: {item}");
                await Task.Delay(10, cancellationToken);
            }
        }
    }

    // ✅ GOOD: Dynamic backpressure z metrics
    public class GoodDynamicBackpressure
    {
        private readonly Channel<WorkItem> _channel;
        private int _channelCount = 0;

        public GoodDynamicBackpressure(int capacity)
        {
            _channel = Channel.CreateBounded<WorkItem>(new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            });
        }

        public async Task ProducerWithMetricsAsync(CancellationToken cancellationToken = default)
        {
            for (int i = 0; i < 1000; i++)
            {
                var item = new WorkItem { Id = i, Data = $"Item {i}" };

                await _channel.Writer.WriteAsync(item, cancellationToken);
                Interlocked.Increment(ref _channelCount);

                // Logowanie gdy channel się zapełnia
                int currentCount = _channelCount;
                if (currentCount > 80)
                {
                    Console.WriteLine($"WARNING: Channel nearly full ({currentCount} items)");
                }

                if (i % 100 == 0)
                {
                    Console.WriteLine($"Produced {i} items, channel count: {currentCount}");
                }

                await Task.Delay(5, cancellationToken);
            }

            _channel.Writer.Complete();
        }

        public async Task ConsumerWithMetricsAsync(int consumerId, CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();
            int processed = 0;

            await foreach (var item in _channel.Reader.ReadAllAsync(cancellationToken))
            {
                await Task.Delay(10, cancellationToken);
                processed++;
                Interlocked.Decrement(ref _channelCount);

                if (processed % 50 == 0)
                {
                    double rate = processed / sw.Elapsed.TotalSeconds;
                    Console.WriteLine($"Consumer {consumerId}: {processed} items, rate: {rate:F2} items/sec");
                }
            }

            Console.WriteLine($"Consumer {consumerId} finished: {processed} items in {sw.Elapsed.TotalSeconds:F2}s");
        }
    }

    public class WorkItem
    {
        public int Id { get; set; }
        public string Data { get; set; }
    }

    public class GoodBackpressureUsage
    {
        public static async Task Main()
        {
            Console.WriteLine("=== Good Example 1: Backpressure ===\n");
            await RunBackpressureDemo();

            Console.WriteLine("\n=== Good Example 2: Throttling ===\n");
            await RunThrottlingDemo();

            Console.WriteLine("\n=== Good Example 3: Rate Limiting ===\n");
            await RunRateLimitingDemo();

            Console.WriteLine("\n=== Good Example 4: Dynamic Backpressure ===\n");
            await RunDynamicBackpressureDemo();
        }

        private static async Task RunBackpressureDemo()
        {
            var pipeline = new GoodBackpressure(capacity: 100);
            var cts = new CancellationTokenSource();

            var producer = pipeline.FastProducerAsync(cts.Token);
            var consumer = pipeline.SlowConsumerAsync(cts.Token);

            await Task.WhenAll(producer, consumer);
            Console.WriteLine("Backpressure demo completed");
        }

        private static async Task RunThrottlingDemo()
        {
            var items = new List<string>();
            for (int i = 0; i < 100; i++)
            {
                items.Add($"Item {i}");
            }

            var throttling = new GoodThrottling(maxConcurrency: 5);
            await throttling.ProcessManyItemsAsync(items);
            Console.WriteLine("Throttling demo completed");
        }

        private static async Task RunRateLimitingDemo()
        {
            var items = new List<string>();
            for (int i = 0; i < 20; i++)
            {
                items.Add($"Item {i}");
            }

            var rateLimiter = new GoodRateLimiting(capacity: 10, itemsPerSecond: 5);
            var cts = new CancellationTokenSource();

            var producer = rateLimiter.ProduceWithRateLimitAsync(items, cts.Token);
            var consumer = rateLimiter.ConsumeAsync(cts.Token);

            await Task.WhenAll(producer, consumer);
            Console.WriteLine("Rate limiting demo completed");
        }

        private static async Task RunDynamicBackpressureDemo()
        {
            var pipeline = new GoodDynamicBackpressure(capacity: 100);
            var cts = new CancellationTokenSource();

            var producer = pipeline.ProducerWithMetricsAsync(cts.Token);
            var consumer1 = pipeline.ConsumerWithMetricsAsync(1, cts.Token);
            var consumer2 = pipeline.ConsumerWithMetricsAsync(2, cts.Token);

            await Task.WhenAll(producer, consumer1, consumer2);
            Console.WriteLine("Dynamic backpressure demo completed");
        }
    }
}
