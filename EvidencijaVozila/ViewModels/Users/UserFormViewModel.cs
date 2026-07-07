using System.ComponentModel.DataAnnotations;
using EvidencijaVozila.Enums;

namespace EvidencijaVozila.ViewModels.Users;

public class UserFormViewModel : IValidatableObject
{
    public int? Id { get; set; }

    public int OrganizationalUnitId { get; set; }
    public string OrganizationalUnitName { get; set; } = "Ministarstvo unutarnjih poslova";

    [Required(ErrorMessage = "Ime je obavezno.")]
    [Display(Name = "Ime")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Prezime je obavezno.")]
    [Display(Name = "Prezime")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Korisničko ime je obavezno.")]
    [Display(Name = "Korisničko ime")]
    public string Username { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email nije ispravan.")]
    [Required(ErrorMessage = "Email je obavezan.")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Kontakt")]
    [StringLength(30, ErrorMessage = "Kontakt može imati najviše 30 znakova.")]
    public string? ContactPhone { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Lozinka")]
    public string? Password { get; set; }

    [Required]
    [Display(Name = "Uloga")]
    public UserRole Role { get; set; }

    [Required]
    [Display(Name = "Status")]
    public UserStatus Status { get; set; }

    public int? SectorId { get; set; }
    public int? ServiceDepartmentId { get; set; }

    [Display(Name = "Naziv sektora")]
    public string? SectorName { get; set; }

    [Display(Name = "Sektor službe")]
    public string? ServiceSectorName { get; set; }

    [Display(Name = "Naziv službe")]
    public string? ServiceDepartmentName { get; set; }

    [Required]
    [Display(Name = "Pripadnost")]
    public UserAssignmentType AssignmentType { get; set; }

    [Required]
    [Display(Name = "Funkcija")]
    public UserPosition Position { get; set; } = UserPosition.Zaposlenik;

    public IEnumerable<string> SectorSuggestions { get; set; } = [];
    public IEnumerable<string> ServiceDepartmentSuggestions { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (AssignmentType == UserAssignmentType.Sektor && string.IsNullOrWhiteSpace(SectorName))
        {
            yield return new ValidationResult("Unesite naziv sektora.", [nameof(SectorName)]);
        }

        if (AssignmentType == UserAssignmentType.Sluzba && string.IsNullOrWhiteSpace(ServiceSectorName))
        {
            yield return new ValidationResult("Unesite sektor kojem služba pripada.", [nameof(ServiceSectorName)]);
        }

        if (AssignmentType == UserAssignmentType.Sluzba && string.IsNullOrWhiteSpace(ServiceDepartmentName))
        {
            yield return new ValidationResult("Unesite naziv službe.", [nameof(ServiceDepartmentName)]);
        }

        if (AssignmentType == UserAssignmentType.Sektor && Position == UserPosition.VoditeljSluzbe)
        {
            yield return new ValidationResult("Za pripadnost sektoru nije moguće odabrati funkciju voditelja službe.", [nameof(Position)]);
        }

        if (AssignmentType == UserAssignmentType.Sluzba && Position == UserPosition.VoditeljSektora)
        {
            yield return new ValidationResult("Za pripadnost službi nije moguće odabrati funkciju voditelja sektora.", [nameof(Position)]);
        }
    }
}