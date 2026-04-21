using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GLMS_Assignment2.Models.Enums;
namespace GLMS_Assignment2.Models
{
    public class Contract
    {
        public int ContractId { get; set; }
        [Required][Display(Name = "Client")] public int ClientId { get; set; }
        [ForeignKey("ClientId")] public Client? Client { get; set; }
        [Required][DataType(DataType.Date)][Display(Name = "Start Date")] public DateTime StartDate { get; set; }
        [Required][DataType(DataType.Date)][Display(Name = "End Date")] public DateTime EndDate { get; set; }
        [Required] public ContractStatus Status { get; set; } = ContractStatus.Draft;
        [Required][Display(Name = "Service Level")] public ServiceLevel ServiceLevel { get; set; } = ServiceLevel.Standard;
        [Display(Name = "Signed Agreement")] public string? SignedAgreementPath { get; set; }
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    }
}