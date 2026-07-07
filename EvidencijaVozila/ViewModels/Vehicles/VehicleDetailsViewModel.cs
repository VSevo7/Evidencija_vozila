namespace EvidencijaVozila.ViewModels.Vehicles;

public class VehicleDetailsViewModel
{
    public int Id { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string BrandModel { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;
    public string TransmissionType { get; set; } = string.Empty;
    public string CurrentTires { get; set; } = string.Empty;
    public int CurrentMileage { get; set; }
    public decimal PurchasePrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ActiveOrderNumber { get; set; }
    public IReadOnlyList<VehicleTireChangeDetailsViewModel> TireChanges { get; set; } = [];
}

public class VehicleTireChangeDetailsViewModel
{
    public DateTime ChangedAt { get; set; }
    public string TireType { get; set; } = string.Empty;
    public int MileageAtChange { get; set; }
}
