using System;

namespace Entity.Dividend
{
    public class ATTHolderHistory
    {

        public string FiscalYr { get; set; }
        public string Status { get; set; }
        public string WarrantNo { get; set; }
        public string HolderNo { get; set; }
        public string BOID { get; set; }

        public string kitta { get; set; }

        public string DividendAmt { get; set; }
        public string DivTax { get; set; }
        public string BonusTax { get; set; }
        public string BonusAdj { get; set; }
        public string PrevAdj { get; set; }
        public string PayableAmt { get; set; }

        public DateTime? IssueDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public DateTime? CreditedDt { get; set; }

        public string Bankname { get; set; }
        public string Bankaccno { get; set; }
        public string Remarks { get; set; }

        public string CertNo { get; set; }
        public string SrnoFrom { get; set; }
        public string SrnoTo { get; set; }

        public string PrevFraction { get; set; }
        public string ActualBonus { get; set; }
        public string FractionWithAB { get; set; }
        public string IssueBonus { get; set; }
        public string RemFraction { get; set; }
        public DateTime? ApprovedDate { get; set; }

    }
}
