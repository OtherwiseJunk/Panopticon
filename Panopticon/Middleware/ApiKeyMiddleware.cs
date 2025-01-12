using System.Net;
using Panopticon.Data.Contexts;
using Panopticon.Data.Interfaces;

namespace Panopticon.Middleware;

public class ApiKeyMiddleware(RequestDelegate next, IApiKeyService apiKeyService)
{
    private readonly RequestDelegate _next = next;
    private readonly IApiKeyService _apiKeyService = apiKeyService;
    
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("ApiKey", out var extractedApiKey))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Api Key was not provided.");
            return;
        }

        var apiKey = _apiKeyService.TryGetApiKey(extractedApiKey!);

        if (apiKey == null)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Unauthorized client.");
            return;
        }

        await _next(context);
    }
}