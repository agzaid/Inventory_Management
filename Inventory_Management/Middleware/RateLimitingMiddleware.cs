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
        private readonly int _maxRequests;
        private readonly TimeSpan _timeWindow;

        // Key: IP address, Value: list of request timestamps
        private static readonly ConcurrentDictionary<string, ConcurrentQueue<DateTime>> _requests
            = new ConcurrentDictionary<string, ConcurrentQueue<DateTime>>();

        public RateLimitingMiddleware(RequestDelegate next, int maxRequests, TimeSpan timeWindow)
        {
            _next = next;
            _maxRequests = maxRequests;
            _timeWindow = timeWindow;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // File extensions you want to ignore
            var excludedExtensions = new[]
            {
        ".js", ".css", ".png", ".jpg", ".jpeg", ".gif", ".ico", ".svg", ".woff", ".woff2", ".ttf", ".map"
    };

            var path = context.Request.Path.Value ?? "";

            // Skip if request is for static files
            if (excludedExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            {
                await _next(context);
                return;
            }

            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var now = DateTime.UtcNow;

            var requestTimes = _requests.GetOrAdd(clientIp, _ => new ConcurrentQueue<DateTime>());
            requestTimes.Enqueue(now);

            // Remove old requests
            while (requestTimes.TryPeek(out var oldest) && (now - oldest) > _timeWindow)
            {
                requestTimes.TryDequeue(out _);
            }

            if (requestTimes.Count > _maxRequests)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Rate limit exceeded. Please wait before trying again.");
                return;
            }

            await _next(context);
        }


    }

}
