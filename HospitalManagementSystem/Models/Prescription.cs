using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Models
{
    public class Prescription
    {
        public int Id { get; set; }

        public int? AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        [Required]
        public int DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        public DateTime PrescribedDate { get; set; } = DateTime.UtcNow;

        [Required, StringLength(200)]
        public string Diagnosis { get; set; } = string.Empty;

        [Required, StringLength(2000)]
        public string Medicines { get; set; } = string.Empty; // Formatted items: Medicine, Dosage, Frequency, Duration

        [StringLength(1000)]
        public string? AdviceInstructions { get; set; }

        public DateTime? FollowUpDate { get; set; }
    }

    public class PatientVital
    {
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        [Required, StringLength(100)]
        public string RecordedByNurse { get; set; } = string.Empty;

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

        [Required, StringLength(20)]
        public string BloodPressure { get; set; } = "120/80"; // mmHg

        public int HeartRate { get; set; } = 72; // bpm

        public decimal Temperature { get; set; } = 98.6m; // Fahrenheit

        public int RespiratoryRate { get; set; } = 18; // breaths/min

        public decimal OxygenSaturation { get; set; } = 98m; // SpO2 %

        public decimal? BloodSugar { get; set; } // mg/dL

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}

