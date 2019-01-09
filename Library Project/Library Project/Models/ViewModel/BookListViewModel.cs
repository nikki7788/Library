using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.ViewModel
{
    public class BookListViewModel
    {

        public int BookId { get; set; }

        [Display(Name = "نام کتاب")]
        public string BookName { get; set; }

        [Display(Name = "تصویر کتاب")]
        public string BookImage { get; set; }

        [Display(Name = "تعداد صفحات کتاب")]
        public int BookPageCount { get; set; }

        [Display(Name = "نویسنده")]
        public string AuthorName { get; set; }

        public int AuthorId { get; set; }

        [Display(Name = "نام گروه بندی کتاب")]
        public string BookGroupName { get; set; }

        public int BookGroupId { get; set; }


    }
}
