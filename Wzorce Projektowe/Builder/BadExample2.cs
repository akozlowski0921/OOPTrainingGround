using System;
using System.Collections.Generic;

namespace DesignPatterns.Builder.Bad2
{
    // ❌ BAD: Complex constructor with many optional parameters
    public class HttpRequest
    {
        public string Url { get; set; }
        public string Method { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public Dictionary<string, string> QueryParams { get; set; }
        public string Body { get; set; }
        public int Timeout { get; set; }
        public bool FollowRedirects { get; set; }
        public string UserAgent { get; set; }

        // Problem: Too many parameters, hard to remember order
        public HttpRequest(
            string url,
            string method = "GET",
            Dictionary<string, string> headers = null,
            Dictionary<string, string> queryParams = null,
            string body = null,
            int timeout = 30000,
            bool followRedirects = true,
            string userAgent = "MyApp/1.0")
        {
            Url = url;
            Method = method;
            Headers = headers ?? new Dictionary<string, string>();
            QueryParams = queryParams ?? new Dictionary<string, string>();
            Body = body;
            Timeout = timeout;
            FollowRedirects = followRedirects;
            UserAgent = userAgent;
        }
    }

    // Problem: Creating requests is confusing
    public class Example
    {
        public void CreateRequests()
        {
            // Hard to read, easy to mix up parameters
            var request1 = new HttpRequest(
                "https://api.example.com",
                "POST",
                new Dictionary<string, string> { { "Content-Type", "application/json" } },
                null,
                "{\"name\":\"John\"}",
                60000,
                false,
                "CustomAgent/1.0"
            );
        }
    }
}
