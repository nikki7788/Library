using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReflectionIT.Mvc.Paging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Library.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ManageRequestController : Controller
    {

        private readonly ApplicationDbContext _contex;
        private readonly IServiceProvider _iServicePovider;

        public ManageRequestController(ApplicationDbContext context, IServiceProvider iServiceProvider)
        {
            _contex = context;
            _iServicePovider = iServiceProvider;
        }

        #region############################ Index #############################
        // GET: /<controller>/
        public async Task<IActionResult> Index(int page = 1)
        {
            //List<ManageReqestedBookViewModel> model = new List<ManageReqestedBookViewModel>();
            //model = (from br in _contex.BorrowRequestedBooks
            //         join b in _contex.Books on br.BookId equals b.BookId
            //         join u in _contex.Users on br.UserId equals u.Id
            //         select new ManageReqestedBookViewModel
            //         {
            //             Id = br.Id,
            //             BookId = br.BookId,
            //             UserId = br.UserId,
            //             UserFullName = u.FirstName + " " + u.LastName,
            //             BookStock = b.BookStock,
            //             BookName = b.BookName,
            //             Flag = br.Flag,
            //             RequestDate = br.RequestDate,
            //             AnswerDate = br.AnswerDate,
            //             ReturnDate = br.ReturnDate,
            //             FlageState = (
            //                 br.Flag == 1 ? "درخواست امانت" :
            //                 br.Flag == 2 ? "امانت برده" :
            //                 br.Flag == 3 ? "رد درخواست" :
            //                 br.Flag == 4 ? "برگشت داده" : "نامشخص"
            //             )
            //         }).ToList();
            //return View(model);

            var model = (from br in _contex.BorrowRequestedBooks
                         join b in _contex.Books on br.BookId equals b.BookId
                         join u in _contex.Users on br.UserId equals u.Id
                         select new ManageReqestedBookViewModel
                         {
                             Id = br.Id,
                             BookId = br.BookId,
                             UserId = br.UserId,
                             UserFullName = u.FirstName + " " + u.LastName,
                             BookStock = b.BookStock,
                             BookName = b.BookName,
                             Flag = br.Flag,
                             RequestDate = br.RequestDate,
                             AnswerDate = br.AnswerDate,
                             ReturnDate = br.ReturnDate,
                             FlageState = (
                                 br.Flag == 1 ? "درخواست امانت" :
                                 br.Flag == 2 ? "امانت برده" :
                                 br.Flag == 3 ? "رد درخواست" :
                                 br.Flag == 4 ? "برگشت داده" : "نامشخص"
                             )
                         }).AsNoTracking().OrderBy(m => m.Id);
            PagingList<ManageReqestedBookViewModel> modelPaging = await PagingList.CreateAsync(model, 3, page);
            return View(modelPaging);

        }
        #endregion####################################################

        #region######################## SearchInRequset Aync ##############################
        public async Task<IActionResult> SearchInRequestAsync(string bookSearch, string fromDate, string toDate, int page = 1)
        {
            var model = (from br in _contex.BorrowRequestedBooks
                         join b in _contex.Books on br.BookId equals b.BookId
                         join u in _contex.Users on br.UserId equals u.Id
                         select new ManageReqestedBookViewModel
                         {
                             Id = br.Id,
                             BookId = br.BookId,
                             UserId = br.UserId,
                             UserFullName = u.FirstName + " " + u.LastName,
                             BookStock = b.BookStock,
                             BookName = b.BookName,
                             Flag = br.Flag,
                             RequestDate = br.RequestDate,
                             AnswerDate = br.AnswerDate,
                             ReturnDate = br.ReturnDate,
                             FlageState = (
                                 br.Flag == 1 ? "درخواست امانت" :
                                 br.Flag == 2 ? "امانت برده" :
                                 br.Flag == 3 ? "رد درخواست" :
                                 br.Flag == 4 ? "برگشت داده" : "نامشخص"
                             )
                         }).AsNoTracking().OrderBy(m => m.Id);
            PagingList<ManageReqestedBookViewModel> modelPaging = await PagingList.CreateAsync(model, 3, page);
            if (bookSearch != null)
            {
                bookSearch = bookSearch.TrimEnd().TrimStart();
                model = model.Where(m => m.BookName.Contains(bookSearch)).AsNoTracking().OrderBy(m => m.Id);
                modelPaging = await PagingList.CreateAsync(model, 3, page);
            }
            if (fromDate != null && toDate == null)
            {
                model = model.Where(m => m.RequestDate.CompareTo(fromDate) >= 0).AsNoTracking().OrderBy(m => m.Id);
                modelPaging = await PagingList.CreateAsync(model, 3, page);

            }
            if (fromDate != null && toDate != null)
            {
                model = model.Where(m => m.RequestDate.CompareTo(fromDate) >= 0 && m.RequestDate.CompareTo(toDate) <= 0).AsNoTracking().OrderBy(m => m.Id);
                modelPaging = await PagingList.CreateAsync(model, 3, page);

            }
            if (toDate != null && fromDate == null)
            {
                model = model.Where(m => m.RequestDate.CompareTo(toDate) <= 0).AsNoTracking().OrderBy(m => m.Id);
                modelPaging = await PagingList.CreateAsync(model, 3, page);

            }
            return View("Index", modelPaging);

        }
        #endregion##################################################
       
        #region############################ RejectRequest Get #############################
        [HttpGet]
        public IActionResult RejectRequest(int id)
        {
            //نام کتاب و کاربر را در پارشال رد د ر خواست نمایش میدهیم
            List<ManageReqestedBookViewModel> model = new List<ManageReqestedBookViewModel>();
            model = (from br in _contex.BorrowRequestedBooks
                     join b in _contex.Books on br.BookId equals b.BookId
                     join u in _contex.Users on br.UserId equals u.Id
                     where br.Id == id   //ای دی در خواست
                     select new ManageReqestedBookViewModel
                     {
                         UserFullName = u.FirstName + " " + u.LastName,
                         BookName = b.BookName,
                     }).ToList();

            if (model == null)
            {
                return RedirectToAction("Index");
            }

            //یعنی در حالت رد درخواست پارشال را نمایش دهد
            ViewBag.partialMode = 1;
            return PartialView("_RejectRequestPartial", model);
        }
        #endregion####################################################

        #region############################ RejectConfirm  Post#############################
        [HttpPost, ActionName("RejectRequest")]
        [ValidateAntiForgeryToken]
        public IActionResult RejectConfirm(int id)
        {
            //چون به صورت فرم تعریف شده فرم در متد گت ای دی را گرفتیم و اینجا در اختیار داریم
            using (var db = _iServicePovider.GetRequiredService<ApplicationDbContext>())
            {
                var query = db.BorrowRequestedBooks.Where(br => br.Id == id);
                // var query=(from br in db.BorrowRequestedBooks where br.Id == id select br);
                var result = query.SingleOrDefault();
                if (query.Count() > 0)
                {
                    if (result != null)
                    {
                        //بدست آوردن تاریخ شمسی
                        var currentDay = DateTime.Now;
                        PersianCalendar pcalender = new PersianCalendar();
                        int year = pcalender.GetYear(currentDay);
                        int month = pcalender.GetMonth(currentDay);
                        int day = pcalender.GetDayOfMonth(currentDay);

                        string ShamsiDate = string.Format("{0:yyyy/MM/dd}", Convert.ToDateTime(year + "/" + month + "/" + day));

                        result.Flag = 3; //درخواست رد شد
                        result.AnswerDate = ShamsiDate;

                        db.BorrowRequestedBooks.Attach(result);
                        db.Entry(result).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        db.SaveChanges();
                    }

                }
            }
            return RedirectToAction("Index");

        }
        #endregion####################################################

        #region############################ AcceptRequest Get #############################

        [HttpGet]
        public IActionResult AcceptRequest(int id)
        {
            List<ManageReqestedBookViewModel> model = new List<ManageReqestedBookViewModel>();
            model = (from br in _contex.BorrowRequestedBooks
                     join b in _contex.Books on br.BookId equals b.BookId
                     join u in _contex.Users on br.UserId equals u.Id
                     where br.Id == id
                     select new ManageReqestedBookViewModel
                     {
                         UserFullName = u.FirstName + " " + u.LastName,
                         BookName = b.BookName,
                     }).ToList();

            if (model == null)
            {
                return RedirectToAction("Index");
            }
            //یعنی در حالت تایید درخواست پارشال را نمایش دهد
            ViewBag.partialMode = 2;
            return PartialView("_RejectRequestPartial", model);
        }

        #endregion####################################################

        #region############################ AcceptRequest Post #############################
        [HttpPost, ActionName("AcceptRequest")]
        public IActionResult AcceptConfirm(int id)
        {
            using (var db = _iServicePovider.GetRequiredService<ApplicationDbContext>())
            {

                //string DateString = "1396/12/19";
                //IFormatProvider culture = new CultureInfo("fa-Ir", true);
                //DateTime dateVal = DateTime.ParseExact(DateString, "yyyy/MM/dd", culture);

                //بدست آوردن تاریخ شمسی
                var currentDate = DateTime.Now;
                PersianCalendar persianCalendar = new PersianCalendar();
                int year = persianCalendar.GetYear(currentDate);
                int month = persianCalendar.GetMonth(currentDate);
                int day = persianCalendar.GetDayOfMonth(currentDate);
                //0:yyyy/MM/dd این دستوز به شکل زیرعمل میکند
                //1397/3/5 ==> 1397/03/05
                //اگر سال چهاررقم نباشد یا ماه وروز دورقم ثبت نشده باشد بجایش صفر میکذارد
                string sD = string.Format("{0:yyyy/MM/dd}", Convert.ToDateTime(year + "/" + month + "/" + day));

                //برای دسترسی به جدول درخواست ها و برای تایید وردوبرگرداندن کتاب
                var result = db.BorrowRequestedBooks.Where(br => br.Id == id).SingleOrDefault();
                //var query = (from br in db.BorrowRequestedBooks where br.Id == id select br);
                //var result = query.SingleOrDefault();

                //برای دسترسی به موجودی کتاب برای کم و زیاد کردن ان
                var findBook = db.Books.Where(b => b.BookId == result.BookId).SingleOrDefault();
                if (findBook != null)
                {
                    findBook.BookStock--;
                    db.Books.Attach(findBook);
                    db.Entry(findBook).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }




                //if (query.Count() != 0)
                //{
                if (result != null)
                {
                    result.Flag = 2; //یعنی درخواست تایید شده است
                    result.AnswerDate = sD.ToString();
                    db.BorrowRequestedBooks.Attach(result);
                    db.Entry(result).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
                //}
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        #endregion####################################################

        #region############################ Return Request GET #############################
        [HttpGet]
        public IActionResult ReturnRequest(int id)
        {
            List<ManageReqestedBookViewModel> model = new List<ManageReqestedBookViewModel>();
            model = (from br in _contex.BorrowRequestedBooks
                     join b in _contex.Books on br.BookId equals b.BookId
                     join u in _contex.Users on br.UserId equals u.Id
                     where br.Id == id
                     select new ManageReqestedBookViewModel
                     {
                         UserFullName = u.FirstName + " " + u.LastName,
                         BookName = b.BookName,
                     }).ToList();

            if (model == null)
            {
                return RedirectToAction("Index");
            }
            //یعنی در حالت برگرداندن درخواست پارشال را نمایش دهد
            ViewBag.partialMode = 3;
            return PartialView("_RejectRequestPartial", model);
        }
        #endregion####################################################

        #region############################ Return Request POST #############################


        [HttpPost, ActionName("ReturnRequest")]
        public IActionResult ReturnConfirm(int id)
        {
            using (var db = _iServicePovider.GetRequiredService<ApplicationDbContext>())
            {
                //بدست آوردن تاریخ شمسی
                var currentDay = DateTime.Now;
                PersianCalendar pcalender = new PersianCalendar();
                int year = pcalender.GetYear(currentDay);
                int month = pcalender.GetMonth(currentDay);
                int day = pcalender.GetDayOfMonth(currentDay);
                string shamsiDate = string.Format("{0:yyyy/MM/dd}", Convert.ToDateTime(year + "/" + month + "/" + day));

                //برای دسترسی به جدول درخواست هابرای برگرداندن کتاب
                var result = db.BorrowRequestedBooks.Where(br => br.Id == id).SingleOrDefault();
                //var query = (from br in db.BorrowRequestedBooks where br.Id == id select br);
                //var result = query.SingleOrDefault();

                //برای دسترسی به موجودی کتاب برای کم و زیاد کردن ان
                var findBook = db.Books.Where(b => b.BookId == result.BookId).SingleOrDefault();
                if (findBook != null)
                {
                    findBook.BookStock++;     //کتاب برگردانده شده و موجودی باید اضافه شود
                    db.Books.Attach(findBook);
                    db.Entry(findBook).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }

                //if (query.Count() != 0)
                //{
                if (result != null)
                {
                    result.Flag = 4; //یعنی درخواست برگردانده شده است
                    result.ReturnDate = shamsiDate;
                    db.BorrowRequestedBooks.Attach(result);
                    db.Entry(result).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
                //}
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        #endregion####################################################




        //------------------------------------------- for more information it isnot used---------------------------------------------------------
        #region############################ RejectRequest Post -for more informotion-not use #############################
        //public IActionResult RejectRequest(int id, bool t)
        //{
        //    using (var db = _iServicePovider.GetRequiredService<ApplicationDbContext>())
        //    {
        //        var query = (from br in db.BorrowRequestedBooks where br.Id == id select br);
        //        var result = query.SingleOrDefault();

        //        //بدست آوردن تاریخ شمسی
        //        var currentDay = DateTime.Now;
        //        PersianCalendar pcalender = new PersianCalendar();
        //        int year = pcalender.GetYear(currentDay);
        //        int month = pcalender.GetMonth(currentDay);
        //        int day = pcalender.GetDayOfMonth(currentDay);

        //        string ShamsiDate = string.Format("{0:yyyy/MM/dd}", Convert.ToDateTime(day + "/" + month + "/" + year));

        //        if (query.Count() != 0)
        //        {
        //            result.Flag = 3;
        //            result.AnswerDate = ShamsiDate;
        //            db.BorrowRequestedBooks.Attach(result);
        //            db.Entry(result).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

        //        }
        //        db.SaveChanges();
        //    }
        //    return RedirectToAction("Index");
        //}
        #endregion####################################################













    }
}
