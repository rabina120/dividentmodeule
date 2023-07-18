
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
using System.Linq;

namespace Repository.Security
{
    public class UserMenu : IUserMenu
    {
        IOptions<ReadConfig> connectionString;
        IConfiguration _configuration;
        public UserMenu(IOptions<ReadConfig> _connectionString, IConfiguration configuration)
        {
            connectionString = _connectionString;
            _configuration = configuration;
        }
        #region User Menu List
        public JsonResponse GetUserMenu(string RoleId)
        {
            JsonResponse jsonResponse = new JsonResponse();
            string isLDAP = _configuration.GetSection("LDAPAuthentication").GetSection("Enabled").Value;
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {

                jsonResponse.Message = "Record Not Found";
                try
                {
                    //Changing Password for the Login USer     
                    DynamicParameters param = new DynamicParameters();
                    param.Add("ROLE_ID", Convert.ToInt32(RoleId));
                    List<ATTMenu> aTTMenu = connection.Query<ATTMenu>("LoginUserMenuByRoleId", param, commandType: CommandType.StoredProcedure).ToList();
                    if (aTTMenu != null)
                    {
                        jsonResponse.Message = "User Menu Found";
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = aTTMenu;
                        int UserMenuID = isLDAP.ToUpper() == "TRUE" ? 4 : 7;//Choosing Userenu for LDAP
                        aTTMenu.Remove(aTTMenu.Find(x => x.MenuId == UserMenuID));
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.ResponseData = ex;
                    jsonResponse.HasError = true;
                    jsonResponse.IsSuccess = false;
                }
                return jsonResponse;
            }
        }
        #endregion

        #region Menu List
        public JsonResponse GetMenuList()
        {
            JsonResponse jsonResponse = new JsonResponse();
            string isLDAP = _configuration.GetSection("LDAPAuthentication").GetSection("Enabled").Value;

            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                jsonResponse.Message = "No Menu Found";

                try
                {
                    connection.Open();
                    List<ATTMenu> aTTMenu = connection.Query<ATTMenu>("GetMenuList", null, commandType: CommandType.StoredProcedure).ToList();
                    if (aTTMenu != null)
                    {
                        jsonResponse.Message = "";
                        jsonResponse.ResponseData = aTTMenu;
                        jsonResponse.IsSuccess = true;
                        int UserMenuID = isLDAP.ToUpper() == "TRUE" ? 4 : 7;//Choosing Userenu for LDAP
                        aTTMenu.Remove(aTTMenu.Find(x => x.MenuId == UserMenuID));
                    }

                }
                catch (Exception ex)
                {
                    jsonResponse.ResponseData = ex;
                    jsonResponse.HasError = true;
                    jsonResponse.IsSuccess = false;
                }
                return jsonResponse;
            }

        }

        #endregion

        #region Add/Update Menu Rights 
        public JsonResponse AddRights(string userId, string[] menuList, string addUpdate, string UserName)
        {
            JsonResponse response = new JsonResponse();
            JsonResponse deleteresponse = new JsonResponse();
            var responseFromDatabase = false;
            response.Message = "Failed to Update";
            response.IsSuccess = false;

            SqlConnection connectionU = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection));
            using (connectionU)
            {
                try
                {
                    connectionU.Open();
                    SqlTransaction tran = connectionU.BeginTransaction();
                    try
                    {
                        DynamicParameters dparam = new DynamicParameters();
                        dparam.Add("P_USER_ID", userId == null ? "" : userId);
                        deleteresponse = connectionU.Query<JsonResponse>("DELETE_USER_RIGHTS", dparam, tran, commandType: CommandType.StoredProcedure)?.FirstOrDefault();


                    }
                    catch (Exception ex)
                    {

                        deleteresponse.ResponseData = ex;
                        deleteresponse.HasError = true;
                        deleteresponse.IsSuccess = false;
                        tran.Rollback();

                        return deleteresponse;

                    }
                    try
                    {
                        DataTable dt = new DataTable();
                        dt.Columns.Add("UserId");
                        dt.Columns.Add("MenuID");
                        dt.Columns.Add("IsAccessed");

                        foreach (var menu in menuList)
                        {
                            dt.Rows.Add(Convert.ToInt32(userId), Convert.ToInt32(menu), "true");
                        }
                        SqlCommand cmd = new SqlCommand("ADD_USER_RIGHTS", connectionU);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Transaction = tran;
                        SqlParameter param = cmd.Parameters.AddWithValue("@udt", dt);
                        param.Direction = ParameterDirection.Input;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {

                            if (reader.Read())
                            {
                                if (reader.GetString(0) == "1")
                                {
                                    responseFromDatabase = true;
                                }

                            }
                        }

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();

                        response.ResponseData = ex;
                        response.HasError = true;
                        response.IsSuccess = false;
                    }
                }
                catch (Exception ex)
                {

                    response.ResponseData = ex;
                    response.HasError = true;
                    response.IsSuccess = false;
                }
            }

            if (responseFromDatabase)
            {
                response.Message = "Updated Successfully.";
                response.IsSuccess = true;

            }
            else
            {
                response.Message = "Updated Failed.";
                response.IsSuccess = false;

            }
            AuditRepo audit = new AuditRepo(connectionString);
            audit.auditSave(UserName, " Menu List", "Menu User Access of " + userId);

            return response;
        }

        #endregion


        #region GetRoleMenu
        public JsonResponse GetMenuByRole(int roleId, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                try
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("Role_ID", roleId);
                    param.Add("UserName", UserName);
                    param.Add("IPAddress", IPAddress);
                    param.Add("EntryDate", DateTime.Now);
                    List<ATTRoleMenu> aTTMenu = connection.Query<ATTRoleMenu>("MenuByRole", param, commandType: CommandType.StoredProcedure).ToList();
                    if (aTTMenu.Count > 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = aTTMenu;
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


        #region MenuaccessbyRole

        public JsonResponse AddRightsByRole(string roleId, string[] menuList, string addUpdate, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            var responseFromDatabase = false;

            SqlConnection connectionU = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection));
            using (connectionU)
            {
                try
                {
                    connectionU.Open();
                    SqlTransaction tran = connectionU.BeginTransaction();

                    DataTable dt = new DataTable();
                    dt.Columns.Add("MenuID");
                    dt.Columns.Add("IsAccessed");

                    foreach (var menu in menuList)
                    {
                        dt.Rows.Add(Convert.ToInt32(menu), "true");
                    }
                    SqlCommand cmd = new SqlCommand("ADD_ROLE_RIGHTS", connectionU);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = tran;
                    SqlParameter param = cmd.Parameters.AddWithValue("@udt", dt);
                    param = cmd.Parameters.AddWithValue("@RoleID", roleId);
                    param = cmd.Parameters.AddWithValue("@USERNAME", UserName);
                    param = cmd.Parameters.AddWithValue("@IPAddress", IPAddress);
                    param = cmd.Parameters.AddWithValue("@EntryDate", DateTime.Now);

                    param.Direction = ParameterDirection.Input;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        if (reader.Read())
                        {
                            if (reader.GetString(0) == "1")
                            {
                                responseFromDatabase = true;
                            }
                        }
                    }
                    if (responseFromDatabase)
                        tran.Commit();
                    else
                    {
                        tran.Rollback();
                        response.Message = ATTMessages.CANNOT_SAVE;
                    }

                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.HasError = true;
                    response.ResponseData = ex;
                }
            }

            if (responseFromDatabase)
            {
                response.Message = ATTMessages.SAVED_SUCCESS;
                response.IsSuccess = true;
            }
            else
            {
                response.Message = ATTMessages.CANNOT_SAVE;
                response.IsSuccess = false;

            }


            return response;
        }
        #endregion


    }
}
