namespace ENTITY.FundTransfer
{
    public class ATTEncryptedDetails
    {
        public string data { get; set; }
        public string secret_key { get; set; }
        public string signature { get; set; }
        public string client_id { get; set; }
        public string code { get; set; }
        public string message { get; set; }
        public string details { get; set; }
        public bool error { get; set; }
    }
}
