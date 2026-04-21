using GLMS_Assignment2.Models;
using GLMS_Assignment2.Models.Enums;

namespace GLMS_Assignment2.ViewModels
{
    // ViewModel used for the contract search/filter page.
    // Contains the filter criteria and the search results.
    public class ContractSearchViewModel
    {
        // Optional: filter by start date
        public DateTime? StartDate { get; set; }

        // Optional: filter by end date
        public DateTime? EndDate { get; set; }

        // Optional: filter by contract status
        public ContractStatus? Status { get; set; }

        // The list of contracts that match the search criteria
        public List<Contract> Results { get; set; } = new();
    }
}