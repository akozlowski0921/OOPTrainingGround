using System;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SpecyfikaDotNet.AsyncPatterns
{
    // ✅ GOOD: Używanie ValueTask dla redukcji memory pressure
    public class GoodMemoryPressure
    {
        public async Task<int> ProcessManyItemsWithValueTask()
        {
            int sum = 0;

            // ValueTask - znacznie mniej alokacji!
            for (int i = 0; i < 1_000_000; i++)
            {
                // ValueTask to value type - może uniknąć alokacji heap
                int result = await GetValueWithValueTask(i);
                sum += result;
            }

            return sum;
        }

        private ValueTask<int> GetValueWithValueTask(int value)
        {
            // Większość przypadków zwraca od razu - ZERO alokacji!
            if (value % 100 != 0)
            {
                // ValueTask opakowuje wartość bez alokacji heap
                return new ValueTask<int>(value * 2);
            }

            // Tylko 1% przypadków faktycznie async - alokuje Task
            return GetValueSlowPath(value);
        }

        private async ValueTask<int> GetValueSlowPath(int value)
        {
            await Task.Delay(1);
            return value * 2;
        }

        public async Task RunBenchmark()
        {
            var sw = Stopwatch.StartNew();
            
            // Sprawdź pamięć przed
            long memBefore = GC.GetTotalMemory(true);
            
            await ProcessManyItemsWithValueTask();
            
            // Sprawdź pamięć po
            long memAfter = GC.GetTotalMemory(false);
            
            sw.Stop();
            
            Console.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");
            Console.WriteLine($"Memory used: {(memAfter - memBefore) / 1024.0 / 1024.0:F2} MB");
            Console.WriteLine($"GC Collections (Gen 0/1/2): {GC.CollectionCount(0)}/{GC.CollectionCount(1)}/{GC.CollectionCount(2)}");
        }
    }

    // Porównanie performance Task vs ValueTask
    public class PerformanceComparison
    {
        // Benchmark dla Task
        public async Task<long> BenchmarkTask(int iterations)
        {
            var sw = Stopwatch.StartNew();
            long sum = 0;

            for (int i = 0; i < iterations; i++)
            {
                sum += await GetWithTask(i);
            }

            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        // Benchmark dla ValueTask
        public async Task<long> BenchmarkValueTask(int iterations)
        {
            var sw = Stopwatch.StartNew();
            long sum = 0;

            for (int i = 0; i < iterations; i++)
            {
                sum += await GetWithValueTask(i);
            }

            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        private async Task<int> GetWithTask(int value)
        {
            // 90% fast path
            if (value % 10 != 0)
                return value;

            await Task.Yield();
            return value;
        }

        private ValueTask<int> GetWithValueTask(int value)
        {
            // 90% fast path - zero alokacji
            if (value % 10 != 0)
                return new ValueTask<int>(value);

            return GetWithValueTaskSlow(value);
        }

        private async ValueTask<int> GetWithValueTaskSlow(int value)
        {
            await Task.Yield();
            return value;
        }

        public async Task RunComparison()
        {
            const int iterations = 100_000;
            
            Console.WriteLine($"=== Performance Comparison ({iterations:N0} iterations) ===\n");

            // Warm-up
            await BenchmarkTask(1000);
            await BenchmarkValueTask(1000);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            // Task benchmark
            Console.WriteLine("--- Task ---");
            long memBeforeTask = GC.GetTotalMemory(true);
            int gc0BeforeTask = GC.CollectionCount(0);
            int gc1BeforeTask = GC.CollectionCount(1);
            int gc2BeforeTask = GC.CollectionCount(2);

            long timeTask = await BenchmarkTask(iterations);

            long memAfterTask = GC.GetTotalMemory(false);
            Console.WriteLine($"Time: {timeTask}ms");
            Console.WriteLine($"Memory: {(memAfterTask - memBeforeTask) / 1024.0:F2} KB");
            Console.WriteLine($"GC (Gen 0/1/2): {GC.CollectionCount(0) - gc0BeforeTask}/{GC.CollectionCount(1) - gc1BeforeTask}/{GC.CollectionCount(2) - gc2BeforeTask}");

            // Cleanup between benchmarks
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            await Task.Delay(100);

            // ValueTask benchmark
            Console.WriteLine("\n--- ValueTask ---");
            long memBeforeValueTask = GC.GetTotalMemory(true);
            int gc0BeforeValueTask = GC.CollectionCount(0);
            int gc1BeforeValueTask = GC.CollectionCount(1);
            int gc2BeforeValueTask = GC.CollectionCount(2);

            long timeValueTask = await BenchmarkValueTask(iterations);

            long memAfterValueTask = GC.GetTotalMemory(false);
            Console.WriteLine($"Time: {timeValueTask}ms");
            Console.WriteLine($"Memory: {(memAfterValueTask - memBeforeValueTask) / 1024.0:F2} KB");
            Console.WriteLine($"GC (Gen 0/1/2): {GC.CollectionCount(0) - gc0BeforeValueTask}/{GC.CollectionCount(1) - gc1BeforeValueTask}/{GC.CollectionCount(2) - gc2BeforeValueTask}");

            // Summary
            Console.WriteLine("\n=== Summary ===");
            double timeImprovement = ((double)(timeTask - timeValueTask) / timeTask) * 100;
            long memoryDiff = (memAfterTask - memBeforeTask) - (memAfterValueTask - memBeforeValueTask);
            
            Console.WriteLine($"Performance improvement: {timeImprovement:F1}%");
            Console.WriteLine($"Memory saved: {memoryDiff / 1024.0:F2} KB");
            Console.WriteLine($"ValueTask is better for hot paths with mostly synchronous completions");
        }
    }

    public class GoodMemoryPressureExample
    {
        public static async Task Main()
        {
            Console.WriteLine("=== Good Example: ValueTask optimizations ===\n");
            
            var good = new GoodMemoryPressure();
            await good.RunBenchmark();

            Console.WriteLine("\n" + new string('=', 60) + "\n");

            var comparison = new PerformanceComparison();
            await comparison.RunComparison();

            // Korzyści:
            // - Znacznie mniejsza alokacja pamięci
            // - Mniej GC collections
            // - Niższe latency
            // - Lepsza wydajność
        }
    }
}
