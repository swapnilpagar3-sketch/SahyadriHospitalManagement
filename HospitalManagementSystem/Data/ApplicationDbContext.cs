using Microsoft.EntityFrameworkCore;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Ward> Wards => Set<Ward>();
        public DbSet<Bed> Beds => Set<Bed>();
        public DbSet<BedAdmission> BedAdmissions => Set<BedAdmission>();
        public DbSet<Prescription> Prescriptions => Set<Prescription>();
        public DbSet<PatientVital> PatientVitals => Set<PatientVital>();
        public DbSet<Invoice> Invoices => Set<Invoice>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Email Unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Patient Code Unique
            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.PatientCode)
                .IsUnique();

            // Relationships
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithOne(u => u.DoctorProfile)
                .HasForeignKey<Doctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Department)
                .WithMany(dept => dept.Doctors)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithOne(u => u.PatientProfile)
                .HasForeignKey<Patient>(p => p.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prescription>()
                .HasOne(pr => pr.Appointment)
                .WithOne(a => a.Prescription)
                .HasForeignKey<Prescription>(pr => pr.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Prescription>()
                .HasOne(pr => pr.Patient)
                .WithMany(p => p.Prescriptions)
                .HasForeignKey(pr => pr.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prescription>()
                .HasOne(pr => pr.Doctor)
                .WithMany(d => d.Prescriptions)
                .HasForeignKey(pr => pr.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bed>()
                .HasOne(b => b.Ward)
                .WithMany(w => w.Beds)
                .HasForeignKey(b => b.WardId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BedAdmission>()
                .HasOne(ba => ba.Patient)
                .WithMany(p => p.BedAdmissions)
                .HasForeignKey(ba => ba.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BedAdmission>()
                .HasOne(ba => ba.Bed)
                .WithMany(b => b.Admissions)
                .HasForeignKey(ba => ba.BedId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PatientVital>()
                .HasOne(pv => pv.Patient)
                .WithMany(p => p.Vitals)
                .HasForeignKey(pv => pv.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Patient)
                .WithMany(p => p.Invoices)
                .HasForeignKey(i => i.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

