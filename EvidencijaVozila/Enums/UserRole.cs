using System.ComponentModel.DataAnnotations;

namespace EvidencijaVozila.Enums;

public enum UserRole
{
    [Display(Name = "Administrator")]
    Administrator = 1,

    [Display(Name = "Korisnik")]
    Korisnik = 2
}
