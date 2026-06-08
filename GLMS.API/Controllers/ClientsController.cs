using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GLMS.API.DTOs;
using GLMS.API.Models;
using GLMS.API.Repositories;
namespace GLMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientsController : ControllerBase
{
    private readonly IClientRepository _repo;
    public ClientsController(IClientRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clients = await _repo.GetAllAsync();
        return Ok(clients.Select(c => new ClientDto
        {
            ClientId = c.ClientId,
            Name = c.Name,
            ContactDetails = c.ContactDetails,
            Region = c.Region
        }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var c = await _repo.GetByIdAsync(id);
        if (c == null) return NotFound(new { message = $"Client {id} not found" });
        return Ok(new ClientDto
        {
            ClientId = c.ClientId,
            Name = c.Name,
            ContactDetails = c.ContactDetails,
            Region = c.Region
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClientDto dto)
    {
        var c = new Client { Name = dto.Name, ContactDetails = dto.ContactDetails, Region = dto.Region };
        var created = await _repo.CreateAsync(c);
        return CreatedAtAction(nameof(GetById), new { id = created.ClientId },
            new ClientDto
            {
                ClientId = created.ClientId,
                Name = created.Name,
                ContactDetails = created.ContactDetails,
                Region = created.Region
            });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _repo.DeleteAsync(id)) return NotFound();
        return NoContent();
    }
}