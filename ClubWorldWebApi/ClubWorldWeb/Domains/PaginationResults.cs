using System;
using System.Collections.Generic;
using System.Text;

namespace ClubWorldWeb.Domains
{
    public class PaginationResults
    {
        public int RecordsTotal { get; set; }
        public int RecordsPerPage { get; set; }
        public int PagesTotal { get; set; }
        public int PageIndex { get; set; }
    }

    public class PaginationResults<T> : PaginationResults
    {
        public List<T> PageRecords { get; set; } = new List<T>();
    }
}
