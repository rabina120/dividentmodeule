using Entity.ShareHolder;

namespace Entity.Certificate
{
    public class ATTCERTIFICATE
    {
        public string CertNo { get; set; }
        public string ShholderNo { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string SrNoFrom { get; set; }
        public string SrNoTo { get; set; }
        public string ShOwnerType { get; set; }
        public string ShareType { get; set; }
        public string ShKitta { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string TotalKitta { get; set; }

        public int? DistCert { get; set; }

        public string CertDistDt { get; set; }
        public string UserName { get; set; }

        public string split_no { get; set; }
        public string split_remarks { get; set; }
        public string split_dt { get; set; }
        public string kitta { get; set; }
        public string certStatus { get; set; }

        public string compcode { get; set; }
        public string App_date { get; set; }
        public string App_remarks { get; set; }

        public string PSLNo { get; set; }
        public ATTShHolder aTTShHolder { get; set; }

    }
}
