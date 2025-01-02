namespace Panopticon.Models.Libcoin;

public class LibcoinTransaction
{
    ulong SendingUser { get; set; }
    ulong ReceivingUser { get; set; }
    ulong Amount { get; set; }
    string TransactionMessage { get; set; }
    DateTime TransactionDate { get; set; }
}