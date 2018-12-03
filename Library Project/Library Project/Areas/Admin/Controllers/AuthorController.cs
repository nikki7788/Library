using Library.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class AuthorController : Controller
    {

        private readonly ApplicationDbContext _contex;
        private readonly IServiceProvider _iServicePovider;

        public AuthorController(ApplicationDbContext context, IServiceProvider iServiceProvider)
        {
            _contex = context;
            _iServicePovider = iServiceProvider;
        }


        [HttpGet]
        public IActionResult Index()
        {

            List<Author> model = new List<Author>();

            model = _contex.Authors.Select(a => new Author
            {
                AuthorId = a.AuthorId,
                AuthorName = a.AuthorName,
                AuthorDescription = a.AuthorDescription

            }).ToList();


            return View(model);
        }

    }
}
