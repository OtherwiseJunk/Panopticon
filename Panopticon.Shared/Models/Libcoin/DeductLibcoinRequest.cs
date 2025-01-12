namespace Panopticon.Models.Libcoin;

public class DeductLibcoinRequest
{
    public string UserId { get; set; }
    public double Amount { get; set; }
    public string Message { get; set; }
}