using System.ComponentModel.DataAnnotations;

namespace EvidencijaVozila.Enums;

public enum VehicleType
{
    [Display(Name = "Osobno")]
    Osobno = 1,

    [Display(Name = "Teretno")]
    Teretno = 2
}
