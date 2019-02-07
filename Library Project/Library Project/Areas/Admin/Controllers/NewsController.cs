using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _iServiceProvider;
        private readonly IHostingEnvironment _appEnvironment;
        public NewsController(ApplicationDbContext context, IServiceProvider iServiceProvider, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _iServiceProvider = iServiceProvider;
            _appEnvironment = appEnvironment;
        }
        #region######################## Index ###################################
        public IActionResult Index()
        {
            List<News> model = new List<News>();
            model = _context.News.Select(n => new News
            {
                NewsId = n.NewsId,
                NewsTitle = n.NewsTitle,
                NewsContent = n.NewsContent,
                NewsDate = n.NewsDate,
                NewsImage = n.NewsImage
            }).ToList();
            ViewBag.imgPath = "/upload/normalimage/";
            return View(model);
        }
        #endregion#####################

        #region ####################### Search ####################################
        public IActionResult SearchNews(string fromDate, string toDate, string newsTitleSearch)
        {
            List<News> model = new List<News>();
            model = _context.News.Select(n => new News
            {
                NewsId = n.NewsId,
                NewsTitle = n.NewsTitle,
                NewsContent = n.NewsContent,
                NewsDate = n.NewsDate,
                NewsImage = n.NewsImage
            }).ToList();

            //  برای مقایسه رشته ها بکارمیرودبراساس کد اسکی حروف مقایسه میکند ومقدار عدد برمیگرداند 0 1 -1  compareto
            //صفر یعنی مقایسه برابر است   یک یعنی بزرگتر و منفی یک یعنی کوچکتر
            if (fromDate != null && toDate == null)
            {
                //  مقایسه میکند تاریخ های موجود درسرور را با تارخ ارسالی و تاریخ های کوچکتر و قبل از تاریخ ارسالی از سرچ را می اورد   compareto 
                model = model.Where(n => n.NewsDate.CompareTo(fromDate) >= 0).ToList();
            }

            if (toDate != null && fromDate == null)
            {
                //  مقایسه میکند تاریخ های موجود درسرور را با تارخ ارسالی و تاریخ های بعد و بزرگتر از تاریخ ارسالی را میاورد   compareto 
                model = model.Where(n => n.NewsDate.CompareTo(toDate) <= 0).ToList();
            }

            if (toDate != null && fromDate != null)
            {
                //  مقایسه میکند تاریخ های موجود درسرور را با تارخ ارسالی و تاریخ بین تاریخ های ارسالی را می اورد   compareto 
                model = model.Where(n => n.NewsDate.CompareTo(fromDate) >= 0 && n.NewsDate.CompareTo(toDate) <= 0).ToList();
            }

            if (newsTitleSearch != null)
            {
                newsTitleSearch = newsTitleSearch.TrimEnd().TrimStart();
                model = model.Where(n => n.NewsTitle.Contains(newsTitleSearch)).ToList();
            }

            ViewBag.imgPath = "/upload/normalimage/";
            return View("Index",model);
        }



        #endregion################################################################
        [HttpGet]
        public IActionResult AddEditNews(int id)
        {
            var model = new News();

            if (id != 0)
            {
                //update
                using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
                {
                    model = _context.News.Where(n => n.NewsId == id).SingleOrDefault();
                    if (model == null)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }


            //بدست آوردن تاریخ شمسی
            var currentDate = DateTime.Now;
            PersianCalendar persianCalendar = new PersianCalendar();
            int year = persianCalendar.GetYear(currentDate);
            int month = persianCalendar.GetMonth(currentDate);
            int day = persianCalendar.GetDayOfMonth(currentDate);
            //0:yyyy/MM/dd این دستوز به شکل زیرعمل میکند
            //1397/3/5 ==> 1397/03/05
            //اگر سال چهاررقم نباشد یا ماه وروز دورقم ثبت نشده باشد بجایش صفر میکذارد
            string shamsiDate = string.Format("{0:yyyy/MM/dd}", Convert.ToDateTime(year + "/" + month + "/" + day));
            ViewBag.pDate = shamsiDate;
            ViewData["imgRoot"] = "/upload/normalimage/";

            return PartialView("_AddEdiNewsPartial", model);
        }

        [HttpPost]
        public async Task<IActionResult> AddEditNews(News model, int NEwsId, string imgName, IEnumerable<IFormFile> files)
        {

            if (ModelState.IsValid)
            {
                //---------------------------------- uploading file------------------------------------
                var uploads = Path.Combine(_appEnvironment.WebRootPath, "upload\\normalimage\\");
                foreach (var item in files)
                {
                    if (item != null && item.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(item.FileName);
                        using (var fs = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                        {
                            await item.CopyToAsync(fs);
                            model.NewsImage = fileName;
                        }
                    }
                }
                //----------------------------------** End uploading file**------------------------------------

                if (model.NewsId == 0)
                {
                    //inserting mode = adding
                    if (model.NewsImage == null)
                    {
                        model.NewsImage = "notidefault.jpg";

                    }
                    using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
                    {
                        await db.News.AddAsync(model);
                        await db.SaveChangesAsync();
                    }
                    return Json(new { status = "success", message = "خبر با موفقیت اضافه شد" });

                }
                else
                {
                    //updating mode = editing
                    if (model.NewsImage == null)
                    {
                        model.NewsImage = imgName;
                    }

                    using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
                    {
                        db.News.Update(model);
                        await db.SaveChangesAsync();
                    }
                    return Json(new { status = "success", message = "خبر با موفقیت ویرایش شد" });

                }
            }    //end modelState

            //display validation with jquery ajax
            var list = new List<string>();
            foreach (var validation in ViewData.ModelState.Values)
            {
                list.AddRange(validation.Errors.Select(error => error.ErrorMessage));
            }
            return Json(new { status = "error", error = list });
        }

        //---------------------------#########---- Delete Get----#########----------------------------------------------
        [HttpGet]
        public IActionResult DeleteNews(int id)
        {
            if (id != 0)
            {
                News news = new News();
                using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
                {
                    //author =await db.Authors.SingleOrDefaultAsync(a => a.AuthorId == id); 
                    news = db.News.SingleOrDefault(n => n.NewsId == id);
                    if (news != null)
                    {
                        return PartialView("_DeleteNewsPartial", news.NewsTitle);
                        //return PartialView("_DeleteAuthorPartial", author);
                    }
                }
            }
            return RedirectToAction("Index");
        }

        //-------------------------------------************** Delete Post ***********----------------------------------------------
        [HttpPost, ActionName("DeleteNews")]
        public IActionResult DeleteConfirm(int id)
        {
            using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
            {
                //var book = db.Books.SingleOrDefault(b => b.BookId==id);
                var news = db.News.Where(n => n.NewsId == id).SingleOrDefault();


                //----------------------------------------delete image ------------------------------------------------------------
                if (news.NewsImage != "notidefault.jpg")        //برای جلوگیری از حذف تصویر پیش فرض
                {
                    //مسیر عکس سایزاورجینال
                    var pathNormal = Path.Combine(_appEnvironment.WebRootPath, "upload\\normalimage\\") + news.NewsImage;

                    if (System.IO.File.Exists(pathNormal))
                    {
                        //اگر عکس وجود داشت پاک شود
                        System.IO.File.Delete(pathNormal);
                    }
                }
                //--------------####---------------------------

                db.News.Remove(news);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

        }
        //-----------------------------------------*************  Details  ***************-----------------------------------------------------

        [HttpGet]
        [AllowAnonymous]
        public IActionResult NewsDetails(int id)
        {
            if (id != 0)
            {
                News model = new News();
                model = _context.News.SingleOrDefault(n => n.NewsId == id);

                if (model != null)
                {
                    return View(model);
                }
            }
            return RedirectToAction("NotFounds");

        }

        [AllowAnonymous]
        public IActionResult NotFounds()
        {
            return View("NotFounds");
        }




    }
}


