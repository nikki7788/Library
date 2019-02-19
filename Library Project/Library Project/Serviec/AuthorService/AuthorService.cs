using Library.Models;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Serviec.AuthorService
{
    public class AuthorService : IAuthorService
    {
        private readonly ApplicationDbContext _contex;
        public AuthorService(ApplicationDbContext contex)
        {
            _contex = contex;
        }


        /// <summary>
        /// لیست نویسنده هارا میاورد  Index
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<List<Author>> PagingAuthorAsync(int page = 1)
        {
            var model = _contex.Authors.AsNoTracking().Include(a => a.Books).OrderByDescending(a => a.AuthorId);
            PagingList<Author> modelPaging = await PagingList.CreateAsync(model, 4, page);
            return modelPaging;
        }



        /// <summary>
        /// موارد سرچ شده را میاورد
        /// </summary>
        /// <param name="authorSearch"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<List<Author>> SearchAuthorAsync(string authorSearch, int page = 1)
        {
            var model = _contex.Authors.AsNoTracking().Include(a => a.Books).OrderBy(a => a.AuthorId);
            PagingList<Author> modelPaging = await PagingList.CreateAsync(model, 4, page);
            if (authorSearch != null)
            {
                authorSearch = authorSearch.TrimEnd().TrimStart();
                model = model.Where(a => a.AuthorName.Contains(authorSearch)).AsNoTracking().Include(a => a.Books).OrderByDescending(a => a.AuthorId);
                modelPaging = await PagingList.CreateAsync(model, 4, page);

            }

            return modelPaging;
        }




        #region################### این دستورات مال خود لایه سرویس است ه نوشیم    ####################################


        /// <summary>
        /// نویسنده متناطر با ایدی را پیداکرده و برمی گرداند   Read
        ///  عمل میکند   get    هرجا که بخواهیم رکردی از جدول نویسنده ها بیاوریم و از روی دیتابیس بخوانیم از این استفاه میکنیم مانند متد 
        /// </summary  >
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Author> GetAuthorByIdAsync(int id)
        {
            Author author = new Author();
            // میتوانیم مستقیم هم نوع نویسنده را قبل از نام متغیر بنویسیم و از ان نمونه نسازیم 
            //Author author=await......
            author = await _contex.Authors.Where(a => a.AuthorId == id).SingleOrDefaultAsync();
            return author;
        }



        //  نو.شتن ان فیده ای ندارد async متدی که در اخل ان دستوری به صورت غیر همزمان وجود ندارد 

        // کنیم  await  ٫ اگر ان را غیرهمزمان بگیریمن در کنترلر نمیتوانیم ان را    void وقتی متد بدون مقدار بازگشتی باشد   
        // کنیم  await میتواینم ان را درنطر بگیریم در کنترلر  task ولی وقتی از نوع      


        //public async void InsertAuthorAsync(Author author)
        //{
        //    await _contex.Authors.AddAsync(author);
        //}

        //public  void InsertAutho(Author author)
        //{
        //    await _contex.Authors.Add(author);
        //}



            /// <summary>
            /// متد افزودن 
            /// create
            /// </summary>
            /// <param name="author"></param>
            /// <returns></returns>
        public async Task InsertAuthorAsync(Author author)
        {
            await _contex.Authors.AddAsync(author);
        }


        /// <summary>
        /// متد ویرایش
        /// Upadate
        /// </summary>
        /// <param name="author"></param>
        public void EditAuthor(Author author)
        {
            _contex.Entry(author).State = EntityState.Modified;
        }

        /// <summary>
        /// متد حذف
        /// delete
        /// </summary>
        /// <param name="author"></param>
        public void DeleteAuthor(Author author)
        {
            _contex.Entry(author).State = EntityState.Deleted;
        }


        /// <summary>
        /// متد ذخیره اظلاعات در دیتابیس
        /// save
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            await _contex.SaveChangesAsync();
        }



        //  نو.شتن ان فیده ای ندارد async متدی که در اخل ان دستوری به صورت غیر همزمان وجود ندارد 

        // کنیم  await  ٫ اگر ان را غیرهمزمان بگیریمن در کنترلر نمیتوانیم ان را    void وقتی متد بدون مقدار بازگشتی باشد   
        // کنیم  await میتواینم ان را درنطر بگیریم در کنترلر  task ولی وقتی از نوع      


        //public void Save()
        //{
        //    _contex.SaveChanges();
        //}

        #endregion


    }

}