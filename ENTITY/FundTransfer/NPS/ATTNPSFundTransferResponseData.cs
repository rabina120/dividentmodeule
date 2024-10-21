namespace ENTITY.FundTransfer.NPS
{
    public class ATTNPSFundTransferResponseData
    {
        public string MerchantId { get; set; }
        public string MerchantTxnId { get; set; }
        public string TransactionId { get; set; }
        public string TransactionStatus { get; set; }
    }

    public class ATTNPSFundTransferSuccessResponse : ATTNPSResponse
    {
        public ATTNPSFundTransferResponseData data { get; set; }
    }
}
