using System.ComponentModel.DataAnnotations;

namespace EvidencijaVozila.Enums;

public enum UserPosition
{
    [Display(Name = "Zaposlenik")]
    Zaposlenik = 1,

    [Display(Name = "Voditelj sektora")]
    VoditeljSektora = 2,

    [Display(Name = "Voditelj službe")]
    VoditeljSluzbe = 3
}
