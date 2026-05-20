namespace EvidencijaVozila.ViewModels.Vehicles;

public class VehicleIndexItemViewModel
{
    public int Id { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string BrandModel { get; set; } = string.Empty;
    public int CurrentMileage { get; set; }
    public string Status { get; set; } = string.Empty;
}
