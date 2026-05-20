using System.ComponentModel.DataAnnotations;

namespace EvidencijaVozila.Enums;

public enum TransmissionType
{
    [Display(Name = "Ručni")]
    Rucni = 1,

    [Display(Name = "Automatski")]
    Automatski = 2
}
