using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models
{
    public class News
    {
        [Key]
        public int NewsId { get; set; }

        [Display(Name ="عنوان خبر")]
        [Required(ErrorMessage ="لطفا عنوان خبر را وارد کنید")]
        public string NewsTitle { get; set; }

        [Display(Name = "متن خبر")]
        [Required(ErrorMessage = "لطفا متن خبر را وارد کنید")]
        public string NewsContent { get; set; }

        [Display(Name = "تاریخ خبر")]
        public string NewsDate { get; set; }

        [Display(Name ="تصویر")]
        public string NewsImage { get; set; }

    }
}
