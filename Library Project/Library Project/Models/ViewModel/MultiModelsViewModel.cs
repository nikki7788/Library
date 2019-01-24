using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.ViewModel
{
    public class MultiModelsViewModel
    {
        public List<Book> LastBook { get; set; }            //با این پارپرتی اخرین کتاب های ثبت شده را برمی گرداند

        public List<Book> ScientificBook { get; set; }      // این پراپرتی کتاب های علمی را برمیگرداند 

        public List<ApplicationUser> LastRegistedUser { get; set; }

        public List<News> LastNews { get; set; }

        public List<Book> SearchBooks { get; set; }

        public List<BookDetailsViewModel> BookDetails { get; set; }

        public List<Book> MostViewedBook { get; set; }

    }
}
