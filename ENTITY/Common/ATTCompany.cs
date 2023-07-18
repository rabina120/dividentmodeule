namespace Entity.Common
{
    public class ATTCompany
    {
        public string CompCode { get; set; }
        public string CompEnName { get; set; }
        public string CompEnAdd1 { get; set; }
        public string CompEnAdd2 { get; set; }
        public string CompNpName { get; set; }
        public string CompNpAdd1 { get; set; }
        public string CompNpAdd2 { get; set; }
        public string TelNo { get; set; }
        public string PBoxNo { get; set; }
        public string Email { get; set; }
        public double MaxKitta { get; set; }
        public double PerShVal { get; set; }
        public int fstCallVal { get; set; }
        public string fstCallDt { get; set; }
        public int sndCallVal { get; set; }
        public string sndCallDt { get; set; }
        public int trdCallVal { get; set; }
        public string trdCallDt { get; set; }
        public string SoftInstDt { get; set; }
        public int SoftInstNo { get; set; }
        public string AgentName { get; set; }
        public string SignDir { get; set; }
        public string SoldSoftCode { get; set; }
        public string CurAgmNo { get; set; }
        public bool IsEngDt { get; set; }
        public bool isLDAP { get; set; }
    }
}
