using Panopticon.Enums;
using Panopticon.Models.Core;
using Panopticon.Shared.Models.Core;
using System.Collections.Generic;

namespace Panopticon.Data.Interfaces;

public interface IApiKeyService
{
    ApiKey? TryGetApiKey(string apiKey);
    bool HasPermission(string apiKey, ApiPermission permission);
    bool HasPermission(ApiKey apiKey, ApiPermission permission);
    bool HasPermissions(string apiKey, ApiPermission[] permissions);
    bool HasPermissions(ApiKey apiKey, ApiPermission[] permissions);
    void CreateApiTransaction(ApiTransaction transaction);
    void CreateApiKey(ApiKey apiKey);
    IDictionary<string, string> GetDeveloperNamesForKeys(IEnumerable<string> apiKeys);
}