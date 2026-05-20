using System.ComponentModel.DataAnnotations;

namespace EvidencijaVozila.Enums;

public enum UserStatus
{
    [Display(Name = "Aktivan")]
    Aktivan = 1,

    [Display(Name = "Neaktivan")]
    Neaktivan = 2
}
