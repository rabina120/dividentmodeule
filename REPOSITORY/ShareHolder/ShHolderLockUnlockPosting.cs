
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
    public class ShHolderLockUnlockPosting : IShHolderLockUnlockPosting
    {
        IOptions<ReadConfig> connectionString;
        public ShHolderLockUnlockPosting(IOptions<ReadConfig> _connectionString)
        {
            this.connectionString = _connectionString;
        }
        public JsonResponse GetLockUnlockData(string CompCode, string FromDate, string ToDate, string RecordType, string Username, string IP)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@P_CompCode", CompCode);
                    dynamicParameters.Add("@P_FromDate", FromDate);
                    dynamicParameters.Add("@P_ToDate", ToDate);
                    dynamicParameters.Add("@P_RecordType", RecordType);
                    dynamicParameters.Add("@P_USERNAME", Username);
                    dynamicParameters.Add("@P_IP_ADDRESS", IP);
                    dynamicParameters.Add("@P_DATE_NOW", DateTime.Now);
                    List<ATTShHolderLockUnlock> aTTShHolderLockUnlocks = connection.Query<ATTShHolderLockUnlock>("GET_LOCK_UNLOCK_FOR_POSTINGRK", param: dynamicParameters, transaction: null, commandType: CommandType.StoredProcedure).ToList();
                    if (aTTShHolderLockUnlocks.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = aTTShHolderLockUnlocks;
                    }
                    else
                    {
                        response.Message = "No Holders Found !!!";
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

        public JsonResponse PostLockUnlockData(string CompCode, string RecordType, List<ATTShHolderLockUnlock> ShHolderLocks, string Remarks, string SelectedAction, string PostingDate, string UserName, string IP)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            DataTable dt = new DataTable();
                            dt.Columns.Add("lock_id");
                            dt.Columns.Add("ShholderNo");
                            ShHolderLocks.ForEach(shHolder =>
                            {
                                dt.Rows.Add(shHolder.lock_id, shHolder.ShholderNo);
                            });

                            SqlCommand cmd = new SqlCommand("POST_SHHOLDER_LOCK_UNLOCKRK", connection);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Transaction = transaction;
                            SqlParameter param = cmd.Parameters.AddWithValue("@udt", dt);
                            param = cmd.Parameters.AddWithValue("@P_Remarks", Remarks);
                            param = cmd.Parameters.AddWithValue("@P_POSTING_Date", PostingDate);
                            param = cmd.Parameters.AddWithValue("@P_USERNAME", UserName);
                            param = cmd.Parameters.AddWithValue("@P_CompCode", CompCode);
                            param = cmd.Parameters.AddWithValue("@P_ActionType", SelectedAction);
                            param = cmd.Parameters.AddWithValue("@P_RecordType", RecordType);
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
                                        if (SelectedAction == "R")
                                            message = "Rejected Records Successfully !!!";
                                        else if (SelectedAction == "D")
                                            message = "Deleted Records Successfully !!!";

                                        response.Message = message;

                                    }
                                    else
                                    {
                                        response.IsSuccess = false;
                                        string message = "Posting Records Failed !!!";
                                        if (SelectedAction == "R")
                                            message = "Rejecting Records Failed !!!";
                                        else
                                            message = "Deleting Records Failed !!!";

                                        response.Message = message;
                                    }

                                }
                            }
                            if (response.IsSuccess)
                                transaction.Commit();
                            else
                                transaction.Rollback();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            response.Message = ex.Message;

                        }
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
