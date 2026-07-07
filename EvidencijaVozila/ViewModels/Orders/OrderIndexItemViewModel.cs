namespace EvidencijaVozila.ViewModels.Orders;

public class OrderIndexItemViewModel
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Vehicle { get; set; } = string.Empty;
    public string Driver { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
    public string ServiceDepartment { get; set; } = string.Empty;
    public DateTime DepartureAt { get; set; }
    public DateTime ReturnAt { get; set; }
    public bool IsCompleted { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class OrderDetailsViewModel
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Vehicle { get; set; } = string.Empty;
    public string Driver { get; set; } = string.Empty;
    public string OrganizationalUnit { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
    public string ServiceDepartment { get; set; } = string.Empty;
    public bool IsBusinessTrip { get; set; }
    public DateTime DepartureAt { get; set; }
    public DateTime ReturnAt { get; set; }
    public bool IsCompleted { get; set; }
    public int MileageBefore { get; set; }
    public int? MileageAfter { get; set; }
    public int? TravelledMileage => MileageAfter.HasValue ? MileageAfter.Value - MileageBefore : null;
    public string? Note { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string VehicleSpecs { get; set; } = string.Empty;
    public CompleteOrderViewModel CompleteOrder { get; set; } = new();
}
