


using Entity.Common;
using Entity.Security;
using System.Collections.Generic;

namespace Interface.Security
{
    public interface ILoginUser
    {
        public JsonResponse Login(string Username, string Password, string IP);
        public JsonResponse Logout(string Username, string IP);
        public JsonResponse ChangePassword(string _loggedInUser, string Username, string Password, string NewPassword, string PasswordChangeAlertDate, string IPAddress);
        public JsonResponse CreateUser(ATTUserProfile aTTUserProfile, string UserName, string IPAddress);
        public JsonResponse UpdateUser(ATTUserProfile aTTUserProfile, string UserName, string IPAddress);
        public List<ATTUserType> GetUserType();
        public List<ATTUserRole> GetUserRole();
        public List<ATTUserStatus> GetUserStatus();
        public JsonResponse LdapAuthentication(string username, string password);
        public JsonResponse FailedLogin(string UserId, string IPAddress);
    }
}
