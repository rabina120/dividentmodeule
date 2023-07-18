using System;

namespace Entity.ShareHolder
{
    public class ATTCertDet
    {
        public string compcode { get; set; }
        public int? ShHolderNo { get; set; }
        public int? CertNo { get; set; }
        public int? SrNoFrom { get; set; }
        public int? SrNoTo { get; set; }
        public int? CertStatus { get; set; }
        public int? TimesofSplit { get; set; }
        public int? ShKitta { get; set; }
        public int? ShOwnerType { get; set; }
        public string ShOwnerTypeText { get; set; }
        public int? ShareType { get; set; }
        public string ShareTypeText { get; set; }
        public DateTime? Issuedate { get; set; }
        public bool DistCert { get; set; }
        public DateTime? CertDistDt { get; set; }
        public DateTime? TranDt { get; set; }
        public int? DupliNo { get; set; }
        public double? PaidAmount { get; set; }
        public double? TotalAmount { get; set; }
        public string UserName { get; set; }
        public DateTime? EntryDate { get; set; }
        public string Remarks { get; set; }
        public int? cert_id { get; set; }
        public int? share_type { get; set; }
        public string description { get; set; }
        public DateTime? issuse_dt { get; set; }
        public string start_srno { get; set; }
        public string end_srno { get; set; }
        public int pslno { get; set; }
        public string entryuser { get; set; }
        public DateTime? ClearedDt { get; set; }
        public string drn_no { get; set; }
        public DateTime? tr_date { get; set; }
        public string demate_regno { get; set; }
        public string regno { get; set; }
        public string bo_acct_no { get; set; }
        public string bonuscode { get; set; }
        public string certStatusText { get; set; }

    }
}
