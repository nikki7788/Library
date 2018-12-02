using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models
{

    /// <summary>
    /// نویسنده ها
    /// Authors
    /// </summary>
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }

        public string aAuthorName { get; set; }

        public string AuthorDescription { get; set; }
    }
}
