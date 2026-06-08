using Microsoft.EntityFrameworkCore;
using GLMS.API.Data;
using GLMS.API.Models;
namespace GLMS.API.Repositories;

public class ServiceRequestRepository : IServiceRequestRepository
{
    private readonly AppDbContext _db;
    public ServiceRequestRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<ServiceRequest>> GetByContractIdAsync(int contractId) =>
        await _db.ServiceRequests.Where(sr => sr.ContractId == contractId).ToListAsync();

    public async Task<ServiceRequest?> GetByIdAsync(int id) =>
        await _db.ServiceRequests.Include(sr => sr.Contract)
                                 .FirstOrDefaultAsync(sr => sr.ServiceRequestId == id);

    public async Task<ServiceRequest> CreateAsync(ServiceRequest sr)
    {
        _db.ServiceRequests.Add(sr);
        await _db.SaveChangesAsync();
        return sr;
    }
}