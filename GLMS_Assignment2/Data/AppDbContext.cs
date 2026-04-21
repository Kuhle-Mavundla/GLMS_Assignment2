using Microsoft.EntityFrameworkCore;
using GLMS.Web.Models;
namespace GLMS.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);
            mb.Entity<Client>(e => { e.HasKey(c => c.ClientId); e.Property(c => c.Name).IsRequired().HasMaxLength(150); e.Property(c => c.ContactDetails).IsRequired().HasMaxLength(250); e.Property(c => c.Region).IsRequired().HasMaxLength(100); });
            mb.Entity<Contract>(e => { e.HasKey(c => c.ContractId); e.HasOne(c => c.Client).WithMany(cl => cl.Contracts).HasForeignKey(c => c.ClientId).OnDelete(DeleteBehavior.Cascade); e.Property(c => c.Status).IsRequired().HasConversion<string>(); e.Property(c => c.ServiceLevel).IsRequired().HasConversion<string>(); e.Property(c => c.SignedAgreementPath).HasMaxLength(500); });
            mb.Entity<ServiceRequest>(e => { e.HasKey(s => s.ServiceRequestId); e.HasOne(s => s.Contract).WithMany(c => c.ServiceRequests).HasForeignKey(s => s.ContractId).OnDelete(DeleteBehavior.Cascade); e.Property(s => s.CostUSD).HasColumnType("decimal(18,2)"); e.Property(s => s.CostZAR).HasColumnType("decimal(18,2)"); e.Property(s => s.ExchangeRateUsed).HasColumnType("decimal(18,6)"); e.Property(s => s.Status).IsRequired().HasConversion<string>(); });
        }
    }
}