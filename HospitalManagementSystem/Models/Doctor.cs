using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    public class Doctor
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        [Required, StringLength(100)]
        public string Specialization { get; set; } = string.Empty;

        [StringLength(100)]
        public string Qualification { get; set; } = string.Empty;

        public int ExperienceYears { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ConsultationFee { get; set; } = 50.00m;

        [StringLength(50)]
        public string RoomNumber { get; set; } = string.Empty;

        [StringLength(100)]
        public string AvailableDays { get; set; } = "Mon, Tue, Wed, Thu, Fri";

        [StringLength(100)]
        public string AvailableTimeSlot { get; set; } = "09:00 AM - 02:00 PM";

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}

