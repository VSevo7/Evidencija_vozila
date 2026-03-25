using System.ComponentModel.DataAnnotations;
using EvidencijaVozila.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EvidencijaVozila.ViewModels.Users;

public class UserFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Ime je obavezno.")]
    [Display(Name = "Ime")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Prezime je obavezno.")]
    [Display(Name = "Prezime")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Korisnicko ime je obavezno.")]
    [Display(Name = "Korisnicko ime")]
    public string Username { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email nije ispravan.")]
    [Required(ErrorMessage = "Email je obavezan.")]
    public string Email { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Lozinka")]
    public string? Password { get; set; }

    [Required]
    [Display(Name = "Uloga")]
    public UserRole Role { get; set; }

    [Required]
    [Display(Name = "Status")]
    public UserStatus Status { get; set; }

    [Required]
    [Display(Name = "Ustrojstvena jedinica")]
    public int OrganizationalUnitId { get; set; }

    [Required]
    [Display(Name = "Sektor")]
    public int SectorId { get; set; }

    [Required]
    [Display(Name = "Sluzba")]
    public int ServiceDepartmentId { get; set; }

    public IEnumerable<SelectListItem> OrganizationalUnits { get; set; } = [];
    public IEnumerable<SelectListItem> Sectors { get; set; } = [];
    public IEnumerable<SelectListItem> ServiceDepartments { get; set; } = [];
}
