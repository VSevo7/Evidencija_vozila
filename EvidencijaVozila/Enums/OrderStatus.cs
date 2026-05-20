using System.ComponentModel.DataAnnotations;

namespace EvidencijaVozila.Enums;

public enum OrderStatus
{
    [Display(Name = "Aktivan")]
    Aktivan = 1,

    [Display(Name = "Završen")]
    Zavrsen = 2
}
