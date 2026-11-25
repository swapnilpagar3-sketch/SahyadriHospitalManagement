using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required, StringLength(20)]
        public string AppointmentCode { get; set; } = string.Empty;

        [Required]
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        [Required]
        public int DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required, StringLength(50)]
        public string TimeSlot { get; set; } = "10:00 AM";

        [StringLength(500)]
        public string Symptoms { get; set; } = string.Empty;

        [Required, StringLength(30)]
        public string Status { get; set; } = "Scheduled"; // Scheduled, Confirmed, In-Progress, Completed, Cancelled

        [Column(TypeName = "decimal(18,2)")]
        public decimal Fee { get; set; } = 50.00m;

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Prescription? Prescription { get; set; }
    }
}

