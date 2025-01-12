using System.ComponentModel.DataAnnotations;
using Panopticon.Enums;

namespace Panopticon.Models.Core;

public class ApiTransaction
{
    [Key]
    public ulong Id { get; set; }
    public string ApiKey { get; set; }
    public ApiTransactionType Type { get; set; }
    public string TransactionData { get; set; }
}