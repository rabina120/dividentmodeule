
using Dapper;
using Entity.Common;
using Entity.ShareHolder;
using Interface.ShareHolder;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.ShareHolder
{
    public class PostingUpdateApplicationRepo : IPostingUpdateApplication
    {
        IOptions<ReadConfig> _connectionString;
        public PostingUpdateApplicationRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }
        public JsonResponse GetAllApplicationList(string UserName, string CompCode, string fromDate, string toDate, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);
                    param.Add("@P_fromDate", fromDate);
                    param.Add("@P_toDate", toDate);
                    param.Add("@P_USERNAME", UserName);
                    param.Add("@P_IP_ADDRESS", IPAddress);
                    param.Add("@P_ENTRY_DATE", DateTime.Now);
                    response.ResponseData = connection.Query<ATTShHolderForUpdate>(sql: "GET_APPENTRY_LIST_FOR_POSTING", param: param, null, commandType: CommandType.StoredProcedure)?.ToList();

                    if (response.ResponseData != null)
                    {
                        response.IsSuccess = true;
                    }
                    else
                    {
                        response.Message = ATTMessages.NO_RECORDS_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    response.HasError = true;
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                }
                return response;
            }
        }

        public JsonResponse SaveApplication(List<ATTShHolderForUpdate> aTTShHolders, string UserName, string CompCode, string PostingDate, string SelectedAction, string PostingRemarks, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {

                        DataTable dt = new DataTable();

                        dt.Columns.Add("ApplicationNo");
                        dt.Columns.Add("ShHolderNo");

                        aTTShHolders.ForEach(x => dt.Rows.Add(x.ApplicationNo, x.ShholderNo));

                        SqlCommand cmd = new SqlCommand("SAVE_SHOLDER_UPDATE_APPLICATION_POSTING", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Transaction = tran;
                        SqlParameter param = cmd.Parameters.AddWithValue("@udt", dt);
                        param = cmd.Parameters.AddWithValue("@P_COMPCODE", CompCode);
                        param = cmd.Parameters.AddWithValue("@P_USERNAME", UserName);
                        param = cmd.Parameters.AddWithValue("@P_SELECTED_ACTION", SelectedAction);
                        param = cmd.Parameters.AddWithValue("@P_POSTING_REMARKS", PostingRemarks);
                        param = cmd.Parameters.AddWithValue("@P_POSTING_DATE", PostingDate);
                        param = cmd.Parameters.AddWithValue("@P_IP_ADDRESS", IPAddress);
                        param = cmd.Parameters.AddWithValue("@P_DATE_NOW", DateTime.Now);
                        param.Direction = ParameterDirection.Input;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader.GetString(0) == "1")
                                {
                                    response.IsSuccess = true;
                                    string msg = string.Empty;
                                    if (SelectedAction == "A")
                                        msg = "Approved";
                                    else if (SelectedAction == "R")
                                        msg = "Rejected";
                                    else
                                        msg = "Deleted";
                                    response.Message = "Holder " + msg + " Successfully !!!";
                                }
                                else
                                {
                                    string msg = string.Empty;
                                    if (SelectedAction == "A")
                                        msg = "Approved";
                                    else if (SelectedAction == "R")
                                        msg = "Rejected";
                                    else
                                        msg = "Deleted";
                                    response.Message = "Failed To " + msg + "Records !!!";
                                }

                            }
                        }
                        if (response.IsSuccess)
                            tran.Commit();
                        else
                            tran.Rollback();
                    }

                }
                catch (Exception ex)
                {

                    response.HasError = true;
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                }
            }
            return response;
        }

    }
}
