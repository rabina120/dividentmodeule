﻿namespace Entity.Dividend
{
    public class ATTPhysicalUnIssuedCashDividendReport
    {
        public string shholderno { get; set; }
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

