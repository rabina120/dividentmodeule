namespace Entity.Dividend
{
    public class ATTDIVIDENDEXCELUPLOAD
    {
        public double WARRANTAMT { get; set; }
        public string WARRANTNO { get; set; }
        public string SHHOLDERNO { get; set; }
        public string AGMNO { get; set; }
        public string DIVTYPE { get; set; }
        public string SEQNO { get; set; }
        public string DPAYABLE { get; set; }
        public string TAXDAMT { get; set; }
        public string BONUSTAX { get; set; }


        public int TOTALCOUNT { get; set; }
        public double DivTax { get; set; }
        public double bonusadj { get; set; }
        public double PREVADJ { get; set; }
        public double TOTSHKITTA { get; set; }
        public double tFRACKITTA { get; set; }
        public double TotalNetPay { get; set; }
        public int TotalIssueBonus { get; set; }

        public double TotalRefractionbonus { get; set; }
        public double TotalBonusTax { get; set; }
        public double Totalprevadj { get; set; }
    }
}
