using Panopticon.Models;

public class Auth0RequestBody
{
    public string client_id { get; set; }
    public string client_secret { get; set; }
    public string audience { get; set; }
    public string grant_type { get; set; }
    public string scope { get; set; }
    public PalantirDiscordData palantirDiscordData { get; set; }

    public Auth0RequestBody(string clientId, string clientSecret, string _audience, string grantType, string _scope, PalantirDiscordData _palantirData)
    {
        client_id = clientId;
        client_secret = clientSecret;
        audience = _audience;
        grant_type = grantType;
        scope = _scope;
        palantirDiscordData = _palantirData;
    }
}