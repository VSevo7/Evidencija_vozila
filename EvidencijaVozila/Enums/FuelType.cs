using System.ComponentModel.DataAnnotations;

namespace EvidencijaVozila.Enums;

public enum FuelType
{
    [Display(Name = "Benzin")]
    Benzin = 1,

    [Display(Name = "Dizel")]
    Dizel = 2,

    [Display(Name = "Električno")]
    Elektricno = 3,

    [Display(Name = "Hibrid")]
    Hibrid = 4
}
