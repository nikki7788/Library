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

        [Display(Name = "نام و نام خانوادگی")]
        [Required(ErrorMessage = "لطفا نام و نام خانوادگی را وارد کنید")]
        public string FullName { get; set; }

        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "لطفا ایمیل را وارد کنید")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "رمز عبور")]
        [Required(ErrorMessage = "لطفا رمز عبور را وارد کنید")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "تایید رمز عبور")]
        [Required(ErrorMessage = "لطفا  تایید رمز عبور را وارد کنید")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        #region---##################-  Dropdown list -################################
        /// for showing Roles in dropDown list

        //مقادیر داخل متغیری از این نوع ریخته میشود و 
        //این پارپرتی مقادیر را برمیکرداند از جدول رل ها که در خود ام وی سی ساخته شده است
        //text in select list=r.Name and value in select list=r.Id
        public List<SelectListItem> ApplicationRoles { get; set; }

        //در اینجا نام کمبو باکس را نمایش می دهد یعنی نقش
        
        [Display(Name = "نقش")]
        public string ApplicationRoleId { get; set; }

        #endregion

    }
}
