using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace OnlineStoreBackend.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string? _apiKey;

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _apiKey = configuration["ApiKey"]; // Can be null, so we use nullable type
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (_apiKey == null)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("API_KEY is not configured.");
                return;
            }

            if (!context.Request.Headers.ContainsKey("API_KEY"))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("API_KEY header is missing");
                return;
            }

            var apiKey = context.Request.Headers["API_KEY"].ToString();

            if (apiKey != _apiKey)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Invalid API key");
                return;
            }

            await _next(context); // Proceed if valid
        }
    }
}
