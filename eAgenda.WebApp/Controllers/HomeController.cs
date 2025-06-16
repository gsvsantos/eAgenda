using Microsoft.AspNetCore.Mvc;

namespace eAgenda.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
