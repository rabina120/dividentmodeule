namespace Entity.Certificate
{
    public class ATTDAKHILTRANSFER
    {
        public string CompCode { get; set; }
        public int CertNo { get; set; }
        public string BuyerName { get; set; }
        public int SHolderNo { get; set; }
        public int BHolderNo { get; set; }
        public int SrNoFrom { get; set; }
        public int SrNoTo { get; set; }
        public string DakhilDt { get; set; }
        public int TranNo { get; set; }
        public int RegNo { get; set; }
        public string Remarks { get; set; }
        public int TranKitta { get; set; }
        public int batchno { get; set; }
        public string CertDistDt { get; set; }

        public string Action { get; set; }
        public int DistCert { get; set; }
        public string ShKitta { get; set; }
    }
}
