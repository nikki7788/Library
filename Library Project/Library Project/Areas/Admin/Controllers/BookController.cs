using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Library.Models;
using Library.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _iServiceProvider;
        private readonly IHostingEnvironment _appEnvironment;   /// for accessing to the root project(wwwroot)
        //   private readonly IMapper _mapper;
        public BookController(ApplicationDbContext context, IServiceProvider iServiceProvider, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _iServiceProvider = iServiceProvider;
            _appEnvironment = appEnvironment;
            //   _mapper = mapper;
        }

        //--------------------------------************* index ***********--------------------------------------------------------
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
            ViewBag.rootPath = "/upload/thumbnailimage/";

            return View(model);
        }

        //--------------------------------************* Add Book Get***********--------------------------------------------------------

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

        //--------------------------------************* Edit Book Get ***********--------------------------------------------------------

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
                        //برای دریافت نام عکس در مودال و نمایش ان  به کمک ویوبگ
                        model.BookImage = book.BookImage;
                    }

                }
            }
            //برای ارسال مسیر عکس ها برای نمایش در مودال ویرایش  کتاب و پیش نمایش ان 
            ViewBag.imgRoot = "/upload/thumbnailimage/";

            return PartialView("_AddEditBookPartial", model);

        }

        //------------------------------------------#### Add Edit Post ######-------------------------------------------

        //برای  قسمت خواندن(گت) افزودن و ویرایش دو اکشن نوشتیم که اکشن ما طولانی نشود
        //چون برای قسمت پست پیرایش و افزودن یک پارشال ویو تعریف کرده ایم باید یک اکشن برای هر دوحالت بنویسیم
        [HttpPost]
        // [ValidateAntiForgeryToken]      // this statement does'nt allow run ajax-jquery

        public async Task<IActionResult> AddEditBook(int bookId, AddEditBookViewModel model, IEnumerable<IFormFile> files, string imgName)
        {
            if (ModelState.IsValid)
            {

                //------################### *** Upload Image *** ------###################
                var uploads = Path.Combine(_appEnvironment.WebRootPath, "upload\\normalimage\\");
                foreach (var item in files)
                {
                    if (item != null && item.Length > 0)
                    {
                        //creating a unique name for each file and then atach the format of each file to the unique name
                        var filename = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(item.FileName);

                        //for saving path of file in data base and saving file in root project(wwwroot)
                        using (var fs = new FileStream(Path.Combine(uploads, filename), FileMode.Create))
                        {
                            await item.CopyToAsync(fs);

                            model.BookImage = filename;
                        }
                        //---------------------------resize Image ---------------------------------

                        //for creating a smaller size copying of uploading photo
                        InsertShowImage.ImageResizer img = new InsertShowImage.ImageResizer();
                        img.Resize(uploads + filename, _appEnvironment.WebRootPath + "\\upload\\thumbnailimage\\" + filename);
                    }
                }
                //------####################------ End Uploadin Images -------#########################

                //Inserting
                if (bookId == 0)
                {
                    //اگر کاربر تصویری برای کتاب مشخص نکرد نصویر پیش فرض ما ذخیره شود
                    if (model.BookImage == null)
                    {
                        model.BookImage = "defaultpic.png";
                    }

                    using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
                    {
                        //برای گفتن صریح اینکه ویو مدل ما همان کلاس بوک مااست به ام وی سی و بتواند ان را روی جدول بوک ذخیره کند
                        //Book bookModel = _mapper.Map<AddEditBookViewModel, Book>(model);
                        Book bookModel = Mapper.Map<AddEditBookViewModel, Book>(model);
                        db.Books.Add(bookModel);
                        db.SaveChanges();
                    }
                    return Json(new { status = "success", message = "کتاب با موفقیت اضافه شد" });
                }
                //Updating

                else
                {

                    //اگر کاربر درحالت ویرایش تصویری وارد نکرد تصویر قبلی ذخیره شده ان دوباره به ان منتصب شود
                    if (model.BookImage == null)
                    {
                        model.BookImage = imgName;
                    }

                    using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
                    {
                        //برای گفتن صریح اینکه ویو مدل ما همان کلاس بوک مااست به ام وی سی و بتواند ان را روی جدول بوک ذخیره کند
                        Book bookModel = Mapper.Map<AddEditBookViewModel, Book>(model);
                        db.Books.Update(bookModel);
                        db.SaveChanges();
                    }
                    return Json(new { status = "success", message = "اطلاعات کتاب با موفقیت ویرایش شد" });
                }
            }//end model state

            // برای نایش کمبو باکس ها بعد خطا در ولیدیشن ها
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
            //display validation with jquery ajax
            var list = new List<string>();
            foreach (var validation in ViewData.ModelState.Values)
            {
                list.AddRange(validation.Errors.Select(error => error.ErrorMessage));
            }

            return Json(new { status = "error", error = list });





            //    // آی دی ورودی حتما باید همنام اتریبیوت
            //    //تگ باشد که درپارشال asp-for="BookId"
            //    // دادیم_AddEditBookPartial

            //    if (ModelState.IsValid )
            //    {
            //        if (bookId == 0)
            //        {
            //            //Insert
            //            using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
            //            {
            //                //برای گفتن صریح اینکه ویو مدل ما همان کلاس بوک مااست به ام وی سی و بتواند ان را روی جدول بوک ذخیره کند
            //                //Book bookModel = _mapper.Map<AddEditBookViewModel, Book>(model);
            //                Book bookModel = Mapper.Map<AddEditBookViewModel, Book>(model);

            //                db.Books.Add(bookModel);
            //                db.SaveChanges();

            //            }
            //        }
            //        else
            //        {
            //            //Update
            //            using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
            //            {
            //                //برای گفتن صریح اینکه ویو مدل ما همان کلاس بوک مااست به ام وی سی و بتواند ان را روی جدول بوک ذخیره کند
            //                Book bookModel = Mapper.Map<AddEditBookViewModel, Book>(model);
            //                db.Books.Update(bookModel);
            //                db.SaveChanges();
            //            }

            //        }
            //        return PartialView("_SuccessfullyResponsePartial", redirectUrl);
            //    }

            //    //برای نایش کمبو باکس ها بعد خطا در ولیدیشن ها
            //    model.Authors = _context.Authors.Select(a => new SelectListItem
            //    {
            //        Text = a.AuthorName,
            //        //the value accepts the string data type as a result we should convert data type to string
            //        Value = a.AuthorId.ToString()
            //    }).ToList();
            //    model.BookGroups = _context.BookGroups.Select(bg => new SelectListItem
            //    {
            //        Text = bg.BookGroupName,
            //        Value = bg.BookGroupId.ToString()
            //    }).ToList();

            //    return PartialView("_AddEditBookPartial", model);
        }


        //-------------------************** Delete Get ***********--------------------------------------

        [HttpGet]
        public IActionResult DeleteBook(int id)
        {
            var model = new Book();
            using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
            {
                model = db.Books.Where(b => b.BookId == id).SingleOrDefault();
                if (model == null)
                {
                    return RedirectToAction("Index");
                }
            }
            return PartialView("_DeleteBookPartial", model.BookName);
        }

        //the best way
        //[HttpGet]
        //public IActionResult DeleteBook(int? id)
        //{
        //    if (id != null)
        //    {
        //        Book book = new Book();
        //        using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
        //        {
        //            //author =await db.Authors.SingleOrDefaultAsync(a => a.AuthorId == id); 
        //            book = db.Books.SingleOrDefault(b => b.BookId == id);
        //            if (book != null)
        //            {
        //                return PartialView("_DeleteBookPartial", book.BookId);
        //                //return PartialView("_DeleteAuthorPartial", author);
        //            }
        //        }
        //    }
        //    return RedirectToAction("Index");
        //}

        //-------------------************** Delete Post ***********--------------------------------------
        [HttpPost, ActionName("DeleteBook")]
        public IActionResult DeleteConfirm(int id)
        {
            using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
            {
                //var book = db.Books.SingleOrDefault(b => b.BookId==id);
                var book = db.Books.Where(b => b.BookId == id).SingleOrDefault();


                //--------------delete image ------------------
                if (book.BookImage != "defaultpic.png")        //برای جلوگیری از حذف تصویر پیش فرض
                {
                    //مسیر عکس سایزاورجینال
                    var pathNormal = Path.Combine(_appEnvironment.WebRootPath, "upload\\normalimage\\") + book.BookImage;
                    //مسیر عکس سایز کوچک شده
                    var pathThumbnail = Path.Combine(_appEnvironment.WebRootPath, "upload\\thumbnailimage\\") + book.BookImage;

                    if (System.IO.File.Exists(pathNormal)) 
                    {
                        //اگر عکس وجود داشت پاک شود
                        System.IO.File.Delete(pathNormal);
                    }
                    if (System.IO.File.Exists(pathThumbnail))
                    {
                        System.IO.File.Delete(pathThumbnail);
                    }
                   
                }
                //--------------####---------------------------

                db.Books.Remove(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

        }

    }
}