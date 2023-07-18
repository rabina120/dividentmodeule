using System;

namespace Entity.Security
{
    public class ATTAuditTrial
    {
        public string UserName { get; set; }
        public string EntryTime { get; set; }
        public string RefFile { get; set; }
        public DateTime ActionDate { get; set; }
        public string Remarks { get; set; }
        public string IPAddress { get; set; }

    }
}
