namespace Entity.BonusDividend
{
    public class ATTSHBonus
    {
        public string compcode { get; set; }
        public int? ShholderNo { get; set; }
        public int? CertNo { get; set; }
        public int? srnofrom { get; set; }
        public int? srnoto { get; set; }
        public int? PrevTotKitta { get; set; }
        public double? Prevfrac { get; set; }
        public double? ActualBonus { get; set; }
        public double? ABwithPFrac { get; set; }
        public int? IssueBonus { get; set; }
        public double? Remfrac { get; set; }
        public double? Bonusadj { get; set; }
        public double? BonusTax { get; set; }
        public int? shownertype { get; set; }
        public string Entryuser { get; set; }
        public string Entrydate { get; set; }
        public string Approveduser { get; set; }
        public string Approveddate { get; set; }
        public string Approved { get; set; }
        public double? TaxPC { get; set; }


    }
}
