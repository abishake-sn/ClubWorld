using System;
using System.Collections.Generic;
using System.Text;

namespace ClubWorldWeb.Domains.Common
{
    public class ClubWorldWebActionResult<T>
    {
        public T Result { get; set; }
        public List<string> ErrorMsgs { get; set; } = new List<string>();
    }
}
