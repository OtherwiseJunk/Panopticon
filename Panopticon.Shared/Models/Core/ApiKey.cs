using System.ComponentModel.DataAnnotations;

namespace Panopticon.Shared.Models.Core;

public class ApiKey
{
    [Key]
    public int Id { get; set; }
    public string Key { get; set; }
    public string DeveloperName { get; set; }
    public string KeyPurpose { get; set; }
    public int Permissions { get; set; }
    public DateTime DateCreated { get; set; }
}