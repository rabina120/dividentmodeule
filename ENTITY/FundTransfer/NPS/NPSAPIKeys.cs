namespace ENTITY.FundTransfer.NPS
{
    public class NPSAPIKeys
    {
        public string LoginUrl { get; set; }
        public string FundTransferUrl { get; set; }
        public string PrivateKey { get; set; }
        public string MerchantId { get; set; }
        public string BulkFundTransferUrl { get; set; }
        public string AuthorizationPayload { get; set; }
        public string AccountValidationUrl { get;  set; }
        public string BankUrl { get;  set; }
        public string TransactionStatusUrl { get;  set; }
        public string BulkTransactionStatusUrl { get;  set; }
        public string ApiUserName { get; set; }
        public string FundTransferBatchSize { get; set; }
    }
}
