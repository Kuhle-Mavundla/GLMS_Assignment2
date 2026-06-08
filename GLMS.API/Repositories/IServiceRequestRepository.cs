using GLMS.API.Models;
namespace GLMS.API.Repositories;

public interface IServiceRequestRepository
{
    Task<IEnumerable<ServiceRequest>> GetByContractIdAsync(int contractId);
    Task<ServiceRequest?> GetByIdAsync(int id);
    Task<ServiceRequest> CreateAsync(ServiceRequest sr);
}