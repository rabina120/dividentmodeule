
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
    public class HolderMergeEntryRepo : IHolderMergeEntry
    {
        IOptions<ReadConfig> _connectionString;

        public HolderMergeEntryRepo(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }

        public JsonResponse GetHolderForMerge(string CompCode, string ShHolderNo, string SelectedAction, string Username, string IP, string MergeNo = null)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_COMPCODE", CompCode);
                    parameters.Add("@P_SHHOLDERNO", ShHolderNo);
                    parameters.Add("@P_SELECTEDACTION", SelectedAction);
                    parameters.Add("@P_USERNAME", Username);
                    parameters.Add("@P_MERGENO", MergeNo);

                    parameters.Add("@P_IP_ADDRESS", IP);
                    ATTShHolder aTTShHolder = connection.Query<ATTShHolder>(sql: "GET_HOLDER_FOR_MERGE", param: parameters, transaction: null, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    if (aTTShHolder != null)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = aTTShHolder;
                    }
                    else
                    {
                        response.Message = "Holder Not Found !!!";
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                }
            }
            return response;
        }

        public JsonResponse GetMaxMergeNo(string CompCode)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_COMPCODE", CompCode);

                    int? maxRegNo = connection.Query<int?>(sql: "GET_MAX_MERGE_NO", param: parameters, transaction: null, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    if (maxRegNo == null)
                    {
                        maxRegNo = 1;
                    }
                    else
                    {
                        maxRegNo++;
                    }
                    response.IsSuccess = true;
                    response.ResponseData = maxRegNo;
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                }
            }
            return response;
        }

        public JsonResponse GetMergeHolderList(string CompCode, string MergeNo = null)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_COMPCODE", CompCode);
                    parameters.Add("@P_MERGENO", MergeNo);

                    List<ATTMergeDetail> aTTShHolders = connection.Query<ATTMergeDetail>(sql: "GET_MERGE_HOLDER_LIST", param: parameters, transaction: null, commandType: CommandType.StoredProcedure).ToList();
                    if (aTTShHolders.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = aTTShHolders;
                    }
                    else
                    {
                        response.Message = "No Holders Found !!!";
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                }
            }
            return response;
        }

        public JsonResponse SaveHolderForMerge(string CompCode, ATTShHolder shholder, ATTShHolder shHolderForMerge, string SelectedAction, string Username, string IP, string Remarks, string MergeDate, string MergeNo = null)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        try
                        {
                            DynamicParameters parameters = new DynamicParameters();
                            parameters.Add("@P_COMPCODE", CompCode);
                            parameters.Add("@P_SHHOLDERNO", shholder.ShholderNo);
                            parameters.Add("@P_TOTALKITTA", shholder.TotalKitta);
                            parameters.Add("@P_SHHOLDERNO_TO", shHolderForMerge.ShholderNo);
                            parameters.Add("@P_TOTALKITTA_TO", shHolderForMerge.TotalKitta);
                            parameters.Add("@P_SELECTEDACTION", SelectedAction);
                            parameters.Add("@P_USERNAME", Username);
                            parameters.Add("@P_REMARKS", Remarks);
                            parameters.Add("@P_MERGEDATE", MergeDate);
                            parameters.Add("@P_MERGENO", MergeNo);
                            parameters.Add("@P_IP_ADDRESS", IP);

                            JsonResponse responseFromDatabase = connection.Query<JsonResponse>(sql: "SAVE_HOLDER_FOR_MERGE", param: parameters, transaction: tran, commandType: CommandType.StoredProcedure).FirstOrDefault();
                            if (responseFromDatabase.IsSuccess)
                                tran.Commit();
                            else
                            {
                                tran.Rollback();
                            }
                            response = responseFromDatabase;
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            response.Message = ex.Message;
                        }
                        connection.Close();
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
