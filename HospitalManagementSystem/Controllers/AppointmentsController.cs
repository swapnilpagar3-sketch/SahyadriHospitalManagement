using System.Security.Claims;
using HospitalManagementSystem.Data;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Appointments
        public async Task<IActionResult> Index(string? status, int? doctorId, DateTime? date)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d!.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d!.Department)
                .AsQueryable();

            if (role == "Doctor")
            {
                query = query.Where(a => a.Doctor != null && a.Doctor.User != null && a.Doctor.User.Email == userEmail);
            }
            else if (role == "Patient")
            {
                query = query.Where(a => a.Patient != null && a.Patient.Email == userEmail);
            }

            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(a => a.Status == status);
            }

            if (doctorId.HasValue && doctorId.Value > 0)
            {
                query = query.Where(a => a.DoctorId == doctorId.Value);
            }

            if (date.HasValue)
            {
                query = query.Where(a => a.AppointmentDate.Date == date.Value.Date);
            }

            ViewBag.Doctors = await _context.Doctors.Include(d => d.User).ToListAsync();
            ViewBag.SelectedStatus = status;
            ViewBag.SelectedDoctor = doctorId;
            ViewBag.SelectedDate = date?.ToString("yyyy-MM-dd");

            var appointments = await query.OrderByDescending(a => a.AppointmentDate).ToListAsync();
            return View(appointments);
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d!.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d!.Department)
                .Include(a => a.Prescription)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null) return NotFound();

            return View(appointment);
        }

        // GET: Appointments/Create
        public async Task<IActionResult> Create(int? patientId, int? doctorId)
        {
            ViewBag.Patients = await _context.Patients.OrderBy(p => p.FirstName).ToListAsync();
            ViewBag.Doctors = await _context.Doctors.Include(d => d.User).Include(d => d.Department).ToListAsync();

            var model = new Appointment
            {
                AppointmentCode = $"APT-{DateTime.Now:yyMM}-{new Random().Next(100, 999)}",
                AppointmentDate = DateTime.Today.AddDays(1),
                TimeSlot = "10:00 AM",
                Status = "Scheduled"
            };

            if (patientId.HasValue) model.PatientId = patientId.Value;
            if (doctorId.HasValue)
            {
                model.DoctorId = doctorId.Value;
                var doc = await _context.Doctors.FindAsync(doctorId.Value);
                if (doc != null) model.Fee = doc.ConsultationFee;
            }

            return View(model);
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                var doc = await _context.Doctors.FindAsync(appointment.DoctorId);
                if (doc != null && appointment.Fee <= 0)
                {
                    appointment.Fee = doc.ConsultationFee;
                }

                appointment.CreatedAt = DateTime.UtcNow;
                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                // Generate automatic invoice
                var invoice = new Invoice
                {
                    InvoiceNumber = $"INV-{DateTime.Now:yyMM}-{new Random().Next(1000, 9999)}",
                    PatientId = appointment.PatientId,
                    AppointmentId = appointment.Id,
                    TotalAmount = appointment.Fee,
                    PaidAmount = 0m,
                    PaymentStatus = "Pending",
                    PaymentMethod = "Cash",
                    InvoiceDate = DateTime.UtcNow,
                    Description = $"Consultation Appointment ({appointment.AppointmentCode})"
                };
                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Appointment {appointment.AppointmentCode} booked successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Patients = await _context.Patients.OrderBy(p => p.FirstName).ToListAsync();
            ViewBag.Doctors = await _context.Doctors.Include(d => d.User).Include(d => d.Department).ToListAsync();
            TempData["ErrorMessage"] = "Failed to book appointment. Please review the inputs.";
            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            ViewBag.Patients = await _context.Patients.OrderBy(p => p.FirstName).ToListAsync();
            ViewBag.Doctors = await _context.Doctors.Include(d => d.User).ToListAsync();
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Appointment appointment)
        {
            if (id != appointment.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(appointment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Appointment updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Patients = await _context.Patients.OrderBy(p => p.FirstName).ToListAsync();
            ViewBag.Doctors = await _context.Doctors.Include(d => d.User).ToListAsync();
            return View(appointment);
        }

        // POST: Appointments/UpdateStatus (AJAX endpoint with Toastr support)
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return Json(new { success = false, message = "Appointment not found." });
            }

            appointment.Status = status;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = $"Appointment status updated to '{status}'.", status = status });
        }

        // POST: Appointments/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Appointment deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

