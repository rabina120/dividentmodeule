using System.Collections.Generic;

namespace Entity.Dividend
{
    public class HolderBOIDResponse
    {
        public string totalKitta { get; set; }
        public string npName { get; set; }
        public string npAdd { get; set; }
        public string fName { get; set; }
        public string lName { get; set; }
        public string address { get; set; }
        public string faName { get; set; }
        public string grFaName { get; set; }
        public string husbandName { get; set; }
        public string telNo { get; set; }
        public string accountNo { get; set; }
        public string distcode { get; set; }

        public List<HolderBOID> HolderBOIDs { get; set; }
    }
}
