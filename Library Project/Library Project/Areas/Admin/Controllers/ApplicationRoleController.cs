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

        #region-##- we must access to the ASpNetUserRoles table is sql and dp.injection ######### 
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _iServiceProvider;
        #endregion

        public ApplicationRoleController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext context, IServiceProvider iServiceProvider)
        {
            _userManager = userManager;
            _roleManager = roleManager;

            //to count number of users in each role------we must access to the ASpNetUserRoles table in sql 
            _context = context;
            _iServiceProvider = iServiceProvider;
        }


        //---------------------------*************    index action    *************---------------------------


        public IActionResult Index()
        {
            //to count number of users in each role------we must access to the ASpNetUserRoles table in sql
            //  List<IdentityUserRole<string>> listUserRoles = _context.UserRoles.ToList();

            List<ApplicationRoleViewModel> model = new List<ApplicationRoleViewModel>();
            model = _roleManager.Roles.Select(r => new ApplicationRoleViewModel
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                //to count number of users in each role------we must access to the ASpNetUserRoles table in sql
                NumberOfUsers = _context.UserRoles.Count(ur => ur.RoleId == r.Id)
            }).ToList();
            return View(model);
        }



        //---------------------------*************    Add Edit Get     *************---------------------------

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


        //---------------------------*************    add Edit Post     *************---------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditRole(ApplicationRoleViewModel model, string id, string redirectUrl)
        {
            if (ModelState.IsValid)
            {
                //String is a class
                //string is a data type
                //String.IsNullOrEmpty(id) :if id is null or empty ,it will return true then because of " ! " will return true
                //روش سی شارپ جدید با شرط معمولی هم میتوان نوشت
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

        //---------------------------*************    Delete Get     *************---------------------------

        [HttpGet]
        public async Task<IActionResult> DeleteRole(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                ////string appRole ="";
                //string appRole = string.Empty;

                ApplicationRole role = await _roleManager.FindByIdAsync(id);
                if (role != null)
                {

                    return PartialView("_DeleteRolePartial", role.Name);
                }
            }

            return RedirectToAction("Index");
        }

        //---------------------------*************    Delete Post     *************---------------------------

        [HttpPost, ActionName("DeleteRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                ApplicationRole appRole = await _roleManager.FindByIdAsync(id);
                if (appRole != null)
                {
                    IdentityResult roleResult =await _roleManager.DeleteAsync(appRole);
                    if (roleResult.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }

            }

            // return View();
            return NotFound();
        }




    }

}
