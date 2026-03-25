using System.ComponentModel.DataAnnotations;

namespace EvidencijaVozila.Models;

public class Sector
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Ustrojstvena jedinica")]
    public int OrganizationalUnitId { get; set; }
    public OrganizationalUnit? OrganizationalUnit { get; set; }

    public ICollection<ServiceDepartment> ServiceDepartments { get; set; } = new List<ServiceDepartment>();
    public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
}
