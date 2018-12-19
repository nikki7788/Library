using Library.Models;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index()
        {

            List<Author> model = new List<Author>();

            model = _contex.Authors.Select(a => new Author
            {
                AuthorId = a.AuthorId,
                AuthorName = a.AuthorName,
                AuthorDescription = a.AuthorDescription

            }).ToList();


            return View(model);
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
                        RedirectToAction("Index");
                    }
                }

            }
            return PartialView("_AddEditAuthorPartial", author);
        }

    }
}
