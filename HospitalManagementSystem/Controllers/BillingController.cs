using System.Security.Claims;
using HospitalManagementSystem.Data;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    [Authorize]
    public class BillingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BillingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Billing
        public async Task<IActionResult> Index(string? status)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var query = _context.Invoices
                .Include(i => i.Patient)
                .Include(i => i.Appointment)
                .AsQueryable();

            if (role == "Patient")
            {
                query = query.Where(i => i.Patient != null && i.Patient.Email == userEmail);
            }

            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(i => i.PaymentStatus == status);
            }

            ViewBag.SelectedStatus = status;
            ViewBag.TotalRevenue = await _context.Invoices.Where(i => i.PaymentStatus == "Paid").SumAsync(i => i.PaidAmount);
            ViewBag.TotalPending = await _context.Invoices.Where(i => i.PaymentStatus != "Paid").SumAsync(i => i.TotalAmount - i.PaidAmount);

            var invoices = await query.OrderByDescending(i => i.InvoiceDate).ToListAsync();
            return View(invoices);
        }

        // GET: Billing/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var invoice = await _context.Invoices
                .Include(i => i.Patient)
                .Include(i => i.Appointment)
                    .ThenInclude(a => a!.Doctor)
                        .ThenInclude(d => d!.User)
                .Include(i => i.BedAdmission)
                    .ThenInclude(ba => ba!.Bed)
                        .ThenInclude(b => b!.Ward)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (invoice == null) return NotFound();

            return View(invoice);
        }

        // GET: Billing/Create
        [Authorize(Roles = "Admin,Warden")]
        public async Task<IActionResult> Create(int? patientId)
        {
            ViewBag.Patients = await _context.Patients.OrderBy(p => p.FirstName).ToListAsync();

            var model = new Invoice
            {
                InvoiceNumber = $"INV-{DateTime.Now:yyMM}-{new Random().Next(1000, 9999)}",
                InvoiceDate = DateTime.UtcNow,
                PaymentStatus = "Pending",
                PaymentMethod = "Cash"
            };

            if (patientId.HasValue) model.PatientId = patientId.Value;

            return View(model);
        }

        // POST: Billing/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Warden")]
        public async Task<IActionResult> Create(Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                invoice.InvoiceDate = DateTime.UtcNow;
                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Invoice {invoice.InvoiceNumber} created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Patients = await _context.Patients.OrderBy(p => p.FirstName).ToListAsync();
            return View(invoice);
        }

        // POST: Billing/Pay (AJAX with Toastr support)
        [HttpPost]
        public async Task<IActionResult> Pay(int id, decimal paidAmount, string paymentMethod)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return Json(new { success = false, message = "Invoice not found." });
            }

            invoice.PaidAmount += paidAmount;
            invoice.PaymentMethod = paymentMethod ?? invoice.PaymentMethod;

            if (invoice.PaidAmount >= invoice.TotalAmount)
            {
                invoice.PaidAmount = invoice.TotalAmount;
                invoice.PaymentStatus = "Paid";
            }
            else if (invoice.PaidAmount > 0)
            {
                invoice.PaymentStatus = "Partially Paid";
            }

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = $"Payment of ${paidAmount:F2} received! Invoice status: {invoice.PaymentStatus}.",
                status = invoice.PaymentStatus,
                paidAmount = invoice.PaidAmount,
                balance = invoice.TotalAmount - invoice.PaidAmount
            });
        }
    }
}

