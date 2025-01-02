using System.Net;
using Panopticon.Data.Contexts;

namespace Panopticon.Middleware;

public class ApiKeyMiddleware(RequestDelegate next, PanopticonContext context)
{
    private readonly RequestDelegate _next = next;
    private readonly PanopticonContext _context = context;
    
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("ApiKey", out var extractedApiKey))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Api Key was not provided.");
            return;
        }

        var apiKey = _context.ApiKeys.SingleOrDefault(x => x.Key == extractedApiKey);

        if (apiKey == null)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Unauthorized client.");
            return;
        }

        await _next(context);
    }
}