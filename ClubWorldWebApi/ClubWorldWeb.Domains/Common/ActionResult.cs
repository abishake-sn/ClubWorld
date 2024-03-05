using System;
using System.Collections.Generic;
using System.Text;

namespace ClubWorldWeb.Domains.Common
{
    public class ClubWorldWebActionResult
    {
        public string result { get; set; }
        public List<string> ErrorMsgs { get; set; } = new List<string>();
    }
}
