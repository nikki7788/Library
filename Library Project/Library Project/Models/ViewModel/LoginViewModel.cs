using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.ViewModel
{
    public class LoginViewModel
    {
        [Display(Name ="نام کاربری :")]
        [Required(ErrorMessage = "لطفا نام کاربری را وارد کنید.")]
        public string UserName { get; set; }

        [Display(Name = "رمز عبور :")]
        [Required(ErrorMessage = "لطفا رمز عبور را وارد کنید.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "مرا بخاطر بسپار")]
        public bool RememberMe { get; set; }

    }
}
