using Entity.ShareHolder;

namespace Entity.CDS
{
    public class ParaComp_Child
    {
        public ATTShHolder aTTShHolder;

        public string CompCode { get; set; }
        public string ISIN_NO { get; set; }
        public int? ShholderNo { get; set; }
        public string Desc_share { get; set; }
        public int? ShownerType { get; set; }

    }
}
