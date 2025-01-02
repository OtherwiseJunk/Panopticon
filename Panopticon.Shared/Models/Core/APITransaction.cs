using System.ComponentModel.DataAnnotations;
using Panopticon.Enums;

namespace Panopticon.Models.Core;

public class APITransaction
{
    [Key]
    public ulong Id { get; set; }
    public ApiTransactionType Type { get; set; }
}