using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Models
{
    public class Patient
    {
        public int Id { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        [Required, StringLength(20)]
        public string PatientCode { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required, StringLength(10)]
        public string Gender { get; set; } = "Other";

        [StringLength(10)]
        public string BloodGroup { get; set; } = "O+";

        [Required, Phone, StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress, StringLength(100)]
        public string? Email { get; set; }

        [StringLength(250)]
        public string Address { get; set; } = string.Empty;

        [StringLength(100)]
        public string EmergencyContactName { get; set; } = string.Empty;

        [StringLength(20)]
        public string EmergencyContactPhone { get; set; } = string.Empty;

        [StringLength(500)]
        public string Allergies { get; set; } = "None";

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public ICollection<PatientVital> Vitals { get; set; } = new List<PatientVital>();
        public ICollection<BedAdmission> BedAdmissions { get; set; } = new List<BedAdmission>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}

