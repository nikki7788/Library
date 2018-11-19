using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Models;
using Library.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
                Email=u.Email
            }).ToList();

            return View(model);
        }
    }
}