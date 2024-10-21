using System.Collections.Generic;

namespace ENTITY.FundTransfer
{
    public class ATTAccountDetails
    {
        public List<ATTTransctionDetails> transaction_details { get; set; }

        public string token { get; set; }
    }
}
