namespace Panopticon.Models.Libcoin;

public class GrantLibcoinRequest
{
    public string UserId { get; set; }
    public double Amount { get; set; }
    public string Message { get; set; }
}