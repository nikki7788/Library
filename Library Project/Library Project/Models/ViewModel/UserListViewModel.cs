using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.ViewModel
{
    public class UserListViewModel
    {
        //for showing in Index view and the list of the user and thier roles
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }


    }
}
