using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeBooks.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize]
public class HomeController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Denied()
    {
        return View();
    }
}