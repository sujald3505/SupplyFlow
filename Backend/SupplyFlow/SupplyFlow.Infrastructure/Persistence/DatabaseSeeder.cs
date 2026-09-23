using Microsoft.EntityFrameworkCore;
using SupplyFlow.Domain.Entities;

namespace SupplyFlow.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        SupplyFlowDbContext context)
    {
        // Apply any pending migrations
        await context.Database.MigrateAsync();

        // =========================
        // COMPANY
        // =========================

        Company company;

        if (!await context.Companies.AnyAsync())
        {
            company = new Company
            {
                Name = "SupplyFlow Demo Company",
                Email = "admin@supplyflow.com",
                Phone = "9999999999",
                Address = "India",
                GSTNumber = "24ABCDE1234F1Z",
                IsActive = true
            };

            context.Companies.Add(company);

            await context.SaveChangesAsync();
        }
        else
        {
            company = await context.Companies
                .FirstAsync();
        }


        // =========================
        // ROLES
        // =========================

        if (!await context.Roles.AnyAsync())
        {
            var roles = new List<Role>
            {
                new Role
                {
                    Name = "Admin",
                    Description = "Full system access"
                },
                new Role
                {
                    Name = "PurchaseManager",
                    Description = "Manage purchase requests and orders"
                },
                new Role
                {
                    Name = "WarehouseManager",
                    Description = "Manage warehouse and inventory"
                },
                new Role
                {
                    Name = "Employee",
                    Description = "Create purchase requests"
                }
            };

            context.Roles.AddRange(roles);

            await context.SaveChangesAsync();
        }


        // =========================
        // ADMIN USER
        // =========================

        var adminRole = await context.Roles
            .FirstAsync(x => x.Name == "Admin");

        if (!await context.Users.AnyAsync())
        {
            var adminUser = new User
            {
                CompanyId = company.Id,
                RoleId = adminRole.Id,
                FullName = "System Administrator",
                Email = "admin@supplyflow.com",

                // Temporary password for development
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),

                Phone = "99999 99999",
                IsActive = true
            };

            context.Users.Add(adminUser);

            await context.SaveChangesAsync();
        }


        // =========================
        // CATEGORIES
        // =========================

        if (!await context.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new Category
                {
                    CompanyId = company.Id,
                    Name = "Electronics",
                    Description = "Electronic products",
                    IsActive = true
                },
                new Category
                {
                    CompanyId = company.Id,
                    Name = "Office Supplies",
                    Description = "Office related products",
                    IsActive = true
                },
                new Category
                {
                    CompanyId = company.Id,
                    Name = "Raw Materials",
                    Description = "Manufacturing raw materials",
                    IsActive = true
                }
            };

            context.Categories.AddRange(categories);

            await context.SaveChangesAsync();
        }


        // =========================
        // UNITS
        // =========================

        if (!await context.Units.AnyAsync())
        {
            var units = new List<Unit>
            {
                new Unit
                {
                    CompanyId = company.Id,
                    Name = "Piece",
                    Symbol = "PCS",
                    IsActive = true
                },
                new Unit
                {
                    CompanyId = company.Id,
                    Name = "Kilogram",
                    Symbol = "KG",
                    IsActive = true
                },
                new Unit
                {
                    CompanyId = company.Id,
                    Name = "Liter",
                    Symbol = "LTR",
                    IsActive = true
                },
                new Unit
                {
                    CompanyId = company.Id,
                    Name = "Box",
                    Symbol = "BOX",
                    IsActive = true
                }
            };

            context.Units.AddRange(units);

            await context.SaveChangesAsync();
        }


        // =========================
        // MAIN WAREHOUSE
        // =========================

        if (!await context.Warehouses.AnyAsync())
        {
            var warehouse = new Warehouse
            {
                CompanyId = company.Id,
                Name = "Main Warehouse",
                Code = "MAIN-001",
                Address = "India",
                ContactPerson = "Warehouse Manager",
                Phone = "9999999999",
                IsActive = true
            };

            context.Warehouses.Add(warehouse);

            await context.SaveChangesAsync();
        }
    }
}