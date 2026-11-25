using System.Security.Claims;
using HospitalManagementSystem.Data;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.Trim().ToLower());

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Invalid email address or password.");
                TempData["ErrorMessage"] = "Invalid email or password. Please try again.";
                return View(model);
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Your account has been deactivated. Please contact administration.");
                TempData["ErrorMessage"] = "Account deactivated. Please contact support.";
                return View(model);
            }

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("ProfilePicture", user.ProfilePicture ?? "/images/avatars/default.svg")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(14) : DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            TempData["SuccessMessage"] = $"Welcome back, {user.FullName} ({user.Role})!";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        // GET: /Account/Register
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            ViewBag.Departments = await _context.Departments.ToListAsync();
            return View(new RegisterViewModel());
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ViewBag.Departments = await _context.Departments.ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == model.Email.Trim().ToLower()))
            {
                ModelState.AddModelError("Email", "An account with this email address already exists.");
                TempData["ErrorMessage"] = "Email address already registered.";
                return View(model);
            }

            // Determine avatar / photo
            string avatar = "/images/avatars/default.svg";
            if (model.PhotoFile != null && model.PhotoFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
                var ext = Path.GetExtension(model.PhotoFile.FileName).ToLowerInvariant();
                if (allowedExtensions.Contains(ext))
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = $"user_{Guid.NewGuid():N}{ext}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.PhotoFile.CopyToAsync(stream);
                    }

                    avatar = $"/uploads/avatars/{uniqueFileName}";
                }
            }
            else if (!string.IsNullOrEmpty(model.SelectedAvatar))
            {
                avatar = model.SelectedAvatar;
            }
            else
            {
                avatar = model.Role.ToLower() switch
                {
                    "doctor" => "/images/avatars/doctor1.svg",
                    "nurse" => "/images/avatars/nurse.svg",
                    "warden" => "/images/avatars/warden.svg",
                    "patient" => "/images/avatars/patient.svg",
                    _ => "/images/avatars/default.svg"
                };
            }

            // Create User
            var user = new User
            {
                FullName = model.FullName.Trim(),
                Email = model.Email.Trim().ToLower(),
                PhoneNumber = model.PhoneNumber?.Trim(),
                Role = model.Role,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                ProfilePicture = avatar,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Role-specific linked entities
            if (model.Role == "Doctor")
            {
                var defaultDept = await _context.Departments.FirstOrDefaultAsync();
                var doctor = new Doctor
                {
                    UserId = user.Id,
                    Specialization = !string.IsNullOrEmpty(model.Specialization) ? model.Specialization : "General Physician",
                    Qualification = !string.IsNullOrEmpty(model.Qualification) ? model.Qualification : "MBBS",
                    ExperienceYears = 2,
                    ConsultationFee = 60.00m,
                    RoomNumber = "Room 101",
                    AvailableDays = "Mon, Tue, Wed, Thu, Fri",
                    AvailableTimeSlot = "09:00 AM - 01:00 PM",
                    DepartmentId = model.DepartmentId ?? defaultDept?.Id ?? 1
                };
                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();
            }
            else if (model.Role == "Patient")
            {
                var patientCount = await _context.Patients.CountAsync();
                var patient = new Patient
                {
                    UserId = user.Id,
                    PatientCode = $"PAT-{1000 + patientCount + 1}",
                    FirstName = model.FullName.Split(' ').FirstOrDefault() ?? model.FullName,
                    LastName = model.FullName.Contains(' ') ? model.FullName.Substring(model.FullName.IndexOf(' ') + 1) : "Patient",
                    DateOfBirth = model.DateOfBirth ?? DateTime.Today.AddYears(-25),
                    Gender = model.Gender ?? "Other",
                    BloodGroup = model.BloodGroup ?? "O+",
                    Phone = model.PhoneNumber ?? "N/A",
                    Email = model.Email,
                    Address = model.Address ?? "Not specified",
                    RegisteredAt = DateTime.UtcNow
                };
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Registration successful! You can now log in with your credentials.";
            return RedirectToAction(nameof(Login));
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.Trim().ToLower());
            if (user == null)
            {
                // For security, don't reveal if user does not exist
                TempData["InfoMessage"] = "If an account matches that email, a password reset link has been prepared.";
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // Generate token
            var token = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
            user.ResetToken = token;
            user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(30);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Reset code generated successfully: {token}";
            return RedirectToAction(nameof(ResetPassword), new { email = user.Email, token });
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(string? email, string? token)
        {
            var model = new ResetPasswordViewModel
            {
                Email = email ?? string.Empty,
                Token = token ?? string.Empty
            };
            return View(model);
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email.ToLower() == model.Email.Trim().ToLower() &&
                u.ResetToken == model.Token &&
                u.ResetTokenExpiry > DateTime.UtcNow);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid or expired reset token.");
                TempData["ErrorMessage"] = "Password reset code is invalid or has expired.";
                return View(model);
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            user.ResetToken = null;
            user.ResetTokenExpiry = null;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Password reset successfully! Please log in with your new password.";
            return RedirectToAction(nameof(Login));
        }

        // GET/POST: /Account/Logout
        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["InfoMessage"] = "You have been logged out successfully.";
            return RedirectToAction(nameof(Login));
        }

        // GET: /Account/Profile
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.Users
                .Include(u => u.DoctorProfile)
                .Include(u => u.PatientProfile)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: /Account/UpdateProfile
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(string fullName, string phoneNumber, string? profilePicture, IFormFile? photoFile)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return NotFound();
            }

            user.FullName = fullName.Trim();
            user.PhoneNumber = phoneNumber?.Trim();

            if (photoFile != null && photoFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
                var ext = Path.GetExtension(photoFile.FileName).ToLowerInvariant();
                if (allowedExtensions.Contains(ext))
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = $"user_{Guid.NewGuid():N}{ext}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photoFile.CopyToAsync(stream);
                    }

                    user.ProfilePicture = $"/uploads/avatars/{uniqueFileName}";
                }
            }
            else if (!string.IsNullOrEmpty(profilePicture))
            {
                user.ProfilePicture = profilePicture;
            }

            await _context.SaveChangesAsync();

            // Refresh cookie with updated info
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("ProfilePicture", user.ProfilePicture ?? "/images/avatars/default.svg")
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Profile));
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

