using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpecyfikaDotNet.ExceptionHandling.Good3
{
    // ✅ GOOD: Prawidłowa obsługa wyjątków w async/await
    public class ApiClient
    {
        private readonly HttpClient _client;

        public ApiClient(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<string> GetDataAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("URL cannot be empty", nameof(url));

            try
            {
                var response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                // Logujemy i rzucamy dalej lub rzucamy własny wyjątek z kontekstem
                throw new ApiException($"Failed to fetch data from {url}", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new ApiException($"Request to {url} timed out", ex);
            }
        }

        public async Task<ApiResult<int>> GetUserCountAsync()
        {
            try
            {
                var data = await GetDataAsync("https://api.example.com/users");

                if (string.IsNullOrWhiteSpace(data))
                {
                    return ApiResult<int>.Failure("Empty response from API");
                }

                if (!int.TryParse(data, out var count))
                {
                    return ApiResult<int>.Failure($"Invalid response format: {data}");
                }

                return ApiResult<int>.Success(count);
            }
            catch (ApiException ex)
            {
                // Zwracamy Result zamiast rzucać - lepsze dla flow control
                return ApiResult<int>.Failure(ex.Message);
            }
        }

        public async Task<Dictionary<string, string>> ProcessMultipleUrlsAsync(string[] urls)
        {
            var results = new Dictionary<string, string>();
            var errors = new List<(string Url, Exception Exception)>();

            foreach (var url in urls)
            {
                try
                {
                    var data = await GetDataAsync(url);
                    results[url] = data;
                }
                catch (Exception ex)
                {
                    // Zbieramy błędy zamiast je ukrywać
                    errors.Add((url, ex));
                }
            }

            // Jeśli były błędy, rzucamy AggregateException z pełnym kontekstem
            if (errors.Any())
            {
                throw new AggregateException(
                    $"Failed to process {errors.Count} of {urls.Length} URLs",
                    errors.Select(e => new ApiException($"Failed to process {e.Url}", e.Exception))
                );
            }

            return results;
        }

        public async Task<ApiResult> TryDeleteResourceAsync(string resourceId)
        {
            if (string.IsNullOrWhiteSpace(resourceId))
                return ApiResult.Failure("Resource ID cannot be empty");

            try
            {
                var response = await _client.DeleteAsync($"https://api.example.com/resources/{resourceId}");
                
                if (response.IsSuccessStatusCode)
                {
                    return ApiResult.Success();
                }

                // Zwracamy szczegóły błędu, nie tylko true/false
                return ApiResult.Failure($"Delete failed with status {response.StatusCode}");
            }
            catch (HttpRequestException ex)
            {
                return ApiResult.Failure($"Network error: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                return ApiResult.Failure("Request timed out");
            }
        }
    }

    // Własny wyjątek dla API
    public class ApiException : Exception
    {
        public ApiException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }

    // Result pattern zamiast wyjątków dla flow control
    public class ApiResult
    {
        public bool IsSuccess { get; }
        public string ErrorMessage { get; }

        protected ApiResult(bool isSuccess, string errorMessage = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        public static ApiResult Success() => new ApiResult(true);
        public static ApiResult Failure(string error) => new ApiResult(false, error);
    }

    public class ApiResult<T> : ApiResult
    {
        public T Value { get; }

        private ApiResult(bool isSuccess, T value, string errorMessage)
            : base(isSuccess, errorMessage)
        {
            Value = value;
        }

        public static ApiResult<T> Success(T value) => new ApiResult<T>(true, value, null);
        public static new ApiResult<T> Failure(string error) => new ApiResult<T>(false, default, error);
    }
}
