using System.ComponentModel.DataAnnotations;

namespace EvidencijaVozila.ViewModels.Account;

public class LoginViewModel
{
    [Required(ErrorMessage = "Unesite korisničko ime.")]
    [Display(Name = "Korisničko ime")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Unesite lozinku.")]
    [DataType(DataType.Password)]
    [Display(Name = "Lozinka")]
    public string Password { get; set; } = string.Empty;
}
