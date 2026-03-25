using EvidencijaVozila.Data;
using EvidencijaVozila.Enums;
using EvidencijaVozila.ViewModels.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvidencijaVozila.Controllers;

[Authorize]
public class DashboardController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var vehicles = await context.Vehicles.AsNoTracking().ToListAsync();
        var orders = await context.VehicleOrders
            .AsNoTracking()
            .Include(x => x.Vehicle)
            .Include(x => x.Driver)
            .OrderByDescending(x => x.CreatedAt)
            .Take(8)
            .ToListAsync();

        var model = new DashboardViewModel
        {
            TotalVehicles = vehicles.Count,
            AvailableVehicles = vehicles.Count(x => x.IsActive && x.Status == VehicleStatus.Slobodno),
            ActiveOrders = await context.VehicleOrders.CountAsync(x => x.Status == OrderStatus.Aktivan),
            TotalUsers = await context.Users.CountAsync(),
            ServiceAlerts = vehicles
                .Where(x => x.IsActive && x.ServiceIntervalKm > 0)
                .Select(x => new ServiceAlertViewModel
                {
                    RegistrationNumber = x.RegistrationNumber,
                    BrandModel = x.BrandModel,
                    CurrentMileage = x.CurrentMileage,
                    ServiceIntervalKm = x.ServiceIntervalKm,
                    KmUntilService = x.ServiceIntervalKm - (x.CurrentMileage % x.ServiceIntervalKm)
                })
                .OrderBy(x => x.KmUntilService)
                .Take(5)
                .ToList(),
            RecentOrders = orders.Select(x => new RecentOrderViewModel
            {
                Id = x.Id,
                OrderNumber = x.OrderNumber,
                Vehicle = $"{x.Vehicle!.RegistrationNumber} / {x.Vehicle.BrandModel}",
                Driver = x.Driver!.FullName,
                DepartureAt = x.DepartureAt,
                ReturnAt = x.ReturnAt,
                Status = x.Status.ToDisplay()
            }).ToList()
        };

        return View(model);
    }
}
