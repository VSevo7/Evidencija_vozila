namespace EvidencijaVozila.ViewModels.Reports;

public class ReportsViewModel
{
    public List<VehicleUsageReportItem> UsageByVehicle { get; set; } = [];
    public List<DriverUsageReportItem> UsageByDriver { get; set; } = [];
    public List<UnitUsageReportItem> UsageByUnit { get; set; } = [];
    public List<ServiceReportItem> ServiceReport { get; set; } = [];
}

public class VehicleUsageReportItem
{
    public string Vehicle { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public int TotalKm { get; set; }
}

public class DriverUsageReportItem
{
    public string Driver { get; set; } = string.Empty;
    public string OrganizationalUnit { get; set; } = string.Empty;
    public int OrderCount { get; set; }
}

public class UnitUsageReportItem
{
    public string OrganizationalUnit { get; set; } = string.Empty;
    public int OrderCount { get; set; }
}

public class ServiceReportItem
{
    public string Vehicle { get; set; } = string.Empty;
    public int CurrentMileage { get; set; }
    public int KmUntilService { get; set; }
}
