namespace Entity.Reports
{
    public class ATTCertificateForReport
    {
        public string seq_no { get; set; }
        public string certno { get; set; }
        public string regno { get; set; }
        public string rev_tran_no { get; set; }
        public string srnofrom { get; set; }
        public string srnoto { get; set; }
        public string shkitta { get; set; }
        public string entrydate { get; set; }
        public string tr_date { get; set; }
        public string rev_date { get; set; }
        public string drn_no { get; set; }
        public string dp_code { get; set; }
        public string dp_name { get; set; }
        public string bo_acct_no { get; set; }
        public string EntryUser { get; set; }

        public string SplitDateFrom { get; set; }
        public string SplitDateTo { get; set; }
        public int? HolderNoFrom { get; set; }
        public int? HolderNoTo { get; set; }
        public int? CertNoFrom { get; set; }
        public int? CertNoTo { get; set; }
        public string remarks { get; set; }

    }
}
