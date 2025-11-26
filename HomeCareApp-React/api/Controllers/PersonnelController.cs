using Microsoft.AspNetCore.Mvc;

public class PersonnelController : Controller
{
    public IActionResult Index()
    {
        return View(); // Looks for Views/Personnel/Index.cshtml (Dashboard)
    }
}
