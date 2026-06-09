using GLMS.API.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GLMS.API.Data;

public static class SeedData
{
    public static async Task EnsureSeedDataAsync(AppDbContext db)
    {
        if (db.Clients.Any()) return;

        var client = new Client
        {
            Name = "Demo Client",
            ContactDetails = "demo@glms.co.za",
            Region = "Gauteng"
        };
        db.Clients.Add(client);
        await db.SaveChangesAsync();

        var contract = new Contract
        {
            ClientId = client.ClientId,
            StartDate = DateTime.UtcNow.Date.AddMonths(-1),
            EndDate = DateTime.UtcNow.Date.AddMonths(11),
            ServiceLevel = "Standard",
            Status = ContractStatus.Active
        };
        db.Contracts.Add(contract);
        await db.SaveChangesAsync();

        var sr = new ServiceRequest
        {
            ContractId = contract.ContractId,
            Description = "Demo service request",
            CostUSD = 100m,
            ExchangeRateUsed = 18.50m,
            CostZAR = 1850m,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };
        db.ServiceRequests.Add(sr);
        await db.SaveChangesAsync();
    }
}
