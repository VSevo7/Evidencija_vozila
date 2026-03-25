using System.ComponentModel.DataAnnotations;

namespace EvidencijaVozila.ViewModels.Orders;

public class CompleteOrderViewModel
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    [Display(Name = "Stanje km po povratku")]
    public int MileageAfter { get; set; }

    [Display(Name = "Napomena")]
    public string? Note { get; set; }
}
