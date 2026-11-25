using HospitalManagementSystem.Data;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    [Authorize]
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Patients
        public async Task<IActionResult> Index(string? search, string? bloodGroup)
        {
            var query = _context.Patients
                .Include(p => p.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(p =>
                    p.FirstName.ToLower().Contains(search) ||
                    p.LastName.ToLower().Contains(search) ||
                    p.PatientCode.ToLower().Contains(search) ||
                    p.Phone.Contains(search) ||
                    (p.Email != null && p.Email.ToLower().Contains(search)));
            }

            if (!string.IsNullOrEmpty(bloodGroup) && bloodGroup != "All")
            {
                query = query.Where(p => p.BloodGroup == bloodGroup);
            }

            ViewBag.Search = search;
            ViewBag.BloodGroup = bloodGroup;

            var patients = await query.OrderByDescending(p => p.RegisteredAt).ToListAsync();
            return View(patients);
        }

        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var patient = await _context.Patients
                .Include(p => p.User)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Doctor)
                        .ThenInclude(d => d!.User)
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.Doctor)
                        .ThenInclude(d => d!.User)
                .Include(p => p.Vitals)
                .Include(p => p.BedAdmissions)
                    .ThenInclude(ba => ba.Bed)
                        .ThenInclude(b => b!.Ward)
                .Include(p => p.Invoices)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (patient == null) return NotFound();

            return View(patient);
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            return View(new Patient
            {
                PatientCode = $"PAT-{DateTime.Now:yyMM}-{new Random().Next(100, 999)}",
                DateOfBirth = DateTime.Today.AddYears(-30)
            });
        }

        // POST: Patients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient patient)
        {
            if (ModelState.IsValid)
            {
                // Ensure code uniqueness
                if (await _context.Patients.AnyAsync(p => p.PatientCode == patient.PatientCode))
                {
                    patient.PatientCode = $"PAT-{DateTime.Now:yyMM}-{new Random().Next(1000, 9999)}";
                }

                patient.RegisteredAt = DateTime.UtcNow;
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Patient {patient.FullName} registered successfully with code {patient.PatientCode}!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Please correct the form errors and try again.";
            return View(patient);
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();

            return View(patient);
        }

        // POST: Patients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Patient patient)
        {
            if (id != patient.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Patient {patient.FullName}'s information updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Patients.AnyAsync(e => e.Id == patient.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Validation failed. Please verify the entered data.";
            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Patient record deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

