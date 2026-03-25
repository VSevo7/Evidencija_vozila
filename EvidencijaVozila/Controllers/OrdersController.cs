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
    public async Task<IActionResult> Index()
    {
        var orders = await context.VehicleOrders
            .AsNoTracking()
            .Include(x => x.Vehicle)
            .Include(x => x.Driver)
            .Include(x => x.OrganizationalUnit)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new OrderIndexItemViewModel
            {
                Id = x.Id,
                OrderNumber = x.OrderNumber,
                Vehicle = $"{x.Vehicle!.RegistrationNumber} / {x.Vehicle.BrandModel}",
                Driver = x.Driver!.FullName,
                OrganizationalUnit = x.OrganizationalUnit!.Name,
                DepartureAt = x.DepartureAt,
                ReturnAt = x.ReturnAt,
                Status = x.Status.ToDisplay()
            })
            .ToListAsync();

        return View(orders);
    }

    public async Task<IActionResult> Create()
    {
        var model = new OrderFormViewModel
        {
            DepartureAt = DateTime.Today.AddHours(8),
            ReturnAt = DateTime.Today.AddHours(16)
        };
        await PopulateDropdownsAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrderFormViewModel model)
    {
        await ValidateOrderAsync(model);

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(model);
            return View(model);
        }

        var order = new VehicleOrder
        {
            OrderNumber = model.OrderNumber,
            VehicleId = model.VehicleId,
            DriverId = model.DriverId,
            OrganizationalUnitId = model.OrganizationalUnitId,
            IsBusinessTrip = model.IsBusinessTrip,
            DepartureAt = model.DepartureAt,
            ReturnAt = model.ReturnAt,
            MileageBefore = model.MileageBefore,
            Note = model.Note,
            CreatedAt = DateTime.Now,
            CreatedByUserId = User.CurrentUserId(),
            Status = OrderStatus.Aktivan
        };

        var vehicle = await context.Vehicles.FindAsync(model.VehicleId);
        if (vehicle is not null)
        {
            vehicle.Status = VehicleStatus.Zauzeto;
        }

        context.VehicleOrders.Add(order);
        await context.SaveChangesAsync();

        TempData["Success"] = "Nalog je uspjesno kreiran.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await context.VehicleOrders
            .AsNoTracking()
            .Include(x => x.Vehicle)
            .Include(x => x.Driver)
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
            IsBusinessTrip = order.IsBusinessTrip,
            DepartureAt = order.DepartureAt,
            ReturnAt = order.ReturnAt,
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
    public async Task<IActionResult> Complete(CompleteOrderViewModel model)
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
            TempData["Error"] = "Povratna kilometraza ne moze biti manja od pocetne.";
            return RedirectToAction(nameof(Details), new { id = model.OrderId });
        }

        order.MileageAfter = model.MileageAfter;
        order.Note = model.Note;
        order.Status = OrderStatus.Zavrsen;

        if (order.Vehicle is not null)
        {
            order.Vehicle.CurrentMileage = model.MileageAfter;
            order.Vehicle.Status = VehicleStatus.Slobodno;
        }

        await context.SaveChangesAsync();
        TempData["Success"] = "Nalog je zavrsen i vozilo je vraceno u evidenciju.";
        return RedirectToAction(nameof(Details), new { id = model.OrderId });
    }

    private async Task PopulateDropdownsAsync(OrderFormViewModel model)
    {
        model.Vehicles = await context.Vehicles
            .Where(x => x.IsActive)
            .OrderBy(x => x.RegistrationNumber)
            .Select(x => new SelectListItem($"{x.RegistrationNumber} / {x.BrandModel}", x.Id.ToString()))
            .ToListAsync();

        model.Drivers = await context.Users
            .Where(x => x.Status == UserStatus.Aktivan)
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(x => new SelectListItem(x.FullName, x.Id.ToString()))
            .ToListAsync();

        model.OrganizationalUnits = await context.OrganizationalUnits
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
            .ToListAsync();
    }

    private async Task ValidateOrderAsync(OrderFormViewModel model)
    {
        if (await context.VehicleOrders.AnyAsync(x => x.OrderNumber == model.OrderNumber))
        {
            ModelState.AddModelError(nameof(model.OrderNumber), "Broj naloga vec postoji.");
        }

        var vehicle = await context.Vehicles.FindAsync(model.VehicleId);
        if (vehicle is null || !vehicle.IsActive || vehicle.Status == VehicleStatus.VanUpotrebe)
        {
            ModelState.AddModelError(nameof(model.VehicleId), "Odabrano vozilo nije raspolozivo.");
            return;
        }

        if (model.MileageBefore < vehicle.CurrentMileage)
        {
            ModelState.AddModelError(nameof(model.MileageBefore), "Pocetna kilometraza ne moze biti manja od trenutne kilometraze vozila.");
        }

        var overlapExists = await context.VehicleOrders.AnyAsync(x =>
            x.VehicleId == model.VehicleId &&
            x.Status == OrderStatus.Aktivan &&
            !(model.ReturnAt <= x.DepartureAt || model.DepartureAt >= x.ReturnAt));

        if (overlapExists)
        {
            ModelState.AddModelError(nameof(model.VehicleId), "Odabrano vozilo je vec zauzeto u trazenom terminu.");
        }
    }
}
