using Microsoft.AspNetCore.Http;
using Serilog;
using System.Net;
using System.Text.Json;

namespace Inventory_Management.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context); // continue down the pipeline
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (context.Request.Headers["Accept"].ToString().Contains("application/json"))
            {
                var code = HttpStatusCode.InternalServerError; // 500 if unexpected

                var result = JsonSerializer.Serialize(new
                {
                    error = "An unexpected error occurred.",
                    details = exception.Message // For production, avoid exposing internal messages
                });

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)code;

                return context.Response.WriteAsync(result);
            }
            else
            {
                 context.Response.Redirect("/Home/Error");
                return Task.CompletedTask;
            }
        }

    }
}
