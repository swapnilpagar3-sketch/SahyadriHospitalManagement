using System.Security.Claims;
using HospitalManagementSystem.Data;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    [Authorize]
    public class PrescriptionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrescriptionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Prescriptions
        public async Task<IActionResult> Index(int? patientId, int? doctorId)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var query = _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                    .ThenInclude(d => d!.User)
                .Include(p => p.Doctor)
                    .ThenInclude(d => d!.Department)
                .AsQueryable();

            if (role == "Doctor")
            {
                query = query.Where(p => p.Doctor != null && p.Doctor.User != null && p.Doctor.User.Email == userEmail);
            }
            else if (role == "Patient")
            {
                query = query.Where(p => p.Patient != null && p.Patient.Email == userEmail);
            }

            if (patientId.HasValue) query = query.Where(p => p.PatientId == patientId.Value);
            if (doctorId.HasValue) query = query.Where(p => p.DoctorId == doctorId.Value);

            var prescriptions = await query.OrderByDescending(p => p.PrescribedDate).ToListAsync();
            return View(prescriptions);
        }

        // GET: Prescriptions/Details/5 (Print view)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                    .ThenInclude(d => d!.User)
                .Include(p => p.Doctor)
                    .ThenInclude(d => d!.Department)
                .Include(p => p.Appointment)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (prescription == null) return NotFound();

            return View(prescription);
        }

        // GET: Prescriptions/Create
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Create(int? patientId, int? appointmentId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var doctor = await _context.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.User != null && d.User.Email == userEmail);

            ViewBag.Patients = await _context.Patients.OrderBy(p => p.FirstName).ToListAsync();
            ViewBag.Doctors = await _context.Doctors.Include(d => d.User).ToListAsync();
            ViewBag.CurrentDoctorId = doctor?.Id;

            var model = new Prescription
            {
                PrescribedDate = DateTime.UtcNow,
                FollowUpDate = DateTime.Today.AddDays(14)
            };

            if (patientId.HasValue) model.PatientId = patientId.Value;
            if (appointmentId.HasValue)
            {
                model.AppointmentId = appointmentId.Value;
                var appt = await _context.Appointments.FindAsync(appointmentId.Value);
                if (appt != null)
                {
                    model.PatientId = appt.PatientId;
                    model.DoctorId = appt.DoctorId;
                }
            }
            else if (doctor != null)
            {
                model.DoctorId = doctor.Id;
            }

            return View(model);
        }

        // POST: Prescriptions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Create(Prescription prescription)
        {
            if (ModelState.IsValid)
            {
                prescription.PrescribedDate = DateTime.UtcNow;
                _context.Prescriptions.Add(prescription);

                // If linked to appointment, set appointment to Completed
                if (prescription.AppointmentId.HasValue)
                {
                    var appt = await _context.Appointments.FindAsync(prescription.AppointmentId.Value);
                    if (appt != null)
                    {
                        appt.Status = "Completed";
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Prescription issued successfully!";
                return RedirectToAction(nameof(Details), new { id = prescription.Id });
            }

            ViewBag.Patients = await _context.Patients.OrderBy(p => p.FirstName).ToListAsync();
            ViewBag.Doctors = await _context.Doctors.Include(d => d.User).ToListAsync();
            TempData["ErrorMessage"] = "Validation failed. Please verify the prescription details.";
            return View(prescription);
        }
    }
}
