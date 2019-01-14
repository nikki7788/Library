using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Models;
using Library.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Library.Area.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]          //باید دقیقا مانند نامی که در نقش ها دادیم باشد.به کوچک و بزرگ بودن حروف حساس است
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
                Name = u.FirstName + " " + u.LastName,     //نمایش نام و نام خانوادگی کاربر باهم
                Email = u.Email
            }).ToList();

            return View(model);
        }

        [HttpGet]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(UserViewModel model, string redirectUrl)
        {
            if (ModelState.IsValid)
            {

                //مقدار دهی یوزر از روی اطالاعات ورودی ب جز نقش و پسورد ان
                ApplicationUser applicationUser = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    UserName = model.UserName,
                    Email = model.Email,
                };

                //for saving password in the database
                //یوزر را به همراه پسورد ان که هش شد توسط خود ام وی سی در داخل دیتابیس ثبت و ذخیره میکند ونتیجه ان که موفقیت امیز است یا خیر را در متغیر
                //ذخیره میکند userResult 
                //نوع از داده است که نتیجه را که یا موفقیت اکیز است یا با شکست همراه است را ذخیره میکند identityResult
                IdentityResult result = await _userManager.CreateAsync(applicationUser, model.Password);

                //اگر عمل ثبت موفیت امیز بود
                if (result.Succeeded)
                {
                    //for saving User's role
                    //باید نقش و رل یوزر که در صفحه مودال توسط کاربر وارد شده است را در جدول رل ها(نقشها) پیداکنیم سپس ان را داخل متغیری از نوع 
                    //نوع رل و نقش بریزیم ApplicationRole applicationRole
                    ApplicationRole applicationRole = await _roleManager.FindByIdAsync(model.ApplicationRoleId);

                    //اگر این رل موجود بود در جدول رل ها
                    if (applicationRole != null)
                    {
                        //this item is saved in the AspNetUserRoles table in iIdentity
                        //آیدی یوزر مورد نظر به همراه آیدی نقش و رل ان را در جدولی به
                        //ثبت و ذخیره میکنیم و نتیجه را که مفوفقیت امیز است یا خیر را داخل متغیری دیگری از نوع  ASPNetUserRoles
                        // میریزیمIdentityResult roleResult
                        IdentityResult roleResult = await _userManager.AddToRoleAsync(applicationUser, applicationRole.Name);

                        //اگر موفقیت امیز بود
                        if (roleResult.Succeeded)
                        {
                            //میرود به صفحه ایندکس ولیست کاربران را نمایش میدهد
                            return PartialView("_SuccessfullyResponsePartial", redirectUrl);

                        }
                    }
                }

            }
            //اگر خطایی در بررسی و تایید اطالاعات ورودی رخ دهد 
            //نباشد modelstate.isvalid 

            //باید دوباره بعد تایید نشدن اطالاعات نقش ها رو از جدول ر ل ها بخوانیم و در کمبو باکس نمایش دهیم
            model.ApplicationRoles = _roleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id,
            }).ToList();

            //اگر خطایی در بررسی و تایید اطالاعات ورودی رخ دهد صفحه مودال به همراه اطالاعات و خطاهای ان نمایش داده میشود
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
            //String is a class & string is a data type
            ///////////////  اطلاعات کاربر انتخاب شده را اوردیم از دیتابیس ///////////////
            if (!String.IsNullOrEmpty(id))
            {
                ApplicationUser user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    model.FirstName = user.FirstName;
                    model.LastName = user.LastName;
                    model.Email = user.Email;
                    //برای نمایش نقش و رل کاربر انتخاب شده برای ویرایش در کمبو باکس
                    model.ApplicationReleId = _roleManager.Roles.Single(r => r.Name == _userManager.GetRolesAsync(user).Result.Single()).Id;
                }
            }
            return PartialView("_EditUserPartial", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string id, EditUserViewModel model, string redirectUrl)
        {

            if (ModelState.IsValid)
            {
                if (!String.IsNullOrEmpty(id))
                {
                    ApplicationUser applicationUser = await _userManager.FindByIdAsync(id);
                    if (applicationUser != null)
                    {
                        //
                        applicationUser.FirstName = model.FirstName;
                        applicationUser.LastName = model.LastName;
                        applicationUser.Email = model.Email;

                        //
                        string isExistingRole = _userManager.GetRolesAsync(applicationUser).Result.Single();

                        //
                        string isExistingRoleId = _roleManager.Roles.Single(r => r.Name == isExistingRole).Id;

                        IdentityResult result = await _userManager.UpdateAsync(applicationUser);

                        if (result.Succeeded)
                        {
                            //اگر نقش تغییر کرده بود
                            //بررسی میکند نقش و رل وارد شده در مودال ویرایش با رل و نقش ثبت شده یوزر در دیتابیس  یکسان است یا خیر 
                            if (model.ApplicationReleId != isExistingRoleId)
                            {
                                //اگر نقش تغییر کرده بود
                                //removing user's role from AspNetUserRoles
                                //RemoveFromRoleAsync returns   IdentityResult datatype
                                IdentityResult roleResult = await _userManager.RemoveFromRoleAsync(applicationUser, isExistingRole);

                                if (roleResult.Succeeded)
                                {
                                    ApplicationRole applicationRole = await _roleManager.FindByIdAsync(model.ApplicationReleId);

                                    if (applicationRole != null)
                                    {
                                        //adding a updating new role ,that is edited in the modal view , on the AspNetUserRoles tables in the database 
                                        IdentityResult newRole = await _userManager.AddToRoleAsync(applicationUser, applicationRole.Name);

                                        if (newRole.Succeeded)
                                        {
                                            return PartialView("_SuccessfullyResponsePartial", redirectUrl);

                                        }
                                    }

                                }

                            }
                            //اگرنقش تغییر نکرده بود
                            return PartialView("_SuccessfullyResponsePartial", redirectUrl);
                        }
                    }

                }
            }
            model.ApplicationRoles = _roleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id
            }).ToList();
            return PartialView("_EditUserPartial", model);

        }

        //-----------****** Delete Get ******----------------------
        [HttpGet]
        public async Task<IActionResult> DeleteUser(string id)
        {
            //string name ="";
            // string name = string.Empty;
            if (!String.IsNullOrEmpty(id))
            {
                ApplicationUser appUser = await _userManager.FindByIdAsync(id);
                if (appUser != null)
                {
                    //  name= appUser.FirstName + " " + appUser.LastName
                    return PartialView("_DeleteUserPartial", appUser.FirstName + " " + appUser.LastName);
                }
            }
            return RedirectToAction("Index");
        }

        //-----------****** Delete Post ******----------------------
        [HttpPost, ActionName("DeleteUser")]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                ApplicationUser appUser = await _userManager.FindByIdAsync(id);
                if (appUser != null)
                {
                    IdentityResult userResult = await _userManager.DeleteAsync(appUser);
                    if (userResult.Succeeded)
                    {
                        return RedirectToAction("index");
                    }
                }
            }
            return View();
            //return NotFound();
        }


    }
}