
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
    public class ShareHolderPostingRepo : IShareHolderPosting
    {
        IOptions<ReadConfig> connectionString;
        public ShareHolderPostingRepo(IOptions<ReadConfig> _connectionString)
        {
            connectionString = _connectionString;
        }

        public JsonResponse GetHolderForApproval(string CompCode, string SelectedAction,string FromDate,string ToDate, string UserName, string IP)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@CompCode", CompCode);
                    param.Add("@P_SelectedAction", SelectedAction);
                    param.Add("@P_FROMDATE", FromDate);
                    param.Add("@P_TODATE", ToDate);
                    param.Add("@P_USERNAME", UserName);
                    param.Add("@P_IP_ADDRESS", IP);
                    param.Add("@P_DATE_NOW", DateTime.Now);
                    response.ResponseData = connection.Query<ATTShHolder>(sql: "Get_Holder_For_ApprovalRK", param: param, null, commandType: CommandType.StoredProcedure).ToList();


                    if (response.ResponseData != null)
                    {
                        response.IsSuccess = true;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = ATTMessages.NO_RECORDS_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.HasError = true;
                    response.ResponseData = ex;
                }
                return response;
            }
        }


        public JsonResponse PostShholderInfo(List<ATTShHolder> aTTShHolder, string CompCode, string Username, string SelectedRecordType, string IP)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        DataTable dt = new DataTable();
                        dt.Columns.Add("ShHolderNo");
                        foreach (ATTShHolder shHolder in aTTShHolder)
                        {
                            dt.Rows.Add(shHolder.ShholderNo);
                        }
                        SqlCommand cmd = new SqlCommand("Post_Shholder_InfoRK", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Transaction = tran;
                        SqlParameter param = cmd.Parameters.AddWithValue("@udt", dt);
                        param = cmd.Parameters.AddWithValue("@P_Remarks", aTTShHolder[0].Remarks);
                        param = cmd.Parameters.AddWithValue("@P_ApprovedDate", aTTShHolder[0].ApprovedDate);
                        param = cmd.Parameters.AddWithValue("@USERNAME", Username);
                        param = cmd.Parameters.AddWithValue("@P_CompCode", CompCode);
                        param = cmd.Parameters.AddWithValue("@P_ActionType", aTTShHolder[0].ActionType);
                        param = cmd.Parameters.AddWithValue("@P_SelectedRecordType", SelectedRecordType);

                        param = cmd.Parameters.AddWithValue("@P_IP_ADDRESS", IP);
                        param = cmd.Parameters.AddWithValue("@P_DATE_NOW", DateTime.Now);
                        param.Direction = ParameterDirection.Input;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {

                            if (reader.Read())
                            {
                                if (reader.GetString(0) == "1")
                                {
                                    response.IsSuccess = true;
                                    string message = "Posted Records Successfully !!!";
                                    if (aTTShHolder[0].ActionType == "R")
                                        message = "Rejected Records Successfully !!!";
                                    else if (aTTShHolder[0].ActionType == "D")
                                        message = "Deleted Records Successfully !!!";

                                    response.Message = message;

                                }
                                else
                                {
                                    response.IsSuccess = false;
                                    string message = "Posting Records Failed !!!";
                                    if (aTTShHolder[0].ActionType == "R")
                                        message = "Rejecting Records Failed !!!";
                                    else
                                        message = "Deleting Records Failed !!!";

                                    response.Message = message;
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

                    response.IsSuccess = false;
                    response.HasError = true;
                    response.ResponseData = ex;
                }
            }
            return response;

        }
    }
}
