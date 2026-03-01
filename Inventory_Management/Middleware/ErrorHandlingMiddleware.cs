using Microsoft.AspNetCore.Http;
using Serilog;
using System;
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
                await _next(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Caught by Global Middleware: {Message}", ex.Message);

                bool isAjax = context.Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                              context.Request.Headers["Accept"].ToString().Contains("application/json");

                if (isAjax)
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    var result = JsonSerializer.Serialize(new
                    {
                        success = false,
                        title = "System Error",
                        message = "Something went wrong",
                        details = ex.Message
                    });

                    await context.Response.WriteAsync(result);
                }
                else
                {
                    var message = Uri.EscapeDataString(ex.Message);
                    context.Response.Redirect($"/Home/Index?status=error&message={message}");
                    await Task.CompletedTask;
                }
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            bool isAjax = context.Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                          context.Request.Headers["Accept"].ToString().Contains("application/json");

            if (isAjax)
            {
                var code = HttpStatusCode.InternalServerError;
                var result = JsonSerializer.Serialize(new
                {
                    success = false,
                    title = "Oops...",
                    message = "An unexpected error occurred.",
                    details = exception.Message
                });

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)code;
                await context.Response.WriteAsync(result);
            }
            else
            {
                context.Response.Redirect("/Home/Error");
                await Task.CompletedTask;
            }
        }

    }
}
