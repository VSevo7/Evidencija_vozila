using System.ComponentModel.DataAnnotations;
using EvidencijaVozila.Enums;

namespace EvidencijaVozila.ViewModels.Vehicles;

public class VehicleFormViewModel
{
    public int? Id { get; set; }

    [Required]
    [Display(Name = "Registracijska oznaka")]
    public string RegistrationNumber { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Marka i tip")]
    public string BrandModel { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Vrsta vozila")]
    public VehicleType VehicleType { get; set; }

    [Required]
    [Display(Name = "Trenutne gume")]
    public string CurrentTires { get; set; } = string.Empty;

    [Range(0, 999999999)]
    [Display(Name = "Nabavna cijena")]
    public decimal PurchasePrice { get; set; }

    [Required]
    [Display(Name = "Vrsta goriva")]
    public FuelType FuelType { get; set; }

    [Required]
    [Display(Name = "Vrsta mjenjača")]
    public TransmissionType TransmissionType { get; set; }

    [Range(0, int.MaxValue)]
    [Display(Name = "Trenutno stanje km")]
    public int CurrentMileage { get; set; }

    [Required]
    [Display(Name = "Status vozila")]
    public VehicleStatus Status { get; set; }

    public List<VehicleTireChangeFormItemViewModel> TireChanges { get; set; } = [];
}

public class VehicleTireChangeFormItemViewModel : IValidatableObject
{
    [Display(Name = "Datum izmjene")]
    [DataType(DataType.Date)]
    public DateTime? ChangedAt { get; set; }

    [Display(Name = "Tip gume")]
    public string? TireType { get; set; }

    [Display(Name = "Stanje km")]
    [Range(0, int.MaxValue, ErrorMessage = "Stanje km mora biti 0 ili veće.")]
    public int? MileageAtChange { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var hasAnyValue = ChangedAt.HasValue
            || !string.IsNullOrWhiteSpace(TireType)
            || MileageAtChange.HasValue;

        if (!hasAnyValue)
        {
            yield break;
        }

        if (!ChangedAt.HasValue)
        {
            yield return new ValidationResult("Unesite datum izmjene guma.", [nameof(ChangedAt)]);
        }

        if (string.IsNullOrWhiteSpace(TireType))
        {
            yield return new ValidationResult("Odaberite tip gume.", [nameof(TireType)]);
        }

        if (!MileageAtChange.HasValue)
        {
            yield return new ValidationResult("Unesite stanje km pri izmjeni guma.", [nameof(MileageAtChange)]);
        }
    }
}
