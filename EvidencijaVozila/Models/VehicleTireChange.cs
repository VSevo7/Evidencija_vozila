using System.ComponentModel.DataAnnotations;

namespace EvidencijaVozila.Models;

public class VehicleTireChange
{
    public int Id { get; set; }

    public int VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }

    [Display(Name = "Datum izmjene")]
    public DateTime ChangedAt { get; set; }

    [Required, StringLength(30)]
    [Display(Name = "Tip gume")]
    public string TireType { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    [Display(Name = "Stanje km")]
    public int MileageAtChange { get; set; }
}
