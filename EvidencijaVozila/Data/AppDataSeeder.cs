using EvidencijaVozila.Enums;
using EvidencijaVozila.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EvidencijaVozila.Data;

public static class AppDataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var unit = await context.OrganizationalUnits.FirstOrDefaultAsync();
        if (unit is null)
        {
            unit = new OrganizationalUnit { Name = "Ministarstvo unutarnjih poslova" };
            context.OrganizationalUnits.Add(unit);
            await context.SaveChangesAsync();
        }
        else if (unit.Name != "Ministarstvo unutarnjih poslova")
        {
            unit.Name = "Ministarstvo unutarnjih poslova";
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

        if (await context.Users.AnyAsync())
        {
            return;
        }

        var passwordHasher = new PasswordHasher<AppUser>();

        var admin = new AppUser
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
        await context.SaveChangesAsync();
    }
}
