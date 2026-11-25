using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        [Required, StringLength(30)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        public int? AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }

        public int? BedAdmissionId { get; set; }
        public BedAdmission? BedAdmission { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PaidAmount { get; set; }

        [Required, StringLength(30)]
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, Partially Paid

        [Required, StringLength(30)]
        public string PaymentMethod { get; set; } = "Cash"; // Cash, Card, Insurance, UPI

        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? Description { get; set; }
    }
}

