namespace HospitalManagementSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Patient"; // Admin, Doctor, Nurse, Warden, Patient
        public string? PhoneNumber { get; set; }
        public string ProfilePicture { get; set; } = "/images/avatars/default.png";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Password Reset Simulation
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }

        // Navigation
        public Doctor? DoctorProfile { get; set; }
        public Patient? PatientProfile { get; set; }
    }
}

