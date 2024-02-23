using IdentiyApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentiyApp.Controllers
{
    public class UsersController : Controller
    {

        //User List 
        //UserManager Class implementation

        private UserManager<IdentityUser> _userManager;
        public UsersController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View(_userManager.Users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newUser = new IdentityUser { UserName = model.UserName, Email = model.Email };
               
               IdentityResult result = await _userManager.CreateAsync(newUser, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach(IdentityError err in result.Errors)
                {
                    ModelState.AddModelError("",err.Description);
                }
            }
            return View(model);
        }
    }
}
