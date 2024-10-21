namespace ENTITY.FundTransfer.NPS
{
    public class ATTNPSFundTransferRequest
    {
        public string Amount { get; set; }
        public string ApiUserName { get; set; }
        public string DestinationAccName { get; set; }
        public string DestinationAccNo { get; set; }
        public string DestinationBank { get; set; }
        public string DestinationCurrency { get; set; }
        public string MerchantId { get; set; }
        public string MerchantProcessID { get; set; }
        public string MerchantTxnId { get; set; }
        public string Signature { get; set; }
        public string SourceAccName { get; set; }
        public string SourceAccNo { get; set; }
        public string SourceBank { get; set; }
        public string SourceCurrency { get; set; }
        public string TimeStamp { get; set; }
        public string TransactionRemarks { get; set; }
        public string TransactionRemarks2 { get; set; }
        public string TransactionRemarks3 { get; set; }
    }
}
