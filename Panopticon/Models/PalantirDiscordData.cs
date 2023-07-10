namespace Panopticon.Models
{
    public class PalantirDiscordData
    {
        public string Username { get; set; }
        public Dictionary<ulong, DiscordGuildPermissions> UserPermissionsByGuildId { get; set; }
        public PalantirDiscordData(string username, Dictionary<ulong, DiscordGuildPermissions> userPermissionsByGuildId) {
            Username = username;
            UserPermissionsByGuildId = userPermissionsByGuildId;
        }
    }
}
