using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
namespace GLMS_Assignment2.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _ctx;
    private static readonly JsonSerializerOptions Opts = new() { PropertyNameCaseInsensitive = true };

    public ApiService(HttpClient http, IHttpContextAccessor ctx) { _http = http; _ctx = ctx; }

    private void Auth()
    {
        var t = _ctx.HttpContext?.Session.GetString("JwtToken");
        if (!string.IsNullOrEmpty(t))
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", t);
    }

    public async Task<T?> GetAsync<T>(string path)
    {
        Auth();
        var r = await _http.GetAsync(path);
        if (!r.IsSuccessStatusCode) return default;
        return JsonSerializer.Deserialize<T>(await r.Content.ReadAsStringAsync(), Opts);
    }

    public async Task<(bool ok, string? err, T? data)> PostAsync<T>(string path, object body)
    {
        Auth();
        var c = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        var r = await _http.PostAsync(path, c);
        var json = await r.Content.ReadAsStringAsync();
        if (!r.IsSuccessStatusCode) return (false, json, default);
        return (true, null, JsonSerializer.Deserialize<T>(json, Opts));
    }

    public async Task<bool> PatchAsync(string path, object body)
    {
        Auth();
        var c = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        return (await _http.PatchAsync(path, c)).IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(string path)
    { Auth(); return (await _http.DeleteAsync(path)).IsSuccessStatusCode; }

    public async Task<string?> LoginAsync(string username, string password)
    {
        var c = new StringContent(
            JsonSerializer.Serialize(new { username, password }), Encoding.UTF8, "application/json");
        var r = await _http.PostAsync("api/auth/login", c);
        if (!r.IsSuccessStatusCode) return null;
        var doc = JsonSerializer.Deserialize<JsonElement>(await r.Content.ReadAsStringAsync(), Opts);
        return doc.GetProperty("token").GetString();
    }
}