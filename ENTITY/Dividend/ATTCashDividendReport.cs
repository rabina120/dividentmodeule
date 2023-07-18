using System;

namespace Entity.Dividend
{
    public class ATTCashDividendReport
    {
        public string shholderno { get; set; }
        public string BO_idno { get; set; }
        public string fullname { get; set; }
        public string warrantno { get; set; }
        public double warrantamt { get; set; }
        public double taxdamt { get; set; }
        public double bonustax { get; set; }
        public double bonusadj { get; set; }
        public DateTime? WIssuedDt { get; set; }
        public string dwiby { get; set; }
        public string remarks { get; set; }
        public double totshkitta { get; set; }
        public string bankname { get; set; }
        public string bankaccno { get; set; }
        public DateTime? creditedDt { get; set; }

    }
}
