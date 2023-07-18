
using Dapper;
using Entity.Common;
using INTERFACE.Reports;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace REPOSITORY.Reports
{
    public class UserReportRepo : IUserReport
    {
        IOptions<ReadConfig> _connectionString;
        public UserReportRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }
        public JsonResponse GenerateReport(string UserName, string DateFrom, string DateTo, string IPAddress, string CurrentUserName, string ReportType)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("UserName", UserName);
                    param.Add("DateFrom", DateFrom);
                    param.Add("DateTo", DateTo);
                    param.Add("IPAddress", IPAddress);
                    param.Add("CurrentUser", CurrentUserName);
                    param.Add("@P_DATE_NOW", DateTime.Now);
                    if (ReportType == "R")
                    {
                        List<dynamic> aTTAuditTrials = connection.Query<dynamic>("Generate_User_Report", param, commandType: CommandType.StoredProcedure).ToList();
                        if (aTTAuditTrials.Count > 0)
                        {
                            jsonResponse.IsSuccess = true;
                            jsonResponse.ResponseData = aTTAuditTrials;
                        }
                        else
                        {
                            jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                        }
                    }
                    else
                    {

                        SqlCommand cmd = new SqlCommand("Generate_User_Report_excel", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter excelParam = new SqlParameter();
                        excelParam = cmd.Parameters.AddWithValue("@UserName", UserName);
                        excelParam = cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                        excelParam = cmd.Parameters.AddWithValue("@DateTo", DateTo);
                        excelParam = cmd.Parameters.AddWithValue("@IPAddress", IPAddress);
                        excelParam = cmd.Parameters.AddWithValue("@CurrentUser", CurrentUserName);
                        excelParam = cmd.Parameters.AddWithValue("@P_DATE_NOW", DateTime.Now);
                        excelParam.Direction = ParameterDirection.Input;
                        DataSet ds = new DataSet("Data");
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = cmd;

                        da.Fill(ds);

                        if (ds.Tables[0].Rows.Count == 0)
                        {
                            jsonResponse.IsSuccess = false;
                            jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                        }
                        else
                        {
                            jsonResponse.IsSuccess = true;
                            jsonResponse.ResponseData = ds;
                        }

                        //List<ATTAuditTrial> aTTAuditTrials = connection.Query<ATTAuditTrial>("Generate_User_Report", param, commandType: CommandType.StoredProcedure).ToList();
                        //if (aTTAuditTrials.Count > 0)
                        //{
                        //    jsonResponse.IsSuccess = true;
                        //    jsonResponse.ResponseData = aTTAuditTrials;
                        //}
                        //else
                        //{
                        //    jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                        //}
                    }

                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.ResponseData = ex;
                    jsonResponse.HasError = true;
                }
                return jsonResponse;
            }
        }
        public JsonResponse GetSecurityMatrixReportForExcel(int roleid, string username, string ip)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {


                    SqlCommand cmd = new SqlCommand("GET_SECURITY_MATRIX_REPORT", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter excelParam = new SqlParameter();
                    excelParam = cmd.Parameters.AddWithValue("@UserName", username);
                    excelParam = cmd.Parameters.AddWithValue("@role_id", roleid);
                    excelParam = cmd.Parameters.AddWithValue("@IPAddress", ip);
                    excelParam = cmd.Parameters.AddWithValue("@ENTRYDATE", DateTime.Now);
                    excelParam.Direction = ParameterDirection.Input;
                    DataSet ds = new DataSet("Data");
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;

                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        jsonResponse.IsSuccess = false;
                        jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                    }
                    else
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = ds;
                    }



                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.ResponseData = ex;
                    jsonResponse.HasError = true;
                }
                return jsonResponse;
            }
        }
        public JsonResponse GetSecurityMatrixReport(int roleid, string username, string ip)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                try
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("Role_ID", roleid);
                    param.Add("UserName", username);
                    param.Add("IPAddress", ip);
                    param.Add("EntryDate", DateTime.Now);
                    List<dynamic> aTTMenu = connection.Query<dynamic>("GET_SECURITY_MATRIX_REPORT", param, commandType: CommandType.StoredProcedure).ToList();
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

        public JsonResponse GetUserDetailsAuditReport(string reportType, string fromDate, string ToDate, string userId, string _loggedInUser, string IpAddress)
        {
            JsonResponse response = new JsonResponse();
            if (reportType == "pdf")
            {
                response = GetUserDetailsAuditReportPDF(fromDate, ToDate, userId, _loggedInUser, IpAddress);
            }
            else if (reportType == "excel")
            {
                response = GetUserDetailsAuditReportExcel(fromDate, ToDate, userId, _loggedInUser, IpAddress);

            }
            return response;
        }

        public JsonResponse GetUserDetailsAuditReportPDF(string fromDate, string ToDate, string userId, string username, string IpAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                try
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("fromDate", fromDate);
                    param.Add("toDate", ToDate);
                    param.Add("UserName", username);
                    param.Add("IPAddress", IpAddress);
                    param.Add("EntryDate", DateTime.Now);
                    param.Add("userId", userId);
                    List<dynamic> aTTMenu = connection.Query<dynamic>("GET_USER_AUDIT_REPORT", param, commandType: CommandType.StoredProcedure).ToList();
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
        public JsonResponse GetUserDetailsAuditReportExcel(string fromDate, string ToDate, string userId, string username, string IpAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {


                    SqlCommand cmd = new SqlCommand("GET_USER_AUDIT_REPORT", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter excelParam = new SqlParameter();
                    excelParam = cmd.Parameters.AddWithValue("@UserName", username);
                    excelParam = cmd.Parameters.AddWithValue("@fromdate", fromDate);
                    excelParam = cmd.Parameters.AddWithValue("@userId", userId);
                    excelParam = cmd.Parameters.AddWithValue("@toDate", ToDate);
                    excelParam = cmd.Parameters.AddWithValue("@IPAddress", IpAddress);
                    excelParam = cmd.Parameters.AddWithValue("@ENTRYDATE", DateTime.Now);
                    excelParam.Direction = ParameterDirection.Input;
                    DataSet ds = new DataSet("Data");
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;

                    da.Fill(ds);

                    if (ds.Tables.Count == 0)
                    {
                        jsonResponse.IsSuccess = false;
                        jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                    }
                    else
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = ds;
                    }



                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.ResponseData = ex;
                    jsonResponse.HasError = true;
                }
                return jsonResponse;
            }

        }


    }
}
