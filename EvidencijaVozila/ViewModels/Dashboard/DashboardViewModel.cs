namespace EvidencijaVozila.ViewModels.Dashboard;

public class DashboardViewModel
{
    public int TotalVehicles { get; set; }
    public int AvailableVehicles { get; set; }
    public int ActiveOrders { get; set; }
    public int TotalUsers { get; set; }
    public List<ServiceAlertViewModel> ServiceAlerts { get; set; } = [];
    public List<RecentOrderViewModel> RecentOrders { get; set; } = [];
}

public class ServiceAlertViewModel
{
    public string RegistrationNumber { get; set; } = string.Empty;
    public string BrandModel { get; set; } = string.Empty;
    public int CurrentMileage { get; set; }
    public int ServiceIntervalKm { get; set; }
    public int KmUntilService { get; set; }
}

public class RecentOrderViewModel
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Vehicle { get; set; } = string.Empty;
    public string Driver { get; set; } = string.Empty;
    public DateTime DepartureAt { get; set; }
    public DateTime ReturnAt { get; set; }
    public string Status { get; set; } = string.Empty;
}
