using Dapper;
using Entity.Common;
using Entity.Dividend;
using ENTITY.FundTransfer;
using INTERFACE.FundTransfer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace REPOSITORY.FundTransfer
{
    public class TransactionProcessingRepo : ITransactionProcessing
    {
        IOptions<ReadConfig> _connectionString;
        private IConfiguration _configuration;
        private readonly IBatchProcessing _batchProcessing;
        public TransactionProcessingRepo(IOptions<ReadConfig> connectionString, IConfiguration configuration, IBatchProcessing batchProcessing)
        {
            _connectionString = connectionString;
            _configuration = configuration;
            _batchProcessing = batchProcessing;
        }
        #region batch operation
        public JsonResponse GetAllActiveBatch(string CompCode, string DivCode, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);
                    param.Add("@P_DivCode", DivCode);
                    param.Add("@P_UserName", UserName);
                    param.Add("@P_IPAddress", IPAddress);
                    param.Add("@P_Entry_Date", DateTime.Now);
                    var batchlists = connection.Query<int>(sql: "FT_TRANSACTIONPROCESSING_GET_ALL_BATCH", param: param, null, commandType: CommandType.StoredProcedure).ToList();
                    if (batchlists.Count < 1)
                    {
                        response = new JsonResponse();
                        response.IsSuccess = false;
                        response.Message = "No Active Batch For Transaction";
                        response.IsValid = true;

                    }
                    else
                    {
                        response.IsSuccess = true;
                        response.ResponseData = batchlists;
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
        public JsonResponse CheckBatchStatus(string CompCode, string DivCode, string BatchID, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);
                    param.Add("@P_DivCode", DivCode);
                    param.Add("@P_batchid", BatchID);
                    param.Add("@P_UserName", UserName);
                    param.Add("@P_IPAddress", IPAddress);
                    param.Add("@P_Entry_Date", DateTime.Now);
                    response = connection.Query<JsonResponse>(sql: "FT_TRANSACTIONPROCESSING_CHECK_BATCH_STATUS", param: param, null, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    if (response == null)
                    {
                        response = new JsonResponse();
                        throw new Exception("Batch is Not Ready For Trnasaction");
                    }
                    else
                    {
                        if (response.ResponseData.ToString().Contains(".")) response.IsValid = true;
                        response.ResponseData = response.ResponseData.ToString().Trim().Split('.')[0];
                    }

                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.HasError = true;
                    response.ResponseData = ex;
                    response.Message = ex.Message;
                }
                return response;
            }
        }
        public JsonResponse CloseBatch(string DivCode, string CompCode, string BatchID, string UserName, string IPAddress, string Remarks)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {


                        DynamicParameters param = new DynamicParameters();
                        param.Add("@P_CompCode", CompCode);
                        param.Add("@P_BATCHID", BatchID);
                        param.Add("@P_DivCode", DivCode);
                        param.Add("@P_UserName", UserName);
                        param.Add("@P_IPAddress", IPAddress);
                        param.Add("@P_Remarks", Remarks);
                        param.Add("@P_Entry_Date", DateTime.Now);

                        response = connection.Query<JsonResponse>(sql: "FT_BATCH_CLOSE_BATCH", param: param, transaction, commandType: CommandType.StoredProcedure).FirstOrDefault();
                        if (response.IsSuccess)
                            transaction.Commit();
                        else
                            transaction.Rollback();
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
        #endregion
        #region source bank

        public JsonResponse GetSourceBanks(string CompCode, string UserName, string IPAddress, string BankID = null)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);
                    param.Add("@P_bankid", BankID);
                    param.Add("@P_UserName", UserName);
                    param.Add("@P_IPAddress", IPAddress);
                    param.Add("@P_Entry_Date", DateTime.Now);
                    List<ATTSourceBank> aTTDividends = connection.Query<ATTSourceBank>(sql: "FT_TRANSACTIONPROCESSING_GETSOURCEBANK", param: param, null, commandType: CommandType.StoredProcedure).ToList();
                    if (aTTDividends.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = aTTDividends;
                    }
                    else
                    {
                        response.Message = ATTMessages.NO_TABLES_FOUND;
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
        #endregion
        #region transaction processing
        public JsonResponse TransactionProcessing(string DivCode, string CompCode, string BatchID, string BankID, string UserName, string IPAddress)
        {
            List<ATTSourceBank> sourceBank = new List<ATTSourceBank>();
            JsonResponse resBank = GetSourceBanks(CompCode, UserName, IPAddress, BankID);
            if (resBank.IsSuccess) sourceBank = (List<ATTSourceBank>)resBank.ResponseData;
            else throw new Exception("Cannot Load Source Bank Data!");
            JsonResponse jsonResponse = new JsonResponse();
            jsonResponse = _batchProcessing.TransactionProcessing(DivCode, CompCode, BatchID, sourceBank[0].SwiftCode, sourceBank[0].AccountNo, sourceBank[0].AccountName, UserName, IPAddress);
                return jsonResponse;
          
        }
        #endregion

        public JsonResponse GetDividendList(string CompCode, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);
                    param.Add("@P_UserName", UserName);
                    param.Add("@P_IPAddress", IPAddress);
                    param.Add("@P_Entry_Date", DateTime.Now);
                    List<ATTDividend> aTTDividends = connection.Query<ATTDividend>(sql: "FT_TRANSACTIONPROCESSING_GETDIVIDENDLIST", param: param, null, commandType: CommandType.StoredProcedure).ToList();
                    if (aTTDividends.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = aTTDividends;
                    }
                    else
                    {
                        response.Message = ATTMessages.NO_TABLES_FOUND;
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


       
    }
}
