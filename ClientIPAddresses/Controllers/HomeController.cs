using Microsoft.AspNetCore.Mvc;

namespace ClientIPAddresses.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
