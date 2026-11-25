using HospitalManagementSystem.Data;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    [Authorize]
    public class WardenController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WardenController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Warden
        public async Task<IActionResult> Index()
        {
            var wards = await _context.Wards
                .Include(w => w.Beds)
                .ToListAsync();

            ViewBag.TotalBeds = wards.SelectMany(w => w.Beds).Count();
            ViewBag.OccupiedBeds = wards.SelectMany(w => w.Beds).Count(b => b.IsOccupied);
            ViewBag.AvailableBeds = ViewBag.TotalBeds - ViewBag.OccupiedBeds;

            return View(wards);
        }

        // GET: Warden/Admissions
        public async Task<IActionResult> Admissions(string? status)
        {
            var query = _context.BedAdmissions
                .Include(ba => ba.Patient)
                .Include(ba => ba.Bed)
                    .ThenInclude(b => b!.Ward)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(ba => ba.Status == status);
            }

            ViewBag.SelectedStatus = status;
            var admissions = await query.OrderByDescending(ba => ba.AdmissionDate).ToListAsync();
            return View(admissions);
        }

        // GET: Warden/Admit
        public async Task<IActionResult> Admit(int? bedId)
        {
            ViewBag.Patients = await _context.Patients.OrderBy(p => p.FirstName).ToListAsync();
            ViewBag.AvailableBeds = await _context.Beds
                .Include(b => b.Ward)
                .Where(b => !b.IsOccupied)
                .ToListAsync();

            var model = new BedAdmission
            {
                AdmissionDate = DateTime.UtcNow,
                Status = "Admitted"
            };

            if (bedId.HasValue) model.BedId = bedId.Value;

            return View(model);
        }

        // POST: Warden/Admit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Admit(BedAdmission admission)
        {
            if (ModelState.IsValid)
            {
                var bed = await _context.Beds.Include(b => b.Ward).FirstOrDefaultAsync(b => b.Id == admission.BedId);
                if (bed == null)
                {
                    TempData["ErrorMessage"] = "Selected bed not found.";
                    return RedirectToAction(nameof(Index));
                }

                if (bed.IsOccupied)
                {
                    TempData["ErrorMessage"] = "Selected bed is already occupied.";
                    return RedirectToAction(nameof(Admit));
                }

                bed.IsOccupied = true;
                admission.AdmissionDate = DateTime.UtcNow;
                admission.Status = "Admitted";

                _context.BedAdmissions.Add(admission);
                await _context.SaveChangesAsync();

                // Generate Bed Stay invoice
                var invoice = new Invoice
                {
                    InvoiceNumber = $"INV-{DateTime.Now:yyMM}-{new Random().Next(1000, 9999)}",
                    PatientId = admission.PatientId,
                    BedAdmissionId = admission.Id,
                    TotalAmount = bed.Ward?.DailyCharge ?? 100m,
                    PaidAmount = 0m,
                    PaymentStatus = "Pending",
                    PaymentMethod = "Cash",
                    InvoiceDate = DateTime.UtcNow,
                    Description = $"Bed Admission ({bed.Ward?.WardName} - {bed.BedNumber}) initial deposit"
                };
                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Patient admitted to bed {bed.BedNumber} successfully!";
                return RedirectToAction(nameof(Admissions));
            }

            ViewBag.Patients = await _context.Patients.OrderBy(p => p.FirstName).ToListAsync();
            ViewBag.AvailableBeds = await _context.Beds.Include(b => b.Ward).Where(b => !b.IsOccupied).ToListAsync();
            TempData["ErrorMessage"] = "Please fill out all required admission details.";
            return View(admission);
        }

        // POST: Warden/Discharge (AJAX & Form supported)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Discharge(int id, string? dischargeSummary)
        {
            var admission = await _context.BedAdmissions
                .Include(ba => ba.Bed)
                .Include(ba => ba.Patient)
                .FirstOrDefaultAsync(ba => ba.Id == id);

            if (admission == null)
            {
                TempData["ErrorMessage"] = "Admission record not found.";
                return RedirectToAction(nameof(Admissions));
            }

            admission.Status = "Discharged";
            admission.DischargeDate = DateTime.UtcNow;
            admission.DischargeSummary = dischargeSummary ?? "Discharged in stable condition.";

            if (admission.Bed != null)
            {
                admission.Bed.IsOccupied = false;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Patient {admission.Patient?.FullName} discharged successfully and bed {admission.Bed?.BedNumber} is now freed!";
            return RedirectToAction(nameof(Admissions));
        }

        // GET: Warden/CreateWard
        [Authorize(Roles = "Admin,Warden")]
        public IActionResult CreateWard()
        {
            return View(new Ward());
        }

        // POST: Warden/CreateWard
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Warden")]
        public async Task<IActionResult> CreateWard(Ward ward, int initialBedCount)
        {
            if (ModelState.IsValid)
            {
                _context.Wards.Add(ward);
                await _context.SaveChangesAsync();

                // Auto-create initial beds
                int bedsToCreate = initialBedCount > 0 ? initialBedCount : ward.Capacity;
                var bedPrefix = ward.WardName.Split(' ').FirstOrDefault()?.Substring(0, Math.Min(3, ward.WardName.Length)).ToUpper() ?? "BED";

                for (int i = 1; i <= bedsToCreate; i++)
                {
                    _context.Beds.Add(new Bed
                    {
                        WardId = ward.Id,
                        BedNumber = $"{bedPrefix}-{i:D2}",
                        IsOccupied = false
                    });
                }
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Ward '{ward.WardName}' created with {bedsToCreate} beds!";
                return RedirectToAction(nameof(Index));
            }

            return View(ward);
        }
    }
}

