namespace Entity.HolderInfo
{
    public partial class ATTDirtyInfromationFromSystem
    {
        public string Compcode { get; set; }
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
    }

    public partial class ATTDirtyInfromationFromSystem
    {

        public int? TotalCount { get; set; }
        public int? FilteredCount { get; set; }
    }

}

