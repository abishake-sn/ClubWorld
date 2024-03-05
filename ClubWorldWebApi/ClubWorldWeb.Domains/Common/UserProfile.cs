using ClubWorldWeb.Domains.Models.Config;
//using ClubWorldWeb.Domains.Models.Master.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClubWorldWeb.Domains.Common
{
    public class UserProfile
    {
        public ConfigUser User { get; set; }
        public ConfigRole Role { get; set; }
        public int? ReelerId { get; set; }
        public int? StaffId { get; set; }
    }
}