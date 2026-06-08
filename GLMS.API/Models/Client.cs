using System.ComponentModel.DataAnnotations;
namespace GLMS.API.Models;

using System.ComponentModel.DataAnnotations;

public class Client
{
    public int ClientId { get; set; }
    [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
    [Required, StringLength(250)] public string ContactDetails { get; set; } = string.Empty;
    [Required, StringLength(100)] public string Region { get; set; } = string.Empty;
    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
