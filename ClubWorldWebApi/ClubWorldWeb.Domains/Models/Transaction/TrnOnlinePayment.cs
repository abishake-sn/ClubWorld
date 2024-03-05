using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ClubWorldWeb.Domains.Models.Transaction
{
    public class TrnOnlinePayment: LogModel
    {
        [JsonPropertyName("acyear")] public int AcYear { get; set; }
        [JsonPropertyName("vouno")] public int VouNo { get; set; }
        [JsonPropertyName("voudate")] public DateTime VouDate { get; set; }
        [JsonPropertyName("memberid")] public int MemberId { get; set; }
        [JsonPropertyName("membercode")] public string MemberCode { get; set; }
        [JsonPropertyName("credit")][Column(TypeName = "decimal(18,2)")] public decimal Credit { get; set; }
        [JsonPropertyName("debit")][Column(TypeName = "decimal(18,2)")] public decimal Debit { get; set; }
        [JsonPropertyName("notes")] public string Notes { get; set; }
        [JsonPropertyName("transactionno")] public string TransactionNo { get; set; }
        [JsonPropertyName("transactionstatus")] public string TransactionStatus { get; set; }
        [JsonPropertyName("paymentType")] public string PaymentType { get; set; }
        [JsonPropertyName("toaccount")] public int ToAccount { get; set; }
        [JsonPropertyName("activestatus")] public string ActiveStatus { get; set; }
        [JsonPropertyName("syncstatus")] public string SyncStatus { get; set; }


    }
}
