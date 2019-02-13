using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Library.Areas.User.Controllers
{
    [Area("Admin")]
    [Authorize("User")]
    public class PaymentController : Controller
    {
        // GET: /<controller>/
        [HttpGet]
        public IActionResult Payment()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Payment(TransactionPayment pTr)
        {
            if (!ModelState.IsValid)
            {
                return View("Payment", pTr);
            }

            //جلسه ۱۲۷و۱۲۸- دستورات پرداخت زرین پال
            //قسمت ارسال به زرین پال
            // فقط تغییر داد       ZarinpalSandbox   استفاده از درگاه تستی- باری درگاه اصلی باید             
            var payment = await new ZarinpalSandbox.Payment(pTr.Amount).PaymentRequest(pTr.Description,
                Url.Action(nameof(PaymentVerify), "Payment", new {

                    description = pTr.Description,
                    email = pTr.Email,
                    mobile = pTr.Mobile },Request.Scheme),pTr.Email,pTr.Description);

            //در صورت موفقیت آمیز بودن درخواست، کاربر را به صفحه پرداخت هدایت کن
            //در غیر این صورت باید خطا نمایش داده شود

            return payment.Status == 100 ? (IActionResult)Redirect(payment.Link) :
                BadRequest($"خطا در پرداخت. کد خطا :  {payment.Status} ");
        }
    }
}
