using Microsoft.EntityFrameworkCore;
using GLMS.API.Data;
using GLMS.API.Models;
namespace GLMS.API.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly AppDbContext _db;
    public ClientRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<Client>> GetAllAsync() =>
        await _db.Clients.ToListAsync();

    public async Task<Client?> GetByIdAsync(int id) =>
        await _db.Clients.FindAsync(id);

    public async Task<Client> CreateAsync(Client c)
    {
        _db.Clients.Add(c);
        await _db.SaveChangesAsync();
        return c;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var c = await _db.Clients.FindAsync(id);
        if (c == null) return false;
        _db.Clients.Remove(c);
        await _db.SaveChangesAsync();
        return true;
    }
}