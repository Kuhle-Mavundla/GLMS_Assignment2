using System.ComponentModel.DataAnnotations;
namespace GLMS.API.Models;

using System.ComponentModel.DataAnnotations;

public class Contract
{
    public int ContractId { get; set; }
    public int ClientId { get; set; }
    public Client? Client { get; set; }
    [Required] public DateTime StartDate { get; set; }
    [Required] public DateTime EndDate { get; set; }
    public ContractStatus Status { get; set; } = ContractStatus.Draft;
    [Required, StringLength(100)] public string ServiceLevel { get; set; } = string.Empty;
    public string? SignedAgreementPath { get; set; }
    public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
}