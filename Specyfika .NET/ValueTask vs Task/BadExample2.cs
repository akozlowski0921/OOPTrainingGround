using System;
using System.Threading.Tasks;

namespace SpecyfikaDotNet.AsyncPatterns
{
    // ❌ BAD: Nieprawidłowe użycie ValueTask
    public class BadValueTaskUsage
    {
        // Problem: Przechowywanie ValueTask w polu
        private ValueTask<string> _cachedTask;

        public async Task<string> GetDataWithCachedValueTask()
        {
            // BŁĄD: ValueTask może być użyty tylko raz!
            if (_cachedTask.Equals(default(ValueTask<string>)))
            {
                _cachedTask = FetchDataAsync();
            }

            // Próba wielokrotnego await na tym samym ValueTask - undefined behavior
            return await _cachedTask;
        }

        // Problem: Zwracanie ValueTask i próba użycia go wielokrotnie
        public ValueTask<int> GetNumberAsync()
        {
            return new ValueTask<int>(42);
        }

        // Problem: Await w pętli na tym samym ValueTask
        public async Task BadLoop()
        {
            ValueTask<string> task = FetchDataAsync();

            for (int i = 0; i < 3; i++)
            {
                // BŁĄD: await na tym samym ValueTask więcej niż raz
                string result = await task;
                Console.WriteLine(result);
            }
        }

        // Problem: Używanie .Result lub .GetAwaiter().GetResult() na ValueTask
        public string GetDataSync()
        {
            ValueTask<string> task = FetchDataAsync();
            
            // NIEBEZPIECZNE: Może powodować deadlock
            return task.GetAwaiter().GetResult();
        }

        // Problem: Konwersja ValueTask do Task bez potrzeby
        public async Task<string> ConvertToTask()
        {
            ValueTask<string> valueTask = FetchDataAsync();
            
            // Niepotrzebna konwersja - traci korzyści ValueTask
            Task<string> task = valueTask.AsTask();
            return await task;
        }

        private async ValueTask<string> FetchDataAsync()
        {
            await Task.Delay(10);
            return "Data";
        }
    }

    public class BadValueTaskUsageExample
    {
        public static async Task Main()
        {
            var bad = new BadValueTaskUsage();

            try
            {
                // To może działać nieprawidłowo - ValueTask użyty wielokrotnie
                string result1 = await bad.GetDataWithCachedValueTask();
                string result2 = await bad.GetDataWithCachedValueTask();
                
                Console.WriteLine($"Results: {result1}, {result2}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            try
            {
                // BŁĄD: Próba wielokrotnego użycia ValueTask
                await bad.BadLoop();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Loop error: {ex.Message}");
            }

            // Synchroniczne blokowanie na ValueTask - zły pomysł
            string syncResult = bad.GetDataSync();
            Console.WriteLine(syncResult);
        }
    }
}
