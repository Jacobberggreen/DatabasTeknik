using Microsoft.AspNetCore.Mvc;

namespace Databas.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
