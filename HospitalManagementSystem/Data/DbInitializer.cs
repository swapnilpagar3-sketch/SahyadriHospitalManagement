using HospitalManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Check if users already exist
            if (context.Users.Any())
            {
                return; // DB has been seeded
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
            var docPasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123");
            var nursePasswordHash = BCrypt.Net.BCrypt.HashPassword("Nurse@123");
            var wardenPasswordHash = BCrypt.Net.BCrypt.HashPassword("Warden@123");
            var patientPasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123");

            // 1. Seed Users
            var adminUser = new User
            {
                FullName = "Dr. Alexander Wright",
                Email = "admin@hospital.com",
                PasswordHash = passwordHash,
                Role = "Admin",
                PhoneNumber = "+1 (555) 019-2834",
                ProfilePicture = "/images/avatars/admin.png",
                CreatedAt = DateTime.UtcNow.AddMonths(-6)
            };

            var doctorUser1 = new User
            {
                FullName = "Dr. Sophia Carter",
                Email = "doctor.carter@hospital.com",
                PasswordHash = docPasswordHash,
                Role = "Doctor",
                PhoneNumber = "+1 (555) 345-6789",
                ProfilePicture = "/images/avatars/doctor1.png",
                CreatedAt = DateTime.UtcNow.AddMonths(-5)
            };

            var doctorUser2 = new User
            {
                FullName = "Dr. Marcus Vance",
                Email = "doctor.vance@hospital.com",
                PasswordHash = docPasswordHash,
                Role = "Doctor",
                PhoneNumber = "+1 (555) 456-7890",
                ProfilePicture = "/images/avatars/doctor2.png",
                CreatedAt = DateTime.UtcNow.AddMonths(-4)
            };

            var nurseUser = new User
            {
                FullName = "Nurse Clara Oswald",
                Email = "nurse.clara@hospital.com",
                PasswordHash = nursePasswordHash,
                Role = "Nurse",
                PhoneNumber = "+1 (555) 567-8901",
                ProfilePicture = "/images/avatars/nurse.png",
                CreatedAt = DateTime.UtcNow.AddMonths(-3)
            };

            var wardenUser = new User
            {
                FullName = "Warden John Miller",
                Email = "warden.john@hospital.com",
                PasswordHash = wardenPasswordHash,
                Role = "Warden",
                PhoneNumber = "+1 (555) 678-9012",
                ProfilePicture = "/images/avatars/warden.png",
                CreatedAt = DateTime.UtcNow.AddMonths(-3)
            };

            var patientUser = new User
            {
                FullName = "Emma Watson",
                Email = "patient.emma@hospital.com",
                PasswordHash = patientPasswordHash,
                Role = "Patient",
                PhoneNumber = "+1 (555) 789-0123",
                ProfilePicture = "/images/avatars/patient.png",
                CreatedAt = DateTime.UtcNow.AddMonths(-2)
            };

            context.Users.AddRange(adminUser, doctorUser1, doctorUser2, nurseUser, wardenUser, patientUser);
            context.SaveChanges();

            // 2. Seed Departments
            var deptCardio = new Department
            {
                Name = "Cardiology",
                Description = "Heart and cardiovascular system care, ECG, angiography and post-operative recovery.",
                Location = "Wing A, 2nd Floor",
                HeadOfDepartment = "Dr. Sophia Carter"
            };

            var deptNeuro = new Department
            {
                Name = "Neurology",
                Description = "Disorders of the nervous system, brain, spine and neuro-rehabilitation.",
                Location = "Wing B, 3rd Floor",
                HeadOfDepartment = "Dr. Marcus Vance"
            };

            var deptPediatrics = new Department
            {
                Name = "Pediatrics",
                Description = "Child healthcare, vaccinations, pediatric intensive care and developmental wellness.",
                Location = "Wing C, 1st Floor",
                HeadOfDepartment = "Dr. Emily Blunt"
            };

            var deptOrthopedics = new Department
            {
                Name = "Orthopedics",
                Description = "Bones, joints, ligaments, spine surgery and trauma rehabilitation.",
                Location = "Wing D, Ground Floor",
                HeadOfDepartment = "Dr. Gregory House"
            };

            var deptGeneral = new Department
            {
                Name = "General Medicine",
                Description = "Comprehensive primary diagnosis, infection treatment, and metabolic disease management.",
                Location = "Wing A, Ground Floor",
                HeadOfDepartment = "Dr. Alexander Wright"
            };

            context.Departments.AddRange(deptCardio, deptNeuro, deptPediatrics, deptOrthopedics, deptGeneral);
            context.SaveChanges();

            // 3. Seed Doctors
            var doc1 = new Doctor
            {
                UserId = doctorUser1.Id,
                Specialization = "Cardiologist",
                Qualification = "MD, FACC, MBBS",
                ExperienceYears = 12,
                ConsultationFee = 90.00m,
                RoomNumber = "Room 204",
                AvailableDays = "Monday, Wednesday, Friday",
                AvailableTimeSlot = "09:00 AM - 01:00 PM",
                DepartmentId = deptCardio.Id
            };

            var doc2 = new Doctor
            {
                UserId = doctorUser2.Id,
                Specialization = "Neurologist",
                Qualification = "MD, DM Neurology, MBBS",
                ExperienceYears = 15,
                ConsultationFee = 110.00m,
                RoomNumber = "Room 312",
                AvailableDays = "Tuesday, Thursday, Saturday",
                AvailableTimeSlot = "10:00 AM - 03:00 PM",
                DepartmentId = deptNeuro.Id
            };

            context.Doctors.AddRange(doc1, doc2);
            context.SaveChanges();

            // 4. Seed Patients
            var pat1 = new Patient
            {
                UserId = patientUser.Id,
                PatientCode = "PAT-1001",
                FirstName = "Emma",
                LastName = "Watson",
                DateOfBirth = new DateTime(1994, 4, 15),
                Gender = "Female",
                BloodGroup = "A+",
                Phone = "+1 (555) 789-0123",
                Email = "patient.emma@hospital.com",
                Address = "742 Evergreen Terrace, Springfield",
                EmergencyContactName = "Arthur Watson",
                EmergencyContactPhone = "+1 (555) 999-8877",
                Allergies = "Penicillin, Peanuts",
                RegisteredAt = DateTime.UtcNow.AddMonths(-2)
            };

            var pat2 = new Patient
            {
                PatientCode = "PAT-1002",
                FirstName = "David",
                LastName = "Beck",
                DateOfBirth = new DateTime(1982, 8, 22),
                Gender = "Male",
                BloodGroup = "O+",
                Phone = "+1 (555) 888-4321",
                Email = "david.beck@example.com",
                Address = "42 Wallaby Way, Sydney",
                EmergencyContactName = "Victoria Beck",
                EmergencyContactPhone = "+1 (555) 777-6655",
                Allergies = "Sulfa drugs",
                RegisteredAt = DateTime.UtcNow.AddMonths(-1)
            };

            var pat3 = new Patient
            {
                PatientCode = "PAT-1003",
                FirstName = "Lucas",
                LastName = "Gray",
                DateOfBirth = new DateTime(2005, 11, 3),
                Gender = "Male",
                BloodGroup = "B+",
                Phone = "+1 (555) 444-1122",
                Email = "lucas.gray@example.com",
                Address = "12 Grimmauld Place, London",
                EmergencyContactName = "Helen Gray",
                EmergencyContactPhone = "+1 (555) 333-2211",
                Allergies = "None",
                RegisteredAt = DateTime.UtcNow.AddDays(-14)
            };

            context.Patients.AddRange(pat1, pat2, pat3);
            context.SaveChanges();

            // 5. Seed Appointments
            var appt1 = new Appointment
            {
                AppointmentCode = "APT-2026-001",
                PatientId = pat1.Id,
                DoctorId = doc1.Id,
                AppointmentDate = DateTime.Today.AddDays(1),
                TimeSlot = "10:30 AM",
                Symptoms = "Mild chest tightness upon exertion, occasional palpitations.",
                Status = "Confirmed",
                Fee = doc1.ConsultationFee,
                Notes = "Routine cardiac stress check requested."
            };

            var appt2 = new Appointment
            {
                AppointmentCode = "APT-2026-002",
                PatientId = pat2.Id,
                DoctorId = doc2.Id,
                AppointmentDate = DateTime.Today,
                TimeSlot = "11:00 AM",
                Symptoms = "Recurring migraine on right temple with aura.",
                Status = "In-Progress",
                Fee = doc2.ConsultationFee,
                Notes = "Patient brought previous MRI scans."
            };

            var appt3 = new Appointment
            {
                AppointmentCode = "APT-2026-003",
                PatientId = pat3.Id,
                DoctorId = doc1.Id,
                AppointmentDate = DateTime.Today.AddDays(-3),
                TimeSlot = "02:00 PM",
                Symptoms = "Annual wellness checkup and sports clearance.",
                Status = "Completed",
                Fee = doc1.ConsultationFee,
                Notes = "ECG normal, blood work within standard limits."
            };

            context.Appointments.AddRange(appt1, appt2, appt3);
            context.SaveChanges();

            // 6. Seed Wards & Beds (Warden Module)
            var wardICU = new Ward
            {
                WardName = "Intensive Care Unit (ICU)",
                WardType = "ICU",
                Capacity = 6,
                DailyCharge = 350.00m,
                WardenInCharge = "John Miller",
                FloorLocation = "Block C, 3rd Floor"
            };

            var wardGenMale = new Ward
            {
                WardName = "General Male Ward",
                WardType = "General",
                Capacity = 10,
                DailyCharge = 80.00m,
                WardenInCharge = "John Miller",
                FloorLocation = "Block A, 1st Floor"
            };

            var wardGenFemale = new Ward
            {
                WardName = "General Female Ward",
                WardType = "General",
                Capacity = 10,
                DailyCharge = 80.00m,
                WardenInCharge = "John Miller",
                FloorLocation = "Block A, 2nd Floor"
            };

            var wardPrivate = new Ward
            {
                WardName = "Deluxe Private Ward",
                WardType = "Private",
                Capacity = 4,
                DailyCharge = 220.00m,
                WardenInCharge = "John Miller",
                FloorLocation = "Block B, 4th Floor"
            };

            context.Wards.AddRange(wardICU, wardGenMale, wardGenFemale, wardPrivate);
            context.SaveChanges();

            // Beds for ICU
            var bed1 = new Bed { WardId = wardICU.Id, BedNumber = "ICU-01", IsOccupied = true };
            var bed2 = new Bed { WardId = wardICU.Id, BedNumber = "ICU-02", IsOccupied = false };
            var bed3 = new Bed { WardId = wardICU.Id, BedNumber = "ICU-03", IsOccupied = false };

            // Beds for General Male
            var bed4 = new Bed { WardId = wardGenMale.Id, BedNumber = "GM-101", IsOccupied = true };
            var bed5 = new Bed { WardId = wardGenMale.Id, BedNumber = "GM-102", IsOccupied = false };
            var bed6 = new Bed { WardId = wardGenMale.Id, BedNumber = "GM-103", IsOccupied = false };

            // Beds for General Female
            var bed7 = new Bed { WardId = wardGenFemale.Id, BedNumber = "GF-201", IsOccupied = false };
            var bed8 = new Bed { WardId = wardGenFemale.Id, BedNumber = "GF-202", IsOccupied = false };

            context.Beds.AddRange(bed1, bed2, bed3, bed4, bed5, bed6, bed7, bed8);
            context.SaveChanges();

            // 7. Seed Bed Admissions
            var adm1 = new BedAdmission
            {
                PatientId = pat2.Id,
                BedId = bed4.Id,
                AdmissionDate = DateTime.UtcNow.AddDays(-2),
                Reason = "Severe migraine monitoring and IV fluid hydration.",
                Status = "Admitted"
            };

            context.BedAdmissions.Add(adm1);
            context.SaveChanges();

            // 8. Seed Prescriptions (Doctor Module)
            var presc1 = new Prescription
            {
                AppointmentId = appt3.Id,
                PatientId = pat3.Id,
                DoctorId = doc1.Id,
                PrescribedDate = DateTime.UtcNow.AddDays(-3),
                Diagnosis = "General fatigue, mild vitamin D deficiency",
                Medicines = "• Cholecalciferol (Vitamin D3) 60,000 IU - Once weekly for 8 weeks\n• Multivitamin Daily Supplement - 1 tablet after breakfast for 30 days",
                AdviceInstructions = "Drink plenty of water (2.5L/day), 20 minutes daily morning sunlight exposure.",
                FollowUpDate = DateTime.Today.AddMonths(2)
            };

            context.Prescriptions.Add(presc1);
            context.SaveChanges();

            // 9. Seed Patient Vitals (Nurse Module)
            var vitals1 = new PatientVital
            {
                PatientId = pat2.Id,
                RecordedByNurse = "Nurse Clara Oswald",
                RecordedAt = DateTime.UtcNow.AddHours(-3),
                BloodPressure = "124/82",
                HeartRate = 74,
                Temperature = 98.4m,
                RespiratoryRate = 16,
                OxygenSaturation = 99m,
                BloodSugar = 95m,
                Notes = "Patient resting comfortably. Headache subsided following infusion."
            };

            var vitals2 = new PatientVital
            {
                PatientId = pat1.Id,
                RecordedByNurse = "Nurse Clara Oswald",
                RecordedAt = DateTime.UtcNow.AddDays(-1),
                BloodPressure = "118/78",
                HeartRate = 68,
                Temperature = 98.6m,
                RespiratoryRate = 18,
                OxygenSaturation = 98m,
                BloodSugar = 88m,
                Notes = "Baseline vitals recorded prior to tomorrow's consultation."
            };

            context.PatientVitals.AddRange(vitals1, vitals2);
            context.SaveChanges();

            // 10. Seed Invoices (Billing Module)
            var inv1 = new Invoice
            {
                InvoiceNumber = "INV-2026-101",
                PatientId = pat3.Id,
                AppointmentId = appt3.Id,
                TotalAmount = 90.00m,
                Discount = 0m,
                PaidAmount = 90.00m,
                PaymentStatus = "Paid",
                PaymentMethod = "Card",
                InvoiceDate = DateTime.UtcNow.AddDays(-3),
                Description = "Consultation Fee - Dr. Sophia Carter (Cardiology)"
            };

            var inv2 = new Invoice
            {
                InvoiceNumber = "INV-2026-102",
                PatientId = pat2.Id,
                BedAdmissionId = adm1.Id,
                TotalAmount = 270.00m,
                Discount = 20.00m,
                PaidAmount = 100.00m,
                PaymentStatus = "Partially Paid",
                PaymentMethod = "UPI",
                InvoiceDate = DateTime.UtcNow.AddDays(-1),
                Description = "Ward Stay (2 Days) + Consultation Fee - Dr. Marcus Vance"
            };

            context.Invoices.AddRange(inv1, inv2);
            context.SaveChanges();
        }
    }
}

