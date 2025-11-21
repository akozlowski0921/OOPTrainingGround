using System;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SpecyfikaDotNet.AsyncPatterns
{
    // ❌ BAD: Brak świadomości memory pressure przy używaniu Task
    public class BadMemoryPressure
    {
        public async Task<int> ProcessManyItemsWithTask()
        {
            int sum = 0;

            // Każda iteracja alokuje Task - 1 milion alokacji!
            for (int i = 0; i < 1_000_000; i++)
            {
                // Task jest reference type - alokacja na heap
                int result = await GetValueWithTask(i);
                sum += result;
            }

            return sum;
        }

        private async Task<int> GetValueWithTask(int value)
        {
            // Większość przypadków zwraca od razu
            if (value % 100 != 0)
            {
                // Mimo że zwracamy synchronicznie, Task jest alokowany
                return value * 2;
            }

            // Tylko 1% przypadków faktycznie async
            await Task.Delay(1);
            return value * 2;
        }

        public async Task RunBenchmark()
        {
            var sw = Stopwatch.StartNew();
            
            // Sprawdź pamięć przed
            long memBefore = GC.GetTotalMemory(true);
            
            await ProcessManyItemsWithTask();
            
            // Sprawdź pamięć po
            long memAfter = GC.GetTotalMemory(false);
            
            sw.Stop();
            
            Console.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");
            Console.WriteLine($"Memory used: {(memAfter - memBefore) / 1024.0 / 1024.0:F2} MB");
            Console.WriteLine($"GC Collections (Gen 0/1/2): {GC.CollectionCount(0)}/{GC.CollectionCount(1)}/{GC.CollectionCount(2)}");
        }
    }

    public class BadMemoryPressureExample
    {
        public static async Task Main()
        {
            Console.WriteLine("=== Bad Example: Task allocations ===");
            
            var bad = new BadMemoryPressure();
            await bad.RunBenchmark();

            // Problem:
            // - Wysoka alokacja pamięci (miliony Task obiektów)
            // - Częste GC collections
            // - Wyższe latency przez GC pauses
            // - Gorsza wydajność
        }
    }
}
