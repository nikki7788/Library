using Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Serviec.AuthorService
{
    public interface IAuthorService
    {

        Task<List<Author>> PagingAuthorAsync(int page = 1);

        Task<List<Author>> SearchAuthorAsync(string authorSearch, int page = 1);

        Task<Author> GetAuthorByIdAsync(int id);

     

        Task InsertAuthorAsync(Author author);

        void EditAuthor(Author author);

        void DeleteAuthor(Author author);

         Task SaveAsync();

       // void Save();
   //  void  InsertAuthorAsync(Author author);

    }
}
