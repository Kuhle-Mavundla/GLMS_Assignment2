using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace GLMS.API.Data;

// Design-time factory so 'dotnet ef' and tools can create the context
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Try to locate the appsettings.json reliably when EF tools run from solution root
        var current = Directory.GetCurrentDirectory();
        var basePath = current;

        // If appsettings.json is not in the current directory, try the project folder (GLMS.API)
        if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
        {
            var alt = Path.Combine(current, "GLMS.API");
            if (Directory.Exists(alt) && File.Exists(Path.Combine(alt, "appsettings.json")))
            {
                basePath = alt;
            }
            else
            {
                // as a last resort try the assembly location
                var asmDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? current;
                if (File.Exists(Path.Combine(asmDir, "appsettings.json"))) basePath = asmDir;
            }
        }

        var builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables();

        var config = builder.Build();
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        var conn = config.GetConnectionString("DefaultConnection") ??
            "Server=(localdb)\\mssqllocaldb;Database=GLMS_DB;Trusted_Connection=True;MultipleActiveResultSets=true";
        optionsBuilder.UseSqlServer(conn);
        return new AppDbContext(optionsBuilder.Options);
    }
}
            