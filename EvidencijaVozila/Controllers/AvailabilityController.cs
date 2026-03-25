using EvidencijaVozila.Data;
using EvidencijaVozila.Enums;
using EvidencijaVozila.ViewModels.Availability;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvidencijaVozila.Controllers;

[Authorize]
public class AvailabilityController(ApplicationDbContext context) : Controller
{
    public IActionResult Index()
    {
        return View(new AvailabilitySearchViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Search(AvailabilitySearchViewModel model)
    {
        model.SearchPerformed = true;

        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var departure = model.DepartureAt!.Value;
        var ret = model.ReturnAt!.Value;

        var query = context.Vehicles
            .AsNoTracking()
            .Where(v => v.IsActive && v.Status != VehicleStatus.VanUpotrebe);

        if (model.VehicleType.HasValue)
        {
            query = query.Where(v => v.VehicleType == model.VehicleType.Value);
        }

        var results = await query
            .Where(v => !context.VehicleOrders.Any(o =>
                o.VehicleId == v.Id &&
                o.Status == OrderStatus.Aktivan &&
                !(ret <= o.DepartureAt || departure >= o.ReturnAt)))
            .OrderBy(v => v.RegistrationNumber)
            .Select(v => new AvailableVehicleViewModel
            {
                Id = v.Id,
                RegistrationNumber = v.RegistrationNumber,
                BrandModel = v.BrandModel,
                VehicleType = v.VehicleType.ToDisplay(),
                FuelType = v.FuelType.ToDisplay(),
                TransmissionType = v.TransmissionType.ToDisplay(),
                CurrentTires = v.CurrentTires,
                Status = v.Status.ToDisplay()
            })
            .ToListAsync();

        model.Results = results;
        return View("Index", model);
    }
}
