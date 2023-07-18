namespace Entity.Esewa
{
    public class ATTAccountValidationResponse
    {
        private string _payee_account_number;
        private string _payee_account_name;
        private string _payee_account_code;
        public string payee_account_number { get { return _payee_account_number; } set { _payee_account_number = value?.Trim() ?? string.Empty; } }
        public string payee_account_name { get { return _payee_account_name; } set { _payee_account_name = value?.Trim() ?? string.Empty; } }
        public string payee_bank_code { get { return _payee_account_code; } set { _payee_account_code = value?.Trim() ?? string.Empty; } }
        public bool error { get; set; }
        public double? percentage { get; set; }
        public string message { get; set; }
        public string code { get; set; }
        public string sub_token { get; set; }
    }
}
