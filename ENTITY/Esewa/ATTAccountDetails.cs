using System.Collections.Generic;

namespace Entity.Esewa
{
    public class ATTAccountDetails
    {
        public List<ATTTransctionDetails> transaction_details { get; set; }

        public string token { get; set; }
    }
}
