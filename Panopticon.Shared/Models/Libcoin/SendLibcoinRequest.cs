namespace Panopticon.Models.Libcoin;

public class SendLibcoinRequest
{
    public required string SendingUserId { get; set; }
    public required string ReceivingUserId { get; set; }
    public double Amount { get; set; }
    public string? Message { get; set; }
}