namespace Entity.Dividend
{
    public class ATTSummaryBulkEntryPosting
    {
        public string WarrantAmt { get; set; }
        public string TaxDamt { get; set; }
        public string BonusTax { get; set; }
        public string BonusAdj { get; set; }
        public string PrevAdj { get; set; }
        public string TotShKitta { get; set; }
        public string PrevTotKitta { get; set; }
        public string PrevFrac { get; set; }
        public string ActualBonus { get; set; }
        public string ABWithPrevFrac { get; set; }
        public string IssueBonus { get; set; }
        public string RemFrac { get; set; }
    }
}
