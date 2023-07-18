namespace Entity.Security
{
    public class RootConfig
    {
        public Logging Logging { get; set; }
        public string AllowedHosts { get; set; } = "*";
        public ConnectionStrings ConnectionStrings { get; set; }
        public LDAPAuthentication LDAPAuthentication { get; set; }
        public EsewaKeys EsewaKeys { get; set; }



    }
    public class Logging
    {
        public LogLevel LogLevel { get; set; }
        public File File { get; set; }
    }
    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; }
        public string ExcelConStringV4 { get; set; } = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;'";
        public string ExcelConStringV12 { get; set; } = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";

    }
    public class LogLevel
    {
        public string Default { get; set; } = "Debug";
        public string Microsoft { get; set; } = "Warning";
        public string MicrosoftHostingLifetime { get; set; } = "Error";
    }
    public class File
    {
        public File()
        {
            Path = "AppLogs/{0:yyyy}-{0:MM}-{0:dd}.log";
            MinLevel = "Information";
            FileSizeLimitBytes = 0;
            MaxRollingFiles = 0;
        }
        public string Path { get; set; } = "AppLogs/{0:yyyy}-{0:MM}-{0:dd}.log";
        public bool Append { get; set; } = true;
        public string MinLevel { get; set; } = "Information";
        public int FileSizeLimitBytes { get; set; } = 0;
        public int MaxRollingFiles { get; set; } = 0;
    }
    public class LDAPAuthentication
    {
        public string Enabled { get; set; } = "False";
        public string Server { get; set; } = " ";
        public LDAPAuthentication()
        {
            Enabled = "False";
            Server = " ";
        }
    }
    public class EsewaKeys
    {
        public EsewaKeys()
        {
            Url = "https://corporate.esewa.com.np";

        }
        public string Url { get; set; } = "https://corporate.esewa.com.np";
        public string ClientPrivateKey { get; set; } = " ";
        public string ClientPublicKey { get; set; } = " ";
        public string EsewaPublicKey { get; set; } = " ";
        public string EsewaPublicKeySig { get; set; } = " ";
        public string ClientID { get; set; } = " ";
    }



}
