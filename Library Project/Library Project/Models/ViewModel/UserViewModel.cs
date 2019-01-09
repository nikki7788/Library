using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.ViewModel
{
    public class UserViewModel
    {

        public string Id { get; set; }

        [Display(Name = "نام کابری")]
        [Required(ErrorMessage = "لطفا نام کاربری  را وارد کنید")]
        public string UserName { get; set; }

        [Display(Name = "نام  ")]
        [Required(ErrorMessage = "لطفا نام  را وارد کنید")]
        public string FirstName { get; set; }

        [Display(Name = " نام خانوادگی")]
        [Required(ErrorMessage = "لطفا  نام خانوادگی را وارد کنید")]
        public string LastName { get; set; }


        [Display(Name = " تلفن")]
       
        public string PhoneNumber { get; set; }

        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "لطفا ایمیل را وارد کنید")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "رمز عبور")]
        [Required(ErrorMessage = "لطفا رمز عبور را وارد کنید")]
        [DataType(DataType.Password,ErrorMessage ="رمز عبور باید شامل حروف بزرگ کوچک و کاراکتر و عدد باشد")]
        [StringLength(20,MinimumLength =6,ErrorMessage ="حداقل رمز باید بین 6 تا 20 کاراکتر  باشد")]
        public string Password { get; set; }

        [Display(Name = "تایید رمز عبور")]
        [Required(ErrorMessage = "لطفا  تایید رمز عبور را وارد کنید")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage = "لطفا  تایید رمز عبور را یکسان وارد کنید")]   //مقایسه با پسورد اول
        public string ConfirmPassword { get; set; }

        #region---##################-  Dropdown list -################################
        /// for showing Roles in dropDown list

        //این پراپرتی کمیو باکس را ایجاد میکند و همه مقادیر موجود برای کبو باکس را از جدول رل ها که در خود ام وی سی ساخته شده است برمیگرداند  
        //text in select list=r.Name and value in select list=r.Id
        //این کمبوباکس را میسازد واظلاعات و مقادیر را در این میریزیم
        public List<SelectListItem> ApplicationRoles { get; set; }

        //در اینجا نام کمبو باکس را نمایش می دهد یعنی نقش
        //در واقع آی دی مورد انتخابی از کمبو باکس یا ادی دی مورد انتخاب شده توسط کاربرانتخابی برای ویرایش   رامیریزیم داخل این متغیر
        //value in selectlist=role.id
        //چون بانوع یوزر و رل کار میکنیم باید ای ید از نوع استربنگ باشد
        [Display(Name = "نقش")]
        public string ApplicationRoleId { get; set; }

        #endregion

    }
}
