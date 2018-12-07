using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Models;
using Library.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Library.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _iServiceProvider;
        public BookController(ApplicationDbContext context, IServiceProvider iServiceProvider)
        {
            _context = context;
            _iServiceProvider = iServiceProvider;
        }


        [HttpGet]
        public IActionResult Index()
        {
            List<BookListViewModel> model = new List<BookListViewModel>();

            var query = from b in _context.Books
                        join a in _context.Authors
                        on b.AuthorId equals a.AuthorId
                        join bg in _context.BookGroups
                        on b.BookGroupId equals bg.BookGroupId
                        select new
                        {
                            b.BookId,
                            b.BookName,
                            b.BookPageCount,
                            b.BookImage,
                            b.AuthorId,
                            b.BookGroupId,
                            bg.BookGroupName,
                            a.AuthorName
                        };
            foreach (var item in query)
            {

                //{
                //    BookListViewModel obj = new BookListViewModel();
                //    obj.BookId = item.BookId;
                //    obj.BookName = item.BookName;
                //    obj.BookImage = item.BookImage;
                //    obj.BookPageCount = item.BookPageCount;
                //    obj.AuthorId = item.AuthorId;
                //    obj.AuthorName = item.AuthorName;
                //    obj.BookGroupId = item.BookGroupId;
                //    obj.BookGroupName = item.BookGroupName;
                //}


                BookListViewModel obj = new BookListViewModel
                {
                    BookId = item.BookId,
                    BookName = item.BookName,
                    BookImage = item.BookImage,
                    BookPageCount = item.BookPageCount,
                    AuthorId = item.AuthorId,
                    AuthorName = item.AuthorName,
                    BookGroupId = item.BookGroupId,
                    BookGroupName = item.BookGroupName
                };
                model.Add(obj);

            }

            return View(model);
        }
    }
}