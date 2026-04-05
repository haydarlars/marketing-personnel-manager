# Marketing Personnel Manager

A single-page web application for managing marketing department personnel and their sales data.

---

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                         BROWSER (Client)                        │
│                                                                 │
│  index.html  ──  Vanilla JS + Bootstrap 5 + Chart.js           │
│       │                                                         │
│       │  HTTP/REST (JSON)                                       │
│       ▼                                                         │
├─────────────────────────────────────────────────────────────────┤
│                     ASP.NET Core 6 Web API                      │
│                                                                 │
│  PersonnelController  /api/personnel                           │
│  SalesController      /api/personnel/{id}/sales                │
│       │                                                         │
│       │  Entity Framework Core 6                               │
│       ▼                                                         │
├─────────────────────────────────────────────────────────────────┤
│                    SQL Server 2016+                             │
│                                                                 │
│   Personnel  ──(1:many)──  Sales                               │
│   (CASCADE DELETE)                                              │
└─────────────────────────────────────────────────────────────────┘
```

---

## Project Structure

```
Project/
├── SQL/
│   └── 01_CreateDatabase.sql    ← Run this FIRST to set up the DB
│
├── Deploy/
│   └── DeploymentGuide.md       ← Step-by-step IIS deployment guide
│   (Copy compiled output here)
│
└── Source/
    └── MarketingApp/
        └── MarketingApp.API/
            ├── Controllers/
            │   ├── PersonnelController.cs   ← GET/POST/PUT/DELETE /api/personnel
            │   └── SalesController.cs       ← GET/POST/DELETE /api/personnel/{id}/sales
            ├── Data/
            │   └── MarketingDbContext.cs    ← EF Core DbContext
            ├── DTOs/
            │   └── Dtos.cs                 ← Data Transfer Objects
            ├── Models/
            │   ├── Personnel.cs            ← EF Entity (Personnel table)
            │   └── Sale.cs                 ← EF Entity (Sales table)
            ├── wwwroot/
            │   └── index.html              ← Single-page frontend
            ├── Program.cs                  ← App startup & DI configuration
            ├── appsettings.json            ← Connection string configuration
            └── web.config                  ← IIS hosting configuration
```

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET    | /api/personnel | List all personnel |
| GET    | /api/personnel/{id} | Get single personnel |
| POST   | /api/personnel | Add new personnel |
| PUT    | /api/personnel/{id} | Edit personnel |
| DELETE | /api/personnel/{id} | Delete personnel + sales |
| GET    | /api/personnel/{id}/sales | Get sales for person |
| POST   | /api/personnel/{id}/sales | Add sales record |
| DELETE | /api/personnel/{id}/sales/{saleId} | Delete sales record |

---

## Running Locally (Development)

### Requirements
- .NET 6 SDK
- SQL Server (Express is fine)

### Steps

```bash
# 1. Restore packages
cd Source/MarketingApp/MarketingApp.API
dotnet restore

# 2. Update connection string in appsettings.json

# 3. Create DB (run SQL/01_CreateDatabase.sql in SSMS)

# 4. Run the application
dotnet run

# Visit: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

---

## Key Design Decisions

- **No Login System**: As per requirements – the app is open access.
- **Cascade Delete**: Configured both in SQL (FK constraint) and EF (DeleteBehavior.Cascade).
- **No Sales Edit**: By design – sales are immutable once added (add or delete only).
- **REST Architecture**: Clean separation; frontend only talks to the API, never the DB directly.
- **Static SPA**: `index.html` is served from `wwwroot` by the same ASP.NET Core app – no separate web server needed.
- **DTOs**: Models are not exposed directly; DTOs decouple the API contract from the database schema.

---

## Suggested Improvements (Bonus)

1. **Pagination** – Add `?page=1&pageSize=10` query params to GET /api/personnel for large datasets.
2. **Search/Filter** – Filter personnel by name or phone on the frontend.
3. **Date range filter** – Allow sales chart to be filtered by month/year.
4. **Export to CSV** – Let managers export sales data.
5. **Authentication** – Add ASP.NET Core Identity or Azure AD for access control.
6. **Audit Log** – Track who added/deleted records with timestamps.
7. **Dark mode** – Toggle via CSS variables already in place.
8. **Unit Tests** – Add xUnit tests for controllers using in-memory EF provider.
