namespace ENTITY.FundTransfer.Esewa
{
    public class ATTEsewaNewResponse
    {
        public string payee_account_number { get; set; }
        public string payee_account_name { get; set; }
        public string payee_bank_code { get; set; }
        public string code { get; set; }
        public string message { get; set; }
        public string percentage { get; set; }
        public bool error { get; set; }
        public string sub_token { get; set; }
    }
}
