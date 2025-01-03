using Panopticon.Enums;

namespace Panopticon.Models.Libcoin;

public class LibcoinTransaction
{
    public string SendingUser { get; set; }
    public string ReceivingUser { get; set; }
    public double Amount { get; set; }
    public string TransactionMessage { get; set; }
    public LibcoinTransactionType TransactionType { get; set; }
    public DateTime TransactionDate { get; set; }
}