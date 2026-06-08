using GLMS.API.Models;
namespace GLMS.API.Repositories;

public interface IContractRepository
{
    Task<IEnumerable<Contract>> GetAllAsync(string? status = null, DateTime? start = null, DateTime? end = null);
    Task<Contract?> GetByIdAsync(int id);
    Task<Contract> CreateAsync(Contract contract);
    Task<Contract?> UpdateStatusAsync(int id, ContractStatus status);
    Task<bool> DeleteAsync(int id);
}