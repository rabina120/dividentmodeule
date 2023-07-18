namespace Entity.Security
{
    public class EsewaAPIKeys
    {
       
        public string Url { get; set; } 
        public ClientKeys ClientKeys { get; set; }
        public SignatureKeys SignatureKeys { get; set; }
        public EsewaPublicKeys EsewaKeys { get; set; } 
        public string ClientID { get; set; } 
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class ClientKeys
    {
        public string ClientPrivateKey { get; set; }
        public string ClientPublicKey { get; set; }
    }
    public class SignatureKeys
    {
        public string ClientPrivateKey { get; set; }
        public string ClientPublicKey { get; set; }
    }
    public class EsewaPublicKeys
    {
        public string SignatureKey { get; set; }
        public string PublicKey { get; set; }
    }

}
