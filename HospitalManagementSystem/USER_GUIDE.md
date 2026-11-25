# 📖 Medicare Hospital Management System - User Operational Guide

This handbook details how each user persona interacts with Medicare HMS.

---

## 1. Authentication, Registration & Profile

### Sign In (Login)
1. Navigate to `/Account/Login` or click **Sign In** from the top navigation bar.
2. Enter your registered email and password.
3. For rapid testing, use the **1-Click Demo Login** chips at the bottom of the form to auto-fill credentials for:
   - **Admin** (`admin@hospital.com`)
   - **Doctor** (`doctor.carter@hospital.com`)
   - **Nurse** (`nurse.clara@hospital.com`)
   - **Warden** (`warden.john@hospital.com`)
   - **Patient** (`patient.emma@hospital.com`)
4. Check **Keep me signed in on this device** to enable a 14-day persistent cookie session.

### Registration (Sign Up)
1. Click **Create an Account** or visit `/Account/Register`.
2. Choose your preferred profile avatar from the visual avatar picker.
3. Select your role:
   - When **Doctor** is chosen, additional inputs for *Specialization*, *Qualification*, and *Clinical Department* appear automatically.
   - When **Patient** is chosen, inputs for *Blood Group*, *Gender*, and *Date of Birth* appear automatically.
4. Submit the form to complete registration. Toastr confirms success and routes you to the login screen.

### Forgot Password Flow
1. Click **Forgot Password?** on the login page.
2. Enter your registered email address and submit.
3. The system generates a secure 10-character token (e.g. `8A3F29C1`) and routes to the reset page.
4. Enter the token, type your new password twice, and confirm.

### User Profile Management & Avatar Display
1. In the top navigation bar, the logged-in user's profile avatar is visible alongside their name and role badge.
2. Click the user dropdown and select **My Profile** (`/Account/Profile`).
3. You can update your Full Name, Phone Number, or select a new avatar image. Changes take effect across the entire UI immediately.

---

## 2. Role Modules Walkthrough

### 👨‍⚕️ Doctor Module
- **Schedule & Directory** (`/Doctors`): View specialized doctors, consulting hours, and consultation fees.
- **Appointments Management** (`/Appointments`): Review patient appointments, filter by status (Scheduled, Confirmed, Completed, Cancelled).
- **Issue Prescriptions** (`/Prescriptions/Create`):
  - Enter Clinical Diagnosis.
  - Type prescribed medicines with dosage, frequency, and duration.
  - Set follow-up date and lifestyle advice.
  - Automatically marks associated appointment as *Completed*.
- **Printable Prescription Slip** (`/Prescriptions/Details/{id}`): Formal Rx slip with hospital vector logo, doctor seal, and patient demographics.

### 👩‍⚕️ Nurse Station Module
- **Live Inpatients Monitor** (`/Nurse`): Lists all admitted patients in hospital beds. Shows latest vital signs (BP, Pulse, SpO2, Temp).
- **Record Vitals** (`/Nurse/RecordVitals`): Record Blood Pressure (mmHg), Heart Rate (bpm), Body Temperature (°F), Oxygen Saturation (SpO2 %), and Blood Sugar (mg/dL).
- **Vitals History Chart** (`/Nurse/VitalsHistory/{patientId}`): Chronological record of patient vital trends.

### 🏢 Warden Module (Ward & Bed Management)
- **Interactive Bed Matrix** (`/Warden`): Live visual cards showing bed status (Green = Available, Red = Occupied).
- **Admit Patient** (`/Warden/Admit`): Select an available bed, assign a patient, and record clinical admission reason. Automatically generates initial bed deposit invoice.
- **Discharge Patient** (`/Warden/Admissions`): Click **Discharge** on any active inpatient. Bed is freed instantly, and discharge summary is saved.
- **Create New Ward** (`/Warden/CreateWard`): Define ward type (ICU, Private, General, Emergency), capacity, daily charge, and auto-generate numbered beds (e.g., `ICU-01`, `ICU-02`).

### 👤 Patient Module
- **Self-Service Appointments** (`/Appointments/Create`): Choose specialist doctor, pick preferred date and time slot.
- **My Medical Record** (`/Patients/Details/{id}`): Comprehensive single-pane view showing personal vitals, active prescriptions, appointments, and bills.
- **Invoices & Receipts** (`/Billing`): View itemized charges and print official receipts.

### 🛠️ Administrator Module
- Full CRUD access over Doctors, Patients, Appointments, Wards, Beds, Departments, and Billing.
- Executive Dashboard (`/Dashboard`) featuring hospital occupancy rate, revenue counters, and operational volume.

