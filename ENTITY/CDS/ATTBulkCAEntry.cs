namespace Entity.CDS
{
    public partial class ATTBulkCAEntry
    {
        public string ShHolderNo { get; set; }
        public string CertNo { get; set; }
        public string SrNoFrom { get; set; }
        public string SrNoTo { get; set; }
        public string Kitta { get; set; }
        public string BOID { get; set; }
        public string DPID { get; set; }
        public string DPCode { get; set; }
        public string ISIN_NO { get; set; }
        public string HolderName { get; set; }
        public string CompCode { get; set; }
        public string OwnerType { get; set; }
        public string CertId { get; set; }
        public string Description { get; set; }
        
    }

    public partial class ATTBulkCAEntry
    {
        public int? TotalCount { get; set; }
        public int? FilteredCount { get; set; }
    }

}
