using System.ComponentModel.DataAnnotations;
using EvidencijaVozila.Enums;

namespace EvidencijaVozila.Models;

public class Vehicle
{
    public int Id { get; set; }

    [Required, StringLength(20)]
    [Display(Name = "Registracijska oznaka")]
    public string RegistrationNumber { get; set; } = string.Empty;

    [Required, StringLength(150)]
    [Display(Name = "Marka i tip")]
    public string BrandModel { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Vrsta vozila")]
    public VehicleType VehicleType { get; set; }

    [Required, StringLength(100)]
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

    public ICollection<VehicleOrder> Orders { get; set; } = new List<VehicleOrder>();
    public ICollection<VehicleTireChange> TireChanges { get; set; } = new List<VehicleTireChange>();
}
