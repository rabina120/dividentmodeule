using System;

namespace Entity.ShareHolder
{
    public class ATTMinor
    {

        public string GEnName { get; set; }
        public string Relation { get; set; }
        public DateTime? DOB { get; set; }

        public string DateOfBirth
        {
            get
            {
                return Convert.ToDateTime(DOB).ToString("yyyy-MM-dd");
            }
            set
            {
                ;
            }
        }

        public int? Age { get; set; }

    }
}
