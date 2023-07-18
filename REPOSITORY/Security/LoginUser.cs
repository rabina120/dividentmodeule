

using Dapper;
using Entity.Common;
using Entity.Security;
using Interface.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;


namespace Repository.Security
{
    public class LoginUser : ILoginUser
    {
        IOptions<ReadConfig> connectionString;
        IConfiguration _congifuration;

        public LoginUser(IOptions<ReadConfig> _connectionString, IConfiguration congifuration)
        {
            connectionString = _connectionString;
            _congifuration = congifuration;
        }
        #region ChangePassword
        public Entity.Common.JsonResponse ChangePassword(string _loggedInUser, string Username, string Password, string NewPassword, string PasswordChangeAlertDate, string IPAddress)
        {
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                Entity.Common.JsonResponse response = new Entity.Common.JsonResponse();
                try
                {
                    connection.Open();
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        //Changing Password for the Login USer     
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@Username", Username.Trim());
                        param.Add("@PASSWORD", Password.Trim());
                        param.Add("@NEWPASSWORD", NewPassword.Trim());
                        param.Add("@ChangeDate", PasswordChangeAlertDate);
                        param.Add("@EntryDate", DateTime.Now);
                        param.Add("@IPADDRESS", IPAddress);

                        response = connection.Query<Entity.Common.JsonResponse>("CHANGE_PASSWORD", param, transaction: tran, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                        if (response.IsSuccess)
                        {
                            response.Message = ATTMessages.USER.PASSWORD_CHANGE_SUCCESS;
                            tran.Commit();
                        }
                        else
                        {
                            response.Message = ATTMessages.USER.PASSWORD_CHANGE_FAILURE;
                            tran.Rollback();
                        }


                    }


                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
                return response;
            }

        }
        #endregion

        #region CreateUser
        public Entity.Common.JsonResponse CreateUser(ATTUserProfile aTTUserProfile, string UserName, string IPAddress)
        {
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                JsonResponse response = new JsonResponse();
                try
                {

                    connection.Open();
                    string sql = _congifuration.GetSection("LDAPAuthentication").GetSection("Enabled").Value.ToLower() == "true" ? "CREATE_USER" : "CREATE_USER_NOLDAP";

                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("P_USERNAME", aTTUserProfile.UserName ?? aTTUserProfile.UserName.Trim());
                        if (_congifuration.GetSection("LDAPAuthentication").GetSection("Enabled").Value.ToLower() == "true")
                        {
                            param.Add("P_USERID", aTTUserProfile.UserId);
                        }
                        param.Add("P_PASSWORD", aTTUserProfile.Password ?? aTTUserProfile.Password.Trim());
                        param.Add("P_VALIDDATE", aTTUserProfile.Validdate);
                        param.Add("P_USERROLE", aTTUserProfile.UserRole ?? aTTUserProfile.UserRole.Trim());
                        param.Add("P_LOCKUNLOCK", aTTUserProfile.LockUnlock);
                        param.Add("P_PWCHANGEALERTDT", aTTUserProfile.Pw_Change_Alert_Dt ?? aTTUserProfile.Pw_Change_Alert_Dt);
                        param.Add("P_CREATEDBY", aTTUserProfile.CreatedBy ?? aTTUserProfile.CreatedBy.Trim());
                        param.Add("P_CREATEDDATE", aTTUserProfile.CreatedDate);
                        param.Add("P_ENTRYDATE", DateTime.Now);
                        param.Add("P_LoggedUserName", UserName);
                        param.Add("P_IPAddress", IPAddress);
                        response = connection.Query<JsonResponse>(sql, param, tran, commandType: CommandType.StoredProcedure).FirstOrDefault();

                        if (response.IsSuccess)
                        {
                            tran.Commit();
                        }
                        else
                        {
                            tran.Rollback();
                            response.Message = ATTMessages.CANNOT_SAVE;
                        }

                    }

                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;

                }
                return response;
            }
        }
        #endregion

        #region Login
        public Entity.Common.JsonResponse Login(string Username, string Password, string IP)
        {
            ATTUserProfile user = new ATTUserProfile();
            Entity.Common.JsonResponse response = new Entity.Common.JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {


                string sql = _congifuration.GetSection("LDAPAuthentication").GetSection("Enabled").Value.ToLower() == "true" ? "GetLogin" : "GetLogin_NOLDAP";
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("P_UserName", Username.Trim());
                    param.Add("P_PASSWORD", Password.Trim());
                    param.Add("P_IP_ADDRESS", IP);
                    param.Add("Cookie_Expire_Date_Now", DateTime.Now);

                    user = connection.Query<ATTUserProfile>(sql, param, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                    if (user.UserName == null)
                    {
                        if (user.Password != null) throw new Exception(user.Password);
                        throw new Exception(ATTMessages.USER.LOGIN_FAILURE);
                    }
                    else
                    {

                        response.IsSuccess = true;
                        user.isLoginSucess = true;
                        response.ResponseData = user;


                    }

                }
                catch (Exception ex)
                {

                    response.Message = ex.Message;


                }
                return response;
            }
        }
        #endregion

        #region Logout
        public Entity.Common.JsonResponse Logout(string Username, string IP)
        {
            Entity.Common.JsonResponse response = new Entity.Common.JsonResponse();

            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {


                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("P_UserName", Username.Trim());
                    param.Add("P_IP_Address", IP);
                    param.Add("Cookie_Expire_Date_Now", DateTime.Now);


                    response = connection.Query<Entity.Common.JsonResponse>("LogOut", param, commandType: CommandType.StoredProcedure)?.FirstOrDefault();

                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
            }
            return response;
        }
        #endregion

        #region Get User Type
        public List<ATTUserType> GetUserType()
        {

            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    List<ATTUserType> aTTUserType = connection.Query<ATTUserType>("GET_USER_TYPE", commandType: CommandType.StoredProcedure).ToList();
                    return aTTUserType;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion

        #region Get User Role
        public List<ATTUserRole> GetUserRole()
        {

            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    List<ATTUserRole> aTTUserRole = connection.Query<ATTUserRole>("GET_USER_ROLE", commandType: CommandType.StoredProcedure).ToList();
                    return aTTUserRole;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion

        #region Get User Status
        public List<ATTUserStatus> GetUserStatus()
        {

            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {

                try
                {
                    connection.Open();
                    List<ATTUserStatus> aTTUserStatus = connection.Query<ATTUserStatus>("GET_USER_STATUS", commandType: CommandType.StoredProcedure).ToList();
                    return aTTUserStatus;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion

        #region LDAP
        public Entity.Common.JsonResponse LdapAuthentication(string username, string password)
        {
            Entity.Common.JsonResponse response = new Entity.Common.JsonResponse();

            try
            {
                string server = _congifuration.GetSection("LDAPAuthentication").GetSection("Server").Value;

                LdapConnection lcon = new LdapConnection(server);
                NetworkCredential nc = new NetworkCredential(username, password, server);
                lcon.Credential = nc;
                lcon.AuthType = AuthType.Negotiate;
                lcon.Bind(nc);
                response.IsSuccess = true;

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }


        #endregion

        public Entity.Common.JsonResponse FailedLogin(string UserId, string IPAddress)
        {
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                Entity.Common.JsonResponse response = new Entity.Common.JsonResponse();
                try
                {

                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("P_USERID", UserId);
                    param.Add("P_IPADDRESS", IPAddress);

                    connection.Query("SAVE_FAILED_LOGIN", param, null, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;

                }
                return response;
            }
        }

        #region UpdateUser
        public JsonResponse UpdateUser(ATTUserProfile aTTUserProfile, string UserName, string IPAddress)
        {
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                JsonResponse response = new JsonResponse();
                try
                {

                    connection.Open();
                    string sql = _congifuration.GetSection("LDAPAuthentication").GetSection("Enabled").Value.ToLower() == "true" ? "UPDATE_USER" : "UPDATE_USER_NOLDAP";

                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("P_USERNAME", aTTUserProfile.UserName ?? aTTUserProfile.UserName.Trim());
                        if (_congifuration.GetSection("LDAPAuthentication").GetSection("Enabled").Value.ToLower() == "true")
                        {
                            param.Add("P_USERID", aTTUserProfile.UserId);
                        }
                        param.Add("P_PASSWORD", aTTUserProfile.Password ?? aTTUserProfile.Password.Trim());
                        param.Add("P_VALIDDATE", aTTUserProfile.Validdate);
                        param.Add("P_USERROLE", aTTUserProfile.UserRole ?? aTTUserProfile.UserRole.Trim());
                        param.Add("P_LOCKUNLOCK", aTTUserProfile.LockUnlock);
                        param.Add("P_PWCHANGEALERTDT", aTTUserProfile.Pw_Change_Alert_Dt ?? aTTUserProfile.Pw_Change_Alert_Dt);
                        param.Add("P_CREATEDBY", aTTUserProfile.CreatedBy ?? aTTUserProfile.CreatedBy.Trim());
                        param.Add("P_CREATEDDATE", aTTUserProfile.CreatedDate);
                        param.Add("P_ENTRYDATE", DateTime.Now);
                        param.Add("P_LoggedUserName", UserName);
                        param.Add("P_IPAddress", IPAddress);
                        response = connection.Query<JsonResponse>(sql, param, tran, commandType: CommandType.StoredProcedure).FirstOrDefault();

                        if (response.IsSuccess)
                        {
                            tran.Commit();
                        }
                        else
                        {
                            tran.Rollback();
                            response.Message = ATTMessages.CANNOT_SAVE;
                        }

                    }

                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.Message = ex.Message;
                    response.HasError = true;

                }
                return response;
            }
        }
        #endregion
    }
}
