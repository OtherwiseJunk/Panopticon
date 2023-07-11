using Panopticon.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Mime;
using System.Text.Json.Nodes;
using System.Text;
using System.Text.Json;

namespace Panopticon.Services
{
    public class TokenService
    {
        private readonly HttpClient _httpClient;
        private string Auth0ClientId { get; set; }
        private string Auth0ClientSecret { get; set; }
        private string Auth0Audience { get; set; }
        private string Auth0GrantType { get; set; }
        private string Auth0Scope { get; set; }
        public Dictionary<ulong, string> cachedTokens = new();
        private JwtSecurityTokenHandler tokenHandler { get; set; }


        public TokenService(HttpClient httpClient) {
            tokenHandler = new();
            _httpClient = httpClient;
            Auth0ClientId = Environment.GetEnvironmentVariable("AUTH0CLIENTID")!;
            Auth0ClientSecret = Environment.GetEnvironmentVariable("AUTH0CLIENTSECRET")!;
            Auth0Audience = Environment.GetEnvironmentVariable("AUTH0AUDIENCE")!;
            Auth0GrantType = Environment.GetEnvironmentVariable("AUTH0GRANTTYPE")!;
            Auth0Scope = Environment.GetEnvironmentVariable("AUTH0SCOPE")!;
        }

        public string? RequestPalantirTokenWithDiscordData(PalantirDiscordData palantirDiscordData, ulong userId)
        {
            if (cachedTokens.ContainsKey(userId) && tokenHandler.ReadJwtToken(cachedTokens[userId]).ValidTo > DateTime.UtcNow.AddMinutes(1))
            {
                return cachedTokens[userId];
            }
            string palantirDiscordDataJson = JsonSerializer.Serialize(palantirDiscordData);

            using (HttpRequestMessage msg = new(HttpMethod.Post, "https://dev-apsgkx34.us.auth0.com/oauth/token"))
            {
                msg.Headers.Add("Accept", MediaTypeNames.Application.Json);
                Auth0RequestBody body = new(Auth0ClientId, Auth0ClientSecret, Auth0Audience, Auth0GrantType, Auth0Scope, palantirDiscordDataJson);
                string jsonJWTRequest = JsonSerializer.Serialize(body);
                msg.Content = new StringContent(jsonJWTRequest, Encoding.UTF8, "application/json");

                using (HttpResponseMessage resp = _httpClient.SendAsync(msg).Result)
                {
                    string? token;
                    switch (resp.StatusCode)
                    {
                        case System.Net.HttpStatusCode.OK:
                            token = JsonSerializer.Deserialize<JsonNode>(resp.Content.ReadAsStringAsync().Result)!["access_token"]!.GetValue<string>();
                            break;
                        default:
                            token = null;
                            break;
                    }

                    if(token is not null)
                    {
                        cachedTokens[userId] = token;
                    }

                    return token;
                }
            }
        }
    }
}
