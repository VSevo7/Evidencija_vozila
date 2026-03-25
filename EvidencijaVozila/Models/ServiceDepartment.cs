using System.ComponentModel.DataAnnotations;

namespace EvidencijaVozila.Models;

public class ServiceDepartment
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Sektor")]
    public int SectorId { get; set; }
    public Sector? Sector { get; set; }

    public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
}
