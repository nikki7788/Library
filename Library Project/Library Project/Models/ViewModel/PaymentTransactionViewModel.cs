using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.ViewModel
{
    public class PaymentTransactionViewModel
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "تاریخ تراکنش")]
        public string TransactionDate { get; set; }

        [Display(Name = "زمان تراکنش")]
        public string TransactionTime { get; set; }

        [Display(Name = "مبلغ")]
        public int Amount { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "ایمیل را وارد نمایید")]
        public string Email { get; set; }

        [Display(Name = "شماره تماس")]
        [Required(ErrorMessage = "شماره تماس را وارد نمایید")]
        public string Mobile { get; set; }

        [Display(Name = "شماره تراکنش")]
        public string TransactionNo { get; set; }

        [Display(Name = "نام کاربر")]
        public string FullNameUser { get; set; }
        /// ////////////////////////////////////////////////
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser Users { get; set; }


    }
}
