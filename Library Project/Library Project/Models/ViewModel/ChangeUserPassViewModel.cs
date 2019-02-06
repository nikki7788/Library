using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.ViewModel
{
    public class ChangeUserPassViewModel
    {
        [Key]
        public string Id { get; set; }    //آی دی کاربر را برمیگرداند

        [Display(Name = " رمز عبور قدیمی :")]
        [Required(ErrorMessage = "لطفا رمز عبور قدیمی را وارد کنید")]
        [DataType(DataType.Password, ErrorMessage = "رمز عبور باید شامل حروف بزرگ کوچک و کاراکتر و عدد باشد")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "حداقل رمز باید بین 6 تا 20 کاراکتر  باشد")]
        public string OldPassword { get; set; }

        [Display(Name = "رمز عبور جدید :")]
        [Required(ErrorMessage = "لطفا رمز عبور جدید را وارد کنید")]
        // [DataType(DataType.Password, ErrorMessage = "رمز عبور باید شامل حروف بزرگ کوچک و کاراکتر و عدد باشد")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "حداقل رمز باید بین 6 تا 20 کاراکتر  باشد")]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{6,}$", ErrorMessage = "رمز عبور باید ترکیبی از حروف کوچک و بزرگ و عدد و علامت باشد")]
        public string NewPassword { get; set; }

        [Display(Name = "تایید رمز عبور جدید :")]
        [Required(ErrorMessage = "لطفا  تایید رمز عبور جدید را وارد کنید")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "لطفا  تایید رمز عبور را یکسان وارد کنید")]
        public string ConfirmNewPassword { get; set; }

    }
}
