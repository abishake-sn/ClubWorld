using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ClubWorldWeb.Domains.Models.Config
{

  public class ConfigRole : Model
  {
    public string RoleName { get; set; }
    public string Type { get; set; }
    public Int16 Isactive { get; set; } = 1;
  }

  public class Role : IdentityRole<int>
  {

  }
}
