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
        public AccountController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
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
                    //در پایین تعریف شده است RedirectToLocal
                    return RedirectToLocal(returnUrl);   
                }
                else
                {
                    //وقتی نام کاربری یا رمز عبور صحیح نباشد
                    ModelState.AddModelError(string.Empty, "error");
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






        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))        //اگر یک مسیر از قبل وجود داشت
            {

                return Redirect(returnUrl);
            }
            else           //اگر مسیر اشتباه بود یا کاربر فقط می خواهد لاگین کند
            {

                //return RedirectToAction(nameof(HomeController.Index), "Home");
                return Redirect("/Admin/User");
                //اگر وی دکمه ورود کلیک کنیم یعنی مسیری همراه خود ندارد و میرود به یوزر

            }

        }
    }
}
