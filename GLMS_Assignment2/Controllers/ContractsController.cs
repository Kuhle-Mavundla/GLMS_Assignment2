using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GLMS_Assignment2.Data;
using GLMS_Assignment2.Models;
using GLMS_Assignment2.Models.Enums;
using GLMS_Assignment2.Services.Interfaces;
using GLMS_Assignment2.ViewModels;

namespace GLMS_Assignment2.Controllers
{
    public class ContractsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;
        private readonly IContractValidationService _validationService;

        public ContractsController(AppDbContext context, IFileService fileService, IContractValidationService validationService)
        {
            _context = context;
            _fileService = fileService;
            _validationService = validationService;
        }

        // GET: Contracts
        public async Task<IActionResult> Index()
        {
            var contracts = await _context.Contracts.Include(c => c.Client).ToListAsync();
            return View(contracts);
        }

        // GET: Contracts/Search
        public IActionResult Search()
        {
            return View(new ContractSearchViewModel());
        }

        // POST: Contracts/Search — LINQ filter by Date Range and Status
        [HttpPost]
        public async Task<IActionResult> Search(ContractSearchViewModel model)
        {
            var query = _context.Contracts.Include(c => c.Client).AsQueryable();

            if (model.StartDate.HasValue)
                query = query.Where(c => c.StartDate >= model.StartDate.Value);

            if (model.EndDate.HasValue)
                query = query.Where(c => c.EndDate <= model.EndDate.Value);

            if (model.Status.HasValue)
                query = query.Where(c => c.Status == model.Status.Value);

            model.Results = await query.OrderByDescending(c => c.StartDate).ToListAsync();
            return View(model);
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var contract = await _context.Contracts.Include(c => c.Client).Include(c => c.ServiceRequests)
                .FirstOrDefaultAsync(c => c.ContractId == id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        // GET: Contracts/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name");
            return View();
        }

        // POST: Contracts/Create
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientId,StartDate,EndDate,Status,ServiceLevel")] Contract contract, IFormFile? signedAgreement)
        {
            if (!_validationService.IsValidDateRange(contract.StartDate, contract.EndDate))
            {
                ModelState.AddModelError("EndDate", "End date must be after the start date.");
            }

            if (signedAgreement != null)
            {
                if (!_fileService.IsValidPdf(signedAgreement))
                {
                    ModelState.AddModelError("SignedAgreementPath", "Only PDF files are allowed.");
                }
            }

            if (ModelState.IsValid)
            {
                if (signedAgreement != null)
                {
                    contract.SignedAgreementPath = await _fileService.SaveFileAsync(signedAgreement);
                }
                _context.Add(contract);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name", contract.ClientId);
            return View(contract);
        }

        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null) return NotFound();
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name", contract.ClientId);
            return View(contract);
        }

        // POST: Contracts/Edit/5
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContractId,ClientId,StartDate,EndDate,Status,ServiceLevel,SignedAgreementPath")] Contract contract, IFormFile? signedAgreement)
        {
            if (id != contract.ContractId) return NotFound();

            if (!_validationService.IsValidDateRange(contract.StartDate, contract.EndDate))
                ModelState.AddModelError("EndDate", "End date must be after the start date.");

            if (signedAgreement != null && !_fileService.IsValidPdf(signedAgreement))
                ModelState.AddModelError("SignedAgreementPath", "Only PDF files are allowed.");

            if (ModelState.IsValid)
            {
                if (signedAgreement != null)
                    contract.SignedAgreementPath = await _fileService.SaveFileAsync(signedAgreement);

                _context.Update(contract);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name", contract.ClientId);
            return View(contract);
        }

        // GET: Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var contract = await _context.Contracts.Include(c => c.Client).FirstOrDefaultAsync(c => c.ContractId == id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")][ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract != null) _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Contracts/Download/5 — Download the signed agreement PDF
        public async Task<IActionResult> Download(int? id)
        {
            if (id == null) return NotFound();
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null || string.IsNullOrEmpty(contract.SignedAgreementPath))
                return NotFound();

            var filePath = _fileService.GetFilePath(contract.SignedAgreementPath);
            if (!System.IO.File.Exists(filePath)) return NotFound();

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/pdf", $"Contract_{contract.ContractId}_Agreement.pdf");
        }
    }
}