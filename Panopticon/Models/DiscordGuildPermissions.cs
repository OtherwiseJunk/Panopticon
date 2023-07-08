namespace Panopticon.Models
{
    public class DiscordGuildPermissions
    {
        public bool IsAdmin { get; set; }
        public bool CanDeleteMessages { get; set; }
        public DiscordGuildPermissions(bool isAdmin, bool canDeleteMessages)
        {
            IsAdmin = isAdmin;
            CanDeleteMessages = canDeleteMessages;
        }
    }
}
