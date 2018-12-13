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
        public IActionResult AddEditBookGroup(int Id)
        {
            BookGroup bookgroup = new BookGroup();
            if (Id!=0)
            {
                using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>()) 
                {
                    bookgroup = _context.BookGroups.Where(b => b.BookGroupId == Id).SingleOrDefault();
                    if (bookgroup==null)
                    {
                        RedirectToAction("Index");
                    }

                }
            }
            return PartialView("_AddEditBookGroupPartial", bookgroup);
        }
    }
}