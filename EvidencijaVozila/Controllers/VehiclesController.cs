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
            .Include(x => x.TireChanges)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (vehicle is null)
        {
            return NotFound();
        }

        var activeOrderNumber = await context.VehicleOrders
            .AsNoTracking()
            .Where(x => x.VehicleId == id && x.Status == OrderStatus.Aktivan)
            .Select(x => x.OrderNumber)
            .FirstOrDefaultAsync();

        var model = new VehicleDetailsViewModel
        {
            Id = vehicle.Id,
            RegistrationNumber = vehicle.RegistrationNumber,
            BrandModel = vehicle.BrandModel,
            VehicleType = vehicle.VehicleType.ToDisplay(),
            FuelType = vehicle.FuelType.ToDisplay(),
            TransmissionType = vehicle.TransmissionType.ToDisplay(),
            CurrentTires = vehicle.CurrentTires,
            CurrentMileage = vehicle.CurrentMileage,
            PurchasePrice = vehicle.PurchasePrice,
            Status = string.IsNullOrWhiteSpace(activeOrderNumber) ? vehicle.Status.ToDisplay() : "Izdano",
            ActiveOrderNumber = activeOrderNumber,
            TireChanges = vehicle.TireChanges
                .OrderByDescending(x => x.ChangedAt)
                .ThenByDescending(x => x.Id)
                .Select(x => new VehicleTireChangeDetailsViewModel
                {
                    ChangedAt = x.ChangedAt,
                    TireType = x.TireType,
                    MileageAtChange = x.MileageAtChange
                })
                .ToList()
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
            PurchasePrice = decimal.Truncate(model.PurchasePrice),
            FuelType = model.FuelType,
            TransmissionType = model.TransmissionType,
            CurrentMileage = model.CurrentMileage,
            Status = model.Status
        };

        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync();

        SyncTireChanges(vehicle, model.TireChanges);
        await context.SaveChangesAsync();

        TempData["Success"] = "Vozilo je uspješno dodano.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = nameof(UserRole.Administrator))]
    public async Task<IActionResult> Edit(int id)
    {
        var vehicle = await context.Vehicles
            .AsNoTracking()
            .Include(x => x.TireChanges)
            .FirstOrDefaultAsync(x => x.Id == id);

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
            PurchasePrice = vehicle.PurchasePrice,
            FuelType = vehicle.FuelType,
            TransmissionType = vehicle.TransmissionType,
            CurrentMileage = vehicle.CurrentMileage,
            Status = vehicle.Status,
            TireChanges = vehicle.TireChanges
                .OrderByDescending(x => x.ChangedAt)
                .ThenByDescending(x => x.Id)
                .Select(x => new VehicleTireChangeFormItemViewModel
                {
                    ChangedAt = x.ChangedAt,
                    TireType = x.TireType,
                    MileageAtChange = x.MileageAtChange
                })
                .ToList()
        });
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Administrator))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VehicleFormViewModel model)
    {
        var vehicle = await context.Vehicles
            .Include(x => x.TireChanges)
            .FirstOrDefaultAsync(x => x.Id == id);

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
        vehicle.PurchasePrice = decimal.Truncate(model.PurchasePrice);
        vehicle.FuelType = model.FuelType;
        vehicle.TransmissionType = model.TransmissionType;
        vehicle.CurrentMileage = model.CurrentMileage;
        vehicle.Status = model.Status;

        SyncTireChanges(vehicle, model.TireChanges);

        await context.SaveChangesAsync();
        TempData["Success"] = "Vozilo je uspješno ažurirano.";
        return RedirectToAction(nameof(Index));
    }

    private void SyncTireChanges(Vehicle vehicle, IEnumerable<VehicleTireChangeFormItemViewModel>? items)
    {
        context.VehicleTireChanges.RemoveRange(vehicle.TireChanges);
        vehicle.TireChanges.Clear();

        if (items is null)
        {
            return;
        }

        foreach (var item in items.Where(x => x.ChangedAt.HasValue && !string.IsNullOrWhiteSpace(x.TireType) && x.MileageAtChange.HasValue))
        {
            var changedAt = item.ChangedAt ?? DateTime.MinValue;
            var mileageAtChange = item.MileageAtChange ?? 0;

            vehicle.TireChanges.Add(new VehicleTireChange
            {
                VehicleId = vehicle.Id,
                ChangedAt = changedAt.Date,
                TireType = InputNormalizer.NormalizeRequired(item.TireType),
                MileageAtChange = mileageAtChange
            });
        }
    }
}

