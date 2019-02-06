using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Library.Models;
using Library.Models.ViewModel;
using Microsoft.AspNetCore.Identity;

namespace Library.Controllers
{

    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(ApplicationDbContext context, IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _serviceProvider = serviceProvider;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            // --------------------------------- شش کتاب اخر وارد شده را برمیکرداند  -----------------------------
            var model = new MultiModelsViewModel();
            model.LastBook = (from b in _context.Books orderby b.BookId descending select b).Take(6).ToList();

            ///---------------------------نمایش اخرین کتاب های رمان ثبت شده----------------------------------------


            //model.ScientificBook = _context.Books.Where(b => b.BookGroupId == 4).OrderByDescending(b => b.BookId).Take(6).ToList();
            //or
            //model.ScientificBook = (from b in _context.Books where b.BookGroupId == 3 orderby b.BookId descending select b).Take(6).ToList();
            //or
            //model.ScientificBook = _context.BookGroups.Join
            //     (_context.Books, b => b.BookGroupId, b => b.BookGroupId, (bg, b)
            //     => b)
            //     .Where(b=>b.BookGroups.BookGroupName == "رمان").OrderByDescending(b => b.BookId).Take(4).ToList();
            //or

            model.ScientificBook = (from b in _context.Books
                                    join bg in _context.BookGroups
                                    on b.BookGroupId equals bg.BookGroupId
                                    where bg.BookGroupName == "رمان"
                                    orderby b.BookId descending
                                    select b).Take(6).ToList();

            //--------------------------نمایش اخرین کاربران ثبت شده---------------------------------------------------
            model.LastRegistedUser = _userManager.Users.OrderByDescending(u => u.Id).Take(10).ToList();
            //model.LastRegistedUser=(from u in _userManager.Users orderby u.Id descending select u).Take(10).ToList();

            //--------------------------نمایش اخرین خبرهای ثبت شده---------------------------------------------------
            //چون از لامبدا استفاده کردیم بید حتما از دستور
            // استفاده کنیمusing
            //model.LastNews = _context.News.OrderByDescending(n => n.NewsId).Take(6).ToList();      
            model.LastNews = (from n in _context.News orderby n.NewsId descending select n).Take(6).ToList();

            //--------------------------نمایش پربازدید ترین کتاب ها---------------------------------------------------
            //به جز کلاس های ایدننینی یعنی کلاس هایی خودمان ایجاد کردیم نیاز چون از لامبدا استفاده کردیم بید حتما از دستور
            // استفاده کنیمusing
            // model.MostViewedBook = _context.Books.OrderByDescending(b => b.BookViews).Take(6).ToList();
            model.MostViewedBook = (from b in _context.Books orderby b.BookId descending select b).Take(6).ToList();

            //-----------------------------------ارسال تصویر به ویو---------------------------------------------
            ViewBag.imgPath = "/upload/normalimage/";


            return View(model);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult Search(string txtSearch)
        {
            MultiModelsViewModel model = new MultiModelsViewModel();
            model.LastNews = _context.News.OrderByDescending(n => n.NewsId).Take(6).ToList();
            model.LastRegistedUser = _userManager.Users.OrderByDescending(u => u.Id).Take(6).ToList();
           // string tSearch;
            ///search
            if (txtSearch != null)
            {
                //پاک کردن فضای خالی قبل و بعد
                txtSearch = txtSearch.TrimEnd().TrimStart();
            }
            //else
            //{
            //    tSearch = txtSearch;
            //}
           model.SearchBooks = _context.Books.Where(b => b.BookName.Contains(txtSearch))
                             .OrderByDescending(b => b.BookId).Take(15).ToList();


            //-------------sending img------------
            ViewBag.imgPath = "/upload/normalimage/";
            ViewBag.searchWord = txtSearch;

            return View(model);

        }

    }
}
