using GLMS_Assignment2.Data;
using GLMS_Assignment2.Services;
using GLMS_Assignment2.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

// This is the entry point of the application.
// It sets up all the services and middleware needed to run the MVC app.

var builder = WebApplication.CreateBuilder(args);

// --- Register MVC controllers and views ---
builder.Services.AddControllersWithViews();

// --- Register Entity Framework Core with SQL Server ---
// The connection string is read from appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Register our custom services using Dependency Injection ---
// Scoped means a new instance is created for each HTTP request
builder.Services.AddScoped<IContractValidationService, ContractValidationService>();
builder.Services.AddScoped<IFileService, FileService>();

// Register HttpClient for the Currency Service (used to call the exchange rate API)
builder.Services.AddHttpClient<ICurrencyService, CurrencyService>();

var app = builder.Build();

// --- Configure the HTTP request pipeline ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();    // Serves files from wwwroot (CSS, uploads, etc.)
app.UseRouting();
app.UseAuthorization();

// Set the default route: Home/Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();