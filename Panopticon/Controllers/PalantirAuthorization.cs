
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
using Discord.WebSocket;

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
        private string BotToken { get; set; }
        private ulong[] DiscordGuildIds { get; set; }
        private DiscordSocketClient _socketClient { get; set; }
        private bool socketClientReady = false;
        private JwtSecurityTokenHandler tokenHandler = new();


        private static Dictionary<ulong, string> cachedTokens = new();

        public PalantirAuthorizationController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            Auth0ClientId = Environment.GetEnvironmentVariable("AUTH0CLIENTID");
            Auth0ClientSecret = Environment.GetEnvironmentVariable("AUTH0CLIENTSECRET");
            Auth0Audience = Environment.GetEnvironmentVariable("AUTH0AUDIENCE");
            Auth0GrantType = Environment.GetEnvironmentVariable("AUTH0GRANTTYPE");
            Auth0Scope = Environment.GetEnvironmentVariable("AUTH0SCOPE");
            BotToken = Environment.GetEnvironmentVariable("DEEPSTATE");

            _socketClient = new(new DiscordSocketConfig { AlwaysDownloadUsers = true, GatewayIntents = GatewayIntents.All});
            _socketClient.Ready += async () => { 
                socketClientReady = true;
                DiscordGuildIds = _socketClient.Guilds.Select(guild => guild.Id).ToArray();
            };
            _socketClient.LoginAsync(TokenType.Bot, BotToken).Wait();
            _socketClient.StartAsync().Wait();
            while (!socketClientReady) { Console.WriteLine("Waiting for client to be ready... Sleeping 500ms"); Thread.Sleep(500); }
        }

        [HttpPost("auth")]
        public async Task<ActionResult<string>> GetPanopticonToken([FromQuery] string accessToken)
        {            
            using (DiscordRestClient client = new DiscordRestClient() )
            {
                await client.LoginAsync(TokenType.Bearer, accessToken);
                ulong userId = client.CurrentUser.Id;
                if (cachedTokens.ContainsKey(userId) && tokenHandler.ReadJwtToken(cachedTokens[userId]).ValidTo > DateTime.UtcNow.AddMinutes(1))
                {
                    return cachedTokens[userId];
                }
                Dictionary<ulong, DiscordGuildPermissions> guildPermissions = new();
                ulong[] usersGuildIds = client.GetGuildSummariesAsync().FlattenAsync().Result.Select(guild => guild.Id).Where(guildId => DiscordGuildIds.Contains(guildId)).ToArray();
                foreach (ulong guildId in usersGuildIds)
                {
                    GuildPermissions userPerms = GetUserPermissions(guildId, userId);
                    guildPermissions[guildId] = new DiscordGuildPermissions(userPerms.Administrator, userPerms.ManageMessages);
                }
                string jwt = RequestJWT(new PalantirDiscordData(client.CurrentUser.Username, guildPermissions));
                cachedTokens[userId] = jwt;

                return jwt;
            }
        }

        private GuildPermissions GetUserPermissions(ulong guildId, ulong id)
        {
            var user = _socketClient.Guilds.Where(guild => guild.Id == guildId).First().GetUser(id);
            return user.GuildPermissions;
        }

        private async Task<List<GuildPermissions>> GetRolePermissions(ulong[] roles, ulong guildId)
        {
            List<GuildPermissions> permissions = new();            
            IGuild guild = _socketClient.GetGuild(guildId);
            foreach (ulong roleId in roles)
            {
                var userPerms = guild.GetRole(roleId).Permissions;
                permissions.Add(userPerms);
            }
            return permissions;
        }

        private string RequestJWT(PalantirDiscordData palantirDiscordData)
        {
            string palantirDiscordDataJson = JsonSerializer.Serialize(palantirDiscordData);
            JwtSecurityTokenHandler tokenHandler = new();

            using (HttpRequestMessage msg = new(HttpMethod.Post, "https://dev-apsgkx34.us.auth0.com/oauth/token"))
            {
                msg.Headers.Add("Accept", MediaTypeNames.Application.Json);
                Auth0RequestBody body = new(Auth0ClientId, Auth0ClientSecret, Auth0Audience, Auth0GrantType, Auth0Scope, palantirDiscordDataJson);
                string jsonJWTRequest = JsonSerializer.Serialize(body);
                msg.Content = new StringContent(jsonJWTRequest, Encoding.UTF8,"application/json");

                using (HttpResponseMessage resp = _httpClient.SendAsync(msg).Result)
                {
                    if (resp.IsSuccessStatusCode)
                    {
                        return JsonSerializer.Deserialize<JsonNode>(resp.Content.ReadAsStringAsync().Result)["access_token"].GetValue<string>();
                    }
                    return null;
                }
            }
        }
    }
}
