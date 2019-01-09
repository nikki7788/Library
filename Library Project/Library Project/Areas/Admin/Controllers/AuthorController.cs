using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class AuthorController : Controller
    {

        private readonly ApplicationDbContext _contex;
        private readonly IServiceProvider _iServicePovider;

        public AuthorController(ApplicationDbContext context, IServiceProvider iServiceProvider)
        {
            _contex = context;
            _iServicePovider = iServiceProvider;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _contex.Authors.Include(a => a.Books).ToListAsync();
            return View(model);

            //or
            //متد غیر همزمان نباشد دستورات زیراست وگرنه باید دستورات زیر را غیر همزمان بنویسیم 
            //List<Author> model = new List<Author>();
            //model = _contex.Authors.Select(a => new Author
            //{
            //    AuthorId = a.AuthorId,
            //    AuthorName = a.AuthorName,
            //    AuthorDescription = a.AuthorDescription
            //}).ToList();
            //return View(model);
        }



        //چون دستور ما خرجی 
        //نوع SingleOrDefault()
        // ندارد بنابراین async
        // نوشتن اکشن هم فایده ای ندارد و خطا میدهد چون حتما در دستور باید از async task<>
        // استفاده کنیمawait ....async()

        [HttpGet]
        public IActionResult AddEditAuthor(int id)
        {
            Author author = new Author();
            if (id != 0)
            {
                //برای دیپندنسی ابنجکشن
                //چون داریم از دیتابیس اطلاعات میخوانبم و خالت وبرایش است ولی در خالت اضافه کردن این دیندنسی ابجکشن را نمیخواهد 
                using (var db = _iServicePovider.GetRequiredService<ApplicationDbContext>())
                {
                    author = _contex.Authors.Where(a => a.AuthorId == id).SingleOrDefault();
                    if (author == null)
                    {
                        return RedirectToAction("Index");
                    }
                }

            }
            return PartialView("_AddEditAuthorPartial", author);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEditAuthor(Author model, int id, string redirectUrl)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    using (var db = _iServicePovider.GetRequiredService<ApplicationDbContext>())
                    {
                        //inserting
                        db.Authors.Add(model);
                        db.SaveChanges();
                    }
                    return PartialView("_SuccessfullyResponsePartial", redirectUrl);
                }
                else
                {
                    //updatin
                    using (var db = _iServicePovider.GetRequiredService<ApplicationDbContext>())
                    {
                        //insert
                        db.Authors.Update(model);
                        db.SaveChanges();
                    }
                    return PartialView("_SuccessfullyResponsePartial", redirectUrl);
                }
            }
            else
            {
                return PartialView("_AddEditAuthorPartial", model);
            }
        }

        //---------------------------########### Delete Get  ###########--------------------------
        [HttpGet]
        public IActionResult DeleteAuthor(int id)
        {
            Author author = new Author();
            using (var db = _iServicePovider.GetRequiredService<ApplicationDbContext>())
            {
                //author = db.Authors.SingleOrDefault(a => a.AuthorId == id);                  روش  ۱  
                author = db.Authors.Where(a => a.AuthorId == id).SingleOrDefault();          //  روش 2
                if (author != null)
                {
                    //نام گروه را که از نوع رشته است برای پارشال ویو ارسال میکنیم
                    return PartialView("_DeleteAuthorPartial", author.AuthorName);
                }
                return RedirectToAction("Index");
            }
        }
        ////the best way
        //[HttpGet]
        //public async Task<IActionResult> DeleteAuthor(int? id)
        //{

        //    if (id != null)
        //    {
        //        Author author = new Author();
        //        using (var db = _iServicePovider.GetRequiredService<ApplicationDbContext>())
        //        {
        //            //author =await db.Authors.SingleOrDefaultAsync(a => a.AuthorId == id); 
        //            author = await db.Authors.SingleOrDefaultAsync(a => a.AuthorId == id);
        //            if (author != null)
        //            {
        //                return PartialView("_DeleteAuthorPartial", author.AuthorName);
        //                //return PartialView("_DeleteAuthorPartial", author);
        //            }
        //        }
        //    }
        //    return RedirectToAction("Index");
        //}

        //or



        //---------------------------########### Delete Post  ###########--------------------------

        [HttpPost, ActionName("DeleteAuthor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            if (id != 0)
            {
                using (var db = _iServicePovider.GetRequiredService<ApplicationDbContext>())
                {
                    //author = db.Authors.Where(a => a.AuthorId == id).SingleOrDefault();
                    var author = await db.Authors.SingleOrDefaultAsync(a => a.AuthorId == id);
                    db.Authors.Remove(author);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }

        //or

        //[HttpPost, ActionName("DeleteAuthor")]
        //[ValidateAntiForgeryToken]
        //public  IActionResult DeleteConfirm(int? id)
        //{
        //    if (id==null)
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    using (var db = _iServicePovider.GetRequiredService<ApplicationDbContext>())
        //    {
        //        var author = db.Authors.SingleOrDefault(a => a.AuthorId == id);
        //        db.Authors.Remove(author);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //}

    }
}
