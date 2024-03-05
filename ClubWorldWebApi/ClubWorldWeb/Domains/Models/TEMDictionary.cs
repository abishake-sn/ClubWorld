using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ClubWorldWeb.Domains
{
    public static class TEMDictionary
    {
        public static string MemberUser = "member";
        public static string AdminUser = "admin";
        public static string CustomerUser = "Customer";
        public static string ActiveStaus = "Active";
        public static string InactiveStaus = "Inactive";
        public static Int16 Isactive = 1;
        public static Int16 InActive = 0;
        public static string ResultSuccess = "Success";
        public static string ResetPassword = "ResetPassword";
        public static string FirstLogin = "No"; 

        public static class Error
        {
            public static string ResultFailed = "Failed";
            public static string RecordNotFound = "Record Not Found";
            public static string ActiveStatus = "Expired";
        }
        
    }  
    
}
