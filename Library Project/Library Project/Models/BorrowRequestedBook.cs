using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models
{
    public class BorrowRequestedBook
    {
        //برای ثبت درخواست هاي کاربر در دیتابیس یک جدول ایجاد میکنیم

        [Key]
        public int Id { get; set; }                            //آی دی درخواست

        public int BookId { get; set; }                        //آی دی کتاب درخواستی

        public string UserId { get; set; }                 //ای دی کاربری که درخواست داده

        //وضعیت درخواست را مشخص میکند
        //     تایید 2       کنسل3     ۱درخواست داده شده     
        public byte Flag { get; set; }

        [Display(Name = "تاریخ درخواست")]
        public string RequestDate { get; set; }

        [Display(Name = "تاریخ پاسخ")]
        public string AnswerDate { get; set; }

        [Display(Name = "تاریخ برگشت")]
        public string ReturnDate { get; set; }

        [Display(Name = "قیمت کتاب")]
        public int Price { get; set; }


    }
}
