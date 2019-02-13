﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReflectionIT.Mvc.Paging;

namespace Library.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> Index(int page = 1)
        {
            //  include   بادستور
            //ما تزریق انجام داده ایم و باید حتا اسینکرون بشد
            //وبااین دستور پراپرتی آی کالکشن و ویرچوال کتاب را که تعریف کرده بودیم در جدول گروه بندی به ان دسترسی داریم 
            //یعنی به اطلاعات کتاب دسترسی داریم دراینجا الان
            //بقیه پراپرتی ها را هم خودش مقداردهی میکند
            //var model = await _context.BookGroups.Include(bg => bg.Books).ToListAsync();
            //return View(model);
            //or

            //List<BookGroup> model = new List<BookGroup>();
            //model = _context.BookGroups.Select(bg => new BookGroup
            //{
            //    BookGroupId = bg.BookGroupId,
            //    BookGroupName = bg.BookGroupName,
            //    BookGroupDescription = bg.BookGroupDescription
            //}).ToList();
            //return View(model);

            var model =  _context.BookGroups.AsNoTracking().Include(b => b.Books).OrderBy(bg=>bg.BookGroupId);
            PagingList<BookGroup> modelPaging =await PagingList.CreateAsync(model, 2, page);
            return View(modelPaging);
        }

        public async Task<IActionResult> SearchBookGroup(string bookGroupSearch,int page=1) {

            //var model = _context.BookGroups.Include(bg => bg.Books);
            //if (bookGroupSearch != null)
            //{
            //    bookGroupSearch = bookGroupSearch.TrimEnd().TrimStart();
            //     model = model.Where(bg=>bg.BookGroupName.Contains(bookGroupSearch)).Include(bg => bg.Books);
            //}
            //return View("Index",await model.ToListAsync());

            var model = _context.BookGroups.AsNoTracking().Include(b => b.Books).OrderBy(bg => bg.BookGroupId);
            PagingList<BookGroup> modelPaging = await PagingList.CreateAsync(model, 2, page);
            if (bookGroupSearch!=null)
            {
                model= _context.BookGroups.Where(bg=>bg.BookGroupName.Contains(bookGroupSearch)).OrderBy(bg => bg.BookGroupId);
            }
            return View(modelPaging);

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
                        return RedirectToAction("Index");
                    }

                }
            }
            return PartialView("_AddEditBookGroupPartial", bookgroup);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEditBookGroup(BookGroup model, int id, string redirectUrl)
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


        //--------------------------------************* Delete Get **********--------------------------------

        //این روش بهتر است
        //[HttpGet]
        //public async Task<IActionResult> DeleteBookGroup(int? id)        //آی دی را قابل نال بودن در نظر گرفتیم
        //{
        //    BookGroup bookGroup = new BookGroup();
        //    if (id == null)
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
        //    {
        //        bookGroup = await db.BookGroups.SingleOrDefaultAsync(bg => bg.BookGroupId == id);
        //        if (bookGroup == null)
        //        {
        //            return RedirectToAction("Index");
        //        }
        //        return PartialView("_DeleteBookGroupPartial", bookGroup.BookGroupName);
        //    }
        //}


        [HttpGet]
        public IActionResult DeleteBookGroup(int id)
        {
            BookGroup bookGroup = new BookGroup();
            using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
            {
                //bookGroup = db.BookGroups.SingleOrDefault(bg => bg.BookGroupId == id);                  روش  ۱  
                bookGroup = db.BookGroups.Where(bg => bg.BookGroupId == id).SingleOrDefault();          //  روش 2
                if (bookGroup == null)
                {
                    return RedirectToAction("Index");
                }
                //نام گروه را که از نوع رشته است برای پپارشال ویو ارسال میکنیم
                return PartialView("_DeleteBookGroupPartial", bookGroup.BookGroupName);
            }
        }

        //--------------------------------************* Delete Get **********--------------------------------

        //روش بهتر
        //[HttpPost, ActionName("DeleteBookGroup")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirm(int? id)
        //{
        //    if (id == null)
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
        //    {
        //        var bookGroup =await db.BookGroups.SingleOrDefaultAsync(bg => bg.BookGroupId == id);
        //        db.BookGroups.Remove(bookGroup);
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //}

        [HttpPost, ActionName("DeleteBookGroup")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirm(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }
            using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
            {
                //var bookGroup = db.BookGroups.SingleOrDefault(bg => bg.BookGroupId == id);
                var bookGroup = db.BookGroups.Where(bg => bg.BookGroupId == id).SingleOrDefault();

                db.BookGroups.Remove(bookGroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

    }
}