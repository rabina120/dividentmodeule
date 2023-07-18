using Entity.ShareHolder;
using System;

namespace Entity.CDS
{
    public class CertificateDemateDetails
    {
        public string compcode { get; set; }
        public int? demate_regno { get; set; }
        public int? seq_no { get; set; }
        public double? drn_no { get; set; }
        public int? shholderno { get; set; }
        public int? certno { get; set; }
        public int? srnofrom { get; set; }
        public int? srnoto { get; set; }
        public int? shkitta { get; set; }
        public string dp_code { get; set; }
        public string bo_acct_no { get; set; }
        public DateTime? tr_date { get; set; }
        public string remarks { get; set; }
        public string entrydate { get; set; }
        public string EntryUser { get; set; }
        public string Approved { get; set; }
        public string App_status { get; set; }
        public string Approved_By { get; set; }
        public DateTime App_date { get; set; }
        public string App_remarks { get; set; }
        public string Tran_type { get; set; }
        public string isin_no { get; set; }
        public int? regno { get; set; }
        public string bonuscode { get; set; }

        public ATTShHolder aTTShHolder { get; set; }

    }
}
