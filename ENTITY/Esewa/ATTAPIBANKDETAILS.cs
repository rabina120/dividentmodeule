using System.Collections.Generic;

namespace Entity.Esewa
{
    public class ATTBanks
    {
        public List<APIBanksDetail> banks { get; set; }

        public bool Error { get; set; }

        public string Message { get; set; }
    }
}
