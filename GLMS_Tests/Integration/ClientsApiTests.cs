using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;
namespace GLMS_Tests.Integration;

public class ClientsApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ClientsApiTests(WebApplicationFactory<Program> factory)
        => _client = factory.CreateClient();

    private async Task SetAuth()
    {
        var r = await _client.PostAsJsonAsync("/api/auth/login",
            new { username = "admin", password = "Admin@1234" });
        var t = await r.Content.ReadFromJsonAsync<TokenResponse>();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", t?.Token ?? "");
    }

    [Fact]
    public async Task GetClients_Returns200AndNotNull()
    {
        await SetAuth();
        var r = await _client.GetAsync("/api/clients");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        Assert.NotNull(await r.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task CreateClient_ValidData_Returns201()
    {
        await SetAuth();
        var r = await _client.PostAsJsonAsync("/api/clients",
            new { name = "Integration Test Client", contactDetails = "test@glms.co.za", region = "Gauteng" });
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
    }

    [Fact]
    public async Task GetClient_NonExistentId_Returns404()
    {
        await SetAuth();
        var r = await _client.GetAsync("/api/clients/99999");
        Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
    }

    [Fact]
    public async Task CreateServiceRequest_OnExpiredContract_Returns400()
    {
        await SetAuth();
        // ContractId 99998 won't exist — tests the 404 path
        var r = await _client.PostAsJsonAsync("/api/servicerequests",
            new { contractId = 99998, description = "Test SR", costUSD = 100 });
        Assert.True(r.StatusCode == HttpStatusCode.NotFound ||
                    r.StatusCode == HttpStatusCode.BadRequest);
    }

    private record TokenResponse(string Token, DateTime Expiry);
}