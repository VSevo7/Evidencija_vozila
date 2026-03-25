using System.ComponentModel.DataAnnotations;
using EvidencijaVozila.Enums;

namespace EvidencijaVozila.Models;

public class AppUser
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }

    [Required]
    public UserStatus Status { get; set; }

    [Display(Name = "Ustrojstvena jedinica")]
    public int OrganizationalUnitId { get; set; }
    public OrganizationalUnit? OrganizationalUnit { get; set; }

    [Display(Name = "Sektor")]
    public int SectorId { get; set; }
    public Sector? Sector { get; set; }

    [Display(Name = "Sluzba")]
    public int ServiceDepartmentId { get; set; }
    public ServiceDepartment? ServiceDepartment { get; set; }

    public ICollection<VehicleOrder> DrivenOrders { get; set; } = new List<VehicleOrder>();
    public ICollection<VehicleOrder> CreatedOrders { get; set; } = new List<VehicleOrder>();

    public string FullName => $"{FirstName} {LastName}";
}
