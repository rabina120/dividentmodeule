using Dapper;
using Entity.Common;
using Entity.DemateDividend;
using Entity.Dividend;
using Interface.DividendManagement;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.DividendManagement
{
    public class BoToBoTransferRepo : IBoToBoTransfer
    {
        IOptions<ReadConfig> _connectionString;

        public BoToBoTransferRepo(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }

        public JsonResponse GetHolderInformation(string CompCode, string BOID, string UserName, string IPAddress, string Action)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);
                    param.Add("@P_BOID", BOID);
                    param.Add("@P_UserName", UserName);
                    param.Add("@P_IPADdress", IPAddress);
                    param.Add("@p_Entry_Date", DateTime.Now);
                    param.Add("@P_SELECTEDACTION", Action);
                    param.Add("@P_ID", null);
                    List<ATTDemateDividend> obj = connection.Query<ATTDemateDividend>("dbo.GET_HOLDER_FOR_BOID_CHANGE", param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                    if (obj == null || obj.Count<=0)
                    {
                        response.Message = "Data not found!!!!!";
                        response.IsSuccess = false;
                    }
                    else
                    {
                        response.ResponseData = obj;
                        response.IsSuccess = true;
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                    response.IsSuccess = false;
                }
                return response;
            }
        }

        public JsonResponse SaveHolderForBoidChange(ATTHolderForBoidChange HolderInfo)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                using (SqlTransaction trans = connection.BeginTransaction())
                {
                    try
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@P_CompCode", HolderInfo.CompCode);
                        param.Add("@P_BOID", HolderInfo.PreviousBOID);
                        param.Add("@P_BOID_NEW", HolderInfo.NewBoid);
                        param.Add("@P_NAME", HolderInfo.NewName);
                        param.Add("@P_FANAME", HolderInfo.NewFName);
                        param.Add("@P_GRFANAME", HolderInfo.NewGFName);
                        param.Add("@P_ADDRESS", HolderInfo.NewAddress);
                        param.Add("@P_TRANTYPE", HolderInfo.TranType);
                        param.Add("@P_UserName", HolderInfo.UserName);
                        param.Add("@P_IPADdress", HolderInfo.IPAddress);
                        param.Add("@p_Entry_Date", DateTime.Now);
                        param.Add("@P_SELECTEDACTION", HolderInfo.Action);
                        param.Add("@P_ID", null);
                        response = connection.Query<JsonResponse>("dbo.SAVE_HOLDER_FOR_BOID_CHANGE", param, trans, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        response.Message = ex.Message;
                        response.IsSuccess = false;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                return response;
            }
        }

        public JsonResponse GetHolderChangelistForPosting(ATTHolderForBoidChange postingData)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    DynamicParameters parma = new DynamicParameters();
                    parma.Add("@P_CompCode", postingData.CompCode);
                    parma.Add("@P_UserName", postingData.UserName);
                    parma.Add("@P_TRANTYPE", postingData.TranType);
                    parma.Add("@P_IPADdress", postingData.IPAddress);
                    parma.Add("@p_Entry_Date", DateTime.Now);
                    parma.Add("@P_PARENTID", postingData.ParentId);
                    List<ATTDemateDividend> list = connection.Query<ATTDemateDividend>("dbo.GET_HOLDER_FOR_BOID_CHANGE_POSTING", parma, commandType: System.Data.CommandType.StoredProcedure).ToList();
                    response.ResponseData = list;
                    response.IsSuccess = true;
                }
                catch(Exception ex)
                {
                    response.Message = ex.Message;
                    response.IsSuccess = false;
                }

            }
            return response;
        }

        public JsonResponse VerifyRejectHolderList(ATTHolderForBoidChange postingData)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();

                using (SqlTransaction tran = connection.BeginTransaction())
                {
                    try
                    {
                        DataTable dt = new DataTable();
                        dt.Columns.Add("Column1");
                        foreach (ATTDemateDividend boidHolder in postingData.SelectedList)
                        {
                            dt.Rows.Add(boidHolder.Id);
                        }
                        SqlCommand cmd = new SqlCommand("dbo.SAVE_HOLDER_FOR_BOID_CHANGE_POSTING", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Transaction = tran;
                        SqlParameter param = cmd.Parameters.AddWithValue("@udt", dt);
                        param = cmd.Parameters.AddWithValue("@P_Posting_Remarks", postingData.Remarks);
                        param = cmd.Parameters.AddWithValue("@P_App_Date", postingData.ApprovedDate);
                        param = cmd.Parameters.AddWithValue("@P_UserName", postingData.UserName);
                        param = cmd.Parameters.AddWithValue("@P_CompCode", postingData.CompCode);
                        param = cmd.Parameters.AddWithValue("@P_SELECTEDACTION", postingData.ActionType);
                        //param = cmd.Parameters.AddWithValue("@P_SelectedRecordType", postingData.TranType);
                        param = cmd.Parameters.AddWithValue("@P_IPADdress", postingData.IPAddress);
                        param = cmd.Parameters.AddWithValue("@p_Entry_Date", DateTime.Now);
                        param.Direction = ParameterDirection.Input;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {

                            if (reader.Read())
                            {
                                if (reader.GetInt32(0) == 1)
                                {
                                    response.IsSuccess = true;
                                    string message = "Posted Records Successfully !!!";
                                    if (postingData.ActionType == "R")
                                        message = "Rejected Records Successfully !!!";
                                    else if (postingData.ActionType == "D")
                                        message = "Deleted Records Successfully !!!";

                                    response.Message = message;
                                    

                                }
                                else
                                {
                                    response.IsSuccess = false;
                                    string message = "Posting Records Failed !!!";
                                    if (postingData.ActionType == "R")
                                        message = "Rejecting Records Failed !!!";
                                    else
                                        message = "Deleting Records Failed !!!";

                                   
                                    response.Message = message;
                                }

                            }
                        }
                        if (!response.IsSuccess)
                        {
                            tran.Rollback();
                        }
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        response.IsSuccess = false;
                        response.Message = ex.Message;
                    }
                }
            }
            return response;
        }
    }
}

