
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
        DiscordService _discord { get; set; }
        TokenService _token { get; set; }
        const string DiscordAuthenticationFailedError = "Discord Authentication Failed";
        const string NoCommonServersError = "No Common Servers";
        const string PanopticonAuthenticationFailedError = "Panopticon Authentication Failed";


        public PalantirAuthorizationController(DiscordService discord, TokenService token)
        {
            
            _discord = discord;
            _token = token;
        }

        [HttpPost("auth")]
        public async Task<ActionResult<string>> GetPanopticonToken([FromQuery] string accessToken)
        {            
            using (DiscordRestClient client = await _discord.GetUserClient(accessToken))
            {
                if(client.CurrentUser is null)
                {
                    // failed to connect with access token; 401 unauthorized
                    return StatusCode(401, DiscordAuthenticationFailedError);
                }
                Dictionary<ulong, DiscordGuildPermissions> guildPermissions = _discord.GetGuildPermissionsForData(client);

                if(guildPermissions.Keys.Count() == 0) {
                    // Discord user is not in any servers with the bot; 403 forbidden
                    return StatusCode(403, NoCommonServersError);
                }
                
                string? jwt = _token.RequestPalantirTokenWithDiscordData(new PalantirDiscordData(client.CurrentUser.Username, guildPermissions), client.CurrentUser.Id);

                if (jwt is null)
                {
                    // Failed to get panopticon access token; 401 unauthoirzed
                    return StatusCode(401, PanopticonAuthenticationFailedError);
                }

                return jwt;
            }
        }
    }
}
