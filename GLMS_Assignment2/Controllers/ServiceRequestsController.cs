using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GLMS_Assignment2.Data;
using GLMS_Assignment2.Services;
using GLMS_Assignment2.Models;
using GLMS_Assignment2.Models.Enums;
using GLMS_Assignment2.Services.Interfaces;
using GLMS_Assignment2.ViewModels;

namespace GLMS_Assignment2.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ICurrencyService _currencyService;
        private readonly IContractValidationService _validationService;
        private readonly ApiService _api;

        public ServiceRequestsController(AppDbContext context, ICurrencyService currencyService, IContractValidationService validationService, ApiService api)
        {
            _context = context;
            _currencyService = currencyService;
            _validationService = validationService;
            _api = api;
        }

        // GET: ServiceRequests
        public async Task<IActionResult> Index()
        {
            var requests = await _context.ServiceRequests.Include(sr => sr.Contract)
                .ThenInclude(c => c!.Client).ToListAsync();
            return View(requests);
        }

        // GET: ServiceRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var sr = await _context.ServiceRequests.Include(s => s.Contract)
                .ThenInclude(c => c!.Client).FirstOrDefaultAsync(s => s.ServiceRequestId == id);
            if (sr == null) return NotFound();
            return View(sr);
        }

        // GET: ServiceRequests/Create
        public async Task<IActionResult> Create(int? contractId)
        {
            var vm = new ServiceRequestCreateViewModel();

            // Try to get the current exchange rate
            try
            {
                vm.CurrentRate = await _currencyService.GetUsdToZarRateAsync();
            }
            catch
            {
                ViewBag.CurrencyError = "Unable to fetch current exchange rate. The rate will be retried on submission.";
            }

            if (contractId.HasValue)
            {
                var contract = await _context.Contracts.Include(c => c.Client)
                    .FirstOrDefaultAsync(c => c.ContractId == contractId.Value);

                if (contract != null)
                {
                    // Validate using the separated validation service
                    var (isValid, error) = _validationService.CanCreateServiceRequest(contract);
                    if (!isValid)
                    {
                        TempData["Error"] = error;
                        return RedirectToAction("Details", "Contracts", new { id = contractId });
                    }
                    vm.ContractId = contract.ContractId;
                    vm.ContractTitle = $"Contract #{contract.ContractId} — {contract.Client?.Name}";
                }
            }

            // Try to fetch contracts from API first, otherwise fall back to local DB
            List<Contract> contracts = null;
            List<Client> clients = null;
            try
            {
                contracts = await _api.GetAsync<List<Contract>>("api/contracts");
                clients = await _api.GetAsync<List<Client>>("api/clients");
            }
            catch { contracts = null; clients = null; }

            if (contracts == null || contracts.Count == 0)
            {
                contracts = await _context.Contracts.Include(c => c.Client)
                    .Where(c => c.Status != ContractStatus.Expired && c.Status != ContractStatus.OnHold)
                    .ToListAsync();
            }
            // ensure we have a clients list for name lookups
            if (clients == null || clients.Count == 0)
            {
                clients = await _context.Clients.ToListAsync();
            }

            // If still empty, provide a small hard-coded fallback so the dropdown is never empty
            if (contracts == null || contracts.Count == 0)
            {
                var fallbackClient = new Client { ClientId = -1, Name = "Fallback Client", ContactDetails = "n/a", Region = "n/a" };
                var fallbackContracts = new List<Contract>
                {
                    new Contract { ContractId = -101, ClientId = -1, Client = fallbackClient, StartDate = DateTime.UtcNow.Date.AddMonths(-1), EndDate = DateTime.UtcNow.Date.AddMonths(11), Status = ContractStatus.Active, ServiceLevel = ServiceLevel.Standard },
                    new Contract { ContractId = -102, ClientId = -1, Client = fallbackClient, StartDate = DateTime.UtcNow.Date.AddMonths(-2), EndDate = DateTime.UtcNow.Date.AddMonths(10), Status = ContractStatus.Draft, ServiceLevel = ServiceLevel.Premium }
                };
                contracts = fallbackContracts;
                // ensure clients list contains fallback client for lookup
                if (clients == null) clients = new List<Client>();
                if (!clients.Any(c => c.ClientId == -1)) clients.Add(fallbackClient);
            }

            var items = contracts.Select(c => new
            {
                c.ContractId,
                Display = $"#{c.ContractId} — { (c.Client?.Name ?? clients.FirstOrDefault(x => x.ClientId == c.ClientId)?.Name ?? "(Unknown)") } ({c.Status})"
            });

            ViewData["ContractId"] = new SelectList(items, "ContractId", "Display", vm.ContractId);

            return View(vm);
        }

        // POST: ServiceRequests/Create
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequestCreateViewModel vm)
        {
            // Re-validate the contract status (server-side)
            var contract = await _context.Contracts.FindAsync(vm.ContractId);
            if (contract == null)
            {
                ModelState.AddModelError("ContractId", "Contract not found.");
            }
            else
            {
                var (isValid, error) = _validationService.CanCreateServiceRequest(contract);
                if (!isValid)
                    ModelState.AddModelError("ContractId", error);
            }

            if (ModelState.IsValid)
            {
                // Get exchange rate and convert
                decimal rate;
                try
                {
                    rate = await _currencyService.GetUsdToZarRateAsync();
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    await PopulateContractDropdown(vm);
                    return View(vm);
                }

                var serviceRequest = new ServiceRequest
                {
                    ContractId = vm.ContractId,
                    Description = vm.Description,
                    CostUSD = vm.CostUSD,
                    ExchangeRateUsed = rate,
                    CostZAR = _currencyService.ConvertUsdToZar(vm.CostUSD, rate),
                    Status = RequestStatus.Pending,
                    CreatedDate = DateTime.Now
                };

                // If API reachable, post to API; otherwise save locally
                try
                {
                    var (ok, err, _) = await _api.PostAsync<object>("api/servicerequests", new
                    {
                        contractId = serviceRequest.ContractId,
                        description = serviceRequest.Description,
                        costUSD = serviceRequest.CostUSD
                    });
                    if (!ok)
                    {
                        // fallback to local save
                        _context.Add(serviceRequest);
                        await _context.SaveChangesAsync();
                    }
                }
                catch
                {
                    _context.Add(serviceRequest);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }

            await PopulateContractDropdown(vm);
            return View(vm);
        }

        private async Task PopulateContractDropdown(ServiceRequestCreateViewModel vm)
        {
            ViewData["ContractId"] = new SelectList(
                await _context.Contracts.Include(c => c.Client)
                    .Where(c => c.Status != ContractStatus.Expired && c.Status != ContractStatus.OnHold)
                    .Select(c => new { c.ContractId, Display = $"#{c.ContractId} — {c.Client!.Name} ({c.Status})" })
                    .ToListAsync(),
                "ContractId", "Display", vm.ContractId);
        }

        // GET: ServiceRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var sr = await _context.ServiceRequests.Include(s => s.Contract).FirstOrDefaultAsync(s => s.ServiceRequestId == id);
            if (sr == null) return NotFound();
            return View(sr);
        }

        // POST: ServiceRequests/Delete/5
        [HttpPost, ActionName("Delete")][ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sr = await _context.ServiceRequests.FindAsync(id);
            if (sr != null) _context.ServiceRequests.Remove(sr);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}