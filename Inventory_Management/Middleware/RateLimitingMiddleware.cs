// ProjectName.Api/Middleware/RateLimitingMiddleware.cs
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_Management.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ConcurrentDictionary<string, RateLimitInfo> _requestTracker = new ConcurrentDictionary<string, RateLimitInfo>();
        private readonly int _maxRequests;
        private readonly TimeSpan _timeWindow;

        public RateLimitingMiddleware(RequestDelegate next, int maxRequests, TimeSpan timeWindow)
        {
            _next = next;
            _maxRequests = maxRequests;
            _timeWindow = timeWindow;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var userKey = context.Connection.RemoteIpAddress?.ToString(); // Use IP address to identify user
            if (userKey == null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Unable to identify user.");
                return;
            }

            var currentTime = DateTime.UtcNow;
            var requestInfo = _requestTracker.GetOrAdd(userKey, new RateLimitInfo(currentTime));

            bool isRateLimited = false;
            lock (requestInfo)
            {
                // Clear expired requests
                requestInfo.Requests = requestInfo.Requests.Where(r => r > currentTime - _timeWindow).ToList();

                // Check if user exceeds the rate limit
                if (requestInfo.Requests.Count >= _maxRequests)
                {
                    isRateLimited = true;
                }
                else
                {
                    // Add the current request timestamp
                    requestInfo.Requests.Add(currentTime);
                }
            }

            if (isRateLimited)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Too many requests. Please try again later.");
                return;
            }

            // Allow the request to continue to the next middleware
            await _next(context);
        }

        private class RateLimitInfo
        {
            public RateLimitInfo(DateTime initialTime)
            {
                Requests = new List<DateTime> { initialTime };
            }

            public List<DateTime> Requests { get; set; }
        }
    }

}
