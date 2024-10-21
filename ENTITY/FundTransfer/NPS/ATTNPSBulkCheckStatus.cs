using System.Collections.Generic;

namespace ENTITY.FundTransfer.NPS
{
    public class ATTNPSBulkCheckStatusRequest
    {
        public string ApiUserName { get; set; }
        public string MerchantId { get; set; }
        public string MerchantProcessId { get; set; }
        public string Signature { get; set; }
        public string TimeStamp { get; set; }
    }
    public class ATTNPSBulkCheckStatusResponse : ATTNPSResponse
    {
        public List<ATTNPSCheckStatusResponseData> data { get; set; }
    }
}
