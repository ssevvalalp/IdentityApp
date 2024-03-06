using IdentiyApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentiyApp.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<AppRole> _roleManager;
        public RolesController(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;

        }

        public IActionResult Index()
        {
            return View(_roleManager.Roles); //Roles -> IQueryble
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task <IActionResult> Create(AppRole model)
        {
            if (ModelState.IsValid)
            {
                //var role = new AppRole { Name = model.Name };

                var result = await _roleManager.CreateAsync(model);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach(var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }
            return View(model);
        }

    }
}
