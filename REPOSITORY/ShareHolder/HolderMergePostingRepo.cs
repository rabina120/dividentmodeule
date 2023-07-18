
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
    public class HolderMergePostingRepo : IHolderMergePosting
    {
        public IOptions<ReadConfig> _connectionString;

        public HolderMergePostingRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }

        public JsonResponse GetHolderForPosting(string CompCode, string FromDate, string ToDate, string Username)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_CompCode", CompCode);
                    parameters.Add("@P_FROMDATE", FromDate);
                    parameters.Add("@P_TODATE", ToDate);
                    parameters.Add("@P_UserName", Username);

                    List<ATTMergeDetail> aTTMergeDetails = connection.Query<ATTMergeDetail>(sql: "GET_HOLDER_MERGE_DETAIL_FOR_POSTING", param: parameters, commandType: CommandType.StoredProcedure).ToList();
                    if (aTTMergeDetails.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = aTTMergeDetails;
                    }
                    else
                    {
                        response.Message = "Cannot Find Any Holders !!!";
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                }

            }
            return response;
        }

        public JsonResponse SaveHolderPosting(string CompCode, string Username, List<ATTMergeDetail> attHolderMergeLists, string SelectedAction, string PostingDate, string Remarks)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                try
                {
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        try
                        {
                            DataTable dt = new DataTable();
                            dt.Columns.Add("merge_id");
                            dt.Columns.Add("holdernofrom");
                            dt.Columns.Add("holdernoto");
                            attHolderMergeLists.ForEach(shHolder =>
                            {
                                dt.Rows.Add(shHolder.merge_id, shHolder.holdernofrom, shHolder.holdernoto);

                            });

                            SqlCommand cmd = new SqlCommand("POST_SHHOLDER_MERGE", connection);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Transaction = tran;
                            SqlParameter param = cmd.Parameters.AddWithValue("@udt", dt);
                            param = cmd.Parameters.AddWithValue("@P_Remarks", Remarks);
                            param = cmd.Parameters.AddWithValue("@P_POSTING_Date", PostingDate);
                            param = cmd.Parameters.AddWithValue("@P_USERNAME", Username);
                            param = cmd.Parameters.AddWithValue("@P_CompCode", CompCode);
                            param = cmd.Parameters.AddWithValue("@P_ActionType", SelectedAction);
                            param.Direction = ParameterDirection.Input;

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {

                                if (reader.Read())
                                {
                                    if (reader.GetString(0) == "1")
                                    {
                                        response.IsSuccess = true;
                                        string message = "Posted Records Successfully !!!";
                                        if (SelectedAction == "R")
                                            message = "Rejected Records Successfully !!!";


                                        response.Message = message;

                                    }
                                    else
                                    {
                                        response.IsSuccess = false;
                                        string message = "Posting Records Failed !!!";
                                        if (SelectedAction == "R")
                                            message = "Rejecting Records Failed !!!";


                                        response.Message = message;
                                    }

                                }
                            }
                            if (response.IsSuccess)
                                tran.Commit();
                            else
                                tran.Rollback();

                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            response.Message = ex.Message;
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                }

            }
            return response;
        }
    }
}
