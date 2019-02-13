using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Library.Models;
using Library.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReflectionIT.Mvc.Paging;

namespace Library.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _iServiceProvider;
        private readonly IHostingEnvironment _appEnvironment;   /// for accessing to the root project(wwwroot)
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly IMapper _mapper;
        public BookController(ApplicationDbContext context, IServiceProvider iServiceProvider, IHostingEnvironment appEnvironment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _iServiceProvider = iServiceProvider;
            _appEnvironment = appEnvironment;
            _userManager = userManager;
            //   _mapper = mapper;
        }


        #region ################################ Index #################################
        //--------------------------------************* index ***********--------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Index(int page=1)
        {
            //List<BookListViewModel> model = new List<BookListViewModel>();
            //var query = from b in _context.Books
            //            join a in _context.Authors
            //            on b.AuthorId equals a.AuthorId
            //            join bg in _context.BookGroups
            //            on b.BookGroupId equals bg.BookGroupId
            //            select new
            //            {
            //                b.BookId,
            //                b.BookName,
            //                b.BookPageCount,
            //                b.BookImage,
            //                b.AuthorId,
            //                b.BookGroupId,
            //                bg.BookGroupName,
            //                a.AuthorName
            //            };
            //foreach (var item in query)
            //{

            //    //{
            //    //    BookListViewModel obj = new BookListViewModel();
            //    //    obj.BookId = item.BookId;
            //    //    obj.BookName = item.BookName;
            //    //    obj.BookImage = item.BookImage;
            //    //    obj.BookPageCount = item.BookPageCount;
            //    //    obj.AuthorId = item.AuthorId;
            //    //    obj.AuthorName = item.AuthorName;
            //    //    obj.BookGroupId = item.BookGroupId;
            //    //    obj.BookGroupName = item.BookGroupName;
            //    //}


            //    BookListViewModel obj = new BookListViewModel
            //    {
            //        BookId = item.BookId,
            //        BookName = item.BookName,
            //        BookImage = item.BookImage,
            //        BookPageCount = item.BookPageCount,
            //        AuthorId = item.AuthorId,
            //        AuthorName = item.AuthorName,
            //        BookGroupId = item.BookGroupId,
            //        BookGroupName = item.BookGroupName
            //    };
            //    model.Add(obj);

            //}

            var query = (from b in _context.Books
                         join a in _context.Authors
                         on b.AuthorId equals a.AuthorId
                         join bg in _context.BookGroups
                         on b.BookGroupId equals bg.BookGroupId
                         select new BookListViewModel
                         {
                            BookId =b.BookId,
                           BookName=  b.BookName,
                         BookPageCount=    b.BookPageCount,
                            BookImage= b.BookImage,
                            AuthorId= b.AuthorId,
                            BookGroupId= b.BookGroupId,
                          BookGroupName=   bg.BookGroupName,
                           AuthorName=  a.AuthorName
                         }).AsNoTracking().OrderBy(b=>b.BookId);
            PagingList<BookListViewModel> modelPaging =await PagingList.CreateAsync(query, 4, page);

            ViewBag.rootPath = "/upload/thumbnailimage/";
            return View(modelPaging);
        }
        #endregion ###################################################################################### 

        #region####################################### Search Book ##########################################

        public async Task<IActionResult> SearchBook(string BookSearch, string authorSearch, string bookGroupSearch,int page=1)
        {
            //List<BookListViewModel> model = new List<BookListViewModel>();
            //var query = (from b in _context.Books
            //             join a in _context.Authors
            //             on b.AuthorId equals a.AuthorId
            //             join bg in _context.BookGroups
            //             on b.BookGroupId equals bg.BookGroupId
            //             select new
            //             {
            //                 b.BookId,
            //                 b.BookName,
            //                 b.BookPageCount,
            //                 b.BookImage,
            //                 b.AuthorId,
            //                 b.BookGroupId,
            //                 bg.BookGroupName,
            //                 a.AuthorName
            //             });
            //foreach (var item in query)
            //{
            //    BookListViewModel obj = new BookListViewModel
            //    {
            //        BookId = item.BookId,
            //        BookName = item.BookName,
            //        BookImage = item.BookImage,
            //        BookPageCount = item.BookPageCount,
            //        AuthorId = item.AuthorId,
            //        AuthorName = item.AuthorName,
            //        BookGroupId = item.BookGroupId,
            //        BookGroupName = item.BookGroupName
            //    };
            //    model.Add(obj);
            //}

            var model = (from b in _context.Books
                         join a in _context.Authors
                         on b.AuthorId equals a.AuthorId
                         join bg in _context.BookGroups
                         on b.BookGroupId equals bg.BookGroupId
                         select new BookListViewModel()
                         {
                             BookId = b.BookId,
                             BookName = b.BookName,
                             BookPageCount = b.BookPageCount,
                             BookImage = b.BookImage,
                             AuthorId = b.AuthorId,
                             BookGroupId = b.BookGroupId,
                             BookGroupName = bg.BookGroupName,
                             AuthorName = a.AuthorName
                         }).AsNoTracking().OrderBy(b => b.BookId);
            PagingList<BookListViewModel> modelPaging = await PagingList.CreateAsync(model, 4, page);
            //ابتدا براساس نام کتاب میگردد بعد . فیلتر میکند بعد اگر نام نویسنده جست و جو شده باشد 
            //در مدلی که نام کتاب در ان فیلتر شده دنبال ان میگردد
            //واگر نام گروه بندی وار شده باشد در مدل فیلتر شده نام نویسنده
            //یعنی هم زمان قابلیت فیلتر سه سرچ باکس را دارد
            if (BookSearch != null)
            {
                BookSearch = BookSearch.TrimStart().TrimEnd();
                model = model.Where(b => b.BookName.Contains(BookSearch)).OrderBy(b=>b.BookId);
                modelPaging= await PagingList.CreateAsync(model, 4, page);
            }
            if (authorSearch != null)
            {
                authorSearch = authorSearch.TrimStart().TrimEnd();
                model = model.Where(b => b.BookName.Contains(authorSearch)).AsNoTracking().OrderBy(b => b.BookId);
                modelPaging = await PagingList.CreateAsync(model, 4, page);

            }
            if (bookGroupSearch != null)
            {
                bookGroupSearch = bookGroupSearch.TrimStart().TrimEnd();
                model = model.Where(b => b.BookName.Contains(bookGroupSearch)).AsNoTracking().OrderBy(b => b.BookId);
                modelPaging = await PagingList.CreateAsync(model, 4, page);

            }

            ViewBag.rootPath = "/upload/thumbnailimage/";
            return View("Index", modelPaging);
        }
        #endregion



        #region---############################# Add & Edit Book ######################################
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
        //چون برای قسمت پست ویرایش و افزودن یک پارشال ویو تعریف کرده ایم باید یک اکشن برای هر دوحالت بنویسیم
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
                        var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(item.FileName);

                        //for saving path of file in data base and saving file in root project(wwwroot)
                        using (var fs = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                        {
                            await item.CopyToAsync(fs);

                            model.BookImage = fileName;
                        }
                        //---------------------------resize Image ---------------------------------

                        //for creating a smaller size copying of uploading photo
                        InsertShowImage.ImageResizer img = new InsertShowImage.ImageResizer();
                        img.Resize(uploads + fileName, _appEnvironment.WebRootPath + "\\upload\\thumbnailimage\\" + fileName);
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
        #endregion  #############################################################

        #region---------###################### Delete Book ##########################################
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

        #endregion############################################################################

        #region------############################## BookDetails #####################################
        [HttpGet]
        [AllowAnonymous]
        public IActionResult BookDetails(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("NotFounds");
            }
            var model = new MultiModelsViewModel();
            //--------------------------نمایش اخرین کاربران ثبت شده---------------------------------------------------
            model.LastRegistedUser = _userManager.Users.OrderByDescending(u => u.Id).Take(10).ToList();

            //--------------------------نمایش اخرین خبرهای ثبت شده---------------------------------------------------
            //model.LastNews = _context.News.OrderByDescending(n => n.NewsId).Take(6).ToList();
            model.LastNews = (from n in _context.News orderby n.NewsId descending select n).Take(6).ToList();
            //--------------------------نمایش اخرین خبرهای ثبت شده---------------------------------------------------
            model.BookDetails = (from b in _context.Books
                                 join a in _context.Authors on b.AuthorId equals a.AuthorId
                                 join bg in _context.BookGroups on b.BookGroupId equals bg.BookGroupId
                                 where b.BookId == id         //اگر ننویسیم تمام کتاب هارا برمیگرداند
                                 select new BookDetailsViewModel
                                 {
                                     BookId = b.BookId,
                                     BookName = b.BookName,
                                     BookDescription = b.BookDescription,
                                     BookPageCount = b.BookPageCount,
                                     BookImage = b.BookImage,
                                     AuthorName = a.AuthorName,
                                     BookGroupName = bg.BookGroupName,
                                     BookLikeCount = b.BookLikeCount,
                                     BookDislike = b.BookDislike,
                                     BookStock = b.BookStock,
                                     BookViews = b.BookViews

                                 }).ToList();
            //-----------------------------------count views---------------------------------------------
            using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
            {

                var result = db.Books.Where(b => b.BookId == id);
                var currentBook = result.FirstOrDefault();                //مانند تولیست است ولی فقط مقدار اول را برمیگرداند
                if (result.Count() != 0) //اگر کتاب با ایدی مورد نظر پیدا شد
                {
                    currentBook.BookViews++;        //بازدید کتاب را با هربار رفرش یا بازشدن جزییات کتاب یکی زیاد میکند

                    //اپدیت یک فیلد از یک رکورد در دیتابیس
                    db.Books.Attach(currentBook);
                    db.Entry(currentBook).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            //-----------------------------------ارسال تصویر به ویو---------------------------------------------
            ViewBag.imgPath = "/upload/normalimage/";
            return View(model);
        }
        #endregion########################################################################################

        #region ################ Like & Dislike New with Ajax ##################################################
        [AllowAnonymous]
        public async Task<IActionResult> Like(int bookId)  //آی دی کتاب را برمیگردند
        {
            using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
            {
                var query = await db.Books.Where(b => b.BookId == bookId).SingleOrDefaultAsync();
                //checking that the book  exists?
                if (query == null)
                {
                    //return RedirectToAction("‌BookDetails");
                    return Redirect(Request.Headers["Referer"].ToString());      //go to the last action that is run      میرود به اکشن قبلی که اجرا شده است 
                }
                //چک کردن اینکه آیا از قبل کوکی ایجاد شده است یا خیر
                if (Request.Cookies["_LikeLB"] == null)
                {
                    // ای دی باید از نوع استرینگ باشد که میتوانستیم با دستور تواسترینگ به استرینگ تبدیل کنیم یا یک رشته خالی قبل ان بنویسیم
                    //برای اینکه بعد از ثبت ایدی در کوکی بتوانیم انها را پیدا کنیم بینشان یک کاما میگذاریم
                    Response.Cookies.Append("_LikeLB", "," + bookId + ",", new CookieOptions() { Expires = DateTime.Now.AddYears(5) });
                    query.BookLikeCount++; //یکی به لایک اضافه میکند
                    //------------------*****************-------------------------------
                    //اگرکوکب برای دیسلایک وجود داته باشد و کتاب مورد نطردیسلایک شده باشد  تعداد تعداد دیسلایک را یکی کم میکند را کم میکند
                    if (Request.Cookies["_DislikeLB"] != null)
                    {
                        if (Request.Cookies["_DislikeLB"].Contains("," + bookId + ","))
                        {
                            query.BookDislike--;
                        }

                    }
                    //------------------*****************-------------------------------

                    db.Update(query);
                    await db.SaveChangesAsync();
                    //   return Redirect(Request.Headers["Referer"].ToString());
                    return Json(new { status = "success", message = "نظر شما با موفقیت ثبت شد", result = query.BookLikeCount });


                }
                else       //اگر کوکی وجود داشت
                {
                    string cookieContent = Request.Cookies["_LikeLB"].ToString();
                    //چک میکند درکوکی ای دی کتاب مورد نطرکه داخل ان قرار داریم وجود دارد یانه 
                    if (cookieContent.Contains("," + bookId + ","))
                    {
                        //اخرین اکشن اجرا ده را برمیکرداند که اینجا 
                        //bookdetails است 
                        return Redirect(Request.Headers["Referer"].ToString());
                    }
                    else   //اگر ای دی کتاب مورد نظر یافت نشد در کوکی
                    {
                        cookieContent += "," + bookId + ",";
                        //کوکی را ست میکنیم در کوکی که نام ان را لایک دادیم و.ای دی کتاب را در ان به صورت استرینگ ذخیره کردیم و تاریخ انقضا کوکی را ۵سال دادیم
                        Response.Cookies.Append("_LikeLB", cookieContent, new CookieOptions() { Expires = DateTime.Now.AddYears(5) });
                        query.BookLikeCount++;
                        //------------------*****************-------------------------------

                        if (Request.Cookies["_DislikeLB"] != null)
                        {
                            if (Request.Cookies["_DislikeLB"].Contains("," + bookId + ","))
                            {
                                query.BookDislike--;
                            }
                        }
                        //------------------*****************-------------------------------

                        db.Update(query);
                        await db.SaveChangesAsync();
                        // return Redirect(Request.Headers["Referer"].ToString());
                        return Json(new { status = "success", message = "نظر شما با موفقیت ثبت شد", result = query.BookLikeCount });

                    }
                }
            }
        }
        [AllowAnonymous]
        public async Task<IActionResult> Dislike(int bookId)  //آی دی کتاب را برمیگردند
        {
            using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
            {
                var query = await db.Books.Where(b => b.BookId == bookId).SingleOrDefaultAsync();
                //checking that the book  exists?
                if (query == null)
                {
                    //return RedirectToAction("‌BookDetails");
                    return Redirect(Request.Headers["Referer"].ToString());      //go to the last action that is run      میرود به اکشن قبلی که اجرا شده است 
                }
                //چک کردن اینکه آیا از قبل کوکی ایجاد شده است یا خیر
                if (Request.Cookies["_DislikeLB"] == null)
                {
                    // ای دی باید از نوع استرینگ باشد که میتوانستیم با دستور تواسترینگ به استرینگ تبدیل کنیم یا یک رشته خالی قبل ان بنویسیم
                    //برای اینکه بعد از ثبت ایدی در کوکی بتوانیم انها را پیدا کنیم بینشان یک کاما میگذاریم
                    Response.Cookies.Append("_DislikeLB", "," + bookId + ",", new CookieOptions() { Expires = DateTime.Now.AddYears(5) });

                    query.BookDislike++; //یکی به لایک اضافه میکند

                    //----------------******************----------------------------------
                    //اگرکوکب برای لایک وجود داته باشد و کتاب مورد نطرلایک شده باشد  تعداد تعداد لایک را یکی کم میکند را کم میکند

                    if (Request.Cookies["_LikeLB"] != null)
                    {
                        if (Request.Cookies["_LikeLB"].Contains("," + bookId + ","))
                        {
                            query.BookLikeCount--;
                        }
                    }
                    //------------------*****************-------------------------------

                    db.Update(query);
                    await db.SaveChangesAsync();
                    // return Redirect(Request.Headers["Referer"].ToString());
                    return Json(new { status = "success", message = "نظر شما با موفقیت ثبت شد", result = query.BookDislike });

                }
                else       //اگر کوکی وجود داشت
                {
                    string cookieContent = Request.Cookies["_DislikeLB"].ToString();
                    //چک میکند درکوکی ای دی کتاب مورد نطرکه داخل ان قرار داریم وجود دارد یانه 
                    if (cookieContent.Contains("," + bookId + ","))
                    {
                        //اخرین اکشن اجرا ده را برمیکرداند که اینجا 
                        //bookdetails است 
                        return Redirect(Request.Headers["Referer"].ToString());
                    }
                    else   //اگر ای دی کتاب مورد نظر یافت نشد در کوکی
                    {
                        cookieContent += "," + bookId + ",";
                        //کوکی را ست میکنیم در کوکی که نام ان را لایک دادیم و.ای دی کتاب را در ان به صورت استرینگ ذخیره کردیم و تاریخ انقضا کوکی را ۵سال دادیم
                        Response.Cookies.Append("_DislikeLB", cookieContent, new CookieOptions() { Expires = DateTime.Now.AddYears(5) });
                        query.BookDislike++;
                        //----------------******************----------------------------------
                        if (Request.Cookies["_LikeLB"] != null)
                        {
                            if (Request.Cookies["_LikeLB"].Contains("," + bookId + ","))
                            {
                                query.BookLikeCount--;
                            }
                        }
                        //------------------*****************-------------------------------
                        db.Update(query);
                        await db.SaveChangesAsync();
                        //return Redirect(Request.Headers["Referer"].ToString());
                        return Json(new { status = "success", message = "نظر شما با موفقیت ثبت شد", result = query.BookDislike });

                    }
                }
            }
        }
        #endregion

        #region----#########################  BookBorrow    ##########################################################
        [AllowAnonymous]
        [Authorize(Roles = "User")]
        public IActionResult BookBorrow(int id)
        {
            using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
            {
                var query = db.Books.Where(b => b.BookId == id).SingleOrDefault();
                if (query == null)
                {
                    // return RedirectToAction("BookDetails", id);
                    return Json(new { status = "fail", message = "این کتاب وجود ندارد" });
                }
                if (query.BookStock == 0)
                {
                    // return RedirectToAction("BookDetails", id);
                    return Json(new { status = "fail", message = " این کتاب موجودی ندارد" });

                }
                else
                {
                    //اگر کوکی به این نام وجود نداشت
                    if (Request.Cookies["_bb"] == null)
                    {
                        //آی دی کتاب مورد نظر را در کوکی ثبت میکند
                        Response.Cookies.Append("_bb", "," + id + ",", new CookieOptions() { Expires = DateTime.Now.AddMinutes(30) });
                        return Json(new { status = "success", message = " این کتاب به لیست درخواستی شما افزوده شد ", cart = 1 });


                    }
                    else
                    {
                        //اگر کوکی به این نام وجود داشت
                        string cookieContent = Request.Cookies["_bb"].ToString();    //مقدار داخل کوکی را میحواند به استرینگ تبدیل کرده و داخل متغیر میریزد
                        if (cookieContent.Contains("," + id + ","))     //اگر آی دی کتاب مورد نظر در کوکی وجود داشت
                        {
                            //return RedirectToAction("BookDetails", id);
                            return Json(new { status = "done", message = " این کتاب از قبل در لیست درخواستی شما وجود دارد " });
                        }
                        else
                        {

                            //اگر آی دی کتاب مورد نظر در کوکی وجود نداشت
                            //آی دی کتاب مورد نظر را در کوکی ثبت میکند
                            cookieContent += "," + id + ",";
                            Response.Cookies.Append("_bb", cookieContent, new CookieOptions() { Expires = DateTime.Now.AddMinutes(30) });

                            //------------------------------

                            // ویرگول را حذف میکند و به خالی تبدیل میکند           " " : خالی
                            string[] requestCount = cookieContent.Split(",");
                            //درآیه هایی که نال نیستند را میشمرد که همان ای دی ها میشوند
                            requestCount = requestCount.Where(r => r != "").ToArray();   //linq to object

                            //------------------------------                                                             

                            return Json(new { status = "success", message = " این کتاب به لیست درخواستی شما افزوده شد ", cart = requestCount.Count() });

                        }
                    }
                }

            }
        }
        #endregion###################################################################################

        #region----#########################  RequestedBook    ##########################################################
        [AllowAnonymous]
        [Authorize(Roles = "User")]
        public IActionResult RequestedBook()
        {
            MultiModelsViewModel model = new MultiModelsViewModel();

            // model.LastNews = _context.News.OrderByDescending(n => n.NewsId).Take(6).ToList();
            model.LastNews = (from n in _context.News orderby n.NewsId descending select n).Take(6).ToList();
            model.LastRegistedUser = _userManager.Users.OrderByDescending(u => u.Id).Take(6).ToList();

            if (Request.Cookies["_bb"] != null)
            {
                string cookieContent = Request.Cookies["_bb"].ToString();
                string[] requestCount = cookieContent.Split(",");
                requestCount = requestCount.Where(r => r != "").ToArray();
                //نام و ای دی کتاب های درخواستی را ازجدول کتاب میخواند و میارود
                model.RequsetedBook = (from b in _context.Books
                                       where requestCount.Contains(b.BookId.ToString())   //ای دی کتاب ها را میرود داخل کوکی میگردد و انهای که وجود دارد را برمیکرداند
                                       select new Book
                                       {
                                           BookId = b.BookId,
                                           BookName = b.BookName
                                       }).ToList();
            }


            ViewBag.imgPath = "/upload/normalimage/";
            return View(model);
        }

        #endregion######################################################################################################

        #region----#########################  DeleteRequestedBook    ##########################################################

        [AllowAnonymous]
        [Authorize(Roles = "User")]
        public IActionResult DeleteRequestedBook(int id)
        {
            string cookieContent = Request.Cookies["_bb"].ToString();
            string[] requestCount = cookieContent.Split(",");
            requestCount = requestCount.Where(r => r != "").ToArray();

            //برای راحت تر کارگردن باارایه ان را به لیست تبدیل میکنیم تابتوانیم از دستورات ان استفاده کنیم چون ارایه دستورخوبی برای حذف و...ندارد
            List<string> idList = new List<string>(requestCount);
            idList.Remove(id.ToString());         //چون کوکی به صورت رشته ذخیرذه میشود  

            //کوکی را خالی میکنیم وباید دوباره کوکی را از نو بدون ای دی کتاب حذف شده بسازیم
            cookieContent = "";
            for (int i = 0; i < idList.Count(); i++)
            {
                //ایدی کتاب ها را دوباره به کوکی اضافه میکنیم ای دی کتاب حذف شده وجود ندارد
                cookieContent += "," + idList[i] + ",";
            }
            //کوکی را ایجاد میکنیم
            Response.Cookies.Append("_bb", cookieContent, new CookieOptions { Expires = DateTime.Now.AddMinutes(30) });

            ///////////////////////////////***********************************////////////////////////////
            //صفحه را رفرش میکنیم و باید دوباره اخرین اخبار ویوزر ها وکتاب های درخواستی اگر وجود دارد را بیاوریم
            MultiModelsViewModel model = new MultiModelsViewModel();
            if (Request.Cookies["_bb"] != null)
            { //نام و ای دی کتاب های درخواستی را ازجدول کتاب میخواند و میارود
                //اگر کوکی وجود داشت و داخل ان خالی نبود
                string[] requestedBook = cookieContent.Split(",");
                requestedBook = requestedBook.Where(r => r != "").ToArray();

                if (requestedBook.Count() > 0)
                {
                    model.RequsetedBook = (from b in _context.Books
                                           where requestedBook.Contains(b.BookId.ToString())     //ای دی کتاب ها را میرود داخل کوکی میگردد و انهای که وجود دارد را برمیکرداند
                                           select new Book
                                           {
                                               BookId = b.BookId,
                                               BookName = b.BookName
                                           }).ToList();
                }
                else
                {
                    //اگر کتابی در لیست درخواستی ما وجود نداشته باشد میاید به اکشن ایندکس و کوکی را پاک میکند
                    Response.Cookies.Delete("_bb");
                    return Redirect("/Home/Index");
                }

            }

            model.LastNews = _context.News.OrderByDescending(n => n.NewsId).Take(6).ToList();
            model.LastRegistedUser = _userManager.Users.OrderByDescending(u => u.Id).Take(6).ToList();
            //تصویراخبار
            ViewBag.imgPath = "/upload/normalimage/";
            return View("RequestedBook", model);
        }
        #endregion#########################################################################


        #region#######################  BorrowRequesteBook  ######################################################
        [AllowAnonymous]
        [Authorize(Roles = "User")]
        public IActionResult BorrowRequesteBook(string userId)
        {
            //ثبت درخواست کاربر در دیتابیس و منتظر تایید ادمین بود برای تایید و قرض گرفتن

            string cookieContent = Request.Cookies["_bb"].ToString();
            string[] bookRequset = cookieContent.Split(",");
            bookRequset = bookRequset.Where(r => r != "").ToArray();

            if (Request.Cookies["_bb"] != null && bookRequset.Count() > 0)      //  اگر کوکی وجود داشت و داخل ان عضوی وجود داشت
            {
                //----------------------بررسی میکند که این در خواست در سرور وجود دارد یا خیر-------
                //بررسی میکند که در کوکی ای دی های کتاب هایی که وجود دارد مقدار ای دی کتاب و ای دی یوزر در جدول درخواست ها هست یا نه و فلگ را هم بررسی مبکند
                //که یک باشد یعنی در خواست داده شده
                var query = (from b in _context.BorrowRequestedBooks
                             where bookRequset.Contains(b.BookId.ToString())
                             && b.UserId == userId
                             && b.Flag == 1
                             select b);       //میتوان به لیست هم تبدیل کرد

                //نام کتاب هایی که ثبت شده اندودرلیست درخواستی ما بازهم موجود هستند را برمیگرداند
                var qBookName = (from bo in _context.Books
                                 join q in query on bo.BookId equals q.BookId
                                 select bo.BookName).ToList();

                //اگر مقدار کویری بزرگتر از فر یعنی کتاب هایی در دیتابیس در جدول درخواست وجود دارد
                if (query.Count() > 0)
                {
                    return Json(new { status = "warning", message = " قبلا ثبت شده است", rs = qBookName });
                }
                //----------------------***********************************-------------------
                //----------------------------------------------------------------------------
                //ثبت درخواست کاربر در دیتابیس 
                using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
                {

                    //بدست آوردن تاریخ شمسی
                    var currentDate = DateTime.Now;
                    PersianCalendar persianCalendar = new PersianCalendar();
                    int year = persianCalendar.GetYear(currentDate);
                    int month = persianCalendar.GetMonth(currentDate);
                    int day = persianCalendar.GetDayOfMonth(currentDate);
                    //0:yyyy/MM/dd این دستوز به شکل زیرعمل میکند
                    //1397/3/5 ==> 1397/03/05
                    //اگر سال چهاررقم نباشد یا ماه وروز دورقم ثبت نشده باشد بجایش صفر میکذارد
                    string shamsiDate = string.Format("{0:yyyy/MM/dd}", Convert.ToDateTime(year + "/" + month + "/" + day));


                    for (int i = 0; i < bookRequset.Count(); i++)
                    {
                        BorrowRequestedBook bReq = new BorrowRequestedBook
                        {
                            UserId = userId,
                            BookId = Convert.ToInt32(bookRequset[i]),   // جون آی دی کاب از نوع عدد است باید درایه ای کوکی به عدد تبدیل شوند
                            Flag = 1,
                            RequestDate = shamsiDate
                        };

                        //فقط یک کتاب را ثبت میکند در دیتا بیس اگر چند کتاب در لیست باشد
                        //bReq.UserId = userId;
                        //bReq.BookId = Convert.ToInt32(bookRequset[i]);   // جون آی دی کاب از نوع عدد است باید درایه ای کوکی به عدد تبدیل شوند
                        //bReq.Flag = 1;
                        //bReq.RequestDate = ShamsiDate;

                        db.Add(bReq);
                    }
                    db.SaveChanges();
                }

                //----------------------------------------------------------------------------
                //بعد از ثبت درخواست در دیتابیس کوکی را پاک میکند
                Response.Cookies.Delete("_bb");
                return Json(new { status = "success", message = "درخواست شما ثبت شد .منتظر تایید مدیر سیستم باشید..." });
            }

            //اگر کوکی وجود داشته باشد ولی در ان چیزی نباشد این را نمایش می دهد
            //چون وقتی تمام کتاب های لیست را حذف کنیم کوکی وجود دارد ولی مقدارداخلش خالی است 
            return Json(new { status = "warning", message = "لیست شما خالی است" });



        }
        #endregion




        public IActionResult NotFounds()
        {
            return View("NotFounds");
        }



        //----------just foro more information---------------------

        #region ################ Like & Dislike New without Ajax ##################################################
        //[AllowAnonymous]
        //public async Task<IActionResult> Like(int id)  //آی دی کتاب را برمیگردند
        //{
        //    using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
        //    {
        //        var query = await db.Books.Where(b => b.BookId == id).SingleOrDefaultAsync();
        //        //checking that the book  exists?
        //        if (query == null)
        //        {
        //            //return RedirectToAction("‌BookDetails");
        //            return Redirect(Request.Headers["Referer"].ToString());      //go to the last action that is run      میرود به اکشن قبلی که اجرا شده است 
        //        }
        //        //چک کردن اینکه آیا از قبل کوکی ایجاد شده است یا خیر
        //        if (Request.Cookies["_LikeLB"] == null)
        //        {
        //            // ای دی باید از نوع استرینگ باشد که میتوانستیم با دستور تواسترینگ به استرینگ تبدیل کنیم یا یک رشته خالی قبل ان بنویسیم
        //            //برای اینکه بعد از ثبت ایدی در کوکی بتوانیم انها را پیدا کنیم بینشان یک کاما میگذاریم
        //            Response.Cookies.Append("_LikeLB", "," + id + ",", new CookieOptions() { Expires = DateTime.Now.AddYears(5) });
        //            query.BookLikeCount++; //یکی به لایک اضافه میکند
        //            //------------------*****************-------------------------------
        //            //اگرکوکب برای دیسلایک وجود داته باشد و کتاب مورد نطردیسلایک شده باشد  تعداد تعداد دیسلایک را یکی کم میکند را کم میکند
        //            if (Request.Cookies["_DislikeLB"] != null)
        //            {
        //                if (Request.Cookies["_DislikeLB"].Contains("," + id + ","))
        //                {
        //                    query.BookDislike--;
        //                }

        //            }
        //            //------------------*****************-------------------------------

        //            db.Update(query);
        //            await db.SaveChangesAsync();
        //            return Redirect(Request.Headers["Referer"].ToString());


        //        }
        //        else       //اگر کوکی وجود داشت
        //        {
        //            string cookieContent = Request.Cookies["_LikeLB"].ToString();
        //            //چک میکند درکوکی ای دی کتاب مورد نطرکه داخل ان قرار داریم وجود دارد یانه 
        //            if (cookieContent.Contains("," + id + ","))
        //            {
        //                //اخرین اکشن اجرا ده را برمیکرداند که اینجا 
        //                //bookdetails است 
        //                return Redirect(Request.Headers["Referer"].ToString());
        //            }
        //            else   //اگر ای دی کتاب مورد نظر یافت نشد در کوکی
        //            {
        //                cookieContent += "," + id + ",";
        //                //کوکی را ست میکنیم در کوکی که نام ان را لایک دادیم و.ای دی کتاب را در ان به صورت استرینگ ذخیره کردیم و تاریخ انقضا کوکی را ۵سال دادیم
        //                Response.Cookies.Append("_LikeLB", cookieContent, new CookieOptions() { Expires = DateTime.Now.AddYears(5) });
        //                query.BookLikeCount++;
        //                //------------------*****************-------------------------------

        //                if (Request.Cookies["_DislikeLB"] != null)
        //                {
        //                    if (Request.Cookies["_DislikeLB"].Contains("," + id + ","))
        //                    {
        //                        query.BookDislike--;
        //                    }
        //                }
        //                //------------------*****************-------------------------------

        //                db.Update(query);
        //                await db.SaveChangesAsync();
        //                return Redirect(Request.Headers["Referer"].ToString());

        //            }
        //        }
        //    }
        //}
        //[AllowAnonymous]
        //public async Task<IActionResult> Dislike(int id)  //آی دی کتاب را برمیگردند
        //{
        //    using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
        //    {
        //        var query = await db.Books.Where(b => b.BookId == id).SingleOrDefaultAsync();
        //        //checking that the book  exists?
        //        if (query == null)
        //        {
        //            //return RedirectToAction("‌BookDetails");
        //            return Redirect(Request.Headers["Referer"].ToString());      //go to the last action that is run      میرود به اکشن قبلی که اجرا شده است 
        //        }
        //        //چک کردن اینکه آیا از قبل کوکی ایجاد شده است یا خیر
        //        if (Request.Cookies["_DislikeLB"] == null)
        //        {
        //            // ای دی باید از نوع استرینگ باشد که میتوانستیم با دستور تواسترینگ به استرینگ تبدیل کنیم یا یک رشته خالی قبل ان بنویسیم
        //            //برای اینکه بعد از ثبت ایدی در کوکی بتوانیم انها را پیدا کنیم بینشان یک کاما میگذاریم
        //            Response.Cookies.Append("_DislikeLB", "," + id + ",", new CookieOptions() { Expires = DateTime.Now.AddYears(5) });

        //            query.BookDislike++; //یکی به لایک اضافه میکند

        //            //----------------******************----------------------------------
        //            //اگرکوکب برای لایک وجود داته باشد و کتاب مورد نطرلایک شده باشد  تعداد تعداد لایک را یکی کم میکند را کم میکند

        //            if (Request.Cookies["_LikeLB"] != null)
        //            {
        //                if (Request.Cookies["_LikeLB"].Contains("," + id + ","))
        //                {
        //                    query.BookLikeCount--;
        //                }
        //            }
        //            //------------------*****************-------------------------------

        //            db.Update(query);
        //            await db.SaveChangesAsync();
        //            return Redirect(Request.Headers["Referer"].ToString());


        //        }
        //        else       //اگر کوکی وجود داشت
        //        {
        //            string cookieContent = Request.Cookies["_DislikeLB"].ToString();
        //            //چک میکند درکوکی ای دی کتاب مورد نطرکه داخل ان قرار داریم وجود دارد یانه 
        //            if (cookieContent.Contains("," + id + ","))
        //            {
        //                //اخرین اکشن اجرا ده را برمیکرداند که اینجا 
        //                //bookdetails است 
        //                return Redirect(Request.Headers["Referer"].ToString());
        //            }
        //            else   //اگر ای دی کتاب مورد نظر یافت نشد در کوکی
        //            {
        //                cookieContent += "," + id + ",";
        //                //کوکی را ست میکنیم در کوکی که نام ان را لایک دادیم و.ای دی کتاب را در ان به صورت استرینگ ذخیره کردیم و تاریخ انقضا کوکی را ۵سال دادیم
        //                Response.Cookies.Append("_DislikeLB", cookieContent, new CookieOptions() { Expires = DateTime.Now.AddYears(5) });
        //                query.BookDislike++;
        //                //----------------******************----------------------------------
        //                if (Request.Cookies["_LikeLB"] != null)
        //                {
        //                    if (Request.Cookies["_LikeLB"].Contains("," + id + ","))
        //                    {
        //                        query.BookLikeCount--;
        //                    }
        //                }
        //                //------------------*****************-------------------------------
        //                db.Update(query);
        //                await db.SaveChangesAsync();
        //                return Redirect(Request.Headers["Referer"].ToString());

        //            }
        //        }
        //    }
        //}
        #endregion

        #region ############################ Like & Dislike old ##########################################
        //[AllowAnonymous]
        //public async Task<IActionResult> Like(int id)  //آی دی کتاب را برمیگردند
        //{
        //    using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
        //    {
        //        var query = await db.Books.Where(b => b.BookId == id).SingleOrDefaultAsync();
        //        //checking that the book  exists?
        //        if (query == null)
        //        {
        //            //return RedirectToAction("‌BookDetails");
        //            return Redirect(Request.Headers["Referer"].ToString());      //go to the last action that is run      میرود به اکشن قبلی که اجرا شده است 
        //        }
        //        //چک کردن اینکه آیا از قبل کوکی ایجاد شده است یا خیر
        //        if (Request.Cookies["_LikeLB"] == null)
        //        {
        //            // ای دی باید از نوع استرینگ باشد که میتوانستیم با دستور تواسترینگ به استرینگ تبدیل کنیم یا یک رشته خالی قبل ان بنویسیم
        //            //برای اینکه بعد از ثبت ایدی در کوکی بتوانیم انها را پیدا کنیم بینشان یک کاما میگذاریم
        //            Response.Cookies.Append("_LikeLB", "," + id + ",", new CookieOptions() { Expires = DateTime.Now.AddYears(5) });
        //            query.BookLikeCount++; //یکی به لایک اضافه میکند
        //            db.Update(query);
        //            await db.SaveChangesAsync();
        //            return Redirect(Request.Headers["Referer"].ToString());


        //        }
        //        else       //اگر کوکی وجود داشت
        //        {
        //            string cookieContent = Request.Cookies["_LikeLB"].ToString();
        //            //چک میکند درکوکی ای دی کتاب مورد نطرکه داخل ان قرار داریم وجود دارد یانه 
        //            if (cookieContent.Contains("," + id + ","))
        //            {
        //                //اخرین اکشن اجرا ده را برمیکرداند که اینجا 
        //                //bookdetails است 
        //                return Redirect(Request.Headers["Referer"].ToString());
        //            }
        //            else   //اگر ای دی کتاب مورد نظر یافت نشد در کوکی
        //            {
        //                cookieContent += "," + id + ",";
        //                //کوکی را ست میکنیم در کوکی که نام ان را لایک دادیم و.ای دی کتاب را در ان به صورت استرینگ ذخیره کردیم و تاریخ انقضا کوکی را ۵سال دادیم
        //                Response.Cookies.Append("_LikeLB", cookieContent, new CookieOptions() { Expires = DateTime.Now.AddYears(5) });
        //                query.BookLikeCount++;
        //                db.Update(query);
        //                await db.SaveChangesAsync();
        //                return Redirect(Request.Headers["Referer"].ToString());

        //            }
        //        }
        //    }
        //}
        //[AllowAnonymous]
        //public async Task<IActionResult> Dislike(int id)  //آی دی کتاب را برمیگردند
        //{
        //    using (var db = _iServiceProvider.GetRequiredService<ApplicationDbContext>())
        //    {
        //        var query = await db.Books.Where(b => b.BookId == id).SingleOrDefaultAsync();
        //        //checking that the book  exists?
        //        if (query == null)
        //        {
        //            //return RedirectToAction("‌BookDetails");
        //            return Redirect(Request.Headers["Referer"].ToString());      //go to the last action that is run      میرود به اکشن قبلی که اجرا شده است 
        //        }
        //        //چک کردن اینکه آیا از قبل کوکی ایجاد شده است یا خیر
        //        if (Request.Cookies["_disikeLB"] == null)
        //        {
        //            // ای دی باید از نوع استرینگ باشد که میتوانستیم با دستور تواسترینگ به استرینگ تبدیل کنیم یا یک رشته خالی قبل ان بنویسیم
        //            //برای اینکه بعد از ثبت ایدی در کوکی بتوانیم انها را پیدا کنیم بینشان یک کاما میگذاریم
        //            Response.Cookies.Append("_disikeLB", "," + id + ",", new CookieOptions() { Expires = DateTime.Now.AddYears(5) });
        //            query.BookLikeCount--; //یکی به لایک اضافه میکند
        //            db.Update(query);
        //            await db.SaveChangesAsync();
        //            return Redirect(Request.Headers["Referer"].ToString());


        //        }
        //        else       //اگر کوکی وجود داشت
        //        {
        //            string cookieContent = Request.Cookies["_disikeLB"].ToString();
        //            //چک میکند درکوکی ای دی کتاب مورد نطرکه داخل ان قرار داریم وجود دارد یانه 
        //            if (cookieContent.Contains("," + id + ","))
        //            {

        //                //اخرین اکشن اجرا ده را برمیکرداند که اینجا 
        //                //bookdetails است 
        //                return Redirect(Request.Headers["Referer"].ToString());
        //            }
        //            else   //اگر ای دی کتاب مورد نظر یافت نشد در کوکی
        //            {
        //                cookieContent += "," + id + ",";
        //                //کوکی را ست میکنیم در کوکی که نام ان را لایک دادیم و.ای دی کتاب را در ان به صورت استرینگ ذخیره کردیم و تاریخ انقضا کوکی را ۵سال دادیم
        //                Response.Cookies.Append("_disikeLB", cookieContent, new CookieOptions() { Expires = DateTime.Now.AddYears(5) });
        //                query.BookLikeCount--;
        //                db.Update(query);
        //                await db.SaveChangesAsync();
        //                return Redirect(Request.Headers["Referer"].ToString());

        //            }
        //        }
        //    }
        //}

        #endregion

        //--------------------------------------------------------





    }
}