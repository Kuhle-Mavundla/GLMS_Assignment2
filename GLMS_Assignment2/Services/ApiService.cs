using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using GLMS_Assignment2.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace GLMS_Assignment2.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _ctx;
    private readonly AppDbContext? _db;
    private readonly IConfiguration _cfg;
    private static readonly JsonSerializerOptions Opts = new() { PropertyNameCaseInsensitive = true };

    public ApiService(HttpClient http, IHttpContextAccessor ctx, AppDbContext? db, IConfiguration cfg)
    {
        _http = http; _ctx = ctx; _db = db; _cfg = cfg;
    }

    private void Auth()
    {
        var t = _ctx.HttpContext?.Session.GetString("JwtToken");
        if (!string.IsNullOrEmpty(t))
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", t);
    }

    private string EnsureAbsolute(string path)
    {
        if (Uri.IsWellFormedUriString(path, UriKind.Absolute)) return path;
        var baseUrl = _http.BaseAddress?.ToString() ?? _cfg["ApiSettings:BaseUrl"] ?? string.Empty;
        if (string.IsNullOrEmpty(baseUrl)) throw new InvalidOperationException("No API base URL configured and request path is relative.");
        return baseUrl.TrimEnd('/') + "/" + path.TrimStart('/');
    }

    public async Task<T?> GetAsync<T>(string path)
    {
        Auth();
        try
        {
            var r = await _http.GetAsync(EnsureAbsolute(path));
            if (!r.IsSuccessStatusCode) return default;
            return JsonSerializer.Deserialize<T>(await r.Content.ReadAsStringAsync(), Opts);
        }
        catch (HttpRequestException) { }
        catch (SocketException) { }
        catch (SqlException) { }

        // Fallback to local DB data when API is unreachable
        if (_db == null) return default;
        try
        {
            if (path.StartsWith("api/clients", StringComparison.OrdinalIgnoreCase))
            {
                var list = _db.Clients.ToList();
                return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(list), Opts);
            }
            if (path.StartsWith("api/contracts", StringComparison.OrdinalIgnoreCase))
            {
                var list = _db.Contracts.ToList();
                return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(list), Opts);
            }
            // single resource e.g. api/contracts/{id}
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length >= 2 && segments[0].Equals("api", StringComparison.OrdinalIgnoreCase))
            {
                if (segments[1].Equals("contracts", StringComparison.OrdinalIgnoreCase) && segments.Length >= 3 && int.TryParse(segments[2], out var id))
                {
                    var c = _db.Contracts.Find(id);
                    return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(c), Opts);
                }
                if (segments[1].Equals("clients", StringComparison.OrdinalIgnoreCase) && segments.Length >= 3 && int.TryParse(segments[2], out var cid))
                {
                    var c = _db.Clients.Find(cid);
                    return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(c), Opts);
                }
            }
        }
        catch { }

        return default;
    }

    public async Task<(bool ok, string? err, T? data)> PostAsync<T>(string path, object body)
    {
        Auth();
        var c = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        try
        {
            var r = await _http.PostAsync(EnsureAbsolute(path), c);
            var json = await r.Content.ReadAsStringAsync();
            if (!r.IsSuccessStatusCode) return (false, json, default);
            return (true, null, JsonSerializer.Deserialize<T>(json, Opts));
        }
        catch (HttpRequestException) { }
        catch (SocketException) { }
        catch (SqlException) { }

        // fallback: when creating resources locally
        if (_db == null) return (false, "API unreachable and local DB not available", default);
        try
        {
            if (path.StartsWith("api/clients", StringComparison.OrdinalIgnoreCase))
            {
                var client = JsonSerializer.Deserialize<GLMS_Assignment2.Models.Client>(JsonSerializer.Serialize(body), Opts);
                if (client != null)
                {
                    _db.Clients.Add(client);
                    await _db.SaveChangesAsync();
                    return (true, null, JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(client), Opts));
                }
            }
            if (path.StartsWith("api/contracts", StringComparison.OrdinalIgnoreCase))
            {
                var contract = JsonSerializer.Deserialize<GLMS_Assignment2.Models.Contract>(JsonSerializer.Serialize(body), Opts);
                if (contract != null)
                {
                    _db.Contracts.Add(contract);
                    await _db.SaveChangesAsync();
                    return (true, null, JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(contract), Opts));
                }
            }
        }
        catch { }

        return (false, "API unreachable and fallback failed", default);
    }

    public async Task<bool> PatchAsync(string path, object body)
    {
        Auth();
        var c = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        return (await _http.PatchAsync(EnsureAbsolute(path), c)).IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(string path)
    { Auth(); return (await _http.DeleteAsync(EnsureAbsolute(path))).IsSuccessStatusCode; }

    public async Task<string?> LoginAsync(string username, string password)
    {
        var c = new StringContent(
            JsonSerializer.Serialize(new { username, password }), Encoding.UTF8, "application/json");
        try
        {
            var r = await _http.PostAsync(EnsureAbsolute("api/auth/login"), c);
            if (!r.IsSuccessStatusCode) return null;
            var doc = JsonSerializer.Deserialize<JsonElement>(await r.Content.ReadAsStringAsync(), Opts);
            return doc.GetProperty("token").GetString();
        }
        catch (HttpRequestException) { }
        catch (SocketException) { }
        catch (SqlException) { }

        // Fallback: allow login with local admin credentials configured in appsettings
        var localUser = _cfg["AdminCredentials:Username"] ?? "admin";
        var localPass = _cfg["AdminCredentials:Password"] ?? "Admin@1234";
        if (username == localUser && password == localPass)
        {
            // return a simple local token that the web app can store
            return "local-dummy-token";
        }
        return null;
    }
}