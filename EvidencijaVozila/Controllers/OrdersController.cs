using EvidencijaVozila.Data;
using EvidencijaVozila.Enums;
using EvidencijaVozila.Models;
using EvidencijaVozila.ViewModels.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EvidencijaVozila.Controllers;

[Authorize(Roles = nameof(UserRole.Administrator))]
public class OrdersController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index(string? orderNumber)
    {
        var trimmedOrderNumber = InputNormalizer.NormalizeOptional(orderNumber);
        var normalizedOrderNumber = string.IsNullOrWhiteSpace(trimmedOrderNumber)
            ? null
            : InputNormalizer.NormalizeOrderNumber(trimmedOrderNumber);

        var query = context.VehicleOrders
            .AsNoTracking()
            .Include(x => x.Vehicle)
            .Include(x => x.Driver)
                .ThenInclude(x => x!.Sector)
            .Include(x => x.Driver)
                .ThenInclude(x => x!.ServiceDepartment)
            .Include(x => x.OrganizationalUnit)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(normalizedOrderNumber))
        {
            query = query.Where(x => x.OrderNumber.Trim().ToUpper() == normalizedOrderNumber);
        }

        var orders = await query
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new OrderIndexItemViewModel
            {
                Id = x.Id,
                OrderNumber = x.OrderNumber,
                Vehicle = $"{x.Vehicle!.RegistrationNumber} / {x.Vehicle.BrandModel}",
                Driver = x.Driver!.FullName,
                OrganizationalUnit = x.OrganizationalUnit!.Name,
                Sector = x.Driver.Sector != null ? x.Driver.Sector.Name : "-",
                ServiceDepartment = x.Driver.ServiceDepartment != null ? x.Driver.ServiceDepartment.Name : "-",
                DepartureAt = x.DepartureAt,
                ReturnAt = x.ReturnAt,
                IsCompleted = x.Status == OrderStatus.Zavrsen,
                Status = x.Status.ToDisplay()
            })
            .ToListAsync();

        ViewBag.OrderSearchTerm = trimmedOrderNumber;
        ViewBag.OrderSearchPerformed = !string.IsNullOrWhiteSpace(trimmedOrderNumber);
        ViewBag.OrderSearchMissing = !string.IsNullOrWhiteSpace(trimmedOrderNumber) && orders.Count == 0;

        return View(orders);
    }

    public async Task<IActionResult> Create()
    {
        var model = new OrderFormViewModel
        {
            DepartureAt = DateTime.Today.AddHours(8)
        };

        await PopulateDropdownsAsync(model);
        model.MileageBefore = model.VehicleOptions.FirstOrDefault()?.CurrentMileage ?? 0;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrderFormViewModel model)
    {
        await PopulateDropdownsAsync(model);
        await ValidateOrderAsync(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var vehicle = await context.Vehicles.FindAsync(model.VehicleId);
        if (vehicle is null)
        {
            return NotFound();
        }

        model.MileageBefore = vehicle.CurrentMileage;

        var order = new VehicleOrder
        {
            OrderNumber = InputNormalizer.NormalizeOrderNumber(model.OrderNumber),
            VehicleId = model.VehicleId,
            DriverId = model.DriverId,
            OrganizationalUnitId = model.OrganizationalUnitId,
            IsBusinessTrip = model.IsBusinessTrip,
            DepartureAt = model.DepartureAt,
            ReturnAt = model.DepartureAt,
            MileageBefore = model.MileageBefore,
            Note = InputNormalizer.NormalizeOptional(model.Note),
            CreatedAt = DateTime.Now,
            CreatedByUserId = User.CurrentUserId(),
            Status = OrderStatus.Aktivan
        };

        vehicle.Status = VehicleStatus.Zauzeto;
        vehicle.IsActive = true;

        context.VehicleOrders.Add(order);
        await context.SaveChangesAsync();

        TempData["Success"] = "Nalog je uspješno kreiran.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await context.VehicleOrders
            .AsNoTracking()
            .Include(x => x.Vehicle)
            .Include(x => x.Driver)
                .ThenInclude(x => x!.Sector)
            .Include(x => x.Driver)
                .ThenInclude(x => x!.ServiceDepartment)
            .Include(x => x.CreatedByUser)
            .Include(x => x.OrganizationalUnit)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (order is null)
        {
            return NotFound();
        }

        var model = new OrderDetailsViewModel
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Vehicle = $"{order.Vehicle!.RegistrationNumber} / {order.Vehicle.BrandModel}",
            Driver = order.Driver!.FullName,
            OrganizationalUnit = order.OrganizationalUnit!.Name,
            Sector = order.Driver.Sector != null ? order.Driver.Sector.Name : "-",
            ServiceDepartment = order.Driver.ServiceDepartment != null ? order.Driver.ServiceDepartment.Name : "-",
            IsBusinessTrip = order.IsBusinessTrip,
            DepartureAt = order.DepartureAt,
            ReturnAt = order.ReturnAt,
            IsCompleted = order.Status == OrderStatus.Zavrsen,
            MileageBefore = order.MileageBefore,
            MileageAfter = order.MileageAfter,
            Note = order.Note,
            Status = order.Status.ToDisplay(),
            CreatedBy = order.CreatedByUser!.FullName,
            VehicleSpecs = $"{order.Vehicle.VehicleType.ToDisplay()} / {order.Vehicle.FuelType.ToDisplay()} / {order.Vehicle.TransmissionType.ToDisplay()}",
            CompleteOrder = new CompleteOrderViewModel
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                MileageAfter = order.MileageAfter ?? order.MileageBefore,
                Note = order.Note
            }
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete([Bind(Prefix = "CompleteOrder")] CompleteOrderViewModel model)
    {
        var order = await context.VehicleOrders
            .Include(x => x.Vehicle)
            .FirstOrDefaultAsync(x => x.Id == model.OrderId);

        if (order is null)
        {
            return NotFound();
        }

        if (model.MileageAfter < order.MileageBefore)
        {
            TempData["Error"] = "Povratna kilometraža ne može biti manja od početne.";
            return RedirectToAction(nameof(Details), new { id = model.OrderId });
        }

        order.MileageAfter = model.MileageAfter;
        order.Note = InputNormalizer.NormalizeOptional(model.Note);
        order.ReturnAt = DateTime.Now;
        order.Status = OrderStatus.Zavrsen;

        if (order.Vehicle is not null)
        {
            order.Vehicle.CurrentMileage = model.MileageAfter;
            order.Vehicle.Status = VehicleStatus.Aktivno;
            order.Vehicle.IsActive = true;
        }

        await context.SaveChangesAsync();
        TempData["Success"] = "Nalog je završen i vozilo je vraćeno u evidenciju.";
        return RedirectToAction(nameof(Details), new { id = model.OrderId });
    }

    private async Task PopulateDropdownsAsync(OrderFormViewModel model)
    {
        model.OrganizationalUnitId = await context.OrganizationalUnits
            .Select(x => x.Id)
            .FirstOrDefaultAsync();
        model.OrganizationalUnitName = "Ministarstvo unutarnjih poslova";

        model.VehicleOptions = await context.Vehicles
            .Where(x => x.Status == VehicleStatus.Aktivno)
            .OrderBy(x => x.RegistrationNumber)
            .Select(x => new OrderVehicleOptionViewModel
            {
                Id = x.Id,
                Label = $"{x.RegistrationNumber} / {x.BrandModel}",
                CurrentMileage = x.CurrentMileage
            })
            .ToListAsync();

        model.Drivers = await context.Users
            .Where(x => x.Status == UserStatus.Aktivan)
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(x => new SelectListItem(x.FullName, x.Id.ToString()))
            .ToListAsync();
    }

    private async Task ValidateOrderAsync(OrderFormViewModel model)
    {
        var normalizedOrderNumber = InputNormalizer.NormalizeOrderNumber(model.OrderNumber);

        if (!model.VehicleOptions.Any())
        {
            ModelState.AddModelError(string.Empty, "Trenutno nema raspoloživih vozila.");
            return;
        }

        if (await context.VehicleOrders.AnyAsync(x => x.OrderNumber.Trim().ToUpper() == normalizedOrderNumber))
        {
            ModelState.AddModelError(nameof(model.OrderNumber), "Broj naloga već postoji.");
        }

        var vehicle = await context.Vehicles.FindAsync(model.VehicleId);
        if (vehicle is null || vehicle.Status != VehicleStatus.Aktivno)
        {
            ModelState.AddModelError(nameof(model.VehicleId), "Odabrano vozilo nije raspoloživo.");
            return;
        }

        var driver = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == model.DriverId);

        if (driver is null || driver.Status != UserStatus.Aktivan)
        {
            ModelState.AddModelError(nameof(model.DriverId), "Odabrani vozač nije dostupan za nalog.");
        }

        model.MileageBefore = vehicle.CurrentMileage;

        var activeOrderExists = await context.VehicleOrders.AnyAsync(x =>
            x.VehicleId == model.VehicleId &&
            x.Status == OrderStatus.Aktivan);

        if (activeOrderExists)
        {
            ModelState.AddModelError(nameof(model.VehicleId), "Odabrano vozilo je već zauzeto.");
        }
    }
}
