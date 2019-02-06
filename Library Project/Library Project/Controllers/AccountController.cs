using Library.Models;
using Library.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Controllers
{
    public class AccountController : Controller
    {
        //برای لاگین از این کلاس استفاده می شود
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }


        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {

            ViewData["returnUrl"] = returnUrl;
            // ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            // ViewBag.returnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // بدست آوردن نقش کاربر از طریق دیتابیس برای تشخیص نقش کاربر که یوزر است یا ادمین
                    // ۱ سپس ارسال ان به اکشن 
                    var user = await _userManager.FindByNameAsync(model.UserName);
                    string userRole = _userManager.GetRolesAsync(user).Result.Single();

                    //۱
                    //در پایین تعریف شده است RedirectToLocal
                    return RedirectToLocal(returnUrl, userRole);
                }
                else
                {
                    //وقتی نام کاربری یا رمز عبور صحیح نباشد
                    ModelState.AddModelError("Password", "نام کاربری یا رکز عبور اشتباه است");
                    return View(model);
                }
            }

            //وقتی نام کاربری یا رمز عبور خالی باشد
            return View(model);
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }






        private IActionResult RedirectToLocal(string returnUrl, string roleName)
        {
            if (Url.IsLocalUrl(returnUrl))        //اگر یک مسیر از قبل وجود داشت
            {

                return Redirect(returnUrl);
            }
            else           //اگر مسیر اشتباه بود یا کاربر فقط می خواهد لاگین کند
            {

                if (roleName == "User")
                {
                    //کابر به عموان یوزر لاگین کرد
                    return Redirect("/User/UserProfile");
                }
                else if (roleName == "Admin")
                {
                    //کاربر به عنوان ادمین لاگین کرد
                    return Redirect("/Admin/User");       //اگر وی دکمه ورود کلیک کنیم یعنی مسیری همراه خود ندارد و میرود به یوزر
                    

                    //return RedirectToAction(nameof(HomeController.Index), "Home");
                }

                //هیچوقت اجرا نمیشود چون نقش هایی که دادیم ما از این دوحالت خارج نیستند
                //برای اینکه اکشن خطا ندهد میدهیم
                return null;


            }

        }
    }
}
