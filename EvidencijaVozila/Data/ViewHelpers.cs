using System.Security.Claims;
using EvidencijaVozila.Enums;

namespace EvidencijaVozila.Data;

public static class ViewHelpers
{
    public static string BadgeClass(string value) => value.ToLowerInvariant() switch
    {
        "aktivan" or "administrator" or "slobodno" => "success",
        "zauzeto" => "warning",
        "neaktivan" or "vanupotrebe" or "van upotrebe" => "danger",
        "zavrsen" => "secondary",
        _ => "info"
    };

    public static string ToDisplay(this Enum value) =>
        value.ToString()
             .Replace("VanUpotrebe", "Van upotrebe")
             .Replace("Rucni", "Rucni")
             .Replace("Automatski", "Automatski");

    public static int CurrentUserId(this ClaimsPrincipal principal) =>
        int.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
}
