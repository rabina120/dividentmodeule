using System.Collections.Generic;

namespace Entity.Dividend
{
    public class HolderBOID
    {
        public int agmno { get; set; }
        public string fiscalyr { get; set; }
        public string description { get; set; }
        public string tablename { get; set; }

        public List<HolderBOIDTable> holderBOIDInfo { get; set; }
    }
}
