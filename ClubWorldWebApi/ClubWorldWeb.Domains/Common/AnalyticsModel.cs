using ClubWorldWeb.Domains.Models.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClubWorldWeb.Domains.Common
{
    public class AnalyticsModel
    {
        public int KeyId { get; set; }
        public string KeyCode { get; set; }
        public string KeyName { get; set; }
        public int ProjectCount { get; set; }
        public decimal ProjectValue { get; set; }
    }
    
}
