using System;
using System.Collections.Generic;

namespace DesignPatterns.Builder.Good2
{
    // ✅ GOOD: Builder pattern for complex object construction
    public class HttpRequest
    {
        public string Url { get; internal set; }
        public string Method { get; internal set; }
        public Dictionary<string, string> Headers { get; internal set; }
        public Dictionary<string, string> QueryParams { get; internal set; }
        public string Body { get; internal set; }
        public int Timeout { get; internal set; }
        public bool FollowRedirects { get; internal set; }
        public string UserAgent { get; internal set; }

        private HttpRequest() { }

        public class Builder
        {
            private readonly HttpRequest _request = new HttpRequest
            {
                Method = "GET",
                Headers = new Dictionary<string, string>(),
                QueryParams = new Dictionary<string, string>(),
                Timeout = 30000,
                FollowRedirects = true,
                UserAgent = "MyApp/1.0"
            };

            public Builder WithUrl(string url)
            {
                _request.Url = url;
                return this;
            }

            public Builder WithMethod(string method)
            {
                _request.Method = method;
                return this;
            }

            public Builder AddHeader(string key, string value)
            {
                _request.Headers[key] = value;
                return this;
            }

            public Builder AddQueryParam(string key, string value)
            {
                _request.QueryParams[key] = value;
                return this;
            }

            public Builder WithBody(string body)
            {
                _request.Body = body;
                return this;
            }

            public Builder WithTimeout(int timeout)
            {
                _request.Timeout = timeout;
                return this;
            }

            public Builder WithFollowRedirects(bool follow)
            {
                _request.FollowRedirects = follow;
                return this;
            }

            public Builder WithUserAgent(string userAgent)
            {
                _request.UserAgent = userAgent;
                return this;
            }

            public HttpRequest Build()
            {
                if (string.IsNullOrEmpty(_request.Url))
                    throw new InvalidOperationException("URL is required");
                return _request;
            }
        }
    }

    // ✅ Creating requests is clear and readable
    public class Example
    {
        public void CreateRequests()
        {
            var request = new HttpRequest.Builder()
                .WithUrl("https://api.example.com")
                .WithMethod("POST")
                .AddHeader("Content-Type", "application/json")
                .WithBody("{\"name\":\"John\"}")
                .WithTimeout(60000)
                .WithFollowRedirects(false)
                .WithUserAgent("CustomAgent/1.0")
                .Build();
        }
    }
}
