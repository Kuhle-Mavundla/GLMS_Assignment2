
using GLMS_Assignment2.Services;
using GLMS_Assignment2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Microsoft.EntityFrameworkCore;
namespace GLMS_Assignment2.Controllers;

public class ContractsController : Controller
{
    private readonly ApiService _api;
    private readonly GLMS_Assignment2.Data.AppDbContext _db;
    public ContractsController(ApiService api, GLMS_Assignment2.Data.AppDbContext db) { _api = api; _db = db; }

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

        var contracts = await _api.GetAsync<List<Contract>>(url) ?? new List<Contract>();
        return View(contracts);
    }

    public async Task<IActionResult> Details(int id)
    {
        if (!IsLoggedIn) return RequireLogin();
        var c = await _api.GetAsync<Contract?>(($"api/contracts/{id}"));
        if (c == null) return NotFound();
        return View(c);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (!IsLoggedIn) return RequireLogin();
        // populate clients for the dropdown
        var clients = await _api.GetAsync<List<Client>>("api/clients") ?? new List<Client>();
        if (clients.Count == 0)
        {
            // fallback to local DB if API returned nothing
            clients = await _db.Clients.ToListAsync();
        }
        ViewBag.ClientId = new SelectList(clients, "ClientId", "Name");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(int clientId, DateTime startDate, DateTime endDate,
        string status, string serviceLevel)
    {
        if (!IsLoggedIn) return RequireLogin();
        var (ok, err, _) = await _api.PostAsync<Contract>("api/contracts",
            new { clientId, startDate, endDate, status, serviceLevel });
        if (!ok)
        {
            // repopulate dropdown on error
            var clients = await _api.GetAsync<List<Client>>("api/clients") ?? new List<Client>();
            if (clients.Count == 0) clients = await _db.Clients.ToListAsync();
            ViewBag.ClientId = new SelectList(clients, "ClientId", "Name");
            ViewBag.Error = err;
            return View();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Search()
    {
        if (!IsLoggedIn) return RequireLogin();
        return View(new GLMS_Assignment2.ViewModels.ContractSearchViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Search(GLMS_Assignment2.ViewModels.ContractSearchViewModel model)
    {
        if (!IsLoggedIn) return RequireLogin();
        var url = "api/contracts";
        var qs = new List<string>();
        if (model.StartDate.HasValue) qs.Add($"startDate={model.StartDate:yyyy-MM-dd}");
        if (model.EndDate.HasValue) qs.Add($"endDate={model.EndDate:yyyy-MM-dd}");
        if (model.Status.HasValue) qs.Add($"status={model.Status}");
        if (qs.Any()) url += "?" + string.Join("&", qs);
        model.Results = await _api.GetAsync<List<Contract>>(url) ?? new List<Contract>();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, string newStatus)
    {
        if (!IsLoggedIn) return RequireLogin();
        await _api.PatchAsync($"api/contracts/{id}/status", new { status = newStatus });
        return RedirectToAction(nameof(Details), new { id });
    }
}