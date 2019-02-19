using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Models;
using Library.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;

namespace Library.Areas.User.Controllers
{
    /// <summary>
    /// گزراش تراکنش های کاربر
    /// </summary>
    /// 
    [Area("User")]
    [Authorize(Roles ="User")]
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public TransactionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        #region################################# Index  ##############################
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            var model = (from pt in _context.TransactionPayments
                         join u in _context.Users on pt.UserId equals u.Id
                         select new PaymentTransactionViewModel
                         {

                             ID = pt.ID,
                             Amount = pt.Amount,
                             Description = pt.Description,
                             Email = pt.Email,
                             Mobile = pt.Mobile,
                             TransactionDate = pt.TransactionDate,
                             TransactionNo = pt.TransactionNo,
                             TransactionTime = pt.TransactionTime,
                             FullNameUser = u.FirstName + " " + u.LastName,
                             UserId = pt.UserId
                         }).AsNoTracking().OrderByDescending(r => r.ID);

            PagingList<PaymentTransactionViewModel> modelPaging = await PagingList.CreateAsync(model, 5, page);

            //-------------------نام کاربر و مقدار موجودی کیف پول کاربر را برمیگرداند--------------------
            ApplicationUser userFullName = (from u in _context.Users
                                            where u.Id == _userManager.GetUserId(HttpContext.User)
                                            select u).SingleOrDefault();
            //   بنویسیم   HttpContext    میتوانیم بدون      GetUserId(User)

            ViewBag.userFullName = userFullName.FirstName + " " + " " + userFullName.LastName;
            //نام کاربر و مقدار موجودی کیف پول کاربر را برمیگرداند
            ViewBag.wallet = userFullName.Wallet;
            //---------------------------------------------------------------------
            return View(modelPaging);
        }

        #endregion#####################################################################

        #region#################################  SearchInTransactionAsync ##############################

        public async Task<IActionResult> SearchInTransactionAsync(string fromDate, string toDate, string userNameSearch, int page = 1)
        {
            var model = (from pt in _context.TransactionPayments
                         join u in _context.Users on pt.UserId equals u.Id
                         where u.Id==_userManager.GetUserId(User)                 //ای دی یوزرجدول یوزرها برابر ایدی کاربری که لاگین کرده باشد و در واقع این باعث نمایش رکورد های همین کاربرمیشود نه همه کاربران
                         select new PaymentTransactionViewModel
                         {

                             ID = pt.ID,
                             Amount = pt.Amount,
                             Description = pt.Description,
                             Email = pt.Email,
                             Mobile = pt.Mobile,
                             TransactionDate = pt.TransactionDate,
                             FullNameUser = u.FirstName + " " + u.LastName,
                             TransactionNo = pt.TransactionNo,
                             TransactionTime = pt.TransactionTime,
                             UserId = pt.UserId
                         }).AsNoTracking().OrderByDescending(r => r.ID);

            PagingList<PaymentTransactionViewModel> modelPaging = await PagingList.CreateAsync(model, 5, page);

            if (userNameSearch != null)
            {
                userNameSearch = userNameSearch.TrimEnd().TrimStart();
                model = model.Where(m => m.FullNameUser.Contains(userNameSearch)).AsNoTracking().OrderByDescending(m => m.FullNameUser);
                modelPaging = await PagingList.CreateAsync(model, 5, page);
            }
            if (fromDate != null)
            {
                model = model.Where(m => m.TransactionDate.CompareTo(fromDate) > 0).AsNoTracking().OrderByDescending(m => m.TransactionDate);
                modelPaging = await PagingList.CreateAsync(model, 5, page);
            }
            if (toDate != null)
            {
                model = model.Where(m => m.TransactionDate.CompareTo(toDate) < 0).AsNoTracking().OrderByDescending(m => m.TransactionDate);
                modelPaging = await PagingList.CreateAsync(model, 5, page);
            }

            //-------------------نام کاربر و مقدار موجودی کیف پول کاربر را برمیگرداند--------------------
            ApplicationUser userFullName = (from u in _context.Users
                                            where u.Id == _userManager.GetUserId(HttpContext.User)
                                            select u).SingleOrDefault();
            //   بنویسیم   HttpContext    میتوانیم بدون      GetUserId(User)

            ViewBag.userFullName = userFullName.FirstName + " " + " " + userFullName.LastName;
            //نام کاربر و مقدار موجودی کیف پول کاربر را برمیگرداند
            ViewBag.wallet = userFullName.Wallet;
            //---------------------------------------------------------------------

            return View("Index", modelPaging);
        }

        #endregion#####################################################################

    }
}
