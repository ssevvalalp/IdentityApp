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

                foreach (IdentityError err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                return View(new EditViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email

                });
            }
            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, EditViewModel model)

        {
            //modelden gelen id ile route'tan gelen id eşleşmeli
            if (id != model.Id)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                //valid ise güncellenecek user kaydını veri tabanından al

                var user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.FullName = model.FullName;

                    var result = await _userManager.UpdateAsync(user);

                    //if password is not empty -- kullanıcı parolası varsa
                    if (result.Succeeded && !string.IsNullOrEmpty(model.Password))
                    {
                        //kullanıcının parolasını sil

                        await _userManager.RemovePasswordAsync(user);
                        await _userManager.AddPasswordAsync(user, model.Password);
                    }

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }

                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {

                await _userManager.DeleteAsync(user);
            }

            return RedirectToAction("Index");
        }

    }
}
