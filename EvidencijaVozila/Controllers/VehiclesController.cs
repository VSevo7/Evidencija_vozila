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

        ViewBag.SelectedStatus = status.HasValue ? ((int)status.Value).ToString() : null;
        ViewBag.SelectedVehicleType = vehicleType.HasValue ? ((int)vehicleType.Value).ToString() : null;

        var vehicles = await query
            .OrderBy(x => x.RegistrationNumber)
            .Select(x => new VehicleIndexItemViewModel
            {
                Id = x.Id,
                RegistrationNumber = x.RegistrationNumber,
                BrandModel = x.BrandModel,
                CurrentMileage = x.CurrentMileage,
                Status = x.Status.ToDisplay()
            })
            .ToListAsync();

        return View(vehicles);
    }

    public async Task<IActionResult> Details(int id)
    {
        var vehicle = await context.Vehicles
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new
            {
                x.Id,
                x.RegistrationNumber,
                x.BrandModel,
                VehicleType = x.VehicleType.ToDisplay(),
                FuelType = x.FuelType.ToDisplay(),
                TransmissionType = x.TransmissionType.ToDisplay(),
                x.CurrentTires,
                x.TireChangeNote,
                x.CurrentMileage,
                x.PurchasePrice,
                Status = x.Status.ToDisplay(),
                ActiveOrderNumber = context.VehicleOrders
                    .Where(o => o.VehicleId == x.Id && o.Status == OrderStatus.Aktivan)
                    .Select(o => o.OrderNumber)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();

        if (vehicle is null)
        {
            return NotFound();
        }

        var model = new VehicleDetailsViewModel
        {
            Id = vehicle.Id,
            RegistrationNumber = vehicle.RegistrationNumber,
            BrandModel = vehicle.BrandModel,
            VehicleType = vehicle.VehicleType,
            FuelType = vehicle.FuelType,
            TransmissionType = vehicle.TransmissionType,
            CurrentTires = vehicle.CurrentTires,
            TireChangeNote = vehicle.TireChangeNote,
            CurrentMileage = vehicle.CurrentMileage,
            PurchasePrice = vehicle.PurchasePrice,
            Status = string.IsNullOrWhiteSpace(vehicle.ActiveOrderNumber) ? vehicle.Status : "Izdano",
            ActiveOrderNumber = vehicle.ActiveOrderNumber
        };

        return View(model);
    }

    [Authorize(Roles = nameof(UserRole.Administrator))]
    public IActionResult Create()
    {
        return View("Form", new VehicleFormViewModel
        {
            Status = VehicleStatus.Aktivno
        });
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Administrator))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VehicleFormViewModel model)
    {
        var normalizedRegistration = InputNormalizer.NormalizeRegistrationLookup(model.RegistrationNumber);

        if (model.Status == VehicleStatus.Zauzeto)
        {
            ModelState.AddModelError(nameof(model.Status), "Novo vozilo ne može biti u statusu 'Izdano' bez aktivnog naloga.");
        }

        if (await context.Vehicles.AnyAsync(x =>
                x.RegistrationNumber.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpper() == normalizedRegistration))
        {
            ModelState.AddModelError(nameof(model.RegistrationNumber), "Registracijska oznaka već postoji.");
        }

        if (!ModelState.IsValid)
        {
            return View("Form", model);
        }

        var vehicle = new Vehicle
        {
            RegistrationNumber = InputNormalizer.NormalizeRegistrationForStorage(model.RegistrationNumber),
            BrandModel = InputNormalizer.NormalizeRequired(model.BrandModel),
            VehicleType = model.VehicleType,
            CurrentTires = InputNormalizer.NormalizeRequired(model.CurrentTires),
            TireChangeNote = InputNormalizer.NormalizeOptional(model.TireChangeNote),
            PurchasePrice = decimal.Truncate(model.PurchasePrice),
            FuelType = model.FuelType,
            TransmissionType = model.TransmissionType,
            CurrentMileage = model.CurrentMileage,
            Status = model.Status
        };

        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync();
        TempData["Success"] = "Vozilo je uspješno dodano.";
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
            CurrentTires = vehicle.CurrentTires,
            TireChangeNote = vehicle.TireChangeNote,
            PurchasePrice = vehicle.PurchasePrice,
            FuelType = vehicle.FuelType,
            TransmissionType = vehicle.TransmissionType,
            CurrentMileage = vehicle.CurrentMileage,
            Status = vehicle.Status
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

        var normalizedRegistration = InputNormalizer.NormalizeRegistrationLookup(model.RegistrationNumber);
        var hasActiveOrder = await context.VehicleOrders.AnyAsync(x => x.VehicleId == id && x.Status == OrderStatus.Aktivan);

        if (hasActiveOrder && model.Status != VehicleStatus.Zauzeto)
        {
            ModelState.AddModelError(nameof(model.Status), "Vozilo s aktivnim nalogom mora ostati u statusu 'Izdano'.");
        }

        if (!hasActiveOrder && model.Status == VehicleStatus.Zauzeto)
        {
            ModelState.AddModelError(nameof(model.Status), "Status 'Izdano' postavlja se automatski kada postoji aktivan nalog.");
        }

        if (await context.Vehicles.AnyAsync(x =>
                x.Id != id &&
                x.RegistrationNumber.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpper() == normalizedRegistration))
        {
            ModelState.AddModelError(nameof(model.RegistrationNumber), "Registracijska oznaka već postoji.");
        }

        if (!ModelState.IsValid)
        {
            return View("Form", model);
        }

        vehicle.RegistrationNumber = InputNormalizer.NormalizeRegistrationForStorage(model.RegistrationNumber);
        vehicle.BrandModel = InputNormalizer.NormalizeRequired(model.BrandModel);
        vehicle.VehicleType = model.VehicleType;
        vehicle.CurrentTires = InputNormalizer.NormalizeRequired(model.CurrentTires);
        vehicle.TireChangeNote = InputNormalizer.NormalizeOptional(model.TireChangeNote);
        vehicle.PurchasePrice = decimal.Truncate(model.PurchasePrice);
        vehicle.FuelType = model.FuelType;
        vehicle.TransmissionType = model.TransmissionType;
        vehicle.CurrentMileage = model.CurrentMileage;
        vehicle.Status = model.Status;

        await context.SaveChangesAsync();
        TempData["Success"] = "Vozilo je uspješno ažurirano.";
        return RedirectToAction(nameof(Index));
    }
}
