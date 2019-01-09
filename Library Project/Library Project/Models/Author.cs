using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models
{

    /// <summary>
    /// نویسنده ها
    /// Authors
    /// </summary>
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }

        [Display(Name ="نام نویسنده")]
        [Required(ErrorMessage ="لطفا نام نویسنده را وارد کنید")]
        public string AuthorName { get; set; }

        [Display(Name ="توضیحات نویسنده")]
        [Required(ErrorMessage = "لطفا توضیحات نویسنده را وارد کنید")]
        public string AuthorDescription { get; set; }
    }
}
