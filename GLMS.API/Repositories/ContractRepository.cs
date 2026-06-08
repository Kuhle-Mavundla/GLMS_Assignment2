using Microsoft.EntityFrameworkCore;
using GLMS.API.Data;
using GLMS.API.Models;
namespace GLMS.API.Repositories;

public class ContractRepository : IContractRepository
{
    private readonly AppDbContext _db;
    public ContractRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<Contract>> GetAllAsync(string? status = null, DateTime? start = null, DateTime? end = null)
    {
        var q = _db.Contracts.Include(c => c.Client).AsQueryable();
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<ContractStatus>(status, true, out var s))
            q = q.Where(c => c.Status == s);
        if (start.HasValue) q = q.Where(c => c.StartDate >= start.Value);
        if (end.HasValue) q = q.Where(c => c.EndDate <= end.Value);
        return await q.ToListAsync();
    }

    public async Task<Contract?> GetByIdAsync(int id) =>
        await _db.Contracts.Include(c => c.Client)
                           .Include(c => c.ServiceRequests)
                           .FirstOrDefaultAsync(c => c.ContractId == id);

    public async Task<Contract> CreateAsync(Contract c)
    {
        _db.Contracts.Add(c);
        await _db.SaveChangesAsync();
        return c;
    }

    public async Task<Contract?> UpdateStatusAsync(int id, ContractStatus status)
    {
        var c = await _db.Contracts.FindAsync(id);
        if (c == null) return null;
        c.Status = status;
        await _db.SaveChangesAsync();
        return c;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var c = await _db.Contracts.FindAsync(id);
        if (c == null) return false;
        _db.Contracts.Remove(c);
        await _db.SaveChangesAsync();
        return true;
    }
}