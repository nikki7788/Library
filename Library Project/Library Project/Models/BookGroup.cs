using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models
{
    public class BookGroup
    {
        /// <summary>
        /// گروه بندی موضوع کتاب ها
        /// grouping book topics
        /// </summary>

        [Key]
        public int BookGroupId { get; set; }

        public string BookGroupName { get; set; }

        public string BookGroupDescription { get; set; }

    }
}
