using Microsoft.AspNetCore.Mvc;

namespace MultiShop.WebUI.Controllers
{
    public class AccountController : Controller
    {
        // No login required - redirect to home
        public IActionResult Login() => RedirectToAction("Index", "Home");
        public IActionResult Logout() => RedirectToAction("Index", "Home");
    }
}