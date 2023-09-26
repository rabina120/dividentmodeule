
using Dapper;
using Entity.Common;
using Entity.Security;
using Interface.Security;
using Microsoft.Extensions.Options;


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Security
{
    public class UserDetails : IUserDetails
    {
        IOptions<ReadConfig> connectionString;
        public UserDetails(IOptions<ReadConfig> _connectionString)
        {
            connectionString = _connectionString;
        }


        #region GetAllUser
        public List<ATTUserProfile> GetAllUsers(string UserName, string IPAddress)
        {
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_UserName", UserName);
                    param.Add("@P_IPAddress", IPAddress);
                    param.Add("@P_EntryDate", DateTime.Now);
                    List<ATTUserProfile> aTTUserProfiles = connection.Query<ATTUserProfile>("GetAllUsersList", param, commandType: CommandType.StoredProcedure).ToList();

                    return aTTUserProfiles;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion

        #region DeleteUser
        public Entity.Common.JsonResponse EnableDisableUserById(string UserId, string _loggedInUser, bool enable = false)
        {
            Entity.Common.JsonResponse jsonResponse = new Entity.Common.JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("USER_ID", UserId);
                    param.Add("enable",enable);

                    jsonResponse = connection.Query<Entity.Common.JsonResponse>("EnableDisableUserById", param, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                    if (jsonResponse.IsSuccess)
                    {
                        AuditRepo audit = new AuditRepo(connectionString);
                        audit.auditSave(_loggedInUser, (enable?"Enabled":"Disabled")+" User with User Id" + UserId, "Manage User");
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.Message = ex.Message;
                    jsonResponse.IsSuccess = false;
                }
                return jsonResponse;
            }
        }

        #endregion

        #region GetUserByID
        public Entity.Common.JsonResponse EditUserById(string? UserId)
        {
            Entity.Common.JsonResponse jsonResponse = new Entity.Common.JsonResponse();
            jsonResponse.ResponseData = "Record Not Found !!!";
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("USER_ID", UserId);
                    ATTUserProfile aTTUserProfile = connection.Query<ATTUserProfile>("GetUserDetailsById", param, commandType: CommandType.StoredProcedure)?.FirstOrDefault();

                    if (aTTUserProfile != null)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = aTTUserProfile;
                    }

                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.ResponseData = ex;
                }
                return jsonResponse;
            }
        }
        #endregion


        #region UpdateUserDetails

        public Entity.Common.JsonResponse UpdateUserDetails(string _loggedInUser, string UserId, ATTUserProfile aTTUser, string IP)
        {
            Entity.Common.JsonResponse jsonResponse = new Entity.Common.JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    int roleId = Convert.ToInt32(aTTUser.UserRole);
                    param.Add("USER_ID", UserId);
                    if (aTTUser.Password != null) param.Add("PASSWORD", aTTUser.Password.Trim());
                    param.Add("VALID_DATE", aTTUser.Validdate);
                    param.Add("User_Name", aTTUser.UserName.Trim());
                    param.Add("LockUnlock", aTTUser.LockUnlock);
                    param.Add("Entry_date", DateTime.Now);
                    param.Add("IPAddress", IP);
                    param.Add("username", _loggedInUser);
                    param.Add("roleID", roleId);
                    jsonResponse = connection.Query<Entity.Common.JsonResponse>("UpdateUserDetailsById", param, commandType: CommandType.StoredProcedure)?.FirstOrDefault();

                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.Message = ex.Message;

                }
                return jsonResponse;
            }
        }

        public Entity.Common.JsonResponse UpdateUserDetails(string _loggedInUser, string UserId, ATTUserProfile aTTUser)
        {
            Entity.Common.JsonResponse jsonResponse = new Entity.Common.JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("USER_ID", UserId);
                    param.Add("PASSWORD", aTTUser.Password.Trim());
                    param.Add("VALID_DATE", aTTUser.Validdate);
                    param.Add("User_Name", aTTUser.UserName.Trim());
                    param.Add("LockUnlock", aTTUser.LockUnlock);
                    param.Add("PasswordChangeAlertDate", aTTUser.Pw_Change_Alert_Dt);
                    jsonResponse = connection.Query<Entity.Common.JsonResponse>("UpdateUserDetailsById", param, commandType: CommandType.StoredProcedure)?.FirstOrDefault();

                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.ResponseData = ex.Message;

                }
                return jsonResponse;
            }
        }

        #endregion

        #region GetROle
        public Entity.Common.JsonResponse GetAllRoles()
        {
            Entity.Common.JsonResponse jsonResponse = new Entity.Common.JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    List<ATTRole> aTTRoles = connection.Query<ATTRole>("GETALLROLE", null, commandType: CommandType.StoredProcedure).ToList();

                    if (aTTRoles.Count > 0)
                    {
                        jsonResponse.ResponseData = aTTRoles;
                        jsonResponse.IsSuccess = true;
                    }
                    else
                    {
                        jsonResponse.IsSuccess = false;
                        jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.ResponseData = ex;
                    jsonResponse.HasError = true;
                }
            }

            return jsonResponse;
        }

        public Entity.Common.JsonResponse GetUserRights(string UserID, string UserName, string IPAddress)
        {

            Entity.Common.JsonResponse jsonResponse = new Entity.Common.JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_UserID", UserID);
                    List<ATTMenu> aTTRoles = connection.Query<ATTMenu>("USERRIGHTS_GETMENUBYUSERID", parameters, commandType: CommandType.StoredProcedure).ToList();

                    if (aTTRoles.Count > 0)
                    {
                        jsonResponse.ResponseData = aTTRoles;
                        jsonResponse.IsSuccess = true;
                    }
                    else
                    {
                        jsonResponse.IsSuccess = false;
                        jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.ResponseData = ex;
                    jsonResponse.HasError = true;
                }
            }

            return jsonResponse;
        }
        #endregion
    }
}
