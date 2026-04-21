using GLMS_Assignment2.Models;
namespace GLMS_Assignment2.Services.Interfaces
{ public interface IContractValidationService { (bool IsValid, string ErrorMessage) CanCreateServiceRequest(Contract contract); bool IsValidDateRange(DateTime startDate, DateTime endDate); } }