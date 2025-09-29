using Inventory_Management.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_Management.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;

        // Key: IP + endpoint, Value: list of timestamps
        private static readonly ConcurrentDictionary<string, ConcurrentQueue<DateTime>> _requests
            = new();

        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Get endpoint metadata
            var endpoint = context.GetEndpoint();
            var rateLimitAttr = endpoint?.Metadata.GetMetadata<RateLimitAttribute>();

            // If no RateLimit attribute → skip
            if (rateLimitAttr == null)
            {
                await _next(context);
                return;
            }

            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var key = $"{clientIp}:{endpoint.DisplayName}";
            var now = DateTime.UtcNow;

            var requestTimes = _requests.GetOrAdd(key, _ => new ConcurrentQueue<DateTime>());
            requestTimes.Enqueue(now);

            // Remove old requests
            while (requestTimes.TryPeek(out var oldest) && (now - oldest) > rateLimitAttr.TimeWindow)
            {
                requestTimes.TryDequeue(out _);
            }

            if (requestTimes.Count > rateLimitAttr.MaxRequests)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Rate limit exceeded. Please wait before retrying.");
                return;
            }

            await _next(context);
        }
    }
}
