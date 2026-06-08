using GLMS.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
namespace GLMS.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();

    protected override void OnModelCreating(ModelBuilder m)
    {
        base.OnModelCreating(m);
        m.Entity<Client>(e => {
            e.HasKey(c => c.ClientId);
            e.Property(c => c.Name).IsRequired().HasMaxLength(150);
            e.Property(c => c.ContactDetails).IsRequired().HasMaxLength(250);
            e.Property(c => c.Region).IsRequired().HasMaxLength(100);
        });
        m.Entity<Contract>(e => {
            e.HasKey(c => c.ContractId);
            e.Property(c => c.ServiceLevel).IsRequired().HasMaxLength(100);
            e.Property(c => c.Status).HasConversion<string>();
            e.HasOne(c => c.Client).WithMany(cl => cl.Contracts)
             .HasForeignKey(c => c.ClientId).OnDelete(DeleteBehavior.Cascade);
        });
        m.Entity<ServiceRequest>(e => {
            e.HasKey(sr => sr.ServiceRequestId);
            e.Property(sr => sr.CostUSD).HasPrecision(18, 2);
            e.Property(sr => sr.CostZAR).HasPrecision(18, 2);
            e.Property(sr => sr.ExchangeRateUsed).HasPrecision(18, 4);
            e.HasOne(sr => sr.Contract).WithMany(c => c.ServiceRequests)
             .HasForeignKey(sr => sr.ContractId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}