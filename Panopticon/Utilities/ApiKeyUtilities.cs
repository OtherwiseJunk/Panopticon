using System.Security.Cryptography;
using System.Text;

namespace Panopticon.Utilities;

public class ApiKeyUtilities
{
    public static string GenerateApiKey()
    {
        using (var hmac = new HMACSHA256())
        {
            var key = Guid.NewGuid().ToString();
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(key));
            return Convert.ToBase64String(hash);
        }
    }
}