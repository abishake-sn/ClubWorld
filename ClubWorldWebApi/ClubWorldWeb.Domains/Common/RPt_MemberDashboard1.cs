using ClubWorldWeb.Domains.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.AspNetCore.Identity;
using ClubWorldWeb.Domains.Models;


namespace ClubWorldWeb.Domains.Common
{
    public class RPt_MemberDashboard1

    {
        public int Id { get; set; }

        [JsonPropertyName("currentbalance")]
        public decimal? CurrentBalance { get; set; }

        [JsonPropertyName("attendance")]
        public int? Attendance { get; set; }

        [JsonPropertyName("usageamount")]
        public decimal? UsageAmount { get; set; }

        [JsonPropertyName("lastvisit")]
        public DateTime? LastVisit { get; set; }

        [JsonPropertyName("payment")]
        public decimal Payment { get; set; }

    }
   
}
