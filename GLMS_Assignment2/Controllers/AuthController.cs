
using GLMS_Assignment2.Services;
using Microsoft.AspNetCore.Mvc;
namespace GLMS_Assignment2.Controllers;

public class AuthController : Controller
{
    private readonly ApiService _api;
    public AuthController(ApiService api) => _api = api;

    [HttpGet] public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var token = await _api.LoginAsync(username, password);
        if (token == null)
        {
            ViewBag.Error = "Invalid credentials";
            return View();
        }
        HttpContext.Session.SetString("JwtToken", token);
        return RedirectToAction("Index", "Contracts");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}