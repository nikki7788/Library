using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option) : base(option)
        {

        }

        #region#######---------------------    Tables    ----------------------------########
        public DbSet<Author> Authors { get; set; }

        public DbSet<BookGroup> BookGroups { get; set; }

        public DbSet<Book> Books { get; set; }
        #endregion
        public DbSet<News> News { get; set; }
    }
}
