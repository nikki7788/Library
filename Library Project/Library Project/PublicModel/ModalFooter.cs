using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.PublicModel
{
    public class ModalFooter
    {

        //آی دی دکمه ها را مقدار دهی کردیم چون در کل پروژه ثابت هستند
        public string CancelbuttonId { get; set; } = "btn-cancel";

        //چون متن دکمه برگشت ثابت است مقدار ان را همینجا تعریف کردیم به فارسی
        public string CancelbuttonText { get; set; } = "برگشت";
        public string SubmitButtonId { get; set; } = "btn-submit";
        public string SubmitButtonText { get; set; } = "submit";

        //برای مودال هایی که فقط یک دکمه بیشتر ندارند مثلا فقط برگشت مودال اطلاع رسانی و خبری هستند چیزی ارسال نمیکنند
        public bool OnlyButton { get; set; }


    }
}
