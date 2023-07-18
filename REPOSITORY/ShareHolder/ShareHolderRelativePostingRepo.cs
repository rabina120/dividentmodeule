
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
    public class ShareHolderRelativePostingRepo : IShareHolderRelativePosting
    {
        IOptions<ReadConfig> connectionString;
        public ShareHolderRelativePostingRepo(IOptions<ReadConfig> _connectionString)
        {
            connectionString = _connectionString;
        }
        public JsonResponse GetHolderForPosting(string CompCode, string fromDate, string toDate, string UserName, string IP)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_COMPCODE", CompCode);
                    parameters.Add("@P_fromDate", fromDate);
                    parameters.Add("@P_toDate", toDate);
                    parameters.Add("@P_USERNAME", UserName);
                    parameters.Add("@P_IP_ADDRESS", IP);
                    parameters.Add("@P_DATE_NOW", DateTime.Now);
                    List<ATTShHolderForRelative> aTTShHolderForRelatives = connection.Query<ATTShHolderForRelative>(sql: "GET_SHHOLER_RELATIVE_FOR_POSTINGRK", param: parameters, commandType: System.Data.CommandType.StoredProcedure).ToList();
                    if (aTTShHolderForRelatives.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = aTTShHolderForRelatives;
                    }
                    else
                    {
                        response.Message = "Cannot Find Any Holders !!!";
                    }
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

        public JsonResponse SaveHolderPosting(string CompCode, List<ATTShHolderForRelative> attShHolderForRelatives, string SelectedAction, string ApprovedDate, string UserName, string IP)
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
                        dt.Columns.Add("RHolderNo");
                        dt.Columns.Add("SN");

                        attShHolderForRelatives.ForEach(x => dt.Rows.Add(x.ShholderNo, x.RHolderNo, x.SN));

                        SqlCommand cmd = new SqlCommand("SAVE_SHHOLDER_RELATIVE_POSTINGRK", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Transaction = tran;
                        SqlParameter param = cmd.Parameters.AddWithValue("@udt", dt);
                        param = cmd.Parameters.AddWithValue("@P_COMPCODE", CompCode);
                        param = cmd.Parameters.AddWithValue("@P_USERNAME", UserName);
                        param = cmd.Parameters.AddWithValue("@P_SELECTED_ACTION", SelectedAction);
                        param = cmd.Parameters.AddWithValue("@P_APPROVEDDATE", ApprovedDate);
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
                                    string msg = SelectedAction == "A" ? "Approved" : "Rejected";
                                    response.Message = "Holder " + msg + " Successfully !!!";
                                }
                                else
                                {
                                    string msg = SelectedAction == "A" ? "Approve" : "Reject";
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
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
            }
            return response;
        }
    }
}
