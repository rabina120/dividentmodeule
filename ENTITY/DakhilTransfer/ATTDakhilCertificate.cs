
using System;

namespace Entity.DakhilTransfer
{
    public class ATTDakhilCertificate
    {
        public int? ShHolderNo { get; set; }
        public int? SrNoFrom { get; set; }
        public int? SrNoTo { get; set; }
        public int? ShKitta { get; set; }
        public int? ShOwnerType { get; set; }
        public int? ShareType { get; set; }
        public float? TotalAmount { get; set; }
        public float? PaidAmount { get; set; }
        public int? DupliNo { get; set; }
        public int? CertStatus { get; set; }
        public string Name { get; set; }
        public string SellerName { get; set; }
        public string BuyerName { get; set; }
        public string BuyerAddress { get; set; }
        public int? RegNo { get; set; }
        public int? LetterNo { get; set; }
        public int? TranNo { get; set; }
        public string BrokerCode { get; set; }
        public DateTime? DakhilDt { get; set; }
        public int? TradeType { get; set; }
        public int? Charge { get; set; }
        public string BHolderNo { get; set; }
        public bool BHoldExist { get; set; }
        public string Remarks { get; set; }
    }
}
