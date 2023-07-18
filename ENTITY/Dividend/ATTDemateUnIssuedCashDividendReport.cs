namespace Entity.Dividend
{
    public class ATTDemateUnIssuedCashDividendReport
    {

        public string BO_idno { get; set; }
        public string fullname { get; set; }
        public string warrantno { get; set; }
        public double? totshkitta { get; set; }

        public double? warrantamt { get; set; }
        public double? taxdamt { get; set; }
        public double? bonustax { get; set; }
        public double? bonusadj { get; set; }
        public double? netamount { get; set; }


    }
}
