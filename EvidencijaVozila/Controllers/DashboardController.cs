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
    public async Task<IActionResult> Index(string? registrationQuery)
    {
        var model = new DashboardViewModel
        {
            TotalVehicles = await context.Vehicles.CountAsync(),
            AvailableVehicles = await context.Vehicles.CountAsync(x => x.Status == VehicleStatus.Aktivno),
            ActiveOrders = await context.VehicleOrders.CountAsync(x => x.Status == OrderStatus.Aktivan),
            TotalUsers = await context.Users.CountAsync(),
            VehicleSearchTerm = InputNormalizer.NormalizeOptional(registrationQuery)
        };

        var normalizedRegistrationQuery = InputNormalizer.NormalizeRegistrationLookup(model.VehicleSearchTerm);
        if (string.IsNullOrWhiteSpace(normalizedRegistrationQuery))
        {
            return View(model);
        }

        var searchResults = await context.Vehicles
            .AsNoTracking()
            .Where(x => x.RegistrationNumber
                .Replace("-", string.Empty)
                .Replace(" ", string.Empty)
                .ToUpper() == normalizedRegistrationQuery)
            .OrderBy(x => x.RegistrationNumber)
            .Select(x => new
            {
                x.Id,
                x.RegistrationNumber,
                x.BrandModel,
                x.VehicleType,
                x.FuelType,
                x.TransmissionType,
                x.CurrentTires,
                x.CurrentMileage,
                x.Status,
                ActiveOrderNumber = context.VehicleOrders
                    .Where(o => o.VehicleId == x.Id && o.Status == OrderStatus.Aktivan)
                    .Select(o => o.OrderNumber)
                    .FirstOrDefault()
            })
            .ToListAsync();

        model.VehicleSearchResults = searchResults
            .Select(x => new DashboardVehicleSearchResultViewModel
            {
                RegistrationNumber = x.RegistrationNumber,
                BrandModel = x.BrandModel,
                Specification = $"{x.VehicleType.ToDisplay()} / {x.FuelType.ToDisplay()} / {x.TransmissionType.ToDisplay()} / {x.CurrentTires}",
                CurrentMileage = x.CurrentMileage,
                Status = !string.IsNullOrWhiteSpace(x.ActiveOrderNumber)
                    ? "Izdano"
                    : x.Status.ToDisplay(),
                ActiveOrderNumber = x.ActiveOrderNumber
            })
            .ToList();

        return View(model);
    }
}
