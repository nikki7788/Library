using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.ViewModel
{
    public class AddEditBookViewModel
    {
        //the [key] attribute is optional in view models because 
        //view models don't create as a table in the database as a result don't need primary key
        [Key]
        public int BookId { get; set; }

        [Display(Name = "نام کتاب:")]
        [Required(ErrorMessage = "لطفا نام کتاب  را وارد کنید")]
        public string BookName { get; set; }


        [Display(Name = "توضیحات")]
        [Required(ErrorMessage = "لطفا توضیحات را وارد کنید")]
        public string BookDescription { get; set; }

        [Display(Name = "تصویر کتاب")]
        // [Required(ErrorMessage = "لطفا تصویر کتاب  را ارسال کنید")]
        public string BookImage { get; set; }

        [Display(Name = "تعداد صفحات")]
        //  [Required(ErrorMessage = "لطفا تعداد صفخات کتاب را وارد کنید")]
        public int BookPageCount { get; set; }


        [Display(Name = "قیمت کتاب")]
        [Required(ErrorMessage = "قیمت کناب را وارد کنید.توجه کنید مبلغ قیمت کتاب حتماباید بیش از 100 تومان باشد")]
        public int Price { get; set; }
        #region###############----- Dropdown list--######################

        [Display(Name = "گروه بندی کتاب")]
        public int BookGroupId { get; set; }

        public List<SelectListItem> BookGroups { get; set; }

        //--------------------------------------------
        [Display(Name = "نویسنده")]
        public int AuthorId { get; set; }

        public List<SelectListItem> Authors { get; set; }

        #endregion #######################


    }
}
