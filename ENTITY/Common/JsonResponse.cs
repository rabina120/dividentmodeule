namespace Entity.Common
{
    public class JsonResponse
    {

        public bool IsSuccess { get; set; } = false;
        public bool IsValidSubmissionNO { get; set; }
        public bool IsValid { get; set; }
        public int issuedRecords { get; set; }
        public int payedRecords { get; set; }
        public bool IsCalculated { get; set; }
        public string Message { get; set; }
        public dynamic data { get; set; }
        public string Token { get; set; }
        public object ResponseData { get; set; }
        public object Records { get; set; }
        public string OutputParam { get; set; }
        public object TotalRecords { get; set; }
        public string CallBack { get; set; }
        public bool IsToken { get; set; }
        public string ProfileImage { get; set; }
        public string ChangePassword { get; set; }
        public bool HasError { get; set; }
        public bool HasPasswordChanged { get; set; }
        public bool ErrorTrap { get; set; }
        public object ResponseData2 { get; set; }
        public bool withBankDetails { get; set; } = false;


    }

}
