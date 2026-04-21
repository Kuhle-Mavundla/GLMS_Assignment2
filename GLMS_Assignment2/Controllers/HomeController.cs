using Microsoft.AspNetCore.Mvc;
namespace GLMS_Assignment2.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}