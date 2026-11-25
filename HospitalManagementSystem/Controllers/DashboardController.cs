using System.Security.Claims;
using HospitalManagementSystem.Data;
using HospitalManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "Patient";
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var viewModel = new DashboardViewModel
            {
                TotalDoctors = await _context.Doctors.CountAsync(),
                TotalPatients = await _context.Patients.CountAsync(),
                TotalAppointments = await _context.Appointments.CountAsync(),
                TodayAppointments = await _context.Appointments.CountAsync(a => a.AppointmentDate.Date == DateTime.Today),
                TotalBeds = await _context.Beds.CountAsync(),
                OccupiedBeds = await _context.Beds.CountAsync(b => b.IsOccupied),
                TotalRevenue = await _context.Invoices.Where(i => i.PaymentStatus == "Paid").SumAsync(i => i.PaidAmount),
                PendingRevenue = await _context.Invoices.Where(i => i.PaymentStatus != "Paid").SumAsync(i => i.TotalAmount - i.PaidAmount),
                Departments = await _context.Departments.ToListAsync(),
                ActiveDoctors = await _context.Doctors.Include(d => d.User).Include(d => d.Department).Take(5).ToListAsync()
            };

            if (role == "Doctor")
            {
                viewModel.RecentAppointments = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .ThenInclude(d => d!.User)
                    .Where(a => a.Doctor != null && a.Doctor.User != null && a.Doctor.User.Email == userEmail)
                    .OrderByDescending(a => a.AppointmentDate)
                    .Take(10)
                    .ToListAsync();
            }
            else if (role == "Patient")
            {
                viewModel.RecentAppointments = await _context.Appointments
                    .Include(a => a.Doctor)
                    .ThenInclude(d => d!.User)
                    .Include(a => a.Patient)
                    .Where(a => a.Patient != null && a.Patient.Email == userEmail)
                    .OrderByDescending(a => a.AppointmentDate)
                    .Take(10)
                    .ToListAsync();
            }
            else
            {
                // Admin, Nurse, Warden
                viewModel.RecentAppointments = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .ThenInclude(d => d!.User)
                    .OrderByDescending(a => a.AppointmentDate)
                    .Take(10)
                    .ToListAsync();

                viewModel.CurrentAdmissions = await _context.BedAdmissions
                    .Include(ba => ba.Patient)
                    .Include(ba => ba.Bed)
                    .ThenInclude(b => b!.Ward)
                    .Where(ba => ba.Status == "Admitted")
                    .OrderByDescending(ba => ba.AdmissionDate)
                    .Take(5)
                    .ToListAsync();
            }

            ViewBag.UserRole = role;
            return View(viewModel);
        }
    }
}

