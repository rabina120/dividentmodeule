using System.Collections.Generic;

namespace Entity.Reports
{
    public class ATTShholderCertificateListForReport
    {
        public string CompCode { get; set; }
        public int Shholderno { get; set; }
        public int sCertNo { get; set; }
        public int sSrNoFrom { get; set; }
        public int sSrnoTo { get; set; }
        public int sRegNo { get; set; }
        public int sRevTranNo { get; set; }
        public string fName { get; set; }
        public string lName { get; set; }
        public int regNo { get; set; }
        public int sShkitta { get; set; }
        public string Sentrydate { get; set; }
        public string sTrDate { get; set; }
        public string sRevDate { get; set; }
        public string sdp_code { get; set; }
        public string sdp_name { get; set; }
        public string sbo_acct_no { get; set; }
        public string sentry_user { get; set; }
        public string sTranDate { get; set; }
        public string sRemarks { get; set; }
        public List<ATTCertificateForReport> aTTCertificates { get; set; }
    }
}
