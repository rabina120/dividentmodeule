namespace Entity.Certificate
{
    public class ATTPSLEntryPosting
    {
        public string compcode { get; set; }
        public string CertNo { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public int? PSLNo { get; set; }
        public int? ShholderNo { get; set; }
        public int? TotalKitta { get; set; }

        public int? SrNoFrom { get; set; }
        public int? SrNoTo { get; set; }
        public int? PledgeKitta { get; set; }
        public int? Seqno { get; set; }
        public int? Kitta { get; set; }
        public string EntryUser { get; set; }
        public string EntryDate { get; set; }
        public string TranDt { get; set; }
        public string Remark { get; set; }
        public string PostingRemarks { get; set; }
        public string PostingDate { get; set; }
        public string Trantype { get; set; }
        public string psl_approved_remarks { get; set; }
        public string AppDate_PSL { get; set; }

    }
}
