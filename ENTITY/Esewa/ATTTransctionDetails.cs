namespace Entity.Esewa

{
    public class ATTTransctionDetails
    {
        private string _payee_account_number;
        private string _payee_account_name;
        private string _payee_account_code;
        public string source_account_number { get; set; }
        public string source_bank_code { get; set; }
        public string payee_account_number { get { return _payee_account_number; } set { _payee_account_number = value?.Trim() ?? string.Empty; } }
        public string payee_account_name { get { return _payee_account_name; } set { _payee_account_name = value?.Trim() ?? string.Empty; } }
        public string payee_bank_code { get { return _payee_account_code; } set { _payee_account_code = value?.Trim() ?? string.Empty; } }
        public string sub_token { get; set; }
        public double? amount { get; set; }
        public string note { get; set; }

        public string message { get; set; }
        public string code { get; set; }
        public string status { get; set; }
    }
}
