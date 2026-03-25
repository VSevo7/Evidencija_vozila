using System.ComponentModel.DataAnnotations;
using EvidencijaVozila.Enums;

namespace EvidencijaVozila.Models;

public class VehicleOrder
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    [Display(Name = "Broj naloga")]
    public string OrderNumber { get; set; } = string.Empty;

    [Display(Name = "Vozilo")]
    public int VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }

    [Display(Name = "Vozac")]
    public int DriverId { get; set; }
    public AppUser? Driver { get; set; }

    [Display(Name = "Organizacijska jedinica")]
    public int OrganizationalUnitId { get; set; }
    public OrganizationalUnit? OrganizationalUnit { get; set; }

    [Display(Name = "Sluzbeni put")]
    public bool IsBusinessTrip { get; set; }

    [Display(Name = "Datum i vrijeme polaska")]
    public DateTime DepartureAt { get; set; }

    [Display(Name = "Datum i vrijeme povratka")]
    public DateTime ReturnAt { get; set; }

    [Range(0, int.MaxValue)]
    [Display(Name = "Stanje km prije")]
    public int MileageBefore { get; set; }

    [Display(Name = "Stanje km poslije")]
    public int? MileageAfter { get; set; }

    [StringLength(1000)]
    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }
    public AppUser? CreatedByUser { get; set; }

    public OrderStatus Status { get; set; }
}
