using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Panopticon.Data.Contexts;
using Panopticon.Data.Interfaces;
using Panopticon.Enums;
using Panopticon.Models.Core;
using Panopticon.Shared.Models.Core;

namespace Panopticon.Data.Services;

public class ApiKeyService(IDbContextFactory<PanopticonContext> contextFactory, ILogger<ApiKeyService> logger) : IApiKeyService
{
    public ApiKey? TryGetApiKey(string apiKey)
    {
        using PanopticonContext context = contextFactory.CreateDbContext();
        return context.ApiKeys.FirstOrDefault(k => k.Key == apiKey);
    }

    public bool HasPermission(string apiKey, ApiPermission permission)
    {
        ApiKey? key = TryGetApiKey(apiKey);
        if (key == null)
        {
            return false;
        }
        return HasPermission(key, permission);
    }

    public bool HasPermission(ApiKey apiKey, ApiPermission permission)
    {
        return (apiKey.Permissions & (int)permission) == (int)permission;
    }

    public bool HasPermissions(string apiKey, ApiPermission[] permissions)
    {
        ApiKey? key = TryGetApiKey(apiKey);
        if (key == null)
        {
            return false;
        }
        return HasPermissions(key, permissions);
    }

    public bool HasPermissions(ApiKey apiKey, ApiPermission[] permissions)
    {
        foreach (ApiPermission permission in permissions)
        {
            if (!HasPermission(apiKey, permission))
            {
                return false;
            }
        }
        return true;
    }

    public void CreateApiTransaction(ApiTransaction transaction)
    {
        using PanopticonContext context = contextFactory.CreateDbContext();
        context.ApiTransactions.Add(transaction);
        context.SaveChanges();
    }

    public void CreateApiKey(ApiKey apiKey)
    {
        using PanopticonContext context = contextFactory.CreateDbContext();
        context.ApiKeys.Add(apiKey);
        context.SaveChanges();
    }

    public IDictionary<string, string> GetDeveloperNamesForKeys(IEnumerable<string> apiKeys)
    {
        using PanopticonContext context = contextFactory.CreateDbContext();
        return context.ApiKeys
            .Where(k => apiKeys.Contains(k.Key) && k.DeveloperName != null)
            .Select(k => new { k.Key, k.DeveloperName })
            .ToDictionary(k => k.Key, k => k.DeveloperName!); 
    }
}