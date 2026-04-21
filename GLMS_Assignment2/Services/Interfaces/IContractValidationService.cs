using GLMS.Web.Models;
namespace GLMS.Web.Services.Interfaces
{ public interface IContractValidationService { (bool IsValid, string ErrorMessage) CanCreateServiceRequest(Contract contract); bool IsValidDateRange(DateTime startDate, DateTime endDate); } }