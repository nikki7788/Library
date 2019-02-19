using Library.Models;
using Library.Serviec.AuthorService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReflectionIT.Mvc.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AuthorController : Controller
    {
        private readonly IAuthorService _iAuthorService;
        private readonly ApplicationDbContext _contex;
        private readonly IServiceProvider _iServicePovider;

        public AuthorController(ApplicationDbContext context, IServiceProvider iServiceProvider, IAuthorService iAuthorService)
        {
            _contex = context;
            _iServicePovider = iServiceProvider;
            _iAuthorService = iAuthorService;
        }

        #region######################### Index ######################
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            //with pagination and service layer
            var model = await _iAuthorService.PagingAuthorAsync(page);
            return View(model);
        }

        #endregion ####################################################

        #region######################### Search User #############################      

        public async Task<IActionResult> SearchAuthor(string authorSearch, int page = 1)
        {
            //---------------------------  **** with pagination ****----------------------------------------------------------------
            var model = await _iAuthorService.SearchAuthorAsync(authorSearch, page);
            //ViewBag.search = "true";
            return View("Index", model);
        }
        #endregion###############################################################




        [HttpGet]
        public async Task<IActionResult> AddEditAuthor(int id)
        {
            Author author = new Author();
            if (id != 0)
            {

                author = await _iAuthorService.GetAuthorByIdAsync(id);
                if (author == null)
                {
                    return RedirectToAction("Index");
                }

            }
            return PartialView("_AddEditAuthorPartial", author);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditAuthor(Author author, int id, string redirectUrl)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {

                    //inserting
                    await _iAuthorService.InsertAuthorAsync(author);
                    //_iAuthorService.InsertAuthorAsync(author);
                    //   _iAuthorService.Save();
                    await _iAuthorService.SaveAsync();


                    return PartialView("_SuccessfullyResponsePartial", redirectUrl);
                }
                else
                {
                    //updatin
                    _iAuthorService.EditAuthor(author);

                    //   _iAuthorService.Save();

                    await _iAuthorService.SaveAsync();

                    return PartialView("_SuccessfullyResponsePartial", redirectUrl);
                }
            }
            else
            {
                return PartialView("_AddEditAuthorPartial", author);
            }
        }

        //---------------------------########### Delete Get  ###########--------------------------
        [HttpGet]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            Author author = new Author();
            using (var db = _iServicePovider.GetRequiredService<ApplicationDbContext>())
            {
                ////author = db.Authors.SingleOrDefault(a => a.AuthorId == id);                  روش  ۱  
                //author = db.Authors.Where(a => a.AuthorId == id).SingleOrDefault();          //  روش 2

                //from services layer
                author =await _iAuthorService.GetAuthorByIdAsync(id);

                if (author != null)
                {
                    //نام گروه را که از نوع رشته است برای پارشال ویو ارسال میکنیم
                    return PartialView("_DeleteAuthorPartial", author.AuthorName);
                }
                return RedirectToAction("Index");
            }
        }

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
                    //var author = await db.Authors.SingleOrDefaultAsync(a => a.AuthorId == id);

                    //-----------from service layer-------------------
                    Author author = new Author();
                    // میتوانیم مستقیم هم نوع نویسنده را قبل از نام متغیر بنویسیم و از ان نمونه نسازیم 
                    //Author author=await......
                     author =await _iAuthorService.GetAuthorByIdAsync(id);
                    _iAuthorService.DeleteAuthor(author);
                    await _iAuthorService.SaveAsync();

                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }

    }
}
