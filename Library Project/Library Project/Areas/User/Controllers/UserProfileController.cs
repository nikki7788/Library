using Library.Models;
using Library.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Area.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User")]
    public class UserProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        #region############################ Index #############################
        // GET: /<controller>/
        public async Task<IActionResult> Index(int page=1)
        {
            //بدون صفحه بندی
            //List<ManageReqestedBookViewModel> model = new List<ManageReqestedBookViewModel>();
            //model = (from br in _contex.BorrowRequestedBooks
            //         join b in _contex.Books on br.BookId equals b.BookId
            //         join u in _contex.Users on br.UserId equals u.Id

            //         where u.Id == _userManager.GetUserId(HttpContext.User)  //برای اینکه فقط درخواست های مربوط به یوزر لاگین شده رابیاورد نه همه درخواست ها

            //         select new ManageReqestedBookViewModel
            //         {
            //             Id = br.Id,
            //             BookId = br.BookId,
            //             UserId = br.UserId,
            //             UserFullName = u.FirstName + " " + u.LastName,
            //             BookName = b.BookName,
            //             Flag = br.Flag, //وضعیت درخواست را برمیکرداند
            //             RequestDate = br.RequestDate,
            //             AnswerDate = br.AnswerDate,
            //             ReturnDate = br.ReturnDate,
            //             //وضعیت درخواست را نمایش می دهد
            //             FlageState = (
            //                 br.Flag == 1 ? "درخواست امانت" :
            //                 br.Flag == 2 ? "امانت برده" :
            //                 br.Flag == 3 ? "رد درخواست" :
            //                 br.Flag == 4 ? "برگشت داده" : "نامشخص"
            //             )
            //         }).ToList();

            //with pagination
            var model = (from br in _context.BorrowRequestedBooks
                         join b in _context.Books on br.BookId equals b.BookId
                         join u in _context.Users on br.UserId equals u.Id

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
                         }).AsNoTracking().OrderBy(u => u.Id); //AsNoTracking is for pagination
                                                               //رکورد های صفحه بندی میکند می اورد
                                                               //  var modelPaging = await PagingList<ManageReqestedBookViewModel>.CreateAsync(model, 4, page);
            PagingList<ManageReqestedBookViewModel> modelPaging = await PagingList.CreateAsync(model, 4, page); //هر۴رکورد دریک صفحه

            //-------------------یافن و ارسال نام کابر به ویو--------------------
            ApplicationUser userFullName = (from u in _context.Users
                                            where u.Id == _userManager.GetUserId(HttpContext.User)
                                            select u).SingleOrDefault();
            //   بنویسیم   HttpContext    میتوانیم بدون      GetUserId(User)

            ViewBag.userFullName = userFullName.FirstName + " " + " " + userFullName.LastName;
            //---------------------------------------------------------------------


            //نام کاربر و مقدار موجودی کیف پول کاربر را برمیگرداند
            ViewBag.wallet = userFullName.Wallet;

            return View(modelPaging);
        }
        #endregion####################################################

        #region############################ SearchInUser #############################
        // GET: /<controller>/
        public async Task<IActionResult> SearchInUserAsync(string fromDate, string toDate, string bookSearch, int page = 1)
        {
            var model = (from br in _context.BorrowRequestedBooks
                         join b in _context.Books on br.BookId equals b.BookId
                         join u in _context.Users on br.UserId equals u.Id

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
                         }).AsNoTracking().OrderBy(u => u.Id); //AsNoTracking is for pagination
                                                               //رکورد های صفحه بندی میکند می اورد
                                                               //  var modelPaging = await PagingList<ManageReqestedBookViewModel>.CreateAsync(model, 4, page);
            PagingList<ManageReqestedBookViewModel> modelPaging = await PagingList.CreateAsync(model, 4, page); //هر۴رکورد دریک صفحه

            //  برای مقایسه رشته ها بکارمیرودبراساس کد اسکی حروف مقایسه میکند ومقدار عدد برمیگرداند 0 1 -1  compareto
            //صفر یعنی مقایسه برابر است   یک یعنی بزرگتر و منفی یک یعنی کوچکتر
            if (fromDate != null)
            {
                //  مقایسه میکند تاریخ های موجود درسرور را با تارخ ارسالی و تاریخ های کوچکتر و قبل از تاریخ ارسالی از سرچ را می اورد   compareto 

                model = model.Where(m => m.RequestDate.CompareTo(fromDate) >= 0).OrderBy(m => m.Id);
                modelPaging = await PagingList.CreateAsync(model, 4, page);
            }

            if (toDate != null && fromDate == null)
            {
                //  مقایسه میکند تاریخ های موجود درسرور را با تارخ ارسالی و تاریخ های بعد و بزرگتر از تاریخ ارسالی را میاورد   compareto 

                model = model.Where(m => m.RequestDate.CompareTo(toDate) <= 0).OrderBy(m => m.Id);
                modelPaging = await PagingList.CreateAsync(model, 4, page);

            }

            //if (toDate != null && fromDate != null)
            //{
            //    //  مقایسه میکند تاریخ های موجود درسرور را با تارخ ارسالی و تاریخ بین تاریخ های ارسالی را می اورد   compareto 
            //    model = model.Where(m => m.RequestDate.CompareTo(fromDate) >= 0 &&  m.RequestDate.CompareTo(toDate) <= 0).OrderBy(m => m.Id);
            //    modelPaging = await PagingList.CreateAsync(model, 4, page);
            //}


            if (bookSearch != null)
            {
                bookSearch = bookSearch.TrimEnd().TrimStart();
                model = model.Where(m => m.BookName.Contains(bookSearch)).OrderBy(m=>m.Id);
                modelPaging =await PagingList.CreateAsync(model, 4, page);
            }

            //-------------------یافن و ارسال نام کابر به ویو--------------------
            ApplicationUser userFullName = (from u in _context.Users
                                            where u.Id == _userManager.GetUserId(HttpContext.User)
                                            select u).SingleOrDefault();
            //   بنویسیم   HttpContext    میتوانیم بدون      GetUserId(User)

            ViewBag.userFullName = userFullName.FirstName + " " + " " + userFullName.LastName;
            ViewBag.wallet = userFullName.Wallet;

            //---------------------------------------------------------------------

            return View("Index",modelPaging);
        }

        #endregion####################################################

        #region ####################   ChangeUserPass   ###################################

        [HttpGet]
        public IActionResult ChangeUserPass()
        {
            //نام کاربر و مقدار موجودی کیف پول کاربر را برمیگرداند--------------------------------------------
            ApplicationUser userFullName = (from u in _context.Users
                                            where u.Id == _userManager.GetUserId(HttpContext.User)
                                            select u).SingleOrDefault();
            ViewBag.wallet = userFullName.Wallet;
            ViewBag.userFullName = userFullName.FirstName + " " + " " + userFullName.LastName;
            //---------------------------------------------------------------------------------
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
                user = (from u in _context.Users    //ApplicationUsers====>in entity framework
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
