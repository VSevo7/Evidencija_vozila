using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EvidencijaVozila.ViewModels.Orders;

public class OrderFormViewModel : IValidatableObject
{
    [Required]
    [Display(Name = "Broj naloga")]
    public string OrderNumber { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Vozilo")]
    public int VehicleId { get; set; }

    [Required]
    [Display(Name = "Vozac")]
    public int DriverId { get; set; }

    [Required]
    [Display(Name = "Organizacijska jedinica")]
    public int OrganizationalUnitId { get; set; }

    [Display(Name = "Sluzbeni put")]
    public bool IsBusinessTrip { get; set; }

    [Required]
    [Display(Name = "Datum i vrijeme polaska")]
    public DateTime DepartureAt { get; set; }

    [Required]
    [Display(Name = "Datum i vrijeme povratka")]
    public DateTime ReturnAt { get; set; }

    [Range(0, int.MaxValue)]
    [Display(Name = "Stanje km prije polaska")]
    public int MileageBefore { get; set; }

    [Display(Name = "Napomena")]
    public string? Note { get; set; }

    public IEnumerable<SelectListItem> Vehicles { get; set; } = [];
    public IEnumerable<SelectListItem> Drivers { get; set; } = [];
    public IEnumerable<SelectListItem> OrganizationalUnits { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ReturnAt <= DepartureAt)
        {
            yield return new ValidationResult("Vrijeme povratka mora biti nakon vremena polaska.", [nameof(ReturnAt)]);
        }
    }
}
