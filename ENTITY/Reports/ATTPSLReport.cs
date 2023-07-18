using Entity.Certificate;

namespace Entity.Reports
{
    public class ATTPSLReport
    {
        public string CompCode { get; set; }
        public string CompName { get; set; }
        public string PCode { get; set; }
        public string TranType { get; set; }
        public string UserName { get; set; }
        public string ReportType { get; set; }
        public string CompEnName { get; set; }
        public string HolderNoFrom { get; set; }
        public string DataType { get; set; }
        public string OrderBy { get; set; }
        public string ShareType { get; set; }
        public string SelectChoice { get; set; }
        public string HolderNoTo { get; set; }
        public string CertNoFrom { get; set; }
        public string CertNoTo { get; set; }
        public string PSLDateFrom { get; set; }
        public string PSLDateTo { get; set; }
        public ATTCERTIFICATE Certificate { get; set; }


    }
}
