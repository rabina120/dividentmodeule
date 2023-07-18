using Entity.DakhilTransfer;
using System.Collections.Generic;

namespace Entity.Certificate
{
    public class ATTCertificateSplit
    {
        public string compcode { get; set; }

        public int? ShHolderNo { get; set; }
        public int? CertNo { get; set; }
        public int? SrNoFrom { get; set; }
        public int? SrNoTo { get; set; }
        public int? TimesofSplit { get; set; }
        public string centerid { get; set; }
        public int? CertStatus { get; set; }
        public int? ShKitta { get; set; }
        public int? ShOwnerType { get; set; }
        public int? ShareType { get; set; }
        public string Issuedate { get; set; }
        public int? DistCert { get; set; }
        public string CertDistDt { get; set; }
        public string TranDt { get; set; }
        public int? DupliNo { get; set; }
        public double? PaidAmount { get; set; }
        public double? TotalAmount { get; set; }
        public string UserName { get; set; }
        public string EntryDate { get; set; }
        public string Remarks { get; set; }

        public string npname { get; set; }
        public string name { get; set; }
        public string maxisplit { get; set; }

        public List<ATTDakhilCertificateUndo> dakhilCertificateUndos { get; set; }
        public List<ATTCertificateSplitDetail> aTTCertificateSplitDetails { get; set; }
    }
}
