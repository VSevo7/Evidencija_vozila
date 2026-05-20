namespace EvidencijaVozila.ViewModels.Dashboard;

public class DashboardViewModel
{
    public int TotalVehicles { get; set; }
    public int AvailableVehicles { get; set; }
    public int ActiveOrders { get; set; }
    public int TotalUsers { get; set; }
    public string? VehicleSearchTerm { get; set; }
    public List<DashboardVehicleSearchResultViewModel> VehicleSearchResults { get; set; } = [];
    public bool SearchPerformed => !string.IsNullOrWhiteSpace(VehicleSearchTerm);
}

public class DashboardVehicleSearchResultViewModel
{
    public string RegistrationNumber { get; set; } = string.Empty;
    public string BrandModel { get; set; } = string.Empty;
    public string Specification { get; set; } = string.Empty;
    public int CurrentMileage { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ActiveOrderNumber { get; set; }
}
