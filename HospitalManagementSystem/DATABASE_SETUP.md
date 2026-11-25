# 🗄️ Database Setup & Configuration Guide

Medicare Hospital Management System provides out-of-the-box support for both **SQLite** and **Microsoft SQL Server**.

---

## Option 1: SQLite (Default Zero-Configuration Mode)

The project is preconfigured to run with SQLite by default. This requires **no database server installation** and is ideal for immediate local testing and evaluation.

### How it works
In `appsettings.json`:
```json
{
  "DatabaseProvider": "Sqlite",
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=HospitalManagement.db"
  }
}
```

When you execute:
```powershell
dotnet run
```
`DbInitializer.Initialize(context)` automatically creates `HospitalManagement.db` in the application root, creates all tables, and seeds complete mock data.

---

## Option 2: Microsoft SQL Server / LocalDB / Azure SQL

You can set up SQL Server using either the **native `.bak` backup file** or the **T-SQL schema script**.

### Method A: Restore from the Native `.bak` File in SSMS (Fastest)

A valid Microsoft SQL Server full backup is generated: `HospitalManagementDb.bak` (in your `Downloads` folder).

#### Via SQL Server Management Studio (SSMS) GUI:
1. Open **SSMS** and connect to your SQL Server instance (e.g. `.` or `localhost` or `(localdb)\mssqllocaldb`).
2. Right-click on **Databases** in the Object Explorer and select **Restore Database...**.
3. Under **Source**, select **Device**, click the **...** button, and click **Add**.
4. Browse to `C:\Users\Client\Downloads\HospitalManagementDb.bak` and select it.
5. Click **OK**. SSMS will verify the backup header and restore `HospitalManagementDb` with all tables, views, foreign keys, and seed data.

#### Via T-SQL Query in SSMS:
```sql
RESTORE DATABASE [HospitalManagementDb]
FROM DISK = N'C:\Users\Client\Downloads\HospitalManagementDb.bak'
WITH REPLACE, RECOVERY, STATS = 10;
GO
```

### Method B: Execute the T-SQL Script (Alternative)
A comprehensive script is included in the project root: `HospitalManagementSystem_Database.sql`.

1. Open **SQL Server Management Studio (SSMS)** or **Azure Data Studio**.
2. Connect to your SQL Server instance (e.g., `localhost`, `(localdb)\mssqllocaldb`, or your server name).
3. Open `HospitalManagementSystem_Database.sql`.
4. Click **Execute** (F5).
5. The script creates the `HospitalManagementDb` database, defines all 11 tables with foreign keys and indexes, and inserts seed records for users, doctors, wards, beds, and patients.

### Step 2: Configure `appsettings.json`
Change the `DatabaseProvider` to `SqlServer` and verify your connection string:

```json
{
  "DatabaseProvider": "SqlServer",
  "ConnectionStrings": {
    "SqlServerConnection": "Server=localhost;Database=HospitalManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

*For SQL Server Authentication (Username & Password):*
```json
"SqlServerConnection": "Server=localhost;Database=HospitalManagementDb;User Id=sa;Password=YourStrongPassword123;TrustServerCertificate=True"
```

*For Windows LocalDB:*
```json
"SqlServerConnection": "Server=(localdb)\\mssqllocaldb;Database=HospitalManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

### Step 3: Run the Application
```powershell
dotnet run
```
The application will connect to SQL Server, perform authentication checks, and operate directly against your SQL Server database.

---

## Database Verification Queries

Run the following query in SSMS to verify the seeded records:

```sql
USE HospitalManagementDb;
GO

SELECT 'Users' AS TableName, COUNT(*) AS [Count] FROM dbo.Users
UNION ALL
SELECT 'Departments', COUNT(*) FROM dbo.Departments
UNION ALL
SELECT 'Doctors', COUNT(*) FROM dbo.Doctors
UNION ALL
SELECT 'Patients', COUNT(*) FROM dbo.Patients
UNION ALL
SELECT 'Appointments', COUNT(*) FROM dbo.Appointments
UNION ALL
SELECT 'Wards', COUNT(*) FROM dbo.Wards
UNION ALL
SELECT 'Beds', COUNT(*) FROM dbo.Beds
UNION ALL
SELECT 'PatientVitals', COUNT(*) FROM dbo.PatientVitals
UNION ALL
SELECT 'Invoices', COUNT(*) FROM dbo.Invoices;
```

