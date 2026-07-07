using EvidencijaVozila.Enums;
using EvidencijaVozila.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EvidencijaVozila.Data;

public static class AppDataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var hasChanges = false;

        var unit = await context.OrganizationalUnits.FirstOrDefaultAsync();
        if (unit is null)
        {
            unit = new OrganizationalUnit { Name = "Ministarstvo unutarnjih poslova" };
            context.OrganizationalUnits.Add(unit);
            hasChanges = true;
        }
        else if (unit.Name != "Ministarstvo unutarnjih poslova")
        {
            unit.Name = "Ministarstvo unutarnjih poslova";
            hasChanges = true;
        }

        if (hasChanges)
        {
            await context.SaveChangesAsync();
        }

        var adminSector = await context.Sectors.FirstOrDefaultAsync(x => x.Name == "Administracija");
        if (adminSector is null)
        {
            adminSector = new Sector
            {
                Name = "Administracija",
                OrganizationalUnitId = unit.Id
            };

            context.Sectors.Add(adminSector);
            await context.SaveChangesAsync();
        }
        else if (adminSector.OrganizationalUnitId != unit.Id)
        {
            adminSector.OrganizationalUnitId = unit.Id;
            await context.SaveChangesAsync();
        }

        var hasActiveAdministrator = await context.Users.AnyAsync(x =>
            x.Role == UserRole.Administrator &&
            x.Status == UserStatus.Aktivan);

        if (hasActiveAdministrator)
        {
            return;
        }

        var passwordHasher = new PasswordHasher<AppUser>();
        var admin = await context.Users.FirstOrDefaultAsync(x => x.Username == "admin");

        if (admin is null)
        {
            admin = new AppUser
            {
                FirstName = "Admin",
                LastName = "Sustava",
                Username = "admin",
                Email = "admin@organizacija.hr",
                Role = UserRole.Administrator,
                Status = UserStatus.Aktivan,
                OrganizationalUnitId = unit.Id,
                SectorId = adminSector.Id,
                ServiceDepartmentId = null,
                AssignmentType = UserAssignmentType.Sektor,
                Position = UserPosition.VoditeljSektora
            };

            admin.PasswordHash = passwordHasher.HashPassword(admin, "Admin123!");
            context.Users.Add(admin);
        }
        else
        {
            admin.FirstName = "Admin";
            admin.LastName = "Sustava";
            admin.Email = "admin@organizacija.hr";
            admin.Role = UserRole.Administrator;
            admin.Status = UserStatus.Aktivan;
            admin.OrganizationalUnitId = unit.Id;
            admin.SectorId = adminSector.Id;
            admin.ServiceDepartmentId = null;
            admin.AssignmentType = UserAssignmentType.Sektor;
            admin.Position = UserPosition.VoditeljSektora;
        }

        await context.SaveChangesAsync();
    }
}
