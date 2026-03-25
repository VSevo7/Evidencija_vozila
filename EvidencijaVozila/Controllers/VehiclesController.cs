using EvidencijaVozila.Data;
using EvidencijaVozila.Enums;
using EvidencijaVozila.Models;
using EvidencijaVozila.ViewModels.Vehicles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvidencijaVozila.Controllers;

[Authorize]
public class VehiclesController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index(VehicleStatus? status, VehicleType? vehicleType)
    {
        var query = context.Vehicles.AsNoTracking().AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (vehicleType.HasValue)
        {
            query = query.Where(x => x.VehicleType == vehicleType.Value);
        }

        ViewBag.SelectedStatus = status;
        ViewBag.SelectedVehicleType = vehicleType;

        var vehicles = await query
            .OrderBy(x => x.RegistrationNumber)
            .Select(x => new VehicleIndexItemViewModel
            {
                Id = x.Id,
                RegistrationNumber = x.RegistrationNumber,
                BrandModel = x.BrandModel,
                Specification = $"{x.VehicleType.ToDisplay()} / {x.FuelType.ToDisplay()} / {x.TransmissionType.ToDisplay()} / {x.CurrentTires}",
                CurrentMileage = x.CurrentMileage,
                Status = x.Status.ToDisplay(),
                IsActive = x.IsActive
            })
            .ToListAsync();

        return View(vehicles);
    }

    [Authorize(Roles = nameof(UserRole.Administrator))]
    public IActionResult Create()
    {
        return View("Form", new VehicleFormViewModel
        {
            Status = VehicleStatus.Slobodno,
            IsActive = true
        });
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Administrator))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VehicleFormViewModel model)
    {
        if (await context.Vehicles.AnyAsync(x => x.RegistrationNumber == model.RegistrationNumber))
        {
            ModelState.AddModelError(nameof(model.RegistrationNumber), "Registracijska oznaka vec postoji.");
        }

        if (!ModelState.IsValid)
        {
            return View("Form", model);
        }

        var vehicle = new Vehicle
        {
            RegistrationNumber = model.RegistrationNumber.ToUpperInvariant(),
            BrandModel = model.BrandModel,
            VehicleType = model.VehicleType,
            ServiceIntervalKm = model.ServiceIntervalKm,
            CurrentTires = model.CurrentTires,
            PurchasePrice = model.PurchasePrice,
            FuelType = model.FuelType,
            TransmissionType = model.TransmissionType,
            CurrentMileage = model.CurrentMileage,
            Status = model.Status,
            IsActive = model.IsActive
        };

        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync();
        TempData["Success"] = "Vozilo je uspjesno dodano.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = nameof(UserRole.Administrator))]
    public async Task<IActionResult> Edit(int id)
    {
        var vehicle = await context.Vehicles.FindAsync(id);
        if (vehicle is null)
        {
            return NotFound();
        }

        return View("Form", new VehicleFormViewModel
        {
            Id = vehicle.Id,
            RegistrationNumber = vehicle.RegistrationNumber,
            BrandModel = vehicle.BrandModel,
            VehicleType = vehicle.VehicleType,
            ServiceIntervalKm = vehicle.ServiceIntervalKm,
            CurrentTires = vehicle.CurrentTires,
            PurchasePrice = vehicle.PurchasePrice,
            FuelType = vehicle.FuelType,
            TransmissionType = vehicle.TransmissionType,
            CurrentMileage = vehicle.CurrentMileage,
            Status = vehicle.Status,
            IsActive = vehicle.IsActive
        });
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Administrator))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VehicleFormViewModel model)
    {
        var vehicle = await context.Vehicles.FindAsync(id);
        if (vehicle is null)
        {
            return NotFound();
        }

        if (await context.Vehicles.AnyAsync(x => x.RegistrationNumber == model.RegistrationNumber && x.Id != id))
        {
            ModelState.AddModelError(nameof(model.RegistrationNumber), "Registracijska oznaka vec postoji.");
        }

        if (!ModelState.IsValid)
        {
            return View("Form", model);
        }

        vehicle.RegistrationNumber = model.RegistrationNumber.ToUpperInvariant();
        vehicle.BrandModel = model.BrandModel;
        vehicle.VehicleType = model.VehicleType;
        vehicle.ServiceIntervalKm = model.ServiceIntervalKm;
        vehicle.CurrentTires = model.CurrentTires;
        vehicle.PurchasePrice = model.PurchasePrice;
        vehicle.FuelType = model.FuelType;
        vehicle.TransmissionType = model.TransmissionType;
        vehicle.CurrentMileage = model.CurrentMileage;
        vehicle.Status = model.Status;
        vehicle.IsActive = model.IsActive;

        await context.SaveChangesAsync();
        TempData["Success"] = "Vozilo je uspjesno azurirano.";
        return RedirectToAction(nameof(Index));
    }
}
