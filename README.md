# 🚀 SupplyFlow – Inventory Management SaaS

SupplyFlow is a full-stack Inventory Management SaaS application designed to help businesses manage products, warehouses, suppliers, purchasing, sales, stock, and inventory transactions from a centralized system.

The application is built with **Angular** on the frontend and **.NET 8 Web API** with **Entity Framework Core and SQL Server** on the backend.

---

## 📌 Project Overview

SupplyFlow provides a centralized platform for managing the complete inventory lifecycle.

The system includes:

* 🔐 Authentication & Authorization
* 🏢 Company Management
* 🏭 Warehouse Management
* 📦 Product Management
* 🗂️ Category Management
* 📏 Unit Management
* 🚚 Supplier Management
* 👥 Customer Management
* 📊 Warehouse Stock Management
* 🔄 Inventory Transactions
* 📝 Purchase Requests
* 🛒 Purchase Orders
* 📥 Goods Receipts
* 💰 Sales Management
* ↩️ Sales Returns
* 📈 Dashboard
* 📑 Reports
* 👤 User Management
* 🛡️ Role Management

---

# 🛠️ Tech Stack

## Frontend

| Technology   | Purpose              |
| ------------ | -------------------- |
| Angular      | Frontend Framework   |
| TypeScript   | Programming Language |
| Tailwind CSS | UI Styling           |
| HTML5        | Structure            |
| CSS3         | Styling              |

## Backend

| Technology            | Purpose              |
| --------------------- | -------------------- |
| .NET 8                | Backend Framework    |
| ASP.NET Core Web API  | REST API             |
| C#                    | Programming Language |
| Entity Framework Core | ORM                  |
| SQL Server            | Database             |
| AutoMapper            | Object Mapping       |
| JWT                   | Authentication       |
| Swagger / OpenAPI     | API Documentation    |

---

# 🏗️ Architecture

SupplyFlow follows a layered architecture:

```text
                    ┌──────────────────────┐
                    │      Angular UI      │
                    │   TypeScript +       │
                    │    Tailwind CSS      │
                    └──────────┬───────────┘
                               │
                               │ HTTP / REST API
                               ▼
                    ┌──────────────────────┐
                    │   SupplyFlow.API     │
                    │   Controllers        │
                    │   Middleware         │
                    └──────────┬───────────┘
                               │
                               ▼
                    ┌──────────────────────┐
                    │ SupplyFlow.Application│
                    │ DTOs + Interfaces    │
                    │ Business Contracts   │
                    └──────────┬───────────┘
                               │
                               ▼
                    ┌──────────────────────┐
                    │ SupplyFlow.Infrastructure│
                    │ EF Core + Services   │
                    │ Database + Persistence│
                    └──────────┬───────────┘
                               │
                               ▼
                    ┌──────────────────────┐
                    │      SQL Server      │
                    └──────────────────────┘
```

---

# 📂 Project Structure

```text
SupplyFlow/
│
├── Backend/
│   └── SupplyFlow/
│       │
│       ├── SupplyFlow.API/
│       │   ├── Controllers/
│       │   ├── Middleware/
│       │   ├── Properties/
│       │   ├── Program.cs
│       │   ├── appsettings.json
│       │   └── SupplyFlow.API.csproj
│       │
│       ├── SupplyFlow.Application/
│       │   ├── DTOs/
│       │   ├── Interfaces/
│       │   └── SupplyFlow.Application.csproj
│       │
│       ├── SupplyFlow.Domain/
│       │   ├── Entities/
│       │   ├── Enums/
│       │   └── SupplyFlow.Domain.csproj
│       │
│       ├── SupplyFlow.Infrastructure/
│       │   ├── Configurations/
│       │   ├── Migrations/
│       │   ├── Persistence/
│       │   ├── Services/
│       │   └── SupplyFlow.Infrastructure.csproj
│       │
│       └── SupplyFlow.sln
│
├── Frontend/
│   └── supplyflow/
│       ├── src/
│       ├── public/
│       ├── angular.json
│       ├── package.json
│       ├── package-lock.json
│       └── tsconfig.json
│
├── .gitignore
└── README.md
```

---

# ✨ Main Features

## 🔐 Authentication

* User registration
* User login
* Authentication using JWT
* Role-based access
* Current user handling

## 🏭 Warehouse Management

* Create warehouses
* Update warehouse information
* Manage warehouse data
* Track warehouse stock

## 📦 Product Management

* Product creation and management
* Product categories
* Units
* Product images
* Product stock information

## 🚚 Supplier Management

* Supplier management
* Supplier information
* Supplier-related purchasing workflow

## 📝 Purchase Management

SupplyFlow supports the purchasing workflow:

```text
Purchase Request
       ↓
Purchase Order
       ↓
Goods Receipt
       ↓
Warehouse Stock
       ↓
Inventory Transactions
```

## 💰 Sales Management

The system includes:

* Sales management
* Sales items
* Customer management
* Sales returns

## 📊 Inventory Management

Inventory functionality includes:

* Warehouse stock
* Stock transactions
* Low-stock tracking
* Inventory reports

## 📈 Dashboard & Reports

The backend includes dedicated services/controllers for:

* Dashboard
* Reports
* Inventory transactions
* Low-stock reporting
* Purchase order reporting

---

# 🔌 API Documentation

SupplyFlow provides RESTful APIs through the ASP.NET Core Web API.

Swagger/OpenAPI can be used during development to explore and test the API.

### API Modules

| Module                 | Controller                        |
| ---------------------- | --------------------------------- |
| Authentication         | `AuthController`                  |
| Categories             | `CategoriesController`            |
| Company                | `CompanyController`               |
| Customers              | `CustomersController`             |
| Dashboard              | `DashboardController`             |
| Goods Receipts         | `GoodsReceiptsController`         |
| Inventory Transactions | `InventoryTransactionsController` |
| Products               | `ProductsController`              |
| Purchase Orders        | `PurchaseOrdersController`        |
| Purchase Requests      | `PurchaseRequestsController`      |
| Reports                | `ReportsController`               |
| Roles                  | `RolesController`                 |
| Sales                  | `SalesController`                 |
| Sales Returns          | `SalesReturnsController`          |
| Suppliers              | `SuppliersController`             |
| Units                  | `UnitsController`                 |
| Users                  | `UsersController`                 |
| Warehouse Stocks       | `WarehouseStocksController`       |
| Warehouses             | `WarehousesController`            |

These controllers are present in the project's API layer.

### Typical REST Operations

The API is designed around standard REST operations:

```text
GET     → Retrieve data
POST    → Create data
PUT     → Update data
DELETE  → Delete data
```

### Authentication Flow

```text
User
 │
 ▼
Login
 │
 ▼
Auth API
 │
 ▼
JWT Token
 │
 ▼
Angular Application
 │
 ▼
Authenticated API Requests
```

> For the exact endpoint routes, request DTOs, response models, and authorization requirements, use the Swagger/OpenAPI documentation generated by the API.

---

# 🗄️ Database

SupplyFlow uses **Microsoft SQL Server** with **Entity Framework Core**.

The backend contains:

* `SupplyFlowDbContext`
* Entity configurations
* EF Core migrations
* Database seeding
* Persistence layer

The project also contains multiple EF Core migrations for schema evolution.

---

# ⚙️ Setup & Installation

## 1. Clone the Repository

```bash
git clone https://github.com/sujald3505/SupplyFlow.git
```

```bash
cd SupplyFlow
```

---

# 🔧 Backend Setup

### Requirements

Install:

* .NET 8 SDK
* SQL Server
* Visual Studio or VS Code

### Navigate to Backend

```bash
cd Backend/SupplyFlow
```

### Restore Dependencies

```bash
dotnet restore
```

### Build Project

```bash
dotnet build
```

### Configure Database

Open:

```text
Backend/SupplyFlow/SupplyFlow.API/appsettings.json
```

Configure the SQL Server connection string according to your local environment.

> Do not commit production database passwords, JWT secrets, or other sensitive credentials to GitHub.

### Apply EF Core Migrations

From the backend solution directory:

```bash
dotnet ef database update
```

If the EF CLI is not installed:

```bash
dotnet tool install --global dotnet-ef
```

---

# ▶️ Run Backend

Navigate to the API project:

```bash
cd Backend/SupplyFlow/SupplyFlow.API
```

Run:

```bash
dotnet run
```

The API will start using the configured ASP.NET Core launch settings.

Open the Swagger URL shown in the terminal/browser to explore the available APIs.

---

# 💻 Frontend Setup

## Requirements

Install:

* Node.js
* npm
* Angular CLI

### Navigate to Frontend

```bash
cd Frontend/supplyflow
```

### Install Dependencies

```bash
npm install
```

### Run Development Server

```bash
ng serve
```

Or:

```bash
npm start
```

Open the Angular application in your browser at the local development URL displayed by Angular CLI.

---

# 🔗 Frontend + Backend Configuration

Make sure the Angular application is configured to communicate with the running .NET Web API.

```text
Angular Frontend
       │
       │ HTTP Requests
       ▼
.NET 8 Web API
       │
       ▼
SQL Server
```

The API base URL should match the URL where the backend is running.

---

# 🧪 Development

### Backend

```bash
dotnet build
dotnet run
```

### Frontend

```bash
npm install
ng serve
```

---




# 🔄 Inventory Workflow

```text
                    SUPPLYFLOW
                        │
        ┌───────────────┴────────────────┐
        ▼                                ▼
   Purchasing                         Sales
        │                                │
        ▼                                ▼
Purchase Request                   Customer
        │                                │
        ▼                                ▼
Purchase Order                       Sale
        │                                │
        ▼                                ▼
Goods Receipt                     Sales Return
        │
        ▼
Warehouse Stock
        │
        ▼
Inventory Transactions
        │
        ▼
Reports & Dashboard
```

---

# 🧩 Backend Services

The Infrastructure layer contains dedicated services for major business modules, including authentication, categories, company, customers, dashboard, goods receipts, inventory, products, purchase orders, purchase requests, reports, roles, sales, sales returns, suppliers, units, users, warehouses, and warehouse stock.

---

# 🔒 Security

The project includes authentication and authorization infrastructure.

Important production practices:

* Keep JWT secrets private
* Keep database credentials private
* Use environment-specific configuration
* Never commit `.env` files containing secrets
* Never expose production connection strings publicly

---

# 🚧 Project Status

**Under Development**

SupplyFlow is being developed as a full-stack inventory management solution using Angular and .NET 8 Web API.

---

# 👨‍💻 Developer

**Sujal Dudhatra**

GitHub: [@sujald3505](https://github.com/sujald3505)

---

# ⭐ Support

If you find this project useful, consider giving the repository a ⭐.

---

## 📄 License

This project is currently developed for educational and portfolio purposes.
