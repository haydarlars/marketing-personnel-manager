# Marketing Personnel Manager – Deployment Guide
**Audience:** Infrastructure / Server Administrator  
**Time required:** ~30 minutes

---

## Prerequisites

Ensure the following are installed on the Windows Server:

| Component | Version | Download |
|-----------|---------|----------|
| Windows Server | 2016 or later | — |
| IIS | 10+ (enable via Windows Features) | — |
| .NET 6.0 Runtime (Hosting Bundle) | 6.0.x | https://dotnet.microsoft.com/download/dotnet/6.0 |
| SQL Server | 2016 or later | Already installed |

---

## Step 1 – Set Up the Database

1. Open **SQL Server Management Studio (SSMS)**.
2. Connect to your SQL Server instance.
3. Click **New Query** and run the following:
   ```sql
   CREATE DATABASE MarketingDB;
   ```
4. In SSMS, open the file `SQL\01_CreateDatabase.sql` from the project folder.
5. At the top of the script, ensure `USE MarketingDB;` is uncommented.
6. Click **Execute (F5)**. This creates the tables and inserts seed data.

---

## Step 2 – Configure the Connection String

1. Open the `Deploy` folder.
2. Open the file `appsettings.json` in Notepad.
3. Update the `DefaultConnection` string to match your SQL Server:

   **Windows Authentication (recommended for IIS):**
   ```json
   "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=MarketingDB;Trusted_Connection=True;TrustServerCertificate=True"
   ```

   **SQL Login:**
   ```json
   "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=MarketingDB;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True"
   ```

   Replace `YOUR_SERVER_NAME` with the actual SQL Server name (e.g., `localhost`, `.\SQLEXPRESS`, or a network name).

4. Save the file.

---

## Step 3 – Install .NET Hosting Bundle

1. Download the **.NET 6.0 Windows Hosting Bundle** from:  
   `https://dotnet.microsoft.com/download/dotnet/6.0`
2. Run the installer and accept defaults.
3. **Restart IIS** after installation:
   ```
   iisreset
   ```

---

## Step 4 – Create IIS Site

1. Open **IIS Manager** (search "IIS" in Start menu).
2. In the left panel, right-click **Sites** → **Add Website**.
3. Fill in:
   - **Site name:** `MarketingApp`
   - **Physical path:** Browse to the `Deploy` folder (e.g., `C:\inetpub\MarketingApp`)
   - **Port:** `80` (or any available port, e.g., `8080`)
4. Click **OK**.

---

## Step 5 – Copy Deploy Files

1. Copy **all files** from the `Deploy` folder into the physical path you set above  
   (e.g., `C:\inetpub\MarketingApp`).
2. The folder should contain: `MarketingApp.API.dll`, `web.config`, `appsettings.json`,  
   `wwwroot\index.html`, and other `.dll` files.

---

## Step 6 – Set Application Pool

1. In IIS Manager, click **Application Pools** in the left panel.
2. Find the pool named `MarketingApp` (created automatically).
3. Right-click → **Advanced Settings**.
4. Set **.NET CLR Version** to `No Managed Code`.
5. Set **Identity** to `ApplicationPoolIdentity` (or a domain account with DB access if using Windows Auth).
6. Click **OK**.

---

## Step 7 – Grant SQL Server Access (Windows Auth only)

If using Windows Authentication, grant the IIS App Pool account access to SQL Server:

1. In SSMS, run:
   ```sql
   CREATE LOGIN [IIS APPPOOL\MarketingApp] FROM WINDOWS;
   USE MarketingDB;
   CREATE USER [IIS APPPOOL\MarketingApp] FOR LOGIN [IIS APPPOOL\MarketingApp];
   ALTER ROLE db_datareader ADD MEMBER [IIS APPPOOL\MarketingApp];
   ALTER ROLE db_datawriter ADD MEMBER [IIS APPPOOL\MarketingApp];
   ```

---

## Step 8 – Test the Application

1. Open a browser and navigate to: `http://YOUR_SERVER_IP/`  
   (or `http://localhost/` if testing locally)
2. The Marketing Personnel Manager should load with 8 pre-seeded staff members.
3. To verify the API, visit: `http://YOUR_SERVER_IP/swagger`

---

## Troubleshooting

| Problem | Solution |
|---------|----------|
| 502 Bad Gateway | .NET Hosting Bundle not installed, or IIS not restarted after install |
| 500 Internal Server Error | Check `appsettings.json` connection string; enable `stdoutLogEnabled="true"` in `web.config` temporarily |
| Cannot connect to DB | Verify SQL Server name, firewall, and App Pool identity permissions |
| Blank page | Check browser console (F12); ensure `wwwroot\index.html` is in the Deploy folder |

---

*For technical questions, contact the development team.*
