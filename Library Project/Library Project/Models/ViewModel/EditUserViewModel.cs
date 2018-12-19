using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.ViewModel
{
    public class EditUserViewModel
    {

        public string Id { get; set; }

        [Display(Name ="نام و نام خانوادگی")]
        [Required(ErrorMessage = "لطفا نام و نام خانوادگی را وارد نمایید")]
        public string FullName { get; set; }

        [Display(Name ="ایمیل")]
        [Required(ErrorMessage = "لطفا ایمیل را وارد نمایید")]
        public string Email { get; set; }

        #region---##################-  Dropdown list -################################
        /// for showing Roles in dropDown list


        //مقادیر داخل متغیری از این نوع ریخته میشود و 
        //این پارپرتی مقادیر را برمیکرداند از جدول رل ها که در خود ام وی سی ساخته شده است
        //text in select list=r.Name and value in select list=r.Id
        public List<SelectListItem> ApplicationRoles { get; set; }

      
      
        //نقش و رل کاربری که برای ویرایش انتخاب کردیم را در این پارپرتی میریزیم.
        //در واقع آی دی نقش و رل رامیریزیم چون
        //value in selectlist=role.id
        //چون بانوع یوزر و رل کار میکنیم باید ای ید از نوع استربنگ باشد
        [Display(Name ="نقش")]
        public string ApplicationReleId { get; set; }
        #endregion
    }
}
