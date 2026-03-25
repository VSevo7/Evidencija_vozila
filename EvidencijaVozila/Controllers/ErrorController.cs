using System.Diagnostics;
using EvidencijaVozila.Models;
using Microsoft.AspNetCore.Mvc;

namespace EvidencijaVozila.Controllers;

public class ErrorController : Controller
{
    [Route("Error")]
    public IActionResult Index()
    {
        return View("Error", new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
