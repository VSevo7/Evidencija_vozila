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

    [Display(Name = "Datum izmjene guma")]
    public string? TireChangeNote { get; set; }

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
}
