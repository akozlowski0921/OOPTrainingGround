using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SpecyfikaDotNet.Channels
{
    // ✅ GOOD: Porównanie wydajności różnych mechanizmów kolejkowania
    public class QueuePerformanceComparison
    {
        private const int ItemCount = 10000;
        private const int ProcessDelayMs = 1;

        // Test 1: Channel (UnboundedChannel)
        public async Task<TimeSpan> TestChannelAsync()
        {
            var channel = Channel.CreateUnbounded<int>();
            var sw = Stopwatch.StartNew();

            var producer = Task.Run(async () =>
            {
                for (int i = 0; i < ItemCount; i++)
                {
                    await channel.Writer.WriteAsync(i);
                }
                channel.Writer.Complete();
            });

            var consumer = Task.Run(async () =>
            {
                await foreach (var item in channel.Reader.ReadAllAsync())
                {
                    await Task.Delay(ProcessDelayMs);
                }
            });

            await Task.WhenAll(producer, consumer);
            sw.Stop();

            return sw.Elapsed;
        }

        // Test 2: ConcurrentQueue z polling
        public async Task<TimeSpan> TestConcurrentQueueAsync()
        {
            var queue = new ConcurrentQueue<int>();
            bool isCompleted = false;
            var sw = Stopwatch.StartNew();

            var producer = Task.Run(async () =>
            {
                for (int i = 0; i < ItemCount; i++)
                {
                    queue.Enqueue(i);
                    await Task.Yield(); // Minimal yield
                }
                isCompleted = true;
            });

            var consumer = Task.Run(async () =>
            {
                while (!isCompleted || !queue.IsEmpty)
                {
                    if (queue.TryDequeue(out int item))
                    {
                        await Task.Delay(ProcessDelayMs);
                    }
                    else
                    {
                        await Task.Delay(1); // Polling delay
                    }
                }
            });

            await Task.WhenAll(producer, consumer);
            sw.Stop();

            return sw.Elapsed;
        }

        // Test 3: BlockingCollection (synchroniczny)
        public TimeSpan TestBlockingCollection()
        {
            var collection = new BlockingCollection<int>(boundedCapacity: 100);
            var sw = Stopwatch.StartNew();

            var producer = Task.Run(() =>
            {
                for (int i = 0; i < ItemCount; i++)
                {
                    collection.Add(i);
                }
                collection.CompleteAdding();
            });

            var consumer = Task.Run(() =>
            {
                foreach (var item in collection.GetConsumingEnumerable())
                {
                    Thread.Sleep(ProcessDelayMs);
                }
            });

            Task.WaitAll(producer, consumer);
            sw.Stop();

            return sw.Elapsed;
        }
    }

    // ✅ GOOD: Szczegółowe porównanie funkcjonalności
    public class FeatureComparison
    {
        public class ComparisonResult
        {
            public string Name { get; set; }
            public bool SupportsAsync { get; set; }
            public bool HasBackpressure { get; set; }
            public bool ThreadSafe { get; set; }
            public bool SupportsCompletion { get; set; }
            public string PerformanceRating { get; set; }
            public string UseCases { get; set; }
        }

        public static List<ComparisonResult> GetComparison()
        {
            return new List<ComparisonResult>
            {
                new ComparisonResult
                {
                    Name = "Channel (Unbounded)",
                    SupportsAsync = true,
                    HasBackpressure = false,
                    ThreadSafe = true,
                    SupportsCompletion = true,
                    PerformanceRating = "Excellent",
                    UseCases = "Async producer-consumer, pipelines, high-performance"
                },
                new ComparisonResult
                {
                    Name = "Channel (Bounded)",
                    SupportsAsync = true,
                    HasBackpressure = true,
                    ThreadSafe = true,
                    SupportsCompletion = true,
                    PerformanceRating = "Excellent",
                    UseCases = "Backpressure, rate limiting, memory control"
                },
                new ComparisonResult
                {
                    Name = "ConcurrentQueue",
                    SupportsAsync = false,
                    HasBackpressure = false,
                    ThreadSafe = true,
                    SupportsCompletion = false,
                    PerformanceRating = "Good",
                    UseCases = "Simple thread-safe queue, sync scenarios"
                },
                new ComparisonResult
                {
                    Name = "BlockingCollection",
                    SupportsAsync = false,
                    HasBackpressure = true,
                    ThreadSafe = true,
                    SupportsCompletion = true,
                    PerformanceRating = "Good",
                    UseCases = "Legacy sync code, bounded queues"
                },
                new ComparisonResult
                {
                    Name = "Queue + lock",
                    SupportsAsync = false,
                    HasBackpressure = false,
                    ThreadSafe = false,
                    SupportsCompletion = false,
                    PerformanceRating = "Poor",
                    UseCases = "Simple single-threaded scenarios only"
                }
            };
        }

        public static void PrintComparison()
        {
            var results = GetComparison();

            Console.WriteLine("=== Queue Mechanisms Comparison ===\n");
            Console.WriteLine($"{"Mechanism",-25} {"Async",-8} {"Backpres",-10} {"Thread-Safe",-12} {"Completion",-12} {"Performance",-12}");
            Console.WriteLine(new string('-', 100));

            foreach (var result in results)
            {
                Console.WriteLine($"{result.Name,-25} {(result.SupportsAsync ? "Yes" : "No"),-8} {(result.HasBackpressure ? "Yes" : "No"),-10} {(result.ThreadSafe ? "Yes" : "No"),-12} {(result.SupportsCompletion ? "Yes" : "No"),-12} {result.PerformanceRating,-12}");
            }

            Console.WriteLine("\n=== Use Cases ===\n");
            foreach (var result in results)
            {
                Console.WriteLine($"{result.Name}:");
                Console.WriteLine($"  {result.UseCases}\n");
            }
        }
    }

    // ✅ GOOD: Benchmark runner
    public class BenchmarkRunner
    {
        public static async Task RunBenchmarksAsync()
        {
            Console.WriteLine("=== Performance Benchmarks ===");
            Console.WriteLine($"Items: {QueuePerformanceComparison.ItemCount:N0}");
            Console.WriteLine($"Process delay: {1}ms per item\n");

            var comparison = new QueuePerformanceComparison();

            // Warm-up
            await comparison.TestChannelAsync();

            // Benchmark 1: Channel
            Console.WriteLine("Running Channel benchmark...");
            var channelTime = await comparison.TestChannelAsync();
            Console.WriteLine($"Channel time: {channelTime.TotalMilliseconds:F2}ms\n");

            // Benchmark 2: ConcurrentQueue
            Console.WriteLine("Running ConcurrentQueue benchmark...");
            var queueTime = await comparison.TestConcurrentQueueAsync();
            Console.WriteLine($"ConcurrentQueue time: {queueTime.TotalMilliseconds:F2}ms\n");

            // Benchmark 3: BlockingCollection
            Console.WriteLine("Running BlockingCollection benchmark...");
            var blockingTime = comparison.TestBlockingCollection();
            Console.WriteLine($"BlockingCollection time: {blockingTime.TotalMilliseconds:F2}ms\n");

            // Summary
            Console.WriteLine("=== Summary ===");
            Console.WriteLine($"Channel:             {channelTime.TotalMilliseconds:F2}ms (baseline)");
            Console.WriteLine($"ConcurrentQueue:     {queueTime.TotalMilliseconds:F2}ms ({(queueTime.TotalMilliseconds / channelTime.TotalMilliseconds):F2}x)");
            Console.WriteLine($"BlockingCollection:  {blockingTime.TotalMilliseconds:F2}ms ({(blockingTime.TotalMilliseconds / channelTime.TotalMilliseconds):F2}x)");

            Console.WriteLine("\n=== Winner ===");
            var fastest = new[] 
            { 
                ("Channel", channelTime), 
                ("ConcurrentQueue", queueTime), 
                ("BlockingCollection", blockingTime) 
            }
            .OrderBy(x => x.Item2)
            .First();

            Console.WriteLine($"Fastest: {fastest.Item1} ({fastest.Item2.TotalMilliseconds:F2}ms)");

            Console.WriteLine("\n=== Recommendations ===");
            Console.WriteLine("✅ Use Channel for:");
            Console.WriteLine("   - Async/await scenarios");
            Console.WriteLine("   - Producer-consumer patterns");
            Console.WriteLine("   - Backpressure requirements");
            Console.WriteLine("   - Modern .NET applications");

            Console.WriteLine("\n⚠️  Use ConcurrentQueue for:");
            Console.WriteLine("   - Simple sync scenarios");
            Console.WriteLine("   - When you manage signaling yourself");
            Console.WriteLine("   - Legacy codebases");

            Console.WriteLine("\n⚠️  Use BlockingCollection for:");
            Console.WriteLine("   - Legacy sync code");
            Console.WriteLine("   - When migrating from older .NET");
            Console.WriteLine("   - Bounded queue without async");
        }
    }

    public class GoodQueueComparison
    {
        public static async Task Main()
        {
            // Feature comparison
            FeatureComparison.PrintComparison();

            Console.WriteLine("\n" + new string('=', 100) + "\n");

            // Performance benchmarks
            await BenchmarkRunner.RunBenchmarksAsync();

            Console.WriteLine("\n=== Conclusion ===");
            Console.WriteLine("System.Threading.Channels is the modern, async-first choice for:");
            Console.WriteLine("- Producer-consumer patterns");
            Console.WriteLine("- Pipelines and dataflows");
            Console.WriteLine("- Backpressure and rate limiting");
            Console.WriteLine("- High-performance async scenarios");
            Console.WriteLine("\nIt combines the best of async/await with thread-safe queuing!");
        }
    }
}
