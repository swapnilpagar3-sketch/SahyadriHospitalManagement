using System.Security.Claims;
using HospitalManagementSystem.Data;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    [Authorize]
    public class NurseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NurseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Nurse / Vitals Monitor
        public async Task<IActionResult> Index()
        {
            var activeAdmissions = await _context.BedAdmissions
                .Include(ba => ba.Patient)
                    .ThenInclude(p => p!.Vitals)
                .Include(ba => ba.Bed)
                    .ThenInclude(b => b!.Ward)
                .Where(ba => ba.Status == "Admitted")
                .ToListAsync();

            return View(activeAdmissions);
        }

        // GET: Nurse/RecordVitals
        public async Task<IActionResult> RecordVitals(int? patientId)
        {
            ViewBag.Patients = await _context.Patients.OrderBy(p => p.FirstName).ToListAsync();

            var model = new PatientVital
            {
                RecordedAt = DateTime.UtcNow,
                RecordedByNurse = User.FindFirstValue(ClaimTypes.Name) ?? "Nurse",
                BloodPressure = "120/80",
                HeartRate = 72,
                Temperature = 98.6m,
                RespiratoryRate = 16,
                OxygenSaturation = 98m
            };

            if (patientId.HasValue)
            {
                model.PatientId = patientId.Value;
                var patient = await _context.Patients.FindAsync(patientId.Value);
                ViewBag.CurrentPatient = patient;
            }

            return View(model);
        }

        // POST: Nurse/RecordVitals
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordVitals(PatientVital vitals)
        {
            if (ModelState.IsValid)
            {
                vitals.RecordedAt = DateTime.UtcNow;
                if (string.IsNullOrEmpty(vitals.RecordedByNurse))
                {
                    vitals.RecordedByNurse = User.FindFirstValue(ClaimTypes.Name) ?? "Nurse";
                }

                _context.PatientVitals.Add(vitals);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Patient vitals logged successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Patients = await _context.Patients.OrderBy(p => p.FirstName).ToListAsync();
            TempData["ErrorMessage"] = "Failed to record vitals. Check required measurements.";
            return View(vitals);
        }

        // GET: Nurse/VitalsHistory/5
        public async Task<IActionResult> VitalsHistory(int id)
        {
            var patient = await _context.Patients
                .Include(p => p.Vitals)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null) return NotFound();

            return View(patient);
        }
    }
}

