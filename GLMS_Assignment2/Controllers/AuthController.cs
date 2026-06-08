using Microsoft.AspNetCore.Mvc;

namespace GLMS_Assignment2.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
