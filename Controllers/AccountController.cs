using Microsoft.AspNetCore.Mvc;

namespace IdentiyApp.Controllers
{
    public class AccountController:Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
