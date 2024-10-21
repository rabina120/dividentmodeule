using System;
using System.Collections.Generic;

namespace ENTITY.FundTransfer.NPS
{
    public class ATTNPSLoginReq
    {
        public string ApiUserName { get; set; }
        public string MerchantId { get; set; }
        public string timestamp { get; set; } = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");

    }




    public class ATTNPSAccountValidateReq
    {
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string ApiUserName { get; set; }
        public string BankCode { get; set; }
        public string MerchantId { get; set; }
        public string signature { get; set; }
        public string timestamp { get; set; }
    }
    public class ATTNPSLoginReqWithSig : ATTNPSLoginReq
    {
        public string signature { get; set; }

    }
    public class ATTNPSResponse
    {
        public string code { get; set; }
        public string message { get; set; }
        public List<ATTNPSError> errors { get; set; }
    }
    public class ATTNPSError
    {
        public string error_code { get; set; } 
        public string error_message { get; set; }
        public string nps_code { get; set; }
        public string bank_route { get; set; }
    }
    public class ATTNPSLoginData
    {
        public string? AccessToken { get; set; }
        public string TokenType { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public DateTime ExpiryTimestamp { get; set; }
    }
    public class ATTNPSLoginSuccess : ATTNPSResponse
    {
        public ATTNPSLoginData data { get; set; }

    }


    


}
