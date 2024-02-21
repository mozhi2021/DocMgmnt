using DocMgmnt.Interface;
using DocMgmnt.Models;
using Microsoft.Extensions.Options;

namespace DocMgmnt.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IOptions<AWSConfig> awsObj)
        {
            if (!context.Request.Headers.TryGetValue(Constants.ApiKeyHeaderName, out var extractedApiKey))

            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Api Key missing");
                return;
            }

            var apikey = awsObj.Value.ApiKey;

            if (apikey.Equals(extractedApiKey))
            { 
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized client");
                return;
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ApiKeyMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiKeyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyMiddleware>();
        }
    }
}
