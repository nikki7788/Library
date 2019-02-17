using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models
{
    public class ManageReqestedBookViewModel
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int BookId { get; set; }
        public byte Flag { get; set; }

        [Display(Name ="وضعیت درخواست")]
        public string FlageState { get; set; }

        [Display(Name = "موجودی")]
        public int BookStock { get; set; }

        [Display(Name ="نام کتاب")]
        public string BookName { get; set; }

        [Display(Name ="نام کاربر")]
        public string UserFullName { get; set; }

        [Display(Name ="تاریخ درخواست")]
        public string RequestDate { get; set; }

        [Display(Name ="تاریخ پاسخ")]
        public string AnswerDate { get; set; }

        [Display(Name ="تاریخ برگشت")]
        public string ReturnDate { get; set; }

        [Display(Name = "قیمت کتاب")]
        public int Price { get; set; }



    }
}
