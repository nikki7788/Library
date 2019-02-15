using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace Library.Areas.User.Controllers
{


    [Area("User")]
    [Authorize(Roles = "User")]

    public class PaymentTransactionController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public PaymentTransactionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }



        #region ######################  دستورات ارسال تراکنش به درگاه و قبل از پرداخت    #########################################


        public IActionResult Payment()
        {
            //نام کاربر و مقدار موجودی کیف پول کاربر را برمیگرداند
            ApplicationUser userFullName = (from u in _context.Users
                                            where u.Id == _userManager.GetUserId(HttpContext.User)
                                            select u).SingleOrDefault();

            ViewBag.userFullName = userFullName.FirstName + " " + " " + userFullName.LastName;
            ViewBag.wallet = userFullName.Wallet;

            return View();
        }




        [HttpPost]
        public async Task<IActionResult> Payment(TransactionPayment pTr)
        {
            //نام کاربر و مقدار موجودی کیف پول کاربر را برمیگرداند--------------------------------------------
            ApplicationUser userFullName = (from u in _context.Users
                                            where u.Id == _userManager.GetUserId(HttpContext.User)
                                            select u).SingleOrDefault();
            ViewBag.wallet = userFullName.Wallet;
            ViewBag.userFullName = userFullName.FirstName + " " + " " + userFullName.LastName;
            //---------------------------------------------------------------------------------

            if (pTr.Amount == 0)
            {
                //رابه ویو ارسال میکند و با این کلید واژه که نامش دلخواه است خطارا ویو دریافت میکند AmountError
                //   دریافت میشود که خطای مدل و خطای کنترلر را نمایش می دهد    ‌@html.validationMessage()   توسط ددستور                    
                //و مدل نمایش خطاهای خود سیستم به صورت انگلیسی را حل میکند
                ModelState.AddModelError("AmountError", "مبلغ خالی است. لطفا مبلغی را بیشتر از 100 تومان وارد نمایید");
            }
            if (!ModelState.IsValid)
            {
                return View("Payment", pTr);
            }

            //جلسه ۱۲۷و۱۲۸- دستورات پرداخت زرین پال
            //قسمت ارسال به زرین پال
            // فقط تغییر داد       ZarinpalSandbox   استفاده از درگاه تستی- باری درگاه اصلی باید   
            //controllrName= paymenttransaction

            var payment = await new ZarinpalSandbox.Payment(pTr.Amount).PaymentRequest(pTr.Description,

                Url.Action(nameof(PaymentVerifyAsync), "PaymentTransaction", new
                {
                    amount = pTr.Amount,
                    description = pTr.Description,
                    email = pTr.Email,
                    mobile = pTr.Mobile
                }, Request.Scheme), pTr.Email, pTr.Mobile);

            //در صورت موفقیت آمیز بودن درخواست، کاربر را به صفحه پرداخت هدایت کن
            //در غیر این صورت باید خطا نمایش داده شود

            return payment.Status == 100 ? (IActionResult)Redirect(payment.Link) :
                BadRequest($"خطا در پرداخت. کد خطا :  {payment.Status} ");
        }

        #endregion




        #region #################### دستورات بعد از پرداخت و ارتباط با درگاه #########################################
        //دستورات بعد از پرداخت و ارتباط با درگاه
        public async Task<IActionResult> PaymentVerifyAsync(int amount, string description, string Email, string mobile, string Authority, string Status)
        {



            //اگر تراکنش و پرداخت ناموفق بود
            if (Status == "NOK")
            {
                //نام کاربر و مقدار موجودی کیف پول کاربر را برمیگرداند--------------------------------------------
                ApplicationUser userFullName = (from u in _context.Users
                                                where u.Id == _userManager.GetUserId(HttpContext.User)
                                                select u).SingleOrDefault();
                ViewBag.wallet = userFullName.Wallet;
                ViewBag.userFullName = userFullName.FirstName + " " + " " + userFullName.LastName;
                //---------------------------------------------------------------------------------

                return View("FailedPay");

            }


            //گرفتن تاییدیه پرداخت
            var verification = await new ZarinpalSandbox.Payment(amount).Verification(Authority);

            //ارسال به صفحه خطا
            if (verification.Status != 100)
            {
                //نام کاربر و مقدار موجودی کیف پول کاربر را برمیگرداند--------------------------------------------
                ApplicationUser userFullName = (from u in _context.Users
                                                where u.Id == _userManager.GetUserId(HttpContext.User)
                                                select u).SingleOrDefault();
                ViewBag.wallet = userFullName.Wallet;
                ViewBag.userFullName = userFullName.FirstName + " " + " " + userFullName.LastName;
                //---------------------------------------------------------------------------------
                return View("FailedPay");
            }


            //ارسال کد تراکنش جهت نمایش به کاربر
            var refId = verification.RefId;


            /////در صورتی که تراکنش موفق باشد باید اطلاعات کاربر و تراکنش در دیتابیس ثبت شود
            //بدست آوردن تاریخ شمسی تراکنش
            var currentDay = DateTime.Now;
            PersianCalendar pcCalender = new PersianCalendar();
            int year = pcCalender.GetYear(currentDay);
            int month = pcCalender.GetMonth(currentDay);
            int day = pcCalender.GetDayOfMonth(currentDay);
            string shmsiDate = string.Format("{0:yyyy/MM/dd}", Convert.ToDateTime(year + "/" + month + "/" + day));

            //بدست آوردن زمان تراکنش
            string getTime = string.Format("{0:HH:mm:ss}", Convert.ToDateTime(currentDay.TimeOfDay.Hours + ":" +
                currentDay.TimeOfDay.Minutes + ":" + currentDay.TimeOfDay.Seconds));

            //ثبت اطلاعات پرداخت در دیتابیس
            using (var database = _context)
            {
                //  این متد ترنز اکشن است و گر کل دستورات دربلوک درست انجام شد کد BeginTransaction
                //ادامه پیدا میکند وگرنه تمام عملیاتی در بلوک ترنزاکشن مینویسیم برگشت میشود 
                //
                using (var transaction = database.Database.BeginTransaction())
                {
                    //گر پرداخت و یا شاژکیف پول هرکدانجام نشود تراکنش برگشت میشود برای اینکه اگر پرداخت موفق بود ولی کیف پول شارز نشد عملیلات برگردد و پول برگردد ه حساب 
                    // هرکاری کرده را برمیکرداند transaction 
                    try
                    {
                        //ثبت اطلاعات تراکنش
                        TransactionPayment p = new TransactionPayment();
                        p.TransactionDate = shmsiDate;
                        p.TransactionTime = getTime;
                        p.Amount = amount;
                        p.Description = description;
                        p.Mobile = mobile;
                        p.Email = Email;

                        //ای دی تراکنش را برمیگرداند
                        p.TransactionNo = verification.RefId.ToString();

                        //ایدی کابری که لاگین کرده است در واقع کاربری که تراکنش نجام میدهد را مشخص میکند
                        p.UserId = _userManager.GetUserId(User);

                        //ذخیره در دیتابیس
                        database.TransactionPayments.Add(p);


                        //شارژ کیف پول
                        //  یعنی کاربری که الان انلاینه و وصله و داره تراکنش انجام مده  getuserId 
                        //کیف پول کابر را اپدیت میکند
                        var updateQuery = (from U in database.Users where U.Id == _userManager.GetUserId(User) select U).SingleOrDefault();
                        updateQuery.Wallet = updateQuery.Wallet + amount;

                        database.SaveChanges();

                        // اجرای  transaction
                        transaction.Commit();

                    }
                    catch
                    {

                    }

                    //شماره تراکنش را میفرستد
                    ViewBag.TransactionNo = verification.RefId.ToString();

                    // مبلغ تراکنش
                    ViewBag.Amount = amount;

                    //تاریخ تراکنش
                    ViewBag.TransactionDate = shmsiDate;

                    //زمان تراکنش
                    ViewBag.TransactionTime = getTime;

                    //نام کاربر و مقدار موجودی کیف پول کاربر را برمیگرداند--------------------------------------------
                    ApplicationUser userFullName = (from u in _context.Users
                                                    where u.Id == _userManager.GetUserId(HttpContext.User)
                                                    select u).SingleOrDefault();
                    ViewBag.wallet = userFullName.Wallet;
                    ViewBag.userFullName = userFullName.FirstName + " " + " " + userFullName.LastName;
                    //---------------------------------------------------------------------------------

                    return View("SuccessfullyPayment");
                }
            }
        }

        #endregion
    }
}
