using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace _1_ShopApp.Identity
{
    public class User:IdentityUser
    {
        public string FirstName { get; set; }   
        public string LastName { get; set; }    
        
    }
}