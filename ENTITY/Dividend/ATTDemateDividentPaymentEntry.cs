﻿namespace Entity.Dividend
{
    public class ATTDemateDividentPaymentEntry
    {

        public string Compcode { get; set; }
        public string warrantno { get; set; }
        public string BO_idno { get; set; }
        public string fullname { get; set; }
        public string faname { get; set; }
        public string grfaname { get; set; }
        public string address { get; set; }
        public string AgmNo { get; set; }
        public string divType { get; set; }
        public string Seqno { get; set; }
        public bool? DPayable { get; set; }
        public double? warrantamt { get; set; }
        public double? taxdamt { get; set; }
        public double? bonustax { get; set; }
        public double? bonusadj { get; set; }
        public double? prevadj { get; set; }
        public float? totshkitta { get; set; }
        public bool? WIssued { get; set; }
        public string WIssuedDt { get; set; }
        public string dwiby { get; set; }
        public string CenterId { get; set; }
        public string remarks { get; set; }
        public string Payment { get; set; }
        public string PayUser { get; set; }
        public string isin_no { get; set; }
        public string Batchno { get; set; }
        public string wissue_Approved { get; set; }
        public string wissue_Approvedby { get; set; }
        public string wissue_app_date { get; set; }
        public string wissue_status { get; set; }
        public string wissue_auth_remarks { get; set; }
        public bool? WPaid { get; set; }
        public string WAmtPaidDt { get; set; }
        public string WPaidBy { get; set; }
        public string wpaid_approved { get; set; }
        public string wpaid_app_date { get; set; }
        public string wpaid_status { get; set; }
        public string wpaid_auth_remarks { get; set; }
        public string wpaid_approvedby { get; set; }
        public string bankname { get; set; }
        public string bankaccno { get; set; }
        public string bankadd { get; set; }
        public string crediteddt { get; set; }
    }
}
