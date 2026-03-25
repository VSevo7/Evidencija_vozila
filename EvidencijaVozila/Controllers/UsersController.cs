using EvidencijaVozila.Data;
using EvidencijaVozila.Enums;
using EvidencijaVozila.Models;
using EvidencijaVozila.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EvidencijaVozila.Controllers;

[Authorize(Roles = nameof(UserRole.Administrator))]
public class UsersController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var users = await context.Users
            .AsNoTracking()
            .Include(x => x.OrganizationalUnit)
            .Include(x => x.Sector)
            .Include(x => x.ServiceDepartment)
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(x => new UserIndexItemViewModel
            {
                Id = x.Id,
                FullName = x.FullName,
                Username = x.Username,
                Email = x.Email,
                Role = x.Role.ToDisplay(),
                Status = x.Status.ToDisplay(),
                Organization = $"{x.OrganizationalUnit!.Name} / {x.Sector!.Name} / {x.ServiceDepartment!.Name}"
            })
            .ToListAsync();

        return View(users);
    }

    public async Task<IActionResult> Create()
    {
        var model = new UserFormViewModel
        {
            Status = UserStatus.Aktivan,
            Role = UserRole.Korisnik
        };
        await PopulateDropdownsAsync(model);
        return View("Form", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserFormViewModel model)
    {
        if (await context.Users.AnyAsync(x => x.Username == model.Username))
        {
            ModelState.AddModelError(nameof(model.Username), "Korisnicko ime vec postoji.");
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

        var user = new AppUser
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Username = model.Username,
            Email = model.Email,
            Role = model.Role,
            Status = model.Status,
            OrganizationalUnitId = model.OrganizationalUnitId,
            SectorId = model.SectorId,
            ServiceDepartmentId = model.ServiceDepartmentId
        };
        user.PasswordHash = new PasswordHasher<AppUser>().HashPassword(user, model.Password!);

        context.Users.Add(user);
        await context.SaveChangesAsync();

        TempData["Success"] = "Korisnik je uspjesno dodan.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var user = await context.Users.FindAsync(id);
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
            Role = user.Role,
            Status = user.Status,
            OrganizationalUnitId = user.OrganizationalUnitId,
            SectorId = user.SectorId,
            ServiceDepartmentId = user.ServiceDepartmentId
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

        if (await context.Users.AnyAsync(x => x.Username == model.Username && x.Id != id))
        {
            ModelState.AddModelError(nameof(model.Username), "Korisnicko ime vec postoji.");
        }

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(model);
            return View("Form", model);
        }

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Username = model.Username;
        user.Email = model.Email;
        user.Role = model.Role;
        user.Status = model.Status;
        user.OrganizationalUnitId = model.OrganizationalUnitId;
        user.SectorId = model.SectorId;
        user.ServiceDepartmentId = model.ServiceDepartmentId;

        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            user.PasswordHash = new PasswordHasher<AppUser>().HashPassword(user, model.Password);
        }

        await context.SaveChangesAsync();
        TempData["Success"] = "Korisnik je uspjesno azuriran.";
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

        user.Status = user.Status == UserStatus.Aktivan ? UserStatus.Neaktivan : UserStatus.Aktivan;
        await context.SaveChangesAsync();

        TempData["Success"] = "Status korisnika je promijenjen.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync(UserFormViewModel model)
    {
        model.OrganizationalUnits = await context.OrganizationalUnits
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
            .ToListAsync();

        model.Sectors = await context.Sectors
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
            .ToListAsync();

        model.ServiceDepartments = await context.ServiceDepartments
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
            .ToListAsync();
    }
}
