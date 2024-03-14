﻿using IdentiyApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentiyApp.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        public RolesController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
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

        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role != null && role.Name != null)
            {
                ViewBag.Users = await _userManager.GetUsersInRoleAsync(role.Name);
                    return View(role);
  
            }
            return RedirectToAction("Index");

        }

    }
}
