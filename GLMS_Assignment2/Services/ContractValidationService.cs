using GLMS.Web.Models;
using GLMS.Web.Models.Enums;
using GLMS.Web.Services.Interfaces;

namespace GLMS.Web.Services
{
    // This service contains the business validation rules for contracts.
    // Keeping validation separate from controllers makes the code cleaner
    // and easier to test with unit tests.
    public class ContractValidationService : IContractValidationService
    {
        // Checks if a service request can be created for the given contract.
        // Business Rule: requests are NOT allowed on Expired or OnHold contracts.
        public (bool IsValid, string ErrorMessage) CanCreateServiceRequest(Contract contract)
        {
            // Null check — contract must exist
            if (contract == null)
                return (false, "Contract not found.");

            // Block service requests on expired contracts
            if (contract.Status == ContractStatus.Expired)
                return (false, "Cannot create a service request for an Expired contract.");

            // Block service requests on contracts that are on hold
            if (contract.Status == ContractStatus.OnHold)
                return (false, "Cannot create a service request for a contract that is On Hold.");

            // All checks passed — the request is allowed
            return (true, string.Empty);
        }

        // Validates that the end date is after the start date
        public bool IsValidDateRange(DateTime startDate, DateTime endDate)
        {
            return endDate > startDate;
        }
    }
}