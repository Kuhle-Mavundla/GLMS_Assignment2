using System.ComponentModel.DataAnnotations;

namespace GLMS.Web.ViewModels
{
    // ViewModel for the service request creation form.
    // Contains both the form fields and the currency conversion display values.
    public class ServiceRequestCreateViewModel
    {
        [Required]
        public int ContractId { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500)]
        public string Description { get; set; } = "";

        [Required(ErrorMessage = "Cost (USD) is required.")]
        [Range(0.01, double.MaxValue)]
        [Display(Name = "Cost (USD)")]
        public decimal CostUSD { get; set; }

        // The current exchange rate (shown to the user, not submitted)
        public decimal? CurrentRate { get; set; }

        // The estimated ZAR cost (calculated on the page)
        public decimal? EstimatedZAR { get; set; }

        // Display name for the selected contract
        public string? ContractTitle { get; set; }
    }
}