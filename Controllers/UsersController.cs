using IdentiyApp.Models;
using IdentiyApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentiyApp.Controllers
{
    public class UsersController : Controller
    {

        //User List 
        //UserManager'a Class implementation

        private UserManager<AppUser> _userManager; //<IdentityUser>
        public UsersController(UserManager<AppUser> userManager)
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
                var newUser = new AppUser { UserName = model.Email, Email = model.Email, FullName = model.FullName };
               
               IdentityResult result = await _userManager.CreateAsync(newUser, model.Password);
                if (result.Succeeded)//true
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

        public async Task <IActionResult> Edit(string id)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
            }
            var user = await _userManager.FindByIdAsync(id);

            if (user != null) {
                return View(new EditViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email

                });
            }
            return RedirectToAction("Index");

        }
     }
}
