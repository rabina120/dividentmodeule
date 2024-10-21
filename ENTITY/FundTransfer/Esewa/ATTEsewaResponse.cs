using System.Collections.Generic;

namespace ENTITY.FundTransfer.Esewa
{
    public class ATTEsewaResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public bool Error { get; set; }
        public string Details { get; set; }

        public List<ATTTransctionDetails> transaction_details { get; set; }

        //public List<ATTTransctionStatus> transaction_Status { get; set; }
    }
}
