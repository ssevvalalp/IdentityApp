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
        private IEmailSender _emailSender;
        public AccountController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
           
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

                    if (!await _userManager.IsEmailConfirmedAsync(user))
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


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newUser = new AppUser { UserName = model.UserName, Email = model.Email, FullName = model.FullName };

                IdentityResult result = await _userManager.CreateAsync(newUser, model.Password);
                if (result.Succeeded)//true
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser); // ilgili kullanıcı için token bilgisi oluşturur
                    var url = Url.Action("ConfirmEmail","Account", new { newUser.Id ,token }); //{Id = Id , token = token} (action methodda bekleyen parametreler)
                                                                                               //Action       //Controller 
                                                                                               //email 
                    await _emailSender.SendEmailAsync(newUser.Email, "Hesap Onayı", $"Lütfen Email hesabınızdaki onay linkine <a href = 'http://localhost:5144{url}'>tıklayın</a>");

                    TempData["message"] = "Email hesabınızdaki onay mailine tıklayınız";
                    return RedirectToAction("Login","Account");
                }

                foreach (IdentityError err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }
            return View(model);
        }

        //urlye tıklandıgında tetiklenecek method

        public async Task<IActionResult>ConfirmEmail(string Id, string token)
        {
            if(Id == null || token == null)
            {
                TempData["message"] = "Geçersiz token bilgisi";
                return View();
            }

            var user = await _userManager.FindByIdAsync(Id);

            if(user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token); //dbdeki confirmed 0'dan 1'e cevrılır
                if (result.Succeeded)
                {
                    TempData["message"] = "hesabınız onaylandı";
                    return RedirectToAction("Login", "Account");
                }
            }
            TempData["message"] = "kullanıcı bulunamadı";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }

}
