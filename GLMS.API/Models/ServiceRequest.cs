using System.ComponentModel.DataAnnotations;
namespace GLMS.API.Models;

public class ServiceRequest
{
    public int ServiceRequestId { get; set; }
    public int ContractId { get; set; }
    public Contract? Contract { get; set; }
    [Required, StringLength(500)] public string Description { get; set; } = string.Empty;
    [Range(0, double.MaxValue)] public decimal CostUSD { get; set; }
    public decimal CostZAR { get; set; }
    public decimal ExchangeRateUsed { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}