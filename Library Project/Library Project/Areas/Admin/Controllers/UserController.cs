using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Models;
using Library.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Library.Area.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        //Dependency injection for identity role and identity user

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public UserController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            //creating a list of view model
            List<UserListViewModel> model = new List<UserListViewModel>();

            //at first we go to the  database(_userManager)-Users' Table and then selcting properties of Users' table and
            //converting them to the view model type(u => new UserListViewModel) after that put them in properties of vie model(e.g.: Id = u.Id)
            //u => refer to Users table
            model = _userManager.Users.Select(u => new UserListViewModel
            {
                Id = u.Id,
                Name = u.FirstName,
                Email = u.Email
            }).ToList();

            return View(model);
        }

        public IActionResult AddUser()
        {
            UserViewModel model = new UserViewModel();
            model.ApplicationRoles = _roleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id,
            }).ToList();

            return PartialView("_AddUserPartial", model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            //UserViewModel model = new UserViewModel
            //{
            //    ApplicationRoles = _roleManager.Roles.Select(r => new SelectListItem
            //    {
            //        Text = r.Name,
            //        Value = r.Id,
            //    }).ToList()
            //};
            //هردو روش یکی هستنداین روش و روش بالایی 

            //اطالاعات کمبوباکس رااز دیتا بیس اوردیم
            EditUserViewModel model = new EditUserViewModel();
            model.ApplicationRoles = _roleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id,
            }).ToList();

            ///////////////  اطلاعات کاربر انتخاب شده را اوردیم از دیتابیس ///////////////
            if (string.IsNullOrEmpty(id))
            {
                ApplicationUser user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    model.FullName = user.FirstName + " " + user.LastName;
                    model.Email = user.Email;
                    //برای نمایش نقش و رل کاربر در کمبو باکس
                    model.ApplicationReleId = _roleManager.Roles.Single(r => r.Name == _userManager.GetRolesAsync(user).Result.Single()).Id;
                }
            }
            return PartialView("_EditUserPartial", model);
        }
    }
}