using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ClubWorldWeb.Domains.Models.Master
{
    public class MasMember:LogModel
    {
        [JsonPropertyName("memberid")] public int MemberId { get; set; }
        [JsonPropertyName("membercode")] public string MemberCode { get; set; }
        [JsonPropertyName("membername")] public string MemberName { get; set; }
        [JsonPropertyName("mobileno")] public string MobileNo { get; set; }
        [JsonPropertyName("emailid")] public string EmailId { get; set; }
        [JsonPropertyName("membertype")] public string MemberType { get; set; }
        [JsonPropertyName("activestatus")] public string ActiveStatus { get; set; }
        [NotMapped]
        [JsonPropertyName("password")] public string Password { get; set; }

    }
}
