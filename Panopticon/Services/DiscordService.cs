using Discord;
using Discord.WebSocket;
using Panopticon.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Mime;
using System.Text.Json.Nodes;
using System.Text;
using System.Text.Json;
using Discord.Rest;

namespace Panopticon.Services
{
    public class DiscordService
    {
        private readonly DiscordSocketClient _client;

        public DiscordService(DiscordSocketClient client)
        {
            _client = client;

        }

        public GuildPermissions GetUserPermissions(ulong guildId, ulong id)
        {            
            return _client.Guilds.Where(guild => guild.Id == guildId).First().GetUser(id).GuildPermissions;
        }

        public ulong[] GetBotsDiscordGuildList()
        {
            return _client.Guilds.Select(guild => guild.Id).ToArray();
        }

        public async Task<DiscordRestClient> GetUserClient(string accessToken)
        {
            DiscordRestClient client = new DiscordRestClient();
            await client.LoginAsync(TokenType.Bearer, accessToken);
            return client;
        }

        public Dictionary<ulong, DiscordGuildPermissions> GetGuildPermissionsForData(DiscordRestClient userClient)
        {
            Dictionary<ulong, DiscordGuildPermissions> guildPermissions = new();
            ulong[] usersGuildIds = userClient.GetGuildSummariesAsync().FlattenAsync().Result.Select(guild => guild.Id).Where(guildId => GetBotsDiscordGuildList().Contains(guildId)).ToArray();
            foreach (ulong guildId in usersGuildIds)
            {
                GuildPermissions userPerms = this.GetUserPermissions(guildId, userClient.CurrentUser.Id);
                guildPermissions[guildId] = new DiscordGuildPermissions(userPerms.Administrator, userPerms.ManageMessages);
            }

            return guildPermissions;
        }
    }
}
