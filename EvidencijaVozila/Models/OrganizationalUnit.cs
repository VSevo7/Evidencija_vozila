using System.ComponentModel.DataAnnotations;

namespace EvidencijaVozila.Models;

public class OrganizationalUnit
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Sector> Sectors { get; set; } = new List<Sector>();
    public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
    public ICollection<VehicleOrder> Orders { get; set; } = new List<VehicleOrder>();
}
