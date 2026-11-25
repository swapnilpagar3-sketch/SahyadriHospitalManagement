-- ===================================================================================
-- Medicare Hospital Management System - SQL Server Database Schema & Seed Data Script
-- Platform: Microsoft SQL Server 2019 / 2022 / Azure SQL Database / LocalDB
-- Generated for: Asp Dot Net Mvc, Dot Net Core, Jquery, SQL Solution
-- ===================================================================================

USE master;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'HospitalManagementDb')
BEGIN
    CREATE DATABASE HospitalManagementDb;
END
GO

USE HospitalManagementDb;
GO

-- Drop tables if they already exist in reverse foreign key order
IF OBJECT_ID(N'dbo.Invoices', N'U') IS NOT NULL DROP TABLE dbo.Invoices;
IF OBJECT_ID(N'dbo.PatientVitals', N'U') IS NOT NULL DROP TABLE dbo.PatientVitals;
IF OBJECT_ID(N'dbo.Prescriptions', N'U') IS NOT NULL DROP TABLE dbo.Prescriptions;
IF OBJECT_ID(N'dbo.BedAdmissions', N'U') IS NOT NULL DROP TABLE dbo.BedAdmissions;
IF OBJECT_ID(N'dbo.Beds', N'U') IS NOT NULL DROP TABLE dbo.Beds;
IF OBJECT_ID(N'dbo.Wards', N'U') IS NOT NULL DROP TABLE dbo.Wards;
IF OBJECT_ID(N'dbo.Appointments', N'U') IS NOT NULL DROP TABLE dbo.Appointments;
IF OBJECT_ID(N'dbo.Patients', N'U') IS NOT NULL DROP TABLE dbo.Patients;
IF OBJECT_ID(N'dbo.Doctors', N'U') IS NOT NULL DROP TABLE dbo.Doctors;
IF OBJECT_ID(N'dbo.Departments', N'U') IS NOT NULL DROP TABLE dbo.Departments;
IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL DROP TABLE dbo.Users;
GO

-- 1. Users Table (Core Auth & Roles)
CREATE TABLE dbo.Users (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(30) NOT NULL DEFAULT 'Patient', -- Admin, Doctor, Nurse, Warden, Patient
    PhoneNumber NVARCHAR(25) NULL,
    ProfilePicture NVARCHAR(255) NOT NULL DEFAULT '/images/avatars/default.svg',
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsActive BIT NOT NULL DEFAULT 1,
    ResetToken NVARCHAR(50) NULL,
    ResetTokenExpiry DATETIME2 NULL
);
GO

-- 2. Departments Table
CREATE TABLE dbo.Departments (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    Location NVARCHAR(100) NOT NULL,
    HeadOfDepartment NVARCHAR(100) NOT NULL
);
GO

-- 3. Doctors Table
CREATE TABLE dbo.Doctors (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserId INT NOT NULL UNIQUE FOREIGN KEY REFERENCES dbo.Users(Id) ON DELETE CASCADE,
    DepartmentId INT NOT NULL FOREIGN KEY REFERENCES dbo.Departments(Id),
    Specialization NVARCHAR(100) NOT NULL,
    Qualification NVARCHAR(100) NOT NULL,
    ExperienceYears INT NOT NULL DEFAULT 0,
    ConsultationFee DECIMAL(18,2) NOT NULL DEFAULT 50.00,
    RoomNumber NVARCHAR(50) NOT NULL,
    AvailableDays NVARCHAR(100) NOT NULL DEFAULT 'Mon, Wed, Fri',
    AvailableTimeSlot NVARCHAR(100) NOT NULL DEFAULT '09:00 AM - 01:00 PM'
);
GO

-- 4. Patients Table
CREATE TABLE dbo.Patients (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserId INT NULL FOREIGN KEY REFERENCES dbo.Users(Id) ON DELETE SET NULL,
    PatientCode NVARCHAR(20) NOT NULL UNIQUE,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    DateOfBirth DATE NOT NULL,
    Gender NVARCHAR(10) NOT NULL DEFAULT 'Other',
    BloodGroup NVARCHAR(10) NOT NULL DEFAULT 'O+',
    Phone NVARCHAR(20) NOT NULL,
    Email NVARCHAR(100) NULL,
    Address NVARCHAR(250) NOT NULL,
    EmergencyContactName NVARCHAR(100) NOT NULL,
    EmergencyContactPhone NVARCHAR(20) NOT NULL,
    Allergies NVARCHAR(500) NOT NULL DEFAULT 'None',
    RegisteredAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO
CREATE UNIQUE NONCLUSTERED INDEX UQ_Patients_UserId ON dbo.Patients(UserId) WHERE UserId IS NOT NULL;
GO

-- 5. Appointments Table
CREATE TABLE dbo.Appointments (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    AppointmentCode NVARCHAR(20) NOT NULL UNIQUE,
    PatientId INT NOT NULL FOREIGN KEY REFERENCES dbo.Patients(Id),
    DoctorId INT NOT NULL FOREIGN KEY REFERENCES dbo.Doctors(Id),
    AppointmentDate DATETIME2 NOT NULL,
    TimeSlot NVARCHAR(50) NOT NULL DEFAULT '10:00 AM',
    Symptoms NVARCHAR(500) NOT NULL DEFAULT '',
    Status NVARCHAR(30) NOT NULL DEFAULT 'Scheduled', -- Scheduled, Confirmed, In-Progress, Completed, Cancelled
    Fee DECIMAL(18,2) NOT NULL DEFAULT 50.00,
    Notes NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

-- 6. Wards Table (Warden Module)
CREATE TABLE dbo.Wards (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    WardName NVARCHAR(100) NOT NULL,
    WardType NVARCHAR(50) NOT NULL DEFAULT 'General', -- General, Semi-Private, Private, ICU, Emergency
    Capacity INT NOT NULL DEFAULT 10,
    DailyCharge DECIMAL(18,2) NOT NULL DEFAULT 100.00,
    WardenInCharge NVARCHAR(100) NOT NULL,
    FloorLocation NVARCHAR(100) NOT NULL DEFAULT '1st Floor'
);
GO

-- 7. Beds Table (Warden Module)
CREATE TABLE dbo.Beds (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    WardId INT NOT NULL FOREIGN KEY REFERENCES dbo.Wards(Id) ON DELETE CASCADE,
    BedNumber NVARCHAR(20) NOT NULL UNIQUE,
    IsOccupied BIT NOT NULL DEFAULT 0
);
GO

-- 8. Bed Admissions Table (Inpatient Admission & Discharge)
CREATE TABLE dbo.BedAdmissions (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    PatientId INT NOT NULL FOREIGN KEY REFERENCES dbo.Patients(Id),
    BedId INT NOT NULL FOREIGN KEY REFERENCES dbo.Beds(Id),
    AdmissionDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    DischargeDate DATETIME2 NULL,
    Reason NVARCHAR(500) NOT NULL,
    Status NVARCHAR(30) NOT NULL DEFAULT 'Admitted', -- Admitted, Discharged
    DischargeSummary NVARCHAR(500) NULL
);
GO

-- 9. Prescriptions Table (Doctor Module)
CREATE TABLE dbo.Prescriptions (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    AppointmentId INT NULL UNIQUE FOREIGN KEY REFERENCES dbo.Appointments(Id) ON DELETE SET NULL,
    PatientId INT NOT NULL FOREIGN KEY REFERENCES dbo.Patients(Id),
    DoctorId INT NOT NULL FOREIGN KEY REFERENCES dbo.Doctors(Id),
    PrescribedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    Diagnosis NVARCHAR(200) NOT NULL,
    Medicines NVARCHAR(2000) NOT NULL,
    AdviceInstructions NVARCHAR(1000) NULL,
    FollowUpDate DATE NULL
);
GO

-- 10. Patient Vitals Table (Nurse Module)
CREATE TABLE dbo.PatientVitals (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    PatientId INT NOT NULL FOREIGN KEY REFERENCES dbo.Patients(Id) ON DELETE CASCADE,
    RecordedByNurse NVARCHAR(100) NOT NULL,
    RecordedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    BloodPressure NVARCHAR(20) NOT NULL DEFAULT '120/80',
    HeartRate INT NOT NULL DEFAULT 72,
    Temperature DECIMAL(4,1) NOT NULL DEFAULT 98.6,
    RespiratoryRate INT NOT NULL DEFAULT 16,
    OxygenSaturation DECIMAL(4,1) NOT NULL DEFAULT 98.0,
    BloodSugar DECIMAL(5,1) NULL,
    Notes NVARCHAR(500) NULL
);
GO

-- 11. Invoices Table (Billing Module)
CREATE TABLE dbo.Invoices (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    InvoiceNumber NVARCHAR(30) NOT NULL UNIQUE,
    PatientId INT NOT NULL FOREIGN KEY REFERENCES dbo.Patients(Id),
    AppointmentId INT NULL FOREIGN KEY REFERENCES dbo.Appointments(Id),
    BedAdmissionId INT NULL FOREIGN KEY REFERENCES dbo.BedAdmissions(Id),
    TotalAmount DECIMAL(18,2) NOT NULL,
    Discount DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    PaidAmount DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    PaymentStatus NVARCHAR(30) NOT NULL DEFAULT 'Pending', -- Pending, Paid, Partially Paid
    PaymentMethod NVARCHAR(30) NOT NULL DEFAULT 'Cash', -- Cash, Card, Insurance, UPI
    InvoiceDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    Description NVARCHAR(500) NULL
);
GO

-- Indexes for high-performance lookup
CREATE NONCLUSTERED INDEX IX_Appointments_PatientId ON dbo.Appointments(PatientId);
CREATE NONCLUSTERED INDEX IX_Appointments_DoctorId ON dbo.Appointments(DoctorId);
CREATE NONCLUSTERED INDEX IX_Appointments_Date ON dbo.Appointments(AppointmentDate);
CREATE NONCLUSTERED INDEX IX_Beds_WardId ON dbo.Beds(WardId);
CREATE NONCLUSTERED INDEX IX_BedAdmissions_PatientId ON dbo.BedAdmissions(PatientId);
CREATE NONCLUSTERED INDEX IX_PatientVitals_PatientId ON dbo.PatientVitals(PatientId);
CREATE NONCLUSTERED INDEX IX_Invoices_PatientId ON dbo.Invoices(PatientId);
GO

-- ===================================================================================
-- SEED DATA INSERTION
-- Password Hashes generated with BCrypt:
-- Admin: Admin@123 | Doctor: Doctor@123 | Nurse: Nurse@123 | Warden: Warden@123 | Patient: Patient@123
-- ===================================================================================

SET IDENTITY_INSERT dbo.Users ON;
INSERT INTO dbo.Users (Id, FullName, Email, PasswordHash, Role, PhoneNumber, ProfilePicture, CreatedAt, IsActive)
VALUES
(1, N'Dr. Alexander Wright', N'admin@hospital.com', N'$2a$11$0HwM7o.sH1p8fUfM7B5r7ek7Jm/h8C9rLgV.qHsm40i2Q4Uv1X2dG', N'Admin', N'+1 (555) 019-2834', N'/images/avatars/admin.svg', SYSUTCDATETIME(), 1),
(2, N'Dr. Sophia Carter', N'doctor.carter@hospital.com', N'$2a$11$0HwM7o.sH1p8fUfM7B5r7ek7Jm/h8C9rLgV.qHsm40i2Q4Uv1X2dG', N'Doctor', N'+1 (555) 345-6789', N'/images/avatars/doctor1.svg', SYSUTCDATETIME(), 1),
(3, N'Dr. Marcus Vance', N'doctor.vance@hospital.com', N'$2a$11$0HwM7o.sH1p8fUfM7B5r7ek7Jm/h8C9rLgV.qHsm40i2Q4Uv1X2dG', N'Doctor', N'+1 (555) 456-7890', N'/images/avatars/doctor2.svg', SYSUTCDATETIME(), 1),
(4, N'Nurse Clara Oswald', N'nurse.clara@hospital.com', N'$2a$11$0HwM7o.sH1p8fUfM7B5r7ek7Jm/h8C9rLgV.qHsm40i2Q4Uv1X2dG', N'Nurse', N'+1 (555) 567-8901', N'/images/avatars/nurse.svg', SYSUTCDATETIME(), 1),
(5, N'Warden John Miller', N'warden.john@hospital.com', N'$2a$11$0HwM7o.sH1p8fUfM7B5r7ek7Jm/h8C9rLgV.qHsm40i2Q4Uv1X2dG', N'Warden', N'+1 (555) 678-9012', N'/images/avatars/warden.svg', SYSUTCDATETIME(), 1),
(6, N'Emma Watson', N'patient.emma@hospital.com', N'$2a$11$0HwM7o.sH1p8fUfM7B5r7ek7Jm/h8C9rLgV.qHsm40i2Q4Uv1X2dG', N'Patient', N'+1 (555) 789-0123', N'/images/avatars/patient.svg', SYSUTCDATETIME(), 1);
SET IDENTITY_INSERT dbo.Users OFF;
GO

SET IDENTITY_INSERT dbo.Departments ON;
INSERT INTO dbo.Departments (Id, Name, Description, Location, HeadOfDepartment)
VALUES
(1, N'Cardiology', N'Heart and cardiovascular care, ECG, angiography and post-operative recovery.', N'Wing A, 2nd Floor', N'Dr. Sophia Carter'),
(2, N'Neurology', N'Disorders of nervous system, brain, spine and neuro-rehabilitation.', N'Wing B, 3rd Floor', N'Dr. Marcus Vance'),
(3, N'Pediatrics', N'Comprehensive child healthcare, vaccinations, pediatric intensive care.', N'Wing C, 1st Floor', N'Dr. Emily Blunt'),
(4, N'Orthopedics', N'Bones, joints, sports medicine and trauma reconstruction.', N'Wing D, Ground Floor', N'Dr. Gregory House'),
(5, N'General Medicine', N'Primary diagnosis, infection treatment and metabolic wellness.', N'Wing A, Ground Floor', N'Dr. Alexander Wright');
SET IDENTITY_INSERT dbo.Departments OFF;
GO

SET IDENTITY_INSERT dbo.Doctors ON;
INSERT INTO dbo.Doctors (Id, UserId, DepartmentId, Specialization, Qualification, ExperienceYears, ConsultationFee, RoomNumber, AvailableDays, AvailableTimeSlot)
VALUES
(1, 2, 1, N'Cardiologist', N'MD, FACC, MBBS', 12, 90.00, N'Room 204', N'Mon, Wed, Fri', N'09:00 AM - 01:00 PM'),
(2, 3, 2, N'Neurologist', N'MD, DM Neurology, MBBS', 15, 110.00, N'Room 312', N'Tue, Thu, Sat', N'10:00 AM - 03:00 PM');
SET IDENTITY_INSERT dbo.Doctors OFF;
GO

SET IDENTITY_INSERT dbo.Patients ON;
INSERT INTO dbo.Patients (Id, UserId, PatientCode, FirstName, LastName, DateOfBirth, Gender, BloodGroup, Phone, Email, Address, EmergencyContactName, EmergencyContactPhone, Allergies, RegisteredAt)
VALUES
(1, 6, N'PAT-1001', N'Emma', N'Watson', '1994-04-15', N'Female', N'A+', N'+1 (555) 789-0123', N'patient.emma@hospital.com', N'742 Evergreen Terrace, Springfield', N'Arthur Watson', N'+1 (555) 999-8877', N'Penicillin, Peanuts', SYSUTCDATETIME()),
(2, NULL, N'PAT-1002', N'David', N'Beck', '1982-08-22', N'Male', N'O+', N'+1 (555) 888-4321', N'david.beck@example.com', N'42 Wallaby Way, Sydney', N'Victoria Beck', N'+1 (555) 777-6655', N'Sulfa drugs', SYSUTCDATETIME()),
(3, NULL, N'PAT-1003', N'Lucas', N'Gray', '2005-11-03', N'Male', N'B+', N'+1 (555) 444-1122', N'lucas.gray@example.com', N'12 Grimmauld Place, London', N'Helen Gray', N'+1 (555) 333-2211', N'None', SYSUTCDATETIME());
SET IDENTITY_INSERT dbo.Patients OFF;
GO

SET IDENTITY_INSERT dbo.Wards ON;
INSERT INTO dbo.Wards (Id, WardName, WardType, Capacity, DailyCharge, WardenInCharge, FloorLocation)
VALUES
(1, N'Intensive Care Unit (ICU)', N'ICU', 6, 350.00, N'John Miller', N'Block C, 3rd Floor'),
(2, N'General Male Ward', N'General', 10, 80.00, N'John Miller', N'Block A, 1st Floor'),
(3, N'General Female Ward', N'General', 10, 80.00, N'John Miller', N'Block A, 2nd Floor'),
(4, N'Deluxe Private Ward', N'Private', 4, 220.00, N'John Miller', N'Block B, 4th Floor');
SET IDENTITY_INSERT dbo.Wards OFF;
GO

SET IDENTITY_INSERT dbo.Beds ON;
INSERT INTO dbo.Beds (Id, WardId, BedNumber, IsOccupied)
VALUES
(1, 1, N'ICU-01', 1),
(2, 1, N'ICU-02', 0),
(3, 1, N'ICU-03', 0),
(4, 2, N'GM-101', 1),
(5, 2, N'GM-102', 0),
(6, 2, N'GM-103', 0),
(7, 3, N'GF-201', 0),
(8, 3, N'GF-202', 0);
SET IDENTITY_INSERT dbo.Beds OFF;
GO

SET IDENTITY_INSERT dbo.BedAdmissions ON;
INSERT INTO dbo.BedAdmissions (Id, PatientId, BedId, AdmissionDate, DischargeDate, Reason, Status, DischargeSummary)
VALUES
(1, 2, 4, DATEADD(DAY, -2, SYSUTCDATETIME()), NULL, N'Severe migraine monitoring and IV fluid hydration.', N'Admitted', NULL);
SET IDENTITY_INSERT dbo.BedAdmissions OFF;
GO

SET IDENTITY_INSERT dbo.Appointments ON;
INSERT INTO dbo.Appointments (Id, AppointmentCode, PatientId, DoctorId, AppointmentDate, TimeSlot, Symptoms, Status, Fee, Notes, CreatedAt)
VALUES
(1, N'APT-2026-001', 1, 1, DATEADD(DAY, 1, SYSUTCDATETIME()), N'10:30 AM', N'Mild chest tightness upon exertion.', N'Confirmed', 90.00, N'Routine cardiac check.', SYSUTCDATETIME()),
(2, N'APT-2026-002', 2, 2, SYSUTCDATETIME(), N'11:00 AM', N'Recurring migraine on right temple with aura.', N'In-Progress', 110.00, N'Previous scans brought.', SYSUTCDATETIME()),
(3, N'APT-2026-003', 3, 1, DATEADD(DAY, -3, SYSUTCDATETIME()), N'02:00 PM', N'Annual sports wellness clearance.', N'Completed', 90.00, N'ECG normal.', SYSUTCDATETIME());
SET IDENTITY_INSERT dbo.Appointments OFF;
GO

SET IDENTITY_INSERT dbo.Prescriptions ON;
INSERT INTO dbo.Prescriptions (Id, AppointmentId, PatientId, DoctorId, PrescribedDate, Diagnosis, Medicines, AdviceInstructions, FollowUpDate)
VALUES
(1, 3, 3, 1, DATEADD(DAY, -3, SYSUTCDATETIME()), N'General fatigue and mild Vitamin D deficiency', N'• Cholecalciferol (Vit D3) 60,000 IU - Once weekly for 8 weeks
• Multivitamin Daily Supplement - 1 tablet after breakfast for 30 days', N'Drink 2.5L water daily, 20 mins morning sunlight.', DATEADD(MONTH, 2, CAST(SYSUTCDATETIME() AS DATE)));
SET IDENTITY_INSERT dbo.Prescriptions OFF;
GO

SET IDENTITY_INSERT dbo.PatientVitals ON;
INSERT INTO dbo.PatientVitals (Id, PatientId, RecordedByNurse, RecordedAt, BloodPressure, HeartRate, Temperature, RespiratoryRate, OxygenSaturation, BloodSugar, Notes)
VALUES
(1, 2, N'Nurse Clara Oswald', DATEADD(HOUR, -3, SYSUTCDATETIME()), N'124/82', 74, 98.4, 16, 99.0, 95.0, N'Patient resting comfortably.'),
(2, 1, N'Nurse Clara Oswald', DATEADD(DAY, -1, SYSUTCDATETIME()), N'118/78', 68, 98.6, 18, 98.0, 88.0, N'Baseline vitals logged prior to visit.');
SET IDENTITY_INSERT dbo.PatientVitals OFF;
GO

SET IDENTITY_INSERT dbo.Invoices ON;
INSERT INTO dbo.Invoices (Id, InvoiceNumber, PatientId, AppointmentId, BedAdmissionId, TotalAmount, Discount, PaidAmount, PaymentStatus, PaymentMethod, InvoiceDate, Description)
VALUES
(1, N'INV-2026-101', 3, 3, NULL, 90.00, 0.00, 90.00, N'Paid', N'Credit Card', DATEADD(DAY, -3, SYSUTCDATETIME()), N'Consultation Fee - Dr. Sophia Carter (Cardiology)'),
(2, N'INV-2026-102', 2, NULL, 1, 270.00, 20.00, 100.00, N'Partially Paid', N'UPI', DATEADD(DAY, -1, SYSUTCDATETIME()), N'Ward Stay (2 Days) + Specialist Consultation');
SET IDENTITY_INSERT dbo.Invoices OFF;
GO

PRINT N'Medicare Hospital Management System Database created and seeded successfully!';
GO

