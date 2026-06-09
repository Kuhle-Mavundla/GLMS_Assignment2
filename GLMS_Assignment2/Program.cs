
using GLMS_Assignment2.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using GLMS_Assignment2.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Session — stores the JWT token
builder.Services.AddSession(o => {
    o.IdleTimeout = TimeSpan.FromHours(8);
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

// Add cookie authentication so MVC authorization/challenge works in the web app
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/Login";
        options.Cookie.HttpOnly = true;
    });

// HttpClient for calling the API
var apiBase = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5001";
builder.Services.AddHttpClient<ApiService>(c => c.BaseAddress = new Uri(apiBase));

// Add AppDbContext so the web app can use the same database (overridden by docker-compose env)
var webConn = builder.Configuration.GetConnectionString("DefaultConnection") ??
    "Server=(localdb)\\mssqllocaldb;Database=GLMS_Web_DB;Trusted_Connection=True;MultipleActiveResultSets=true";
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(webConn));
// ApiService is registered as a typed HttpClient above; no manual factory required

// Register currency service and contract validation service for DI
builder.Services.AddHttpClient<GLMS_Assignment2.Services.Interfaces.ICurrencyService, GLMS_Assignment2.Services.CurrencyService>();
builder.Services.AddScoped<GLMS_Assignment2.Services.Interfaces.IContractValidationService, GLMS_Assignment2.Services.ContractValidationService>();

var app = builder.Build();

// Attempt to ensure the local web DB schema exists and seed minimal data.
// This helps the web app operate when the API or migrations haven't been
// applied yet (useful during development). We retry a few times to allow
// the database container to become ready.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetService<AppDbContext>();
    var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
    if (db != null)
    {
        var migrated = false;
        for (int attempt = 0; attempt < 6; attempt++)
        {
            try
            {
                db.Database.Migrate();
                migrated = true;
                break;
            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, "Database migrate attempt {Attempt} failed.", attempt + 1);
                // wait before retrying
                await Task.Delay(2000);
            }
        }

        if (migrated)
        {
            try
            {
                if (!db.Clients.Any())
                {
                    var client = new GLMS_Assignment2.Models.Client
                    {
                        Name = "Default Client",
                        ContactDetails = "test@glms.co.za",
                        Region = "Gauteng"
                    };
                    db.Clients.Add(client);
                    await db.SaveChangesAsync();

                    var contract = new GLMS_Assignment2.Models.Contract
                    {
                        ClientId = client.ClientId,
                        StartDate = DateTime.UtcNow.Date.AddMonths(-1),
                        EndDate = DateTime.UtcNow.Date.AddMonths(11),
                        ServiceLevel = GLMS_Assignment2.Models.Enums.ServiceLevel.Standard,
                        Status = GLMS_Assignment2.Models.Enums.ContractStatus.Active
                    };
                    db.Contracts.Add(contract);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Seeding demo data failed.");
            }
        }
        else
        {
            logger?.LogError("Database migration failed after multiple attempts; web app may not function correctly.");
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();       // <-- must be before UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();