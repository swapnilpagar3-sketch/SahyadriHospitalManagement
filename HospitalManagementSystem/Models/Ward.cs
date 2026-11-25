using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    public class Ward
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string WardName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string WardType { get; set; } = "General"; // General, Semi-Private, Private, ICU, Emergency

        public int Capacity { get; set; } = 10;

        [Column(TypeName = "decimal(18,2)")]
        public decimal DailyCharge { get; set; } = 100.00m;

        [StringLength(100)]
        public string WardenInCharge { get; set; } = string.Empty;

        [StringLength(100)]
        public string FloorLocation { get; set; } = "1st Floor";

        public ICollection<Bed> Beds { get; set; } = new List<Bed>();
    }

    public class Bed
    {
        public int Id { get; set; }

        [Required]
        public int WardId { get; set; }
        public Ward? Ward { get; set; }

        [Required, StringLength(20)]
        public string BedNumber { get; set; } = string.Empty;

        public bool IsOccupied { get; set; } = false;

        public ICollection<BedAdmission> Admissions { get; set; } = new List<BedAdmission>();
    }

    public class BedAdmission
    {
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        [Required]
        public int BedId { get; set; }
        public Bed? Bed { get; set; }

        [Required]
        public DateTime AdmissionDate { get; set; } = DateTime.UtcNow;

        public DateTime? DischargeDate { get; set; }

        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        [Required, StringLength(30)]
        public string Status { get; set; } = "Admitted"; // Admitted, Discharged

        [StringLength(500)]
        public string? DischargeSummary { get; set; }
    }
}

