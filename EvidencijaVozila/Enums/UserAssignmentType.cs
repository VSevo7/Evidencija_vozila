using System.ComponentModel.DataAnnotations;

namespace EvidencijaVozila.Enums;

public enum UserAssignmentType
{
    [Display(Name = "Sektor")]
    Sektor = 1,

    [Display(Name = "Služba")]
    Sluzba = 2
}
