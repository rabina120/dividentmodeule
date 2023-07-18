using System;

namespace Entity.Dividend
{
    public class ATTDivMaster
    {
        public string compcode { get; set; }
        public string WarrantNo { get; set; }
        public string ShHolderNo { get; set; }
        public string AgmNo { get; set; }
        public string divType { get; set; }
        public string seqno { get; set; }
        public bool? DPayable { get; set; }
        public double? WarrantAmt { get; set; }
        public double? TaxDamt { get; set; }
        public double? bonustax { get; set; }
        public double? bonusadj { get; set; }
        public double? prevadj { get; set; }
        public float TotShKitta { get; set; }
        public bool? WIssued { get; set; }
        public DateTime? WIssuedDt { get; set; }
        public string dwiby { get; set; }
        public string wissue_Approved { get; set; }
        public DateTime? wissue_app_date { get; set; }
        public string wissue_status { get; set; }
        public string wissue_auth_remarks { get; set; }
        public string wissue_approvedby { get; set; }
        public bool? WPaid { get; set; }
        public DateTime? WAmtPaidDt { get; set; }
        public string WPaidBy { get; set; }
        public string wpaid_approved { get; set; }
        public DateTime? wpaid_app_date { get; set; }
        public string wpaid_status { get; set; }
        public string wpaid_auth_remarks { get; set; }
        public string wpaid_approvedby { get; set; }
        public string CashOrTran { get; set; }
        public string centerid { get; set; }
        public string telno { get; set; }
        public string remarks { get; set; }
        public string batchno { get; set; }
        public string boidno { get; set; }
        public string Column_Name { get; set; }
    }
}
