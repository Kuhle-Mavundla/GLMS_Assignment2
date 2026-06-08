
using GLMS_Assignment2.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Session — stores the JWT token
builder.Services.AddSession(o => {
    o.IdleTimeout = TimeSpan.FromHours(8);
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

// HttpClient for calling the API
var apiBase = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5001";
builder.Services.AddHttpClient<ApiService>(c => c.BaseAddress = new Uri(apiBase + "/"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();       // <-- must be before UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();