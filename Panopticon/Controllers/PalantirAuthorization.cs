
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Panopticon.Models;
using Panopticon.Services;
using System.Web;
using Discord.Rest;
using Discord;
using Newtonsoft.Json.Linq;
using System.Text.Json;

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
        public async Task<IActionResult> GetPanopticonToken([FromQuery] string accessToken)
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

                PalantirDiscordData data = new PalantirDiscordData(client.CurrentUser.Username, guildPermissions);
                string? jwt = _token.RequestPalantirTokenWithDiscordData(data, client.CurrentUser.Id);

                if (jwt is null)
                {                    
                    // Failed to get panopticon access token; 401 unauthoirzed
                    return StatusCode(401, PanopticonAuthenticationFailedError);
                }

                Response.Cookies.Append("X-Access-Token", jwt, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = true });

                return new JsonResult(data);
            }
        }

        [HttpGet("authCheck")]
        [Authorize]
        public IActionResult ReturnSuccess()
        {
            Request.Cookies.TryGetValue("X-Access-Token", out string? token);
            if(token is null)
            {
                return new UnauthorizedResult();
            }

            PalantirDiscordData? data = _token.GetDiscordDataFromToken(token);
            if(data is null)
            {
                return new UnauthorizedResult();
            }

            return new JsonResult(data);
        }
    }
}
