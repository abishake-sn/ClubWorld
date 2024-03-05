using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace ClubWorldWeb.Domains.Models.Config
{

    public class ConfigUser : IdentityUser<int>
    {
        public string Name { get; set; }
      
        public string Password { get; set; }
        public Int16 Isactive { get; set; } = 1;
    
    }
}