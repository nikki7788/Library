using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

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

        // GET: /<controller>/
        public IActionResult Index()
        {
            List<ManageReqestedBookViewModel> model = new List<ManageReqestedBookViewModel>();
            model = (from br in _contex.BorrowRequestedBooks
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
                     }).ToList();

            return View(model);
        }

        [HttpGet]
        public IActionResult RejectRequest(int id)
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
            return PartialView("_RejectRequestPartial", model);
        }

        [HttpPost, ActionName("RejectRequest")]
        [ValidateAntiForgeryToken]
        public IActionResult RejectConfirm(int id)
        {

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
                        var currentDate = DateTime.Now;
                        PersianCalendar prCal = new PersianCalendar();
                        int year = prCal.GetYear(currentDate);
                        int month = prCal.GetMonth(currentDate);
                        int day = prCal.GetDayOfMonth(currentDate);
                        string ShamsiDate = string.Format("{0:yyyy/dd/MM}", Convert.ToDateTime(day + "/" + month + "/" + year));

                        result.Flag = 3;
                        result.AnswerDate = ShamsiDate;

                        db.BorrowRequestedBooks.Attach(result);
                        db.Entry(result).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        db.SaveChanges();
                    }

                }
            }
            return RedirectToAction("Index");

        }


        //[HttpPost]
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








    }
}
