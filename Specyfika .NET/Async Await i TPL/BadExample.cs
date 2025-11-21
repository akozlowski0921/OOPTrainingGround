using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace SpecyfikaDotNet.AsyncAwait
{
    // ❌ BAD: Typowe błędy z async/await
    public class BadAsyncService
    {
        private readonly HttpClient _httpClient = new HttpClient();

        // BŁĄD 1: async void - wyjątki są nieobsługiwalne
        public async void FetchDataAsync(string url)
        {
            var result = await _httpClient.GetStringAsync(url);
            Console.WriteLine(result);
            // Jeśli wystąpi wyjątek, aplikacja może crashować
        }

        // BŁĄD 2: Potencjalny deadlock - blokowanie asynchronicznego kodu
        public string GetDataSync(string url)
        {
            // .Result blokuje wątek i może prowadzić do deadlocka w UI/ASP.NET
            var task = _httpClient.GetStringAsync(url);
            return task.Result; // ❌ Deadlock risk w kontekstach z SynchronizationContext
        }

        // BŁĄD 3: Niepotrzebne .ConfigureAwait(true) lub jego brak w library code
        public async Task<string> LibraryMethodAsync(string url)
        {
            // W kodzie biblioteki powinno być .ConfigureAwait(false)
            var result = await _httpClient.GetStringAsync(url);
            return result.ToUpper();
        }

        // BŁĄD 4: Sekwencyjne wykonywanie niezależnych operacji
        public async Task<List<string>> FetchMultipleUrlsAsync(List<string> urls)
        {
            var results = new List<string>();
            
            // Każde wywołanie czeka na poprzednie, mimo że mogłyby działać równolegle
            foreach (var url in urls)
            {
                var result = await _httpClient.GetStringAsync(url);
                results.Add(result);
            }
            
            return results;
        }

        // BŁĄD 5: async over sync - niepotrzebne async dla synchronicznej operacji
        public async Task<int> CalculateAsync(int a, int b)
        {
            return a + b; // Brak await, zwraca Task<int> bez potrzeby
        }

        // BŁĄD 6: Łapanie wyjątków bez await
        public Task<string> FetchWithoutAwaitAsync(string url)
        {
            try
            {
                return _httpClient.GetStringAsync(url); // Wyjątek nie zostanie złapany
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message); // Nigdy się nie wykona
                return Task.FromResult(string.Empty);
            }
        }
    }

    // BŁĄD 7: Fire and forget - utrata kontroli nad zadaniem
    public class BadBackgroundService
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public void StartBackgroundWork(string url)
        {
            // Fire and forget - nie możemy śledzić statusu ani obsłużyć błędów
            _ = ProcessDataAsync(url);
        }

        private async Task ProcessDataAsync(string url)
        {
            var data = await _httpClient.GetStringAsync(url);
            // Jeśli wystąpi błąd, nikt o tym nie wie
            await Task.Delay(1000);
        }
    }
}
