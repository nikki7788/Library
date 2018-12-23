using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class BookGroupController : Controller
    {

        #region #####----------- dpendency injection ----------- ######
        //for database
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _iServiceProvider;

        public BookGroupController(ApplicationDbContext context, IServiceProvider iServiceProvider)
        {
            _context = context;
            _iServiceProvider = iServiceProvider;
        }

        #endregion #####---------------------------------------- ######

        [HttpGet]
        public IActionResult Index()
        {

            List<BookGroup> model = new List<BookGroup>();

            model = _context.BookGroups.Select(bg => new BookGroup
            {
                BookGroupId = bg.BookGroupId,
                BookGroupName = bg.BookGroupName,
                BookGroupDescription = bg.BookGroupDescription

            }).ToList();


            return View(model);
        }

        [HttpGet]
        public IActionResult AddEditBookGroup(int id)
        {
            BookGroup bookgroup = new BookGroup();
            if (id != 0)
            {
                using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
                {
                    bookgroup = _context.BookGroups.Where(b => b.BookGroupId == id).SingleOrDefault();
                    if (bookgroup == null)
                    {
                      return  RedirectToAction("Index");
                    }

                }
            }
            return PartialView("_AddEditBookGroupPartial", bookgroup);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEditBookGroup(BookGroup model, int id,string redirectUrl)
        {
            //برای چک کرن ولیدیشن هایی که در کلاس 
            // دادیم که همان اتریبیوت هاهستند bookgroup
            if (ModelState.IsValid)
            {
                //inserting(adding) mode
                if (id == 0)
                {
                    //چون داریم روی دیتابیس اطلاعات ثبت میکنیم یا اپدیت اطلاعات میکیم از 
                    //استفاده میکنیم. usin() 
                    using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
                    {
                        db.BookGroups.Add(model);
                        db.SaveChanges();
                    }
                    //return RedirectToAction("Index");
                    return PartialView("_SuccessfullyResponsePartial", redirectUrl);
                 
                }
                //updating mode
                else
                {
                    //چون داریم روی دیتابیس اطلاعات ثبت میکنیم یا اپدیت اطلاعات میکیم از 
                    //استفاده میکنیم. usin() 
                    using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
                    {
                        db.BookGroups.Update(model);
                        db.SaveChanges();
                    }
                    //  return RedirectToAction("Index");
                    return PartialView("_SuccessfullyResponsePartial", redirectUrl);
                }
            }
            else
            {
                //(model)اگر ولیدشن ها رعایت نشده بود مقادیر وارد شده
                //و خطاهارا نمایش میدهددرهمان پارشال ویو
                return PartialView("_AddEditBookGroupPartial", model);
            }
        }
    }
}