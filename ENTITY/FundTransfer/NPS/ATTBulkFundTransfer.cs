using System.Collections.Generic;

namespace ENTITY.FundTransfer.NPS
{
    public class ATTBulkFundTransferRequest
    {
        public string ApiUserName { get; set; }
        public string MerchantId { get; set; }
        public string MerchantProcessId { get; set; }
        public string Signature { get; set; }
        public string SourceAccName { get; set; }
        public string SourceAccNo { get; set; }
        public string SourceBank { get; set; }
        public string SourceCurrency { get; set; }
        public string TimeStamp { get; set; }
        public virtual List<ATTNPSBulkTransactionDetails> TransactionDetail { get; set; }
    }
    public class ATTNPSBulkFundTransferResponse : ATTNPSResponse
    {
        public ATTNPSBulkFundTransferResponseData data { get; set; }
    }

    public class ATTNPSBulkFundTransferResponseData
    {
        public string MerchantProcessId {  get; set; }
    }

    public class ATTNPSBulkTransactionDetails
    {
        public string Amount { get; set; }
        public string DestinationAccName { get; set; }
        public string DestinationAccNo { get; set; }
        public string DestinationBank { get; set; }
        public string DestinationCurrency { get; set; }
        public string IsDestinationMobile { get; set; }
        public string MerchantTxnId { get; set; }
        public string TransactionRemarks { get; set; }

    }
}
