using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ClubWorldWeb.Domains.Models.Config
{
   
    public class ConfigUserRoles : Model
    {
       
        public int UserId { get; set; }
      
        public int RoleId { get; set; }
      

    }
}
