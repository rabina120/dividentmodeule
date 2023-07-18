using System;

namespace Entity.ShareHolder
{
    public class ATTShHolderLockUnlock
    {
        public string compcode { get; set; }
        public string lock_id { get; set; }
        public string ShholderNo { get; set; }
        public DateTime? lock_dt { get; set; }
        public string lock_remarks { get; set; }
        public string status { get; set; }
        public string lock_by { get; set; }
        public string approved { get; set; }
        public string approved_by { get; set; }
        public string app_status { get; set; }
        public DateTime? approved_date { get; set; }
        public string unlock_by { get; set; }
        public DateTime? unlock_dt { get; set; }
        public string unlock_remarks { get; set; }
        public string approved_unlock { get; set; }
        public string approved_by_unlock { get; set; }
        public string app_status_unlock { get; set; }
        public DateTime? approved_unlock_dt { get; set; }
        public string approved_remarks { get; set; }
        public string unlock_approved_remarks { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }

    }
}
