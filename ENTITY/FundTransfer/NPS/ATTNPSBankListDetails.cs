namespace ENTITY.FundTransfer.NPS
{
    public class ATTNPSBankListDetails
    {
        public string InstitutionName { get; set; }
        public string BankSwiftCode { get; set; }
        public string InstrumentCode { get; set; }
        public string LogoUrl { get; set; }
        public string IsMobileEnabled { get; set; }
        public string IsLinkAccountEnabled { get; set; }
        public string AccountName { get; set; } //= "Diwakar Baskota";
        public string AccountNumber { get; set; } //= "1900000000000076";
    }
}
