using System.Collections.Generic;

namespace ENTITY.FundTransfer.Esewa
{
    public class ATTEsewaBanks
    {
        public List<EsewaAPIBanksDetail> banks { get; set; }

        public bool Error { get; set; }

        public string Message { get; set; }
    }
}
