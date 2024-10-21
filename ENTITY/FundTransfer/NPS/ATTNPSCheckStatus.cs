namespace ENTITY.FundTransfer.NPS
{
    public class ATTNPSCheckStatusRequest
    {
        public string ApiUserName { get; set; }
        public string MerchantId { get; set; }
        public string MerchantTxnId { get; set; }
        public string Signature { get; set; }
        public string TimeStamp { get; set; }
    }

    public class ATTNPSCheckStatusResponseData
    {
        public string MerchantId { get; set; }
        public string MerchantTxnId { get; set; }
        public string ProcessId { get; set; }
        public string TransactionId { get; set; }
        public string SourceAccName { get; set; }
        public string SourceAccNo { get; set; }
        public string SourceCurrency { get; set; }
        public string SourceBank { get; set; }
        public string DestinationAccName { get; set; }
        public string DestinationAccNo { get; set; }
        public string DestinationCurrency { get; set; }
        public string DestinationBank { get; set; }
        public string Amount { get; set; }
        public string TransactionRemarks { get; set; }
        public string TransactionStatus { get; set; }
        public string TransactionDate { get; set; }
    }

    public class ATTNPSCheckStatusResponse : ATTNPSResponse
    {
        public ATTNPSCheckStatusResponseData data { get; set; }
    }
}
