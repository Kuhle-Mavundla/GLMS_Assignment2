
using GLMS_Assignment2.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
namespace GLMS_Assignment2.Controllers;

public class ContractsController : Controller
{
    private readonly ApiService _api;
    public ContractsController(ApiService api) => _api = api;

    private bool IsLoggedIn => !string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken"));
    private IActionResult RequireLogin() => RedirectToAction("Login", "Auth");

    public async Task<IActionResult> Index(string? status, DateTime? startDate, DateTime? endDate)
    {
        if (!IsLoggedIn) return RequireLogin();
        var url = "api/contracts";
        var qs = new List<string>();
        if (!string.IsNullOrEmpty(status)) qs.Add($"status={status}");
        if (startDate.HasValue) qs.Add($"startDate={startDate:yyyy-MM-dd}");
        if (endDate.HasValue) qs.Add($"endDate={endDate:yyyy-MM-dd}");
        if (qs.Any()) url += "?" + string.Join("&", qs);

        var contracts = await _api.GetAsync<List<JsonElement>>(url) ?? new();
        return View(contracts);
    }

    public async Task<IActionResult> Details(int id)
    {
        if (!IsLoggedIn) return RequireLogin();
        var c = await _api.GetAsync<JsonElement?>($"api/contracts/{id}");
        if (c == null) return NotFound();
        return View(c);
    }

    [HttpGet] public IActionResult Create() { if (!IsLoggedIn) return RequireLogin(); return View(); }

    [HttpPost]
    public async Task<IActionResult> Create(int clientId, DateTime startDate, DateTime endDate,
        string status, string serviceLevel)
    {
        if (!IsLoggedIn) return RequireLogin();
        var (ok, err, _) = await _api.PostAsync<JsonElement>("api/contracts",
            new { clientId, startDate, endDate, status, serviceLevel });
        if (!ok) { ViewBag.Error = err; return View(); }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, string newStatus)
    {
        if (!IsLoggedIn) return RequireLogin();
        await _api.PatchAsync($"api/contracts/{id}/status", new { status = newStatus });
        return RedirectToAction(nameof(Details), new { id });
    }
}