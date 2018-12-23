using Library.Models;
using Library.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ApplicationRoleController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public ApplicationRoleController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var ur = _userManager.Users;

            List<ApplicationRoleViewModel> model = new List<ApplicationRoleViewModel>();
            model = _roleManager.Roles.Select(r => new ApplicationRoleViewModel
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                //to count number of users in each role------we must access to the users table
                // NumberOfUsers = _userManager.Users.Count()
                NumberOfUsers = ur.Count()
            }).ToList();



            return View(model);
        }
        //وقتی روی افزودن نقش جدید و ویرایش کلیک میکنیم مودال مربوط به رول را باز میکند
        [HttpGet]
        public async Task<IActionResult> AddEditRole(string id)
        {
            ApplicationRoleViewModel model = new ApplicationRoleViewModel();



            if (!String.IsNullOrEmpty(id))
            {
                ApplicationRole applicationRole = await _roleManager.FindByIdAsync(id);
                if (applicationRole != null)  //**
                {
                    model.Id = applicationRole.Id;
                    model.Name = applicationRole.Name;
                    model.Description = applicationRole.Description;
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            //نمیخواهد چون  else
            //(**) if برای  
            //ریترن ننوشتیم و اگر ان برقرار باشد می اید سراغ این خط کد
            return PartialView("_AddEditApplicationRolePartial", model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditRole(ApplicationRoleViewModel model, string id, string redirectUrl)
        {
            if (ModelState.IsValid)
            {
                //String is a class
                //string is a data type
                //String.IsNullOrEmpty(id) :if id is null or empty ,it will return true then because of " ! " will return true

                bool isExist = !String.IsNullOrEmpty(id);
                ApplicationRole applicationRole = isExist ? await _roleManager.FindByIdAsync(id) //updating
                    : new ApplicationRole(); //inserting

                applicationRole.Name = model.Name;
                applicationRole.Description = model.Description;

                //it will return result of the updating or inserting that it will be succeeded or not succeeded
                IdentityResult roleResult = isExist ? await _roleManager.UpdateAsync(applicationRole)  //updating in the database
                    : await _roleManager.CreateAsync(applicationRole);    //inserting in the database

                if (roleResult.Succeeded)
                {
                    //showing Index action and view
                    return PartialView("_SuccessfullyResponsePartial", redirectUrl);
                }
            }
            //showing addedit modal and it's values and errors
            return PartialView("_AddEditApplicationRolePartial", model);
        }
    }
}
