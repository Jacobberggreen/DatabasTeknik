using Microsoft.AspNetCore.Mvc;

namespace Databas.Controllers {
	
	// TestController for testing purposes
    public class TestController : Controller {
        public IActionResult Index(){
            return View();
        }
    }
}
