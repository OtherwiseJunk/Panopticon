using System.ComponentModel.DataAnnotations;

namespace Panopticon.Models.Libcoin;

public class LibcoinUserBalance
{
    [Key]
    public string UserId { get; set; }
    public double Balance { get; set; }
}