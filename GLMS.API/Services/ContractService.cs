using GLMS.API.DTOs;
using GLMS.API.Models;
using GLMS.API.Repositories;

namespace GLMS.API.Services;

public class ContractService : IContractService
{
    private readonly IContractRepository _repo;
    public ContractService(IContractRepository repo) => _repo = repo;

    public async Task<IEnumerable<ContractDto>> GetContractsAsync(string? status, DateTime? start, DateTime? end)
        => (await _repo.GetAllAsync(status, start, end)).Select(ToDto);

    public async Task<ContractDto?> GetContractAsync(int id)
    { var c = await _repo.GetByIdAsync(id); return c == null ? null : ToDto(c); }

    public async Task<ContractDto> CreateContractAsync(CreateContractDto dto)
    {
        var c = new Contract
        {
            ClientId = dto.ClientId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = Enum.Parse<ContractStatus>(dto.Status, true),
            ServiceLevel = dto.ServiceLevel
        };
        return ToDto(await _repo.CreateAsync(c));
    }

    public async Task<ContractDto?> UpdateStatusAsync(int id, UpdateContractStatusDto dto)
    {
        if (!Enum.TryParse<ContractStatus>(dto.Status, true, out var s)) return null;
        var c = await _repo.UpdateStatusAsync(id, s);
        return c == null ? null : ToDto(c);
    }

    private static ContractDto ToDto(Contract c) => new()
    {
        ContractId = c.ContractId,
        ClientId = c.ClientId,
        ClientName = c.Client?.Name ?? string.Empty,
        StartDate = c.StartDate,
        EndDate = c.EndDate,
        Status = c.Status.ToString(),
        ServiceLevel = c.ServiceLevel,
        SignedAgreementPath = c.SignedAgreementPath
    };
}