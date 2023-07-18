using Entity.Common;
using Entity.Security;
using System.Collections.Generic;

namespace Interface.Security
{
    public interface IUserDetails
    {
        List<ATTUserProfile> GetAllUsers(string UserName, string IPAddress);

        JsonResponse EnableDisableUserById(string UserId, string _loggedInUser,bool enable=false);

        JsonResponse EditUserById(string UserId);

        JsonResponse UpdateUserDetails(string _loggedInUser, string UserId, ATTUserProfile aTTUser);
        JsonResponse UpdateUserDetails(string _loggedInUser, string UserId, ATTUserProfile aTTUser, string IPAddress);


        JsonResponse GetAllRoles();
        JsonResponse GetUserRights(string UserID, string UserName, string IPAddress);
    }
}
