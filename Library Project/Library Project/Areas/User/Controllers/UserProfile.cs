using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Area.User.Controllers
{
    [Area("UserProfile")]
    public class UserProfile:Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
