using ClubWorldWeb.Domains.Models.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClubWorldWeb.Domains.Common
{
    public class TrnLedgerRptInfo
    {
        public int Id { get; set; }
        public int Ledger { get; set; }    
        public DateTime? VouDate { get; set; }
        public string VouType { get; set; }
        public string? ReceiptType { get; set; }
        public int VouNo { get; set; }
        public int SNo { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public decimal Balance { get; set; }
        public string Notes { get; set;}
        public string? RefNo { get; set; }
        public string? PaymentType { get; set; }
    }
   
}
