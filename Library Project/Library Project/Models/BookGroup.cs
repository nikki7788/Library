using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models
{
    public class BookGroup
    {
        /// <summary>
        /// گروه بندی موضوع کتاب ها
        /// grouping book topics
        /// </summary>

        [Key]
        public int BookGroupId { get; set; }

        [Display(Name ="نام گروه")]
        [Required(ErrorMessage = "لطفا نام گروه بندی را وارد کنید")]
        public string BookGroupName { get; set; }

        [Display(Name ="توضیحات گروه")]
        [Required(ErrorMessage = "لطفا توضیحات گروه بندی را وارد کیند")]
        public string BookGroupDescription { get; set; }

        //نیاز به ماگریشن ندارد اضافه کردن این پراپرتی
        //کتاب های هرگروه بندی را می دهد
        public virtual ICollection<Book> Books { get; set; }

    }
}
