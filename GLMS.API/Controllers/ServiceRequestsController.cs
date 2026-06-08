using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GLMS.API.DTOs;
using GLMS.API.Models;
using GLMS.API.Repositories;
using GLMS.API.Services;
namespace GLMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ServiceRequestsController : ControllerBase
{
    private readonly IServiceRequestRepository _repo;
    private readonly IContractRepository _contractRepo;
    private readonly ICurrencyService _currency;

    public ServiceRequestsController(IServiceRequestRepository repo,
        IContractRepository contractRepo, ICurrencyService currency)
    { _repo = repo; _contractRepo = contractRepo; _currency = currency; }

    [HttpGet("by-contract/{contractId}")]
    public async Task<IActionResult> GetByContract(int contractId)
    {
        var items = await _repo.GetByContractIdAsync(contractId);
        return Ok(items.Select(ToDto));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateServiceRequestDto dto)
    {
        var contract = await _contractRepo.GetByIdAsync(dto.ContractId);
        if (contract == null) return NotFound(new { message = "Contract not found" });

        // Workflow rule from Part 2
        if (contract.Status == ContractStatus.Expired || contract.Status == ContractStatus.OnHold)
            return BadRequest(new { message = $"Cannot create a service request for a {contract.Status} contract." });

        var rate = await _currency.GetUsdToZarRateAsync();
        var sr = new ServiceRequest
        {
            ContractId = dto.ContractId,
            Description = dto.Description,
            CostUSD = dto.CostUSD,
            CostZAR = _currency.ConvertUsdToZar(dto.CostUSD, rate),
            ExchangeRateUsed = rate,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };
        var created = await _repo.CreateAsync(sr);
        return CreatedAtAction(nameof(GetByContract), new { contractId = created.ContractId }, ToDto(created));
    }

    private static ServiceRequestDto ToDto(ServiceRequest sr) => new()
    {
        ServiceRequestId = sr.ServiceRequestId,
        ContractId = sr.ContractId,
        Description = sr.Description,
        CostUSD = sr.CostUSD,
        CostZAR = sr.CostZAR,
        ExchangeRateUsed = sr.ExchangeRateUsed,
        Status = sr.Status,
        CreatedAt = sr.CreatedAt
    };
}
