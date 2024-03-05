using System;
using System.Collections.Generic;
using System.Text;

namespace ClubWorldWeb.Domains.Common
{
    public class FileUploadRequest
    {
        public int Id { get; set; }
        public string DocumentName { get; set; }
        public string img64 { get; set; }
    }
}