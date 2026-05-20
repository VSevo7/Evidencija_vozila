using System.ComponentModel.DataAnnotations;

namespace EvidencijaVozila.Enums;

public enum VehicleStatus
{
    [Display(Name = "Aktivno")]
    Aktivno = 1,

    [Display(Name = "Izdano")]
    Zauzeto = 2,

    [Display(Name = "Rashod")]
    Rashod = 3
}
