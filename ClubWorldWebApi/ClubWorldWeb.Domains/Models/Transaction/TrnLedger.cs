using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ClubWorldWeb.Domains.Models.Transaction
{
    public class TrnLedger: LogModel
    {
        [JsonPropertyName("externalid")] public int ExternalId { get; set; }
        [JsonPropertyName("acyear")] public int? AcYear { get; set; }
        [JsonPropertyName("voutype")] public string VouType { get; set; }
        [JsonPropertyName("vouno")] public int VouNo { get; set; }
        [JsonPropertyName("voudate")] public DateTime VouDate { get; set; }
        [JsonPropertyName("sno")] public int SNo { get; set; }
        [JsonPropertyName("ledger1")] public int Ledger1 { get; set; }
        [JsonPropertyName("ledger2")] public int Ledger2 { get; set; }
        [JsonPropertyName("ledgername1")] public string Ledger1Name { get; set; }
        [JsonPropertyName("ledgername2")] public string Ledger2Name { get; set; }
        [JsonPropertyName("credit")][Column(TypeName = "decimal(18,2)")] public decimal Credit { get; set; }
        [JsonPropertyName("debit")][Column(TypeName = "decimal(18,2)")] public decimal Debit { get; set; }
        [JsonPropertyName("balance")][Column(TypeName = "decimal(18,2)")] public decimal Balance { get; set; }
        [JsonPropertyName("notes")] public string Notes { get; set; }
        [JsonPropertyName("activestatus")] public string ActiveStatus { get; set; }
        [JsonPropertyName("syncstatus")] public string SyncStatus { get; set; }
        [JsonPropertyName("paymentType")] public string PaymentType { get; set; }

    }
}
