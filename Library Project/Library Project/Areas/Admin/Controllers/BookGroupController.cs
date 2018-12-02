using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Models;
using Microsoft.AspNetCore.Mvc;

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
    }
}