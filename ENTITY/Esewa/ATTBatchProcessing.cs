using System;

namespace Entity.Esewa
{
    public partial class ATTBatchProcessing
    {
        public string Compcode { get; set; }
        public string DivCode { get; set; }
        public string FullName { get; set; }
        public string FatherName { get; set; }
        public string GrandFatherName { get; set; }
        public int? ShHolderNo { get; set; }
        public string BoidNo { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string BankNo { get; set; }
        public string WarrantNo { get; set; }
        public string ContactNo { get; set; }
        public double? TaxDamt { get; set; }
        public double? bonustax { get; set; }
        public double? bonusadj { get; set; }
        public double? TotalAmt { get; set; }
        public double? WarrantAmt { get; set; }
        public string App_status { get; set; }
        public string Approved { get; set; }
        public string approvedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string EsewaCode { get; set; }
        public string EsewaStatus { get; set; }
        public string sub_token { get; set; }
        public bool ValidAccount { get; set; }
        public string ValidateCode { get; set; }
        public string ValidateMessage { get; set; }
        public string TransactionCode { get; set; }
        public string TransactionMessage { get; set; }
        public string TransactionDetail { get; set; }
        public string QueryCode { get; set; }
        public string QueryMessage { get; set; }
        public string QueryDetail { get; set; }
        public string NotificationCode { get; set; }
        public string NotificationMessage { get; set; }
        public string NotificationDetail { get; set; }

    }

    public partial class ATTBatchProcessing
    {

        public int? TotalCount { get; set; }
        public int? FilteredCount { get; set; }
    }

}
