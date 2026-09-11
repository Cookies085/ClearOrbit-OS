using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}