using System;

namespace Entity.Dividend
{
    public class ATTRejectedDividend
    {
        public string compcode { get; set; }
        public string Boid { get; set; }
        public string shholderno { get; set; }
        public string Fullname { get; set; }
        public string FiscalYr { get; set; }
        public string batchno { get; set; }
        public string bankaccno { get; set; }
        public string bankname { get; set; }
        public DateTime? rejecteddate { get; set; }
        public string rejectedby { get; set; }
        public string remarks { get; set; }
    }
}
