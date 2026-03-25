using System.ComponentModel.DataAnnotations;
using EvidencijaVozila.Enums;

namespace EvidencijaVozila.ViewModels.Availability;

public class AvailabilitySearchViewModel : IValidatableObject
{
    [Required]
    [Display(Name = "Datum i vrijeme polaska")]
    public DateTime? DepartureAt { get; set; }

    [Required]
    [Display(Name = "Datum i vrijeme povratka")]
    public DateTime? ReturnAt { get; set; }

    [Display(Name = "Vrsta vozila")]
    public VehicleType? VehicleType { get; set; }

    public List<AvailableVehicleViewModel> Results { get; set; } = [];
    public bool SearchPerformed { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DepartureAt.HasValue && ReturnAt.HasValue && ReturnAt <= DepartureAt)
        {
            yield return new ValidationResult("Vrijeme povratka mora biti nakon vremena polaska.", [nameof(ReturnAt)]);
        }
    }
}

public class AvailableVehicleViewModel
{
    public int Id { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string BrandModel { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;
    public string TransmissionType { get; set; } = string.Empty;
    public string CurrentTires { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
