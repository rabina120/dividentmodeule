using System;

namespace Entity.ShareHolder
{
    public class ATTMergeDetail
    {
        public string compcode { get; set; }
        public string merge_id { get; set; }
        public string holdernofrom { get; set; }
        public string holdernoto { get; set; }
        public string kitta_from { get; set; }
        public string kitta_to { get; set; }
        public DateTime? mergedate { get; set; }
        public string merge_remarks { get; set; }
        public string merge_by { get; set; }
        public string entry_dt { get; set; }
        public string approved { get; set; }
        public string app_status { get; set; }
        public string app_date { get; set; }
        public string approved_by { get; set; }
        public string approved_remarks { get; set; }
        public string holdernofromname { get; set; }
        public string holdernotoname { get; set; }
    }
}
