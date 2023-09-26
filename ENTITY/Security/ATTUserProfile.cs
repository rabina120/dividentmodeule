using System;

namespace Entity.Security
{
    public class ATTUserProfile
    {
        private DateTime _dateTime;
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime? Validdate { get; set; }
        public string UserType { get; set; }
        public string UserRole { get; set; }
        public bool LockUnlock { get; set; }
        public string Pw_Change_Alert_Dt { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public bool HasPswChanged { get; set; }
        public DateTime CookieExpireTime { get; set; }
        public DateTime LastLoggedInDateTime { get { return _dateTime; } set { _dateTime = value; } }
        public string LastLoggedInIPAddress { get; set; }
        public bool LoggedInFromAnotherDevice { get; set; }
        public DateTime CurrentTime { get; set; }
        public bool isLoginSucess { get; set; }
        public bool IsDeleted { get; set; }
        public string AccountType { get; set; }
        public ATTUserProfile()
        {
            this.isLoginSucess = false;
        }
    }

}
