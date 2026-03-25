using EvidencijaVozila.Enums;
using EvidencijaVozila.Models;
using Microsoft.AspNetCore.Identity;

namespace EvidencijaVozila.Data;

public static class AppDataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (context.Users.Any())
        {
            return;
        }

        var units = new[]
        {
            new OrganizationalUnit { Name = "Ravnateljstvo" },
            new OrganizationalUnit { Name = "Operativni centar" },
            new OrganizationalUnit { Name = "Tehnicka podrska" }
        };

        await context.OrganizationalUnits.AddRangeAsync(units);
        await context.SaveChangesAsync();

        var sectors = new[]
        {
            new Sector { Name = "Administracija", OrganizationalUnitId = units[0].Id },
            new Sector { Name = "Terenske operacije", OrganizationalUnitId = units[1].Id },
            new Sector { Name = "IT sektor", OrganizationalUnitId = units[2].Id }
        };

        await context.Sectors.AddRangeAsync(sectors);
        await context.SaveChangesAsync();

        var departments = new[]
        {
            new ServiceDepartment { Name = "Kadrovska sluzba", SectorId = sectors[0].Id },
            new ServiceDepartment { Name = "Vozni park", SectorId = sectors[1].Id },
            new ServiceDepartment { Name = "Informacijska podrska", SectorId = sectors[2].Id }
        };

        await context.ServiceDepartments.AddRangeAsync(departments);
        await context.SaveChangesAsync();

        var passwordHasher = new PasswordHasher<AppUser>();

        var admin = new AppUser
        {
            FirstName = "Admin",
            LastName = "Sustava",
            Username = "admin",
            Email = "admin@organizacija.hr",
            Role = UserRole.Administrator,
            Status = UserStatus.Aktivan,
            OrganizationalUnitId = units[0].Id,
            SectorId = sectors[0].Id,
            ServiceDepartmentId = departments[0].Id
        };
        admin.PasswordHash = passwordHasher.HashPassword(admin, "Admin123!");

        var ivo = new AppUser
        {
            FirstName = "Ivo",
            LastName = "Ivic",
            Username = "iivic",
            Email = "ivo.ivic@organizacija.hr",
            Role = UserRole.Korisnik,
            Status = UserStatus.Aktivan,
            OrganizationalUnitId = units[1].Id,
            SectorId = sectors[1].Id,
            ServiceDepartmentId = departments[1].Id
        };
        ivo.PasswordHash = passwordHasher.HashPassword(ivo, "Admin123!");

        var ana = new AppUser
        {
            FirstName = "Ana",
            LastName = "Anic",
            Username = "aanic",
            Email = "ana.anic@organizacija.hr",
            Role = UserRole.Korisnik,
            Status = UserStatus.Aktivan,
            OrganizationalUnitId = units[2].Id,
            SectorId = sectors[2].Id,
            ServiceDepartmentId = departments[2].Id
        };
        ana.PasswordHash = passwordHasher.HashPassword(ana, "Admin123!");

        await context.Users.AddRangeAsync(admin, ivo, ana);
        var vehicles = new[]
        {
            new Vehicle
            {
                RegistrationNumber = "ZG-101-AA",
                BrandModel = "Skoda Octavia",
                VehicleType = VehicleType.Osobno,
                ServiceIntervalKm = 15000,
                CurrentTires = "Ljetne",
                PurchasePrice = 24500m,
                FuelType = FuelType.Dizel,
                TransmissionType = TransmissionType.Rucni,
                CurrentMileage = 65200,
                Status = VehicleStatus.Slobodno,
                IsActive = true
            },
            new Vehicle
            {
                RegistrationNumber = "ZG-202-BB",
                BrandModel = "Volkswagen Caddy",
                VehicleType = VehicleType.Teretno,
                ServiceIntervalKm = 20000,
                CurrentTires = "Zimske",
                PurchasePrice = 28900m,
                FuelType = FuelType.Dizel,
                TransmissionType = TransmissionType.Rucni,
                CurrentMileage = 88400,
                Status = VehicleStatus.Slobodno,
                IsActive = true
            },
            new Vehicle
            {
                RegistrationNumber = "ZG-303-CC",
                BrandModel = "Renault Clio",
                VehicleType = VehicleType.Osobno,
                ServiceIntervalKm = 15000,
                CurrentTires = "Ljetne",
                PurchasePrice = 18950m,
                FuelType = FuelType.Benzin,
                TransmissionType = TransmissionType.Automatski,
                CurrentMileage = 43100,
                Status = VehicleStatus.VanUpotrebe,
                IsActive = true
            }
        };

        await context.Vehicles.AddRangeAsync(vehicles);
        await context.SaveChangesAsync();

        await context.VehicleOrders.AddAsync(
            new VehicleOrder
            {
                OrderNumber = "NAL-2026-001",
                VehicleId = vehicles[0].Id,
                DriverId = ivo.Id,
                OrganizationalUnitId = units[1].Id,
                IsBusinessTrip = true,
                DepartureAt = new DateTime(2026, 3, 20, 8, 0, 0),
                ReturnAt = new DateTime(2026, 3, 20, 16, 0, 0),
                MileageBefore = 64950,
                MileageAfter = 65200,
                Note = "Terenski obilazak.",
                CreatedAt = new DateTime(2026, 3, 19, 10, 0, 0),
                CreatedByUserId = admin.Id,
                Status = OrderStatus.Zavrsen
            });

        await context.SaveChangesAsync();
    }
}
