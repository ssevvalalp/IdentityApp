using IdentiyApp.Models;
using IdentiyApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//admin işlemleri
namespace IdentiyApp.Controllers
{
    [Authorize(Roles = "admin")]
    public class UsersController : Controller 
    {

        //User List 
        //UserManager'a Class implementation

        private UserManager<AppUser> _userManager; //<IdentityUser>
        private RoleManager<AppRole> _roleManager;
        public UsersController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [AllowAnonymous] //bir kimlik olmadan
        public IActionResult Index()
        {
            //if (!User.IsInRole("admin"))
            //{
            //    return RedirectToAction("Login","Account");
            //}
            return View(_userManager.Users);
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
                ViewBag.Roles = await _roleManager.Roles.Select(i => i.Name).ToListAsync(); //seçilebilecek roller
                return View(new EditViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    SelectedRoles = await _userManager.GetRolesAsync(user) //daha önceden seçilmiş roller

                });
            }
            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, EditViewModel model) //string id -> route üzerinden geliyor

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
                        //kullanıcıdan, kullanıcının rollerini sil
                        await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
                        if(model.SelectedRoles != null)
                        {
                            await _userManager.AddToRolesAsync(user, model.SelectedRoles);

                        }
                        
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
