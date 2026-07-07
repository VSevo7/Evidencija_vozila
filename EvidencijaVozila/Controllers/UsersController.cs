using EvidencijaVozila.Data;
using EvidencijaVozila.Enums;
using EvidencijaVozila.Models;
using EvidencijaVozila.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvidencijaVozila.Controllers;

[Authorize(Roles = nameof(UserRole.Administrator))]
public class UsersController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var users = await context.Users
            .AsNoTracking()
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(x => new UserIndexItemViewModel
            {
                Id = x.Id,
                FullName = x.FullName,
                Username = x.Username,
                Email = x.Email,
                ContactPhone = x.ContactPhone,
                Role = x.Role.ToDisplay(),
                Status = x.Status.ToDisplay(),
                Position = x.Position.ToDisplay(),
                Organization = $"{x.OrganizationalUnit!.Name} / {x.Sector!.Name}{(x.ServiceDepartment != null ? " / " + x.ServiceDepartment.Name : string.Empty)}"
            })
            .ToListAsync();

        return View(users);
    }

    public async Task<IActionResult> Create()
    {
        var model = new UserFormViewModel
        {
            Status = UserStatus.Aktivan,
            Role = UserRole.Korisnik,
            AssignmentType = UserAssignmentType.Sektor,
            Position = UserPosition.Zaposlenik
        };

        await PopulateDropdownsAsync(model);
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserFormViewModel model)
    {
        var firstName = InputNormalizer.NormalizeRequired(model.FirstName);
        var lastName = InputNormalizer.NormalizeRequired(model.LastName);
        var username = InputNormalizer.NormalizeRequired(model.Username);
        var email = InputNormalizer.NormalizeRequired(model.Email);

        if (await context.Users.AnyAsync(x => x.Username == username))
        {
            ModelState.AddModelError(nameof(model.Username), "Korisničko ime već postoji.");
        }

        if (string.IsNullOrWhiteSpace(model.Password))
        {
            ModelState.AddModelError(nameof(model.Password), "Lozinka je obavezna.");
        }

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(model);
            return View("Form", model);
        }

        await ResolveAssignmentAsync(model);

        if (!ModelState.IsValid || !model.SectorId.HasValue || (model.AssignmentType == UserAssignmentType.Sluzba && !model.ServiceDepartmentId.HasValue))
        {
            await PopulateDropdownsAsync(model);
            return View("Form", model);
        }

        var user = new AppUser
        {
            FirstName = firstName,
            LastName = lastName,
            Username = username,
            Email = email,
            ContactPhone = InputNormalizer.NormalizeOptional(model.ContactPhone),
            Role = model.Role,
            Status = model.Status,
            OrganizationalUnitId = model.OrganizationalUnitId,
            SectorId = model.SectorId.Value,
            ServiceDepartmentId = model.ServiceDepartmentId,
            AssignmentType = model.AssignmentType,
            Position = model.Position
        };

        user.PasswordHash = new PasswordHasher<AppUser>().HashPassword(user, model.Password!);

        context.Users.Add(user);
        await context.SaveChangesAsync();

        TempData["Success"] = "Korisnik je uspješno dodan.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var user = await context.Users
            .AsNoTracking()
            .Include(x => x.OrganizationalUnit)
            .Include(x => x.Sector)
            .Include(x => x.ServiceDepartment)
                .ThenInclude(x => x!.Sector)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user is null)
        {
            return NotFound();
        }

        var model = new UserFormViewModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.Username,
            Email = user.Email,
            ContactPhone = user.ContactPhone,
            Role = user.Role,
            Status = user.Status,
            OrganizationalUnitId = user.OrganizationalUnitId,
            OrganizationalUnitName = user.OrganizationalUnit?.Name ?? "Ministarstvo unutarnjih poslova",
            AssignmentType = user.AssignmentType,
            SectorId = user.SectorId,
            ServiceDepartmentId = user.ServiceDepartmentId,
            SectorName = user.AssignmentType == UserAssignmentType.Sektor ? user.Sector?.Name : null,
            ServiceDepartmentName = user.ServiceDepartment?.Name,
            Position = user.Position
        };

        await PopulateDropdownsAsync(model);
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UserFormViewModel model)
    {
        var user = await context.Users.FindAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        var firstName = InputNormalizer.NormalizeRequired(model.FirstName);
        var lastName = InputNormalizer.NormalizeRequired(model.LastName);
        var username = InputNormalizer.NormalizeRequired(model.Username);
        var email = InputNormalizer.NormalizeRequired(model.Email);

        if (await context.Users.AnyAsync(x => x.Username == username && x.Id != id))
        {
            ModelState.AddModelError(nameof(model.Username), "Korisničko ime već postoji.");
        }

        var isRemovingLastActiveAdmin =
            user.Role == UserRole.Administrator &&
            user.Status == UserStatus.Aktivan &&
            (model.Role != UserRole.Administrator || model.Status != UserStatus.Aktivan) &&
            !await context.Users.AnyAsync(x =>
                x.Id != id &&
                x.Role == UserRole.Administrator &&
                x.Status == UserStatus.Aktivan);

        if (isRemovingLastActiveAdmin)
        {
            ModelState.AddModelError(nameof(model.Role), "U sustavu mora ostati barem jedan aktivan administrator.");
        }

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(model);
            return View("Form", model);
        }

        await ResolveAssignmentAsync(model);

        if (!ModelState.IsValid || !model.SectorId.HasValue || (model.AssignmentType == UserAssignmentType.Sluzba && !model.ServiceDepartmentId.HasValue))
        {
            await PopulateDropdownsAsync(model);
            return View("Form", model);
        }

        user.FirstName = firstName;
        user.LastName = lastName;
        user.Username = username;
        user.Email = email;
        user.ContactPhone = InputNormalizer.NormalizeOptional(model.ContactPhone);
        user.Role = model.Role;
        user.Status = model.Status;
        user.OrganizationalUnitId = model.OrganizationalUnitId;
        user.AssignmentType = model.AssignmentType;
        user.Position = model.Position;
        user.SectorId = model.SectorId.Value;
        user.ServiceDepartmentId = model.ServiceDepartmentId;

        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            user.PasswordHash = new PasswordHasher<AppUser>().HashPassword(user, model.Password);
        }

        await context.SaveChangesAsync();
        TempData["Success"] = "Korisnik je uspješno ažuriran.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var user = await context.Users.FindAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        var isDisablingLastActiveAdmin =
            user.Role == UserRole.Administrator &&
            user.Status == UserStatus.Aktivan &&
            !await context.Users.AnyAsync(x =>
                x.Id != id &&
                x.Role == UserRole.Administrator &&
                x.Status == UserStatus.Aktivan);

        if (isDisablingLastActiveAdmin)
        {
            TempData["Error"] = "U sustavu mora ostati barem jedan aktivan administrator.";
            return RedirectToAction(nameof(Index));
        }

        user.Status = user.Status == UserStatus.Aktivan ? UserStatus.Neaktivan : UserStatus.Aktivan;
        await context.SaveChangesAsync();

        TempData["Success"] = "Status korisnika je promijenjen.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync(UserFormViewModel model)
    {
        model.OrganizationalUnitId = await context.OrganizationalUnits
            .Select(x => x.Id)
            .FirstOrDefaultAsync();
        model.OrganizationalUnitName = "Ministarstvo unutarnjih poslova";

        model.SectorSuggestions = await context.Sectors
            .OrderBy(x => x.Name)
            .Select(x => x.Name)
            .Distinct()
            .ToListAsync();

        model.ServiceDepartmentSuggestions = await context.ServiceDepartments
            .OrderBy(x => x.Name)
            .Select(x => x.Name)
            .Distinct()
            .ToListAsync();
    }

    private async Task ResolveAssignmentAsync(UserFormViewModel model)
    {
        if (model.AssignmentType == UserAssignmentType.Sektor)
        {
            model.SectorId = await ResolveSectorIdAsync(model);
            model.ServiceDepartmentId = null;
            return;
        }

        var serviceDepartment = await FindExistingServiceDepartmentAsync(model);
        model.ServiceDepartmentId = serviceDepartment?.Id;
        model.SectorId = serviceDepartment?.SectorId;
    }

    private async Task<int?> ResolveSectorIdAsync(UserFormViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.SectorName))
        {
            return null;
        }

        return await GetOrCreateSectorAsync(model.SectorName, model.OrganizationalUnitId, nameof(model.SectorName));
    }

    private async Task<ServiceDepartment?> FindExistingServiceDepartmentAsync(UserFormViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.ServiceDepartmentName))
        {
            return null;
        }

        var serviceName = InputNormalizer.NormalizeRequired(model.ServiceDepartmentName);
        var normalizedName = NormalizeName(serviceName);
        var matchingServices = await context.ServiceDepartments
            .Include(x => x.Sector)
            .Where(x =>
                x.Sector != null &&
                x.Sector.OrganizationalUnitId == model.OrganizationalUnitId &&
                x.Name.Trim().ToUpper() == normalizedName)
            .OrderBy(x => x.Id)
            .Take(2)
            .ToListAsync();

        if (matchingServices.Count > 1)
        {
            ModelState.AddModelError(nameof(model.ServiceDepartmentName), "Postoji više službi s istim nazivom. Uredite organizaciju prije spremanja korisnika.");
            return null;
        }

        if (matchingServices.Count == 0)
        {
            ModelState.AddModelError(nameof(model.ServiceDepartmentName), "Služba nije pronađena. Prvo je dodajte kroz Organizacije.");
            return null;
        }

        return matchingServices[0];
    }

    private async Task<int?> GetOrCreateSectorAsync(string sectorName, int organizationalUnitId, string fieldName)
    {
        var trimmedSectorName = InputNormalizer.NormalizeRequired(sectorName);
        var normalizedName = NormalizeName(trimmedSectorName);
        var matchingSectors = await context.Sectors
            .Where(x =>
                x.OrganizationalUnitId == organizationalUnitId &&
                x.Name.Trim().ToUpper() == normalizedName)
            .OrderBy(x => x.Id)
            .Take(2)
            .ToListAsync();

        if (matchingSectors.Count > 1)
        {
            ModelState.AddModelError(fieldName, "Postoji više sektora s istim nazivom. Uredite organizaciju prije spremanja korisnika.");
            return null;
        }

        if (matchingSectors.Count == 1)
        {
            return matchingSectors[0].Id;
        }

        var newSector = new Sector
        {
            Name = trimmedSectorName,
            OrganizationalUnitId = organizationalUnitId
        };

        context.Sectors.Add(newSector);
        await context.SaveChangesAsync();
        return newSector.Id;
    }

    private static string NormalizeName(string value) => value.Trim().ToUpperInvariant();
}
