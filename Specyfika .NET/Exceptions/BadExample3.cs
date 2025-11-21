using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpecyfikaDotNet.ExceptionHandling.Bad3
{
    // ❌ BAD: Nieprawidłowa obsługa wyjątków w async/await
    public class ApiClient
    {
        private HttpClient _client = new HttpClient();

        public async Task<string> GetDataAsync(string url)
        {
            try
            {
                var response = await _client.GetAsync(url);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                // BŁĄD: Swallowing exception w async
                Console.WriteLine("API Error");
                return null;
            }
        }

        public string GetDataSync(string url)
        {
            try
            {
                // BŁĄD: .Result blokuje wątek i może prowadzić do deadlocków
                var task = _client.GetAsync(url);
                var response = task.Result;
                return response.Content.ReadAsStringAsync().Result;
            }
            catch (AggregateException ex)
            {
                // BŁĄD: throw ex zamiast throw
                throw ex; // Traci stack trace
            }
        }

        public async Task ProcessMultipleUrls(string[] urls)
        {
            foreach (var url in urls)
            {
                try
                {
                    await GetDataAsync(url);
                }
                catch
                {
                    // BŁĄD: Puste catch - ukrywamy wszystkie błędy
                    // Nawet nie wiemy, które URL zawiodło
                }
            }
        }

        public async Task<int> GetUserCountAsync()
        {
            try
            {
                var data = await GetDataAsync("https://api.example.com/users");
                
                // BŁĄD: Using exception for flow control
                if (data == null)
                    throw new Exception("No data");

                return int.Parse(data);
            }
            catch (FormatException)
            {
                // BŁĄD: Konwersja konkretnego wyjątku na ogólny
                throw new Exception("Error parsing user count");
            }
        }

        public async Task<bool> TryDeleteResourceAsync(string resourceId)
        {
            try
            {
                await _client.DeleteAsync($"https://api.example.com/resources/{resourceId}");
                return true;
            }
            catch (HttpRequestException)
            {
                // BŁĄD: Zwracanie false dla wyjątku
                // Nie wiadomo co poszło nie tak - timeout, 404, 500?
                return false;
            }
            catch (Exception)
            {
                // BŁĄD: Catch-all bez re-throw
                return false;
            }
        }
    }
}
