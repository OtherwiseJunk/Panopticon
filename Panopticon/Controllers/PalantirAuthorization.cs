
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Panopticon.Models;
using Panopticon.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Mime;
using System.Text.Json.Nodes;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using Discord.Rest;
using Discord;

namespace Panopticon.Controllers
{
    [Route("/palantir")]
    [ApiController]
    public class PalantirAuthorizationController : ControllerBase
    {
        public readonly HttpClient _httpClient;
        private string Auth0ClientId { get; set; }
        private string Auth0ClientSecret { get; set; }
        private string Auth0Audience { get; set; }
        private string Auth0GrantType { get; set; }
        private string Auth0Scope { get; set; }
        private ulong[] DiscordGuildIds { get; set; } = new ulong[] { 698639095940907048 };


        private static Dictionary<ulong, JwtSecurityToken> cachedTokens = new();

        public PalantirAuthorizationController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            Auth0ClientId = Environment.GetEnvironmentVariable("AUTH0CLIENTID");
            Auth0ClientSecret = Environment.GetEnvironmentVariable("AUTH0CLIENTSECRET");
            Auth0Audience = Environment.GetEnvironmentVariable("AUTH0AUDIENCE");
            Auth0GrantType = Environment.GetEnvironmentVariable("AUTH0GRANTTYPE");
            Auth0Scope = Environment.GetEnvironmentVariable("AUTH0SCOPE");
        }

        [HttpPost("/auth")]
        public async Task<ActionResult<string>> GetPanopticonToken([FromQuery] string accessToken)
        {
            using (DiscordRestClient client = new DiscordRestClient() )
            {
                await client.LoginAsync(TokenType.Bearer, accessToken);
                Dictionary<ulong, DiscordGuildPermissions> guildPermissions = new();
                foreach (ulong guildId in DiscordGuildIds)
                {
                    RestGuildUser user = await client.GetCurrentUserGuildMemberAsync(guildId);
                    bool isAdmin = false;
                    bool canDeleteMessages = false;
                    foreach(ulong roleId in user.RoleIds)
                    {
                        RestRole role = client.GetGuildAsync(guildId).Result.GetRole(roleId);
                        if (role.Permissions.Administrator)
                        {
                            isAdmin = true;
                        }
                        if (role.Permissions.ManageMessages)
                        {
                            canDeleteMessages = true;
                        }
                    }
                    guildPermissions[guildId] = new DiscordGuildPermissions(isAdmin, canDeleteMessages);
                }
                return RequestJWT(guildPermissions);
            }
        }

        /*private ulong[] GetUsersGuilds(string accessToken)
        {
            using (HttpRequestMessage msg = new(HttpMethod.Get, "https://discord.com/api/v10/users/@me/guilds"))
            {
                msg.Headers.Authorization = new AuthenticationHeaderValue($"Bearer {accessToken}");
            }
        }

        private DiscordGuildPermissions GetUsersGuildPermissions(string accessToken)
        {
            using (HttpRequestMessage msg = new(HttpMethod.Get, "https://discord.com/api/v10"))
            {
                msg.Headers.Authorization = new AuthenticationHeaderValue($"Bearer {accessToken}");
            }
        }*/

        private string RequestJWT(Dictionary<ulong, DiscordGuildPermissions> userPermissionsByGuildId)
        {
            string userPermissionsJson = JsonSerializer.Serialize(userPermissionsByGuildId);
            JwtSecurityTokenHandler tokenHandler = new();

            using (HttpRequestMessage msg = new(HttpMethod.Post, "https://dev-apsgkx34.us.auth0.com/oauth/token"))
            {
                msg.Headers.Add("Accept", MediaTypeNames.Application.Json);
                msg.Content = new StringContent($"{{\"client_id\":\"{Auth0ClientId}\",\"client_secret\":\"{Auth0ClientSecret}\",\"audience\":\"{Auth0Audience}\",\"grant_type\":\"{Auth0GrantType}\",\"scope\":\"{Auth0Scope}\",\"userPermissions\":\"{userPermissionsJson}\"}}",
                Encoding.UTF8,
                                    "application/json");

                using (HttpResponseMessage resp = _httpClient.SendAsync(msg).Result)
                {
                    string jsonToken = JsonSerializer.Deserialize<JsonNode>(resp.Content.ReadAsStringAsync().Result)["access_token"].GetValue<string>();
                    tokenHandler.ReadJwtToken(jsonToken);
                    return jsonToken;
                }
            }
        }
    }
}
