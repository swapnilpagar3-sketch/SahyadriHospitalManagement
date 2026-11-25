# 🏥 Medicare Hospital Management System (HMS)

> A modern, enterprise-grade Hospital Information and Clinical Management System built with **ASP.NET Core MVC (.NET 8/9)**, **Entity Framework Core**, **jQuery**, **Toastr notifications**, and **Microsoft SQL Server / SQLite**.

---

## 🌟 Key Highlights & Requirements Fulfilled

1. **Aesthetic Sign Up (Registration) Page**:
   - Modern medical glassmorphism card layout with embedded SVG hospital emblem.
   - Interactive profile avatar selector (Male/Female Doctor, Nurse, Warden, Patient, Admin).
   - Dynamic form adaptation: displays specialization, qualification, and department inputs when *Doctor* is selected, and blood group, gender, and date of birth when *Patient* is selected.
2. **Aesthetic Sign In (Login) Page**:
   - Hospital vector logo, remember-me support, password visibility toggle.
   - **1-Click Demo Login Chips**: Instant credentials fill for Admin, Doctor, Nurse, Warden, and Patient for rapid testing without manual typing.
3. **Forgot Password Flow**:
   - Aesthetic UI with vector hospital logo and email address verification.
   - Password reset token generation with instant link and reset screen to set a new password.
4. **Role-Based Modules**:
   - **Patient Module**: Electronic registration, medical history, appointment self-service, prescriptions, and billing receipts.
   - **Doctor Module**: Specialist profile, consulting schedule, assigned appointments, clinical diagnosis, and digital prescription builder.
   - **Nurse Station Module**: Inpatient vital signs monitoring (Blood Pressure, Heart Rate, SpO2, Temperature, Blood Glucose), vital trends, and nursing remarks.
   - **Warden Module**: Hospital bed management, visual ward bed matrix (Available vs. Occupied), inpatient bed admission, and discharge processing.
5. **Full CRUD Operations**:
   - Add/Edit/View/Cancel Appointments.
   - Add/Edit/View/Delete Doctors with Department linkages.
   - Register/Edit/View Patients with full clinical history.
   - Create Hospital Wards & automatically generate bed inventories.
   - Issue itemized Invoices and record payments.
6. **Toastr Integration**:
   - Server-side TempData auto-binds to real-time Toastr alerts on every create, update, delete, or authentication action.
   - AJAX endpoints utilize Toastr notifications for instantaneous feedback (e.g., appointment status change and invoice payments).
7. **Dynamic Logged-In User Profile Picture**:
   - Displays real-time user avatar in the top navbar and dropdown menu.
   - Profile management screen allows changing avatar, name, and contact details with instant cookie claims refresh.
8. **Additional Enterprise Modules**:
   - Clinical Departments directory.
   - Billing & Financial Accounts module with partial payments and printable receipts.
   - Interactive Analytics Dashboard with KPIs, live bed occupancy bars, and recent activity.
9. **Dual Database Engine Support**:
   - **SQLite**: Pre-configured out of the box for zero-setup execution.
   - **Microsoft SQL Server**: Full Transact-SQL schema script (`HospitalManagementSystem_Database.sql`) with tables, foreign keys, indexes, and seed data.

---

## 🛠️ Technology Stack

- **Backend Framework**: ASP.NET Core MVC (.NET 8.0 / .NET 9.0)
- **Data Access**: Entity Framework Core 8.0 (Code-First & Migration ready)
- **Databases**: SQLite (zero-config local runtime) & Microsoft SQL Server (production)
- **Security & Auth**: ASP.NET Core Cookie Authentication, Claims-Based Authorization, and BCrypt.Net-Next password hashing
- **Frontend & UI**: Razor Views, Bootstrap 5, Bootstrap Icons, Custom Medical Theme (`site.css`)
- **Client Scripting & UX**: jQuery 3.7, Toastr Notifications, AJAX interactions

---

## 🚀 Quick Start Guide

### Prerequisites
- [.NET 8.0 SDK or .NET 9.0 SDK](https://dotnet.microsoft.com/download)
- Optional: [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms) or Azure Data Studio

### Running the Project (Instant Out-of-the-Box)

```powershell
# Navigate to project directory
cd HospitalManagementSystem

# Run application (seeds database automatically on first run)
dotnet run
```

Open your browser and navigate to:
```
https://localhost:5001 or http://localhost:5000
```

---

## 🔑 Pre-Seeded Demo Credentials

The database automatically initializes with sample clinical departments, doctors, wards, beds, patients, and users.

| Role | Email | Password | Avatar |
| :--- | :--- | :--- | :--- |
| **Admin** | `admin@hospital.com` | `Admin@123` | Dr. Alexander Wright |
| **Doctor (Cardiology)** | `doctor.carter@hospital.com` | `Doctor@123` | Dr. Sophia Carter |
| **Doctor (Neurology)** | `doctor.vance@hospital.com` | `Doctor@123` | Dr. Marcus Vance |
| **Nurse** | `nurse.clara@hospital.com` | `Nurse@123` | Nurse Clara Oswald |
| **Warden (Ward Admin)**| `warden.john@hospital.com` | `Warden@123` | Warden John Miller |
| **Patient** | `patient.emma@hospital.com` | `Patient@123` | Emma Watson |

*(Tip: On the Sign In screen, click any of the **1-Click Demo Login** chips to populate credentials instantly.)*

---

## 📁 Project Directory Structure

```
HospitalManagementSystem/
├── HospitalManagementSystem.sln            # Solution File
├── HospitalManagementSystem/               # ASP.NET Core MVC Project
│   ├── Controllers/                        # MVC Controllers
│   │   ├── AccountController.cs            # Auth, Login, Register, ForgotPassword, Profile
│   │   ├── DashboardController.cs          # Overview stats, KPIs, bed occupancy
│   │   ├── DoctorsController.cs            # Doctor CRUD & scheduling
│   │   ├── PatientsController.cs           # Patient registry & medical records
│   │   ├── AppointmentsController.cs       # Booking, triage, AJAX status updates
│   │   ├── WardenController.cs             # Ward management, Bed matrix, Admissions
│   │   ├── NurseController.cs              # Vitals monitoring & logs
│   │   ├── PrescriptionsController.cs      # Prescription builder & printable Rx
│   │   ├── BillingController.cs            # Invoices, receipts & payments
│   │   ├── DepartmentsController.cs        # Clinical specialties & departments
│   │   └── HomeController.cs               # Landing portal
│   ├── Data/
│   │   ├── ApplicationDbContext.cs         # EF Core DbContext with relationship mappings
│   │   └── DbInitializer.cs                # Realistic seed data loader
│   ├── Models/                             # Domain Entities
│   │   ├── User.cs                         # User entity with auth & role info
│   │   ├── Doctor.cs                       # Doctor entity with specialization & fee
│   │   ├── Patient.cs                      # Patient entity with blood group & allergies
│   │   ├── Appointment.cs                  # Consultation appointment entity
│   │   ├── Ward.cs                         # Ward, Bed, and BedAdmission models
│   │   ├── Prescription.cs                 # Clinical prescription & medicines
│   │   ├── PatientVital.cs                 # Vital measurements (BP, Pulse, Temp, SpO2)
│   │   ├── Invoice.cs                      # Financial invoice & payment status
│   │   ├── Department.cs                   # Hospital clinical department
│   │   └── ViewModels/AuthViewModels.cs    # Auth, register & dashboard view models
│   ├── Views/                              # Razor Views
│   │   ├── Account/                        # Login, Register, ForgotPassword, Profile
│   │   ├── Appointments/                   # Index, Create, Edit, Details
│   │   ├── Billing/                        # Index, Create, Details (Receipt)
│   │   ├── Dashboard/                      # Index (Admin/Doctor/Nurse/Warden dashboard)
│   │   ├── Departments/                    # Index, Create, Edit
│   │   ├── Doctors/                        # Index, Create, Edit, Details
│   │   ├── Home/                           # Index (Public Landing Page), Privacy
│   │   ├── Nurse/                          # Index (Station), RecordVitals, VitalsHistory
│   │   ├── Patients/                       # Index, Create, Edit, Details (Medical Record)
│   │   ├── Prescriptions/                  # Index, Create, Details (Printable Rx)
│   │   ├── Shared/                         # _Layout, _ValidationScriptsPartial
│   │   └── Warden/                         # Index (Bed Matrix), Admissions, Admit, CreateWard
│   ├── wwwroot/                            # Static Web Assets
│   │   ├── css/site.css                    # Aesthetic medical theme styling
│   │   ├── images/hospital-logo.svg        # Vector Hospital Logo
│   │   ├── images/avatars/                 # Vector SVG avatars for all roles
│   │   ├── js/site.js                      # Client AJAX & Toastr helpers
│   │   └── lib/toastr/                     # Toastr notification JS & CSS
│   ├── appsettings.json                    # Configuration & Connection Strings
│   ├── HospitalManagementSystem_Database.sql# Full MS SQL Server schema & seed script
│   └── Program.cs                          # Startup, DI & middleware configuration
├── ARCHITECTURE.md                         # Architecture & design documentation
├── DATABASE_SETUP.md                       # SQL Server & SQLite configuration guide
├── USER_GUIDE.md                           # End-user operational handbook
└── API_ENDPOINTS.md                        # Controller routes & AJAX endpoints guide
```

---

## 📄 Documentation Links
- [Architecture & Design](file:///C:/Users/Client/.gemini/antigravity/scratch/HospitalManagementSystem/ARCHITECTURE.md)
- [Database Setup Guide](file:///C:/Users/Client/.gemini/antigravity/scratch/HospitalManagementSystem/DATABASE_SETUP.md)
- [User Operational Guide](file:///C:/Users/Client/.gemini/antigravity/scratch/HospitalManagementSystem/USER_GUIDE.md)
- [API & Controller Endpoints](file:///C:/Users/Client/.gemini/antigravity/scratch/HospitalManagementSystem/API_ENDPOINTS.md)
- [SQL Server Database Script](file:///C:/Users/Client/.gemini/antigravity/scratch/HospitalManagementSystem/HospitalManagementSystem_Database.sql)

