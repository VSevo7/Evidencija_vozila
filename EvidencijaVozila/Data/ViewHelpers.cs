using System.Security.Claims;
using EvidencijaVozila.Enums;

namespace EvidencijaVozila.Data;

public static class ViewHelpers
{
    public static string BadgeClass(string value) => value.ToLowerInvariant() switch
    {
        "aktivan" or "aktivno" or "administrator" or "voditelj sektora" or "voditelj službe" => "success",
        "izdano" or "zauzeto" => "warning",
        "neaktivan" or "rashod" => "danger",
        "završen" => "secondary",
        _ => "info"
    };

    public static string ToDisplay(this Enum value) => value switch
    {
        VehicleStatus.Aktivno => "Aktivno",
        VehicleStatus.Zauzeto => "Izdano",
        VehicleStatus.Rashod => "Rashod",
        TransmissionType.Rucni => "Ručni",
        TransmissionType.Automatski => "Automatski",
        FuelType.Benzin => "Benzin",
        FuelType.Dizel => "Dizel",
        FuelType.Elektricno => "Električno",
        FuelType.Hibrid => "Hibrid",
        VehicleType.Osobno => "Osobno",
        VehicleType.Teretno => "Teretno",
        UserRole.Administrator => "Administrator",
        UserRole.Korisnik => "Korisnik",
        UserStatus.Aktivan => "Aktivan",
        UserStatus.Neaktivan => "Neaktivan",
        OrderStatus.Aktivan => "Aktivan",
        OrderStatus.Zavrsen => "Završen",
        UserAssignmentType.Sektor => "Sektor",
        UserAssignmentType.Sluzba => "Služba",
        UserPosition.Zaposlenik => "Zaposlenik",
        UserPosition.VoditeljSektora => "Voditelj sektora",
        UserPosition.VoditeljSluzbe => "Voditelj službe",
        _ => value.ToString()
    };

    public static int CurrentUserId(this ClaimsPrincipal principal) =>
        int.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
}
