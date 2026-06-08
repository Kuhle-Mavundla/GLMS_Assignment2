using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;
namespace GLMS_Tests.Integration;

public class ContractsApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public ContractsApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<string> GetTokenAsync()
    {
        var r = await _client.PostAsJsonAsync("/api/auth/login",
            new { username = "admin", password = "Admin@1234" });
        var result = await r.Content.ReadFromJsonAsync<TokenResponse>();
        return result?.Token ?? string.Empty;
    }

    private void SetToken(string token) =>
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

    [Fact]
    public async Task Login_ValidCredentials_Returns200WithToken()
    {
        var r = await _client.PostAsJsonAsync("/api/auth/login",
            new { username = "admin", password = "Admin@1234" });
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var body = await r.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.NotNull(body?.Token);
        Assert.NotEmpty(body!.Token);
    }

    [Fact]
    public async Task Login_InvalidCredentials_Returns401()
    {
        var r = await _client.PostAsJsonAsync("/api/auth/login",
            new { username = "bad", password = "wrong" });
        Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
    }

    [Fact]
    public async Task GetContracts_WithToken_Returns200()
    {
        SetToken(await GetTokenAsync());
        var r = await _client.GetAsync("/api/contracts");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var body = await r.Content.ReadAsStringAsync();
        Assert.NotNull(body);
    }

    [Fact]
    public async Task GetContracts_WithoutToken_Returns401()
    {
        var freshClient = _factory.CreateClient();
        var r = await freshClient.GetAsync("/api/contracts");
        Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
    }

    [Fact]
    public async Task GetContract_NonExistentId_Returns404()
    {
        SetToken(await GetTokenAsync());
        var r = await _client.GetAsync("/api/contracts/99999");
        Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
    }

    [Fact]
    public async Task GetContractsWithFilter_ValidStatus_Returns200()
    {
        SetToken(await GetTokenAsync());
        var r = await _client.GetAsync("/api/contracts?status=Active");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
    }

    private record TokenResponse(string Token, DateTime Expiry);
}
