using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GLMS.Web.Models.Enums;
namespace GLMS.Web.Models
{
    public class ServiceRequest
    {
        public int ServiceRequestId { get; set; }
        [Required][Display(Name = "Contract")] public int ContractId { get; set; }
        [ForeignKey("ContractId")] public Contract? Contract { get; set; }
        [Required(ErrorMessage = "Description is required.")][StringLength(500)]
        public string Description { get; set; } = string.Empty;
        [Required(ErrorMessage = "Cost is required.")][Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cost must be > 0.")][Display(Name = "Cost (USD)")]
        public decimal CostUSD { get; set; }
        [Column(TypeName = "decimal(18,2)")][Display(Name = "Cost (ZAR)")] public decimal CostZAR { get; set; }
        [Column(TypeName = "decimal(18,6)")][Display(Name = "Exchange Rate Used")] public decimal ExchangeRateUsed { get; set; }
        [Required] public RequestStatus Status { get; set; } = RequestStatus.Pending;
        [DataType(DataType.Date)][Display(Name = "Created Date")] public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}