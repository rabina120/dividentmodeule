namespace Entity.HolderInfo
{
    public class ATTPledgeInformation
    {
        public string compcode { get; set; }

        public string FName { get; set; }
        public string Bo_idNo { get; set; }
        public string LName { get; set; }

        public string WarrantNo { get; set; }
        public int? TotShKitta { get; set; }

        public double? WarrantAmt { get; set; }
        public double? TaxDamt { get; set; }
        public double? bonustax { get; set; }
        public double? bonusadj { get; set; }
        public string shholderno { get; set; }
    }
}
