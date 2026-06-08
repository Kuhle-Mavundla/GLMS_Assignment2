using GLMS.API.DTOs;
namespace GLMS.API.Services;

public interface IContractService
{
    Task<IEnumerable<ContractDto>> GetContractsAsync(string? status, DateTime? start, DateTime? end);
    Task<ContractDto?> GetContractAsync(int id);
    Task<ContractDto> CreateContractAsync(CreateContractDto dto);
    Task<ContractDto?> UpdateStatusAsync(int id, UpdateContractStatusDto dto);
}