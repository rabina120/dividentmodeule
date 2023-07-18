using Entity.DemateDividend;
using System.Collections.Generic;

namespace Entity.Dividend
{
    public  class ATTHolderForBoidChange
    {
        public List<ATTDemateDividend> SelectedList { get; set; }
        public string CompCode { get; set; }
        public string PreviousBOID { get; set; }
        public string NewBoid { get; set; }
        public string TranType { get; set; }
        public string UserName { get; set; }
        public string IPAddress { get; set; }
        public string EntryDate { get; set; }
        public int? ParentId { get; set;  }
        public string Remarks { get; set; }
        public string ApprovedDate { get; set; }
        public string ActionType { get; set; }
        public string Action { get; set; }
        //death transfer
        public string NewName { get; set; }
        public string NewFName { get; set; }
        public string NewGFName { get; set; }
        public string NewAddress { get; set; }


    }
}
