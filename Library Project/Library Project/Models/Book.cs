using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models
{
    /// <summary>
    ///  کتاب ها
    /// books
    /// </summary>

    public class Book

    {
        [Key]
        public int BookId { get; set; }

        public string BookName { get; set; }

        public string BookDescription { get; set; }

        public int BookPageCount { get; set; }

        public string BookImage { get; set; }


        #region##---------#######-----   ForeignKeys and thier references   --------######-----------###

        public int AuthorId { get; set; }

        public int BookGroupId { get; set; }

        //--------------  ForeignKey references   ---------------------//

        [ForeignKey("AuthorId")]
        public virtual Author Authors { get; set; }

        [ForeignKey("BookGroupId")]
        public virtual BookGroup BookGroups { get; set; }

        #endregion###############################






    }
}
