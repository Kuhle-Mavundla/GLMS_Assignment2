using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GLMS.API.DTOs;
using GLMS.API.Services;
namespace GLMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContractsController : ControllerBase
{
    private readonly IContractService _svc;
    public ContractsController(IContractService svc) => _svc = svc;

    // GET /api/contracts?status=Active&startDate=2026-01-01&endDate=2026-12-31
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
        => Ok(await _svc.GetContractsAsync(status, startDate, endDate));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var c = await _svc.GetContractAsync(id);
        return c == null ? NotFound(new { message = $"Contract {id} not found" }) : Ok(c);
    }

    // POST /api/contracts
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateContractDto dto)
    {
        var created = await _svc.CreateContractAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.ContractId }, created);
    }

    // PATCH /api/contracts/{id}/status
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateContractStatusDto dto)
    {
        var updated = await _svc.UpdateStatusAsync(id, dto);
        return updated == null
            ? NotFound(new { message = $"Contract {id} not found or invalid status" })
            : Ok(updated);
    }
}
