namespace Entity.DakhilTransfer
{
    public class ATTDakhilTransfer
    {
        public string CompCode { get; set; }
        public int? CertificateNo { get; set; }
        public int? ShHolderNo { get; set; }
        public int? BHolderNo { get; set; }
        public int? SrNoFrom { get; set; }
        public int? SrNoTo { get; set; }
        public string BrokerCode { get; set; }
        public int? LetterNo { get; set; }
        public int? Charge { get; set; }
        public string TradeType { get; set; }
        public string DakhilDate { get; set; }
        public bool BHolderExist { get; set; }
        public string Remarks { get; set; }
        public int? TranKitta { get; set; }
    }
}
