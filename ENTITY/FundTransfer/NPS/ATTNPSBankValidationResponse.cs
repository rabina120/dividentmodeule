namespace ENTITY.FundTransfer.NPS
{
    public class ATTNPSBankValidationResponse : ATTNPSResponse
    {
        public NPSBankValidationResponse data { get; set; }
    }
    public class NPSBankValidationResponse
    {
        public string AccountName { get; set; }
        public string MobileNumber { get; set; }
        public string NameMatchPercentage { get; set; }

    }
}
