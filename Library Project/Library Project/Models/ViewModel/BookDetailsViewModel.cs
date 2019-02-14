using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.ViewModel
{
    public class BookDetailsViewModel
    {
        [Key]
        public int BookId { get; set; }
        [Display(Name = "نام کتاب")]
        public string BookName { get; set; }

        [Display(Name = "توضیحات کتاب")]
        public string BookDescription { get; set; }

        [Display(Name = "تعداد صفحات کتاب")]
        public int BookPageCount { get; set; }

        [Display(Name = "تعداد موجودی کتاب")]
        public int BookStock { get; set; }

        [Display(Name = "تعداد بازدید کتاب")]
        public int BookViews { get; set; }

        public int BookLikeCount { get; set; }

        public int BookDislike { get; set; }

        [Display(Name = "تصویرکتاب")]
        public string BookImage { get; set; }

        [Display(Name = "نویسنده")]
        public string AuthorName { get; set; }

        [Display(Name = "گروه بندی کتاب")]
        public string BookGroupName { get; set; }

        [Display(Name = "قیمت کتاب")]
        public int Price { get; set; }

        public int AuthorId { get; set; }

        public int BookGroupID { get; set; }




    }
}
