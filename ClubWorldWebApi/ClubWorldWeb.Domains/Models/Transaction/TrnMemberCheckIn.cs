using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ClubWorldWeb.Domains.Models.Transaction
{
    public class TrnMemberCheckIn : LogModel
    {
        [JsonPropertyName("acyear")] public int AcYear { get; set; }
        [JsonPropertyName("memberid")] public int MemberId { get; set; }
        [JsonPropertyName("dependantid")] public int DependantId { get; set; }
        [JsonPropertyName("checkintime")] public DateTime CheckInTime { get; set; }
        [JsonPropertyName("checkouttime")] public DateTime CheckOutTime { get; set; }
        [JsonPropertyName("entrytype")] public string EntryType { get; set; }
        [JsonPropertyName("facilityid")] public int FacilityId { get; set; }
        [JsonPropertyName("isactive")] public int Isactive { get; set; }
        [JsonPropertyName("externalid")] public int ExternalId { get; set; }       
    }
}
