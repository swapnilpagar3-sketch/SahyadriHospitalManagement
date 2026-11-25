using HospitalManagementSystem.Data;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    [Authorize]
    public class DoctorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Doctors
        public async Task<IActionResult> Index(string? search, int? departmentId)
        {
            var query = _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Department)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(d =>
                    (d.User != null && d.User.FullName.ToLower().Contains(search)) ||
                    d.Specialization.ToLower().Contains(search) ||
                    (d.Department != null && d.Department.Name.ToLower().Contains(search)));
            }

            if (departmentId.HasValue && departmentId.Value > 0)
            {
                query = query.Where(d => d.DepartmentId == departmentId.Value);
            }

            ViewBag.Departments = await _context.Departments.ToListAsync();
            ViewBag.Search = search;
            ViewBag.SelectedDept = departmentId;

            var doctors = await query.ToListAsync();
            return View(doctors);
        }

        // GET: Doctors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var doctor = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Department)
                .Include(d => d.Appointments)
                .ThenInclude(a => a.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (doctor == null) return NotFound();

            return View(doctor);
        }

        // GET: Doctors/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Departments = await _context.Departments.ToListAsync();
            return View();
        }

        // POST: Doctors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(string fullName, string email, string password, string phone,
            string specialization, string qualification, int experienceYears, decimal consultationFee,
            string roomNumber, string availableDays, string availableTimeSlot, int departmentId, string? profilePicture)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["ErrorMessage"] = "Please fill in all required user fields.";
                ViewBag.Departments = await _context.Departments.ToListAsync();
                return View();
            }

            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == email.Trim().ToLower()))
            {
                TempData["ErrorMessage"] = "A user with this email already exists.";
                ViewBag.Departments = await _context.Departments.ToListAsync();
                return View();
            }

            var user = new User
            {
                FullName = fullName.Trim(),
                Email = email.Trim().ToLower(),
                PhoneNumber = phone?.Trim(),
                Role = "Doctor",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                ProfilePicture = profilePicture ?? "/images/avatars/doctor1.svg",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var doctor = new Doctor
            {
                UserId = user.Id,
                Specialization = specialization.Trim(),
                Qualification = qualification?.Trim() ?? "MBBS",
                ExperienceYears = experienceYears,
                ConsultationFee = consultationFee > 0 ? consultationFee : 50.00m,
                RoomNumber = roomNumber?.Trim() ?? "Room 101",
                AvailableDays = availableDays ?? "Mon, Wed, Fri",
                AvailableTimeSlot = availableTimeSlot ?? "09:00 AM - 01:00 PM",
                DepartmentId = departmentId
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Doctor {user.FullName} has been registered successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Doctors/Edit/5
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var doctor = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Department)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (doctor == null) return NotFound();

            ViewBag.Departments = await _context.Departments.ToListAsync();
            return View(doctor);
        }

        // POST: Doctors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Edit(int id, Doctor model, string? fullName, string? phone, string? profilePicture)
        {
            if (id != model.Id) return NotFound();

            var doctor = await _context.Doctors.Include(d => d.User).FirstOrDefaultAsync(d => d.Id == id);
            if (doctor == null) return NotFound();

            doctor.Specialization = model.Specialization;
            doctor.Qualification = model.Qualification;
            doctor.ExperienceYears = model.ExperienceYears;
            doctor.ConsultationFee = model.ConsultationFee;
            doctor.RoomNumber = model.RoomNumber;
            doctor.AvailableDays = model.AvailableDays;
            doctor.AvailableTimeSlot = model.AvailableTimeSlot;
            doctor.DepartmentId = model.DepartmentId;

            if (doctor.User != null)
            {
                if (!string.IsNullOrEmpty(fullName)) doctor.User.FullName = fullName.Trim();
                if (!string.IsNullOrEmpty(phone)) doctor.User.PhoneNumber = phone.Trim();
                if (!string.IsNullOrEmpty(profilePicture)) doctor.User.ProfilePicture = profilePicture;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Doctor profile updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Doctors/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _context.Doctors.Include(d => d.User).FirstOrDefaultAsync(d => d.Id == id);
            if (doctor != null)
            {
                if (doctor.User != null)
                {
                    _context.Users.Remove(doctor.User);
                }
                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Doctor record deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

