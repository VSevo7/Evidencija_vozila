using EvidencijaVozila.Data;
using EvidencijaVozila.ViewModels.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvidencijaVozila.Controllers;

[Authorize(Roles = "Administrator")]
public class ReportsController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var vehicles = await context.Vehicles
            .AsNoTracking()
            .Include(v => v.Orders)
            .ToListAsync();

        var users = await context.Users
            .AsNoTracking()
            .Include(u => u.OrganizationalUnit)
            .Include(u => u.DrivenOrders)
            .ToListAsync();

        var units = await context.OrganizationalUnits
            .AsNoTracking()
            .Include(u => u.Orders)
            .ToListAsync();

        var model = new ReportsViewModel
        {
            UsageByVehicle = vehicles
                .Select(v => new VehicleUsageReportItem
                {
                    Vehicle = $"{v.RegistrationNumber} / {v.BrandModel}",
                    OrderCount = v.Orders.Count,
                    TotalKm = v.Orders.Sum(o => (o.MileageAfter ?? o.MileageBefore) - o.MileageBefore)
                })
                .OrderByDescending(x => x.OrderCount)
                .ToList(),
            UsageByDriver = users
                .Select(u => new DriverUsageReportItem
                {
                    Driver = u.FullName,
                    OrganizationalUnit = u.OrganizationalUnit?.Name ?? "-",
                    OrderCount = u.DrivenOrders.Count
                })
                .OrderByDescending(x => x.OrderCount)
                .ToList(),
            UsageByUnit = units
                .Select(u => new UnitUsageReportItem
                {
                    OrganizationalUnit = u.Name,
                    OrderCount = u.Orders.Count
                })
                .OrderByDescending(x => x.OrderCount)
                .ToList(),
            ServiceReport = vehicles
                .Where(v => v.IsActive && v.ServiceIntervalKm > 0)
                .Select(v => new ServiceReportItem
                {
                    Vehicle = $"{v.RegistrationNumber} / {v.BrandModel}",
                    CurrentMileage = v.CurrentMileage,
                    KmUntilService = v.ServiceIntervalKm - (v.CurrentMileage % v.ServiceIntervalKm)
                })
                .OrderBy(x => x.KmUntilService)
                .ToList()
        };

        return View(model);
    }
}
