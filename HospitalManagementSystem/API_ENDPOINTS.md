# 🌐 Controller Routes & AJAX Endpoints Reference

This document catalogs all MVC controller actions, HTTP verbs, authorization requirements, and AJAX endpoints in Medicare HMS.

---

## 1. Account & Authentication (`/Account`)

| Method | Route | Auth Required | Description |
| :--- | :--- | :---: | :--- |
| `GET` | `/Account/Login` | Anonymous | Renders Sign In page with Demo User chips |
| `POST` | `/Account/Login` | Anonymous | Authenticates credentials, issues Cookie Claims |
| `GET` | `/Account/Register` | Anonymous | Renders Registration page with dynamic role fields |
| `POST` | `/Account/Register` | Anonymous | Creates user and linked Doctor/Patient record |
| `GET` | `/Account/ForgotPassword` | Anonymous | Renders email recovery form |
| `POST` | `/Account/ForgotPassword` | Anonymous | Generates reset code simulation |
| `GET` | `/Account/ResetPassword` | Anonymous | Renders password reset form |
| `POST` | `/Account/ResetPassword` | Anonymous | Validates token and updates password hash |
| `POST/GET`| `/Account/Logout` | Authorized | Revokes authentication cookie |
| `GET` | `/Account/Profile` | Authorized | Renders profile editor |
| `POST` | `/Account/UpdateProfile` | Authorized | Updates profile name, phone, and avatar |
| `GET` | `/Account/AccessDenied` | Anonymous | Renders 403 Access Denied notification |

---

## 2. Dashboard (`/Dashboard`)

| Method | Route | Auth Required | Description |
| :--- | :--- | :---: | :--- |
| `GET` | `/Dashboard/Index` | Authorized | Aggregates KPIs, visits, bed occupancy, doctor roster |

---

## 3. Doctors Management (`/Doctors`)

| Method | Route | Auth Required | Description |
| :--- | :--- | :---: | :--- |
| `GET` | `/Doctors` | Authorized | Lists doctors with search & department filters |
| `GET` | `/Doctors/Details/{id}` | Authorized | Specialist bio, consulting schedule, visits |
| `GET` | `/Doctors/Create` | Admin | Doctor registration form |
| `POST` | `/Doctors/Create` | Admin | Saves doctor user and clinical record |
| `GET` | `/Doctors/Edit/{id}` | Admin, Doctor | Edit doctor details & consultation fees |
| `POST` | `/Doctors/Edit/{id}` | Admin, Doctor | Updates doctor profile in DB |
| `POST` | `/Doctors/Delete/{id}` | Admin | Removes doctor and user account |

---

## 4. Patients Registry (`/Patients`)

| Method | Route | Auth Required | Description |
| :--- | :--- | :---: | :--- |
| `GET` | `/Patients` | Authorized | Directory with search by name, code, blood group |
| `GET` | `/Patients/Details/{id}`| Authorized | Full Medical Record (Vitals, Prescriptions, History) |
| `GET` | `/Patients/Create` | Authorized | Register new patient form |
| `POST` | `/Patients/Create` | Authorized | Creates patient record with unique PatientCode |
| `GET` | `/Patients/Edit/{id}` | Authorized | Edit patient demographics |
| `POST` | `/Patients/Edit/{id}` | Authorized | Saves edited patient details |
| `POST` | `/Patients/Delete/{id}` | Admin | Deletes patient record |

---

## 5. Appointments & Triage (`/Appointments`)

| Method | Route | Auth Required | Description |
| :--- | :--- | :---: | :--- |
| `GET` | `/Appointments` | Authorized | Lists appointments with doctor & status filters |
| `GET` | `/Appointments/Details/{id}`| Authorized | Appointment slip with patient & doctor info |
| `GET` | `/Appointments/Create` | Authorized | Book consultation appointment |
| `POST` | `/Appointments/Create` | Authorized | Saves appointment and auto-generates invoice |
| `GET` | `/Appointments/Edit/{id}` | Authorized | Reschedule or edit appointment details |
| `POST` | `/Appointments/Edit/{id}` | Authorized | Updates appointment schedule |
| `POST` | `/Appointments/UpdateStatus`| Authorized | **AJAX**: Updates status (Confirmed, Cancelled, Completed) with Toastr |
| `POST` | `/Appointments/Delete/{id}` | Admin | Removes appointment |

---

## 6. Warden Module (`/Warden`)

| Method | Route | Auth Required | Description |
| :--- | :--- | :---: | :--- |
| `GET` | `/Warden` | Authorized | Live visual Bed Grid across all wards |
| `GET` | `/Warden/Admissions` | Authorized | Lists active and historical bed admissions |
| `GET` | `/Warden/Admit` | Authorized | Bed hospitalization form |
| `POST` | `/Warden/Admit` | Authorized | Allocates bed, marks bed as occupied, creates invoice |
| `POST` | `/Warden/Discharge` | Authorized | Discharges patient and frees bed |
| `GET` | `/Warden/CreateWard` | Admin, Warden | New ward creation form |
| `POST` | `/Warden/CreateWard` | Admin, Warden | Creates ward and auto-generates bed inventory |

---

## 7. Nurse Station (`/Nurse`)

| Method | Route | Auth Required | Description |
| :--- | :--- | :---: | :--- |
| `GET` | `/Nurse` | Authorized | Inpatients vitals monitoring station |
| `GET` | `/Nurse/RecordVitals` | Authorized | Vitals logging form (BP, SpO2, Temp, Pulse) |
| `POST` | `/Nurse/RecordVitals` | Authorized | Saves vital measurements to database |
| `GET` | `/Nurse/VitalsHistory/{id}`| Authorized | Chronological vitals trend for a patient |

---

## 8. Prescriptions (`/Prescriptions`)

| Method | Route | Auth Required | Description |
| :--- | :--- | :---: | :--- |
| `GET` | `/Prescriptions` | Authorized | Lists issued clinical prescriptions |
| `GET` | `/Prescriptions/Details/{id}`| Authorized | Official printable prescription slip (Rx) |
| `GET` | `/Prescriptions/Create` | Doctor, Admin | Prescription builder form |
| `POST` | `/Prescriptions/Create` | Doctor, Admin | Issues prescription and marks appointment completed |

---

## 9. Billing & Invoices (`/Billing`)

| Method | Route | Auth Required | Description |
| :--- | :--- | :---: | :--- |
| `GET` | `/Billing` | Authorized | Invoices directory and financial summaries |
| `GET` | `/Billing/Details/{id}` | Authorized | Itemized printable hospital invoice receipt |
| `GET` | `/Billing/Create` | Admin, Warden | Manual invoice generation form |
| `POST` | `/Billing/Create` | Admin, Warden | Saves invoice record |
| `POST` | `/Billing/Pay` | Authorized | **AJAX**: Records payment, calculates balance, fires Toastr |

---

## 10. Clinical Departments (`/Departments`)

| Method | Route | Auth Required | Description |
| :--- | :--- | :---: | :--- |
| `GET` | `/Departments` | Authorized | Specialties directory with doctor counts |
| `GET` | `/Departments/Create` | Admin | Department creation form |
| `POST` | `/Departments/Create` | Admin | Saves department record |
| `GET` | `/Departments/Edit/{id}` | Admin | Edit department details |
| `POST` | `/Departments/Edit/{id}` | Admin | Saves department changes |

