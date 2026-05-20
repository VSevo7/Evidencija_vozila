using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EvidencijaVozila.ViewModels.Orders;

public class OrderFormViewModel
{
    [Required]
    [Display(Name = "Broj naloga")]
    public string OrderNumber { get; set; } = string.Empty;

    [Display(Name = "Ustrojstvena jedinica")]
    public int OrganizationalUnitId { get; set; }

    public string OrganizationalUnitName { get; set; } = "Ministarstvo unutarnjih poslova";

    [Required]
    [Display(Name = "Vozilo")]
    public int VehicleId { get; set; }

    [Required]
    [Display(Name = "Vozač")]
    public int DriverId { get; set; }

    [Display(Name = "Službeni put")]
    public bool IsBusinessTrip { get; set; }

    [Required]
    [Display(Name = "Datum i vrijeme polaska")]
    public DateTime DepartureAt { get; set; }

    [Range(0, int.MaxValue)]
    [Display(Name = "Početna kilometraža")]
    public int MileageBefore { get; set; }

    [Display(Name = "Napomena")]
    public string? Note { get; set; }

    public IEnumerable<SelectListItem> Drivers { get; set; } = [];
    public List<OrderVehicleOptionViewModel> VehicleOptions { get; set; } = [];
}

public class OrderVehicleOptionViewModel
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public int CurrentMileage { get; set; }
}
