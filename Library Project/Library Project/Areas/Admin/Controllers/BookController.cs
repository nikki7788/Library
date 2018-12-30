using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Library.Models;
using Library.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _iServiceProvider;
        //   private readonly IMapper _mapper;
        public BookController(ApplicationDbContext context, IServiceProvider iServiceProvider /*,IMapper mapper*/ )
        {
            _context = context;
            _iServiceProvider = iServiceProvider;
            //   _mapper = mapper;
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

        [HttpGet]
        public IActionResult AddBook()
        {
            //AddEditBookViewModel model = new AddEditBookViewModel
            //{
            //    Authors = _context.Authors.Select(a => new SelectListItem
            //    {
            //        Text = a.AuthorName,
            //        //the value accepts the string data type as a result we should convert data type to string
            //        Value = a.AuthorId.ToString()
            //    }).ToList(),
            //    BookGroups = _context.BookGroups.Select(bg => new SelectListItem
            //    {
            //        Text = bg.BookGroupName,
            //        Value = bg.BookGroupId.ToString()
            //    }).ToList()
            //};

            AddEditBookViewModel model = new AddEditBookViewModel();
            model.Authors = _context.Authors.Select(a => new SelectListItem
            {
                Text = a.AuthorName,
                //the value accepts the string data type as a result we should convert data type to string
                Value = a.AuthorId.ToString()
            }).ToList();
            model.BookGroups = _context.BookGroups.Select(bg => new SelectListItem
            {
                Text = bg.BookGroupName,
                Value = bg.BookGroupId.ToString()
            }).ToList();

            return PartialView("_AddEditBookPartial", model);
        }

        [HttpGet]
        public IActionResult EditBook(int id)
        {

            AddEditBookViewModel model = new AddEditBookViewModel();
            model.BookGroups = _context.BookGroups.Select(bg => new SelectListItem
            {
                Text = bg.BookGroupName,
                Value = bg.BookGroupId.ToString()
            }).ToList();

            model.Authors = _context.Authors.Select(a => new SelectListItem
            {
                Text = a.AuthorName,
                //the value accepts the string data type as a result we should convert data type to string
                Value = a.AuthorId.ToString()
            }).ToList();

            if (id != 0)
            {
                using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
                {
                    Book book = _context.Books.Where(b => b.BookId == id).SingleOrDefault();
                    if (book != null)
                    {
                        model.BookName = book.BookName;
                        model.BookDescription = book.BookDescription;
                        model.BookPageCount = book.BookPageCount;
                        //مقدار کمبو باکس انتخاب شده توسط کاربر انتخابی برای ویرایش را برمیگرداند
                        model.AuthorId = book.AuthorId;
                        //مقدار کمبو باکس انتخاب شده توسط کاربر انتخابی برای ویرایش را برمیگرداند
                        model.BookGroupId = book.BookGroupId;
                        model.BookId = book.BookId;
                    }

                }
            }
            return PartialView("_AddEditBookPartial", model);

        }
        //برای  قسمت خواندن(گت) افزودن و ویرایش دو اکشن نوشتیم که اکشن ما طولانی نشود
        //چون برای قسمت پست پیرایش و افزودن یک پارشال ویو تعریف کرده ایم باید یک اکشن برای هر دوحالت بنویسیم
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public IActionResult AddEditBook(int bookid, AddEditBookViewModel model, string redirectUrl)
        {
            // آی دی ورودی حتما باید همنام اتریبیوت
            //تگ باشد که درپارشال asp-for="BookId"
            // دادیم_AddEditBookPartial

            if (ModelState.IsValid )
            {
                if (bookid == 0)
                {
                    //Insert
                    using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
                    {
                        //برای گفتن صریح اینکه ویو مدل ما همان کلاس بوک مااست به ام وی سی و بتواند ان را روی جدول بوک ذخیره کند
                        //Book bookModel = _mapper.Map<AddEditBookViewModel, Book>(model);
                        Book bookModel = Mapper.Map<AddEditBookViewModel, Book>(model);

                        db.Books.Add(bookModel);
                        db.SaveChanges();

                    }
                }
                else
                {
                    //Update
                    using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
                    {
                        //برای گفتن صریح اینکه ویو مدل ما همان کلاس بوک مااست به ام وی سی و بتواند ان را روی جدول بوک ذخیره کند
                        Book bookModel = Mapper.Map<AddEditBookViewModel, Book>(model);
                        db.Books.Update(bookModel);
                        db.SaveChanges();
                    }

                }
                return PartialView("_SuccessfullyResponsePartial", redirectUrl);
            }

            //برای نایش کمبو باکس ها بعد خطا در ولیدیشن ها
            model.Authors = _context.Authors.Select(a => new SelectListItem
            {
                Text = a.AuthorName,
                //the value accepts the string data type as a result we should convert data type to string
                Value = a.AuthorId.ToString()
            }).ToList();
            model.BookGroups = _context.BookGroups.Select(bg => new SelectListItem
            {
                Text = bg.BookGroupName,
                Value = bg.BookGroupId.ToString()
            }).ToList();

            return PartialView("_AddEditBookPartial", model);
        }
    }
}