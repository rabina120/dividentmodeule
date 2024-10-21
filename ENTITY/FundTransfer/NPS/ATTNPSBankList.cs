using System.Collections.Generic;

namespace ENTITY.FundTransfer.NPS
{
    public class ATTNPSBankList : ATTNPSResponse
    {
        public List<ATTNPSBankListDetails> data { get; set; }
    }
}
