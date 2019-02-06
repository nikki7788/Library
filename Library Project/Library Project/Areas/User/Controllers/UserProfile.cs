using Library.Models;
using Library.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Area.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User")]
    public class UserProfile : Controller
    {
        private readonly ApplicationDbContext _contex;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserProfile(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _contex = context;
            _userManager = userManager;
        }

        #region############################ Index #############################
        // GET: /<controller>/
        public IActionResult Index()
        {
            List<ManageReqestedBookViewModel> model = new List<ManageReqestedBookViewModel>();
            model = (from br in _contex.BorrowRequestedBooks
                     join b in _contex.Books on br.BookId equals b.BookId
                     join u in _contex.Users on br.UserId equals u.Id

                     where u.Id == _userManager.GetUserId(HttpContext.User)  //برای اینکه فقط درخواست های مربوط به یوزر لاگین شده رابیاورد نه همه درخواست ها

                     select new ManageReqestedBookViewModel
                     {
                         Id = br.Id,
                         BookId = br.BookId,
                         UserId = br.UserId,
                         UserFullName = u.FirstName + " " + u.LastName,
                         BookName = b.BookName,
                         Flag = br.Flag, //وضعیت درخواست را برمیکرداند
                         RequestDate = br.RequestDate,
                         AnswerDate = br.AnswerDate,
                         ReturnDate = br.ReturnDate,
                         //وضعیت درخواست را نمایش می دهد
                         FlageState = (
                             br.Flag == 1 ? "درخواست امانت" :
                             br.Flag == 2 ? "امانت برده" :
                             br.Flag == 3 ? "رد درخواست" :
                             br.Flag == 4 ? "برگشت داده" : "نامشخص"
                         )
                     }).ToList();
            //-------------------یافن و ارسال نام کابر به ویو--------------------
            ApplicationUser userFullName = (from u in _contex.Users
                                            where u.Id == _userManager.GetUserId(HttpContext.User)
                                            select u).SingleOrDefault();
            //   بنویسیم   HttpContext    میتوانیم بدون      GetUserId(User)

            ViewBag.userFullName = userFullName.FirstName + " " + " " + userFullName.LastName;
            //---------------------------------------------------------------------

            return View(model);
        }
        #endregion####################################################

        #region ####################   ChangeUserPass   ###################################

        [HttpGet]
        public IActionResult ChangeUserPass()
        {
            //-------------------یافن و ارسال نام کابر به ویو برای نمایش--------------------
            ApplicationUser userFullName = (from u in _contex.Users
                                            where u.Id == _userManager.GetUserId(HttpContext.User)
                                            select u).SingleOrDefault();
            ViewBag.userFullName = userFullName.FirstName + " " + " " + userFullName.LastName;
            //---------------------------------------------------------------------
            return View();

        }
        #endregion
        #region ####################   ChangeUserPass   ###################################

        [HttpPost]
        public async Task<IActionResult> ChangeUserPass(ChangeUserPassViewModel model)
        {
            if (ModelState.IsValid)
            {

                ApplicationUser user = new ApplicationUser();
                user = (from u in _contex.Users    //ApplicationUsers====>in entity framework
                        where u.Id == _userManager.GetUserId(HttpContext.User)   //AspNetUsers====>in Identity
                        select u).SingleOrDefault();
                if (await _userManager.CheckPasswordAsync(user, model.OldPassword))
                {
                    //oldPassword is Correct
                    await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    ViewBag.success = "رمز عبور با موفقیت تغییر کرد";
                    return View(model);
                }
                else
                {
                    //old pass is incorrect
                    ModelState.AddModelError("OldPassword", "رمز عبور قدیمی صحیح نمی باشد");
                    return View(model);

                }
            }
            return View(model);
        }
        #endregion







    }
}
