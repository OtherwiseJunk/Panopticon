using Microsoft.Extensions.Logging;
using Panopticon.Data.Contexts;
using Panopticon.Data.Interfaces;
using Panopticon.Enums;
using Panopticon.Models.Core;
using Panopticon.Shared.Models.Core;

namespace Panopticon.Data.Services;

public class ApiKeyService(PanopticonContext context, ILogger<ApiKeyService> logger) : IApiKeyService
{
    public ApiKey? TryGetApiKey(string apiKey)
    {
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
        context.ApiTransactions.Add(transaction);
        context.SaveChanges();
    }
    
    public void CreateApiKey(ApiKey apiKey)
    {
        context.ApiKeys.Add(apiKey);
        context.SaveChanges();
    }
}