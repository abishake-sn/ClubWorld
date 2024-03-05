using System;
using System.Collections.Generic;
using System.Text;

namespace ClubWorldWeb.Domains.Common
{
  public class AuthResult
  {
    public string Token { get; set; }
    public string UserType { get; set; }
    public int UserId { get; set; }
    public string UserCode { get; set; }
    public string UserName { get; set; }
        public string result { get; set; }
    public List<string> ErrorMsgs { get; set; }
    }
}
