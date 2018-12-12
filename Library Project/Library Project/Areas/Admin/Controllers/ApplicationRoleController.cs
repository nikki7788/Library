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
        public async Task <IActionResult> AddEditRole(string Id)
        {
            ApplicationRoleViewModel model = new ApplicationRoleViewModel();

            ApplicationRole applicationRole =await _roleManager.FindByIdAsync(Id);

            if (!string.IsNullOrEmpty(Id))
            {
                model.Id = applicationRole.Id;
                model.Name = applicationRole.Name;
                model.Description = applicationRole.Description;
            }

            return PartialView("AddEditApplicationRolePartial",model);
        }

    }
}
