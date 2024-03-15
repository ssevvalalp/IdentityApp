using IdentiyApp.Models;
using IdentiyApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentiyApp.Controllers
{
    public class AccountController:Controller
    {
        private UserManager<AppUser> _userManager; //<IdentityUser>
        private RoleManager<AppRole> _roleManager;
        private SignInManager<AppUser> _signInManager; //kullanıcı login oldugunda cookie bilgisini gönderir
        public AccountController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if(user != null)
                {
                    await _signInManager.SignOutAsync(); //kullanıcı daha önce giriş yaptıysa tarayıcısından cookie'yi sil

                    if(!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError("", "Hesabınızı onaylayınız");
                        return View(model);
                    }

                    //yeni cookie oluştur
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true); //PasswordSignInAsync özellikleri

                    if(result.Succeeded) //parola doğruysa lockout ayarlarını sıfırla 
                    {
                        await _userManager.ResetAccessFailedCountAsync(user);
                        await _userManager.SetLockoutEndDateAsync(user, null);

                        return RedirectToAction("Index", "Home"); //succeed oldugunda
                    }
                    else if (result.IsLockedOut) // 5 kere yanlıs girdi hesap 5 dk kilitlendi
                    {
                        var lockoutDate = await _userManager.GetLockoutEndDateAsync(user);
                        var timeleft = lockoutDate.Value - DateTime.UtcNow;
                        ModelState.AddModelError("", $"hesabınız kitlendi, {timeleft.Minutes} dakika sonra deneyiniz");
                    }
                    else  {
                        ModelState.AddModelError("", "hatalı parola");
                    }
                }
                
                else
                {
                    ModelState.AddModelError("", "bu email ile bir hesap bulunamadı");
                }
            }
            return View(model);
        }
    }

}
