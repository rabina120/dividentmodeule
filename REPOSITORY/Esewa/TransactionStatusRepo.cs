using Dapper;
using Entity.Common;
using Entity.Dividend;
using Entity.Esewa;
using Interface.Esewa;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Repository.Esewa.EsewaHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Esewa
{
    public class TransactionStatusRepo : ITransactionStatus
    {
        IOptions<ReadConfig> _connectionString;

        private IConfiguration _configuration;
        string _keys;

        public TransactionStatusRepo(IOptions<ReadConfig> connectionString, IConfiguration configuration)
        {
            _connectionString = connectionString;
            _configuration = configuration;
            _keys = "EsewaKeys";
        }
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
                    List<ATTDividend> aTTDividends = connection.Query<ATTDividend>(sql: "ESEWATRANSACTIONSTATUS_GETDIVIDENDLIST", param: param, null, commandType: CommandType.StoredProcedure).ToList();
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


        public JsonResponse GetBatchList(string CompCode, string DivCode, string UserName, string IPAddress)
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
                    List<int> batch = connection.Query<int>(sql: "ESEWATRANSACTIONSTATUS_GETBATCHLIST", param: param, null, commandType: CommandType.StoredProcedure).ToList();
                    if (batch.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = batch;
                    }
                    else
                    {
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
        public async Task<List<ATTBatchProcessing>> GetAccountValidatedData(ATTDataListRequest request, string CompCode, string DivCode, string BatchID, string BatchStatus)
        {
            try
            {
                using (var connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("SearchValue", request.SearchValue.Trim(), DbType.String);
                    parameters.Add("PageNo", request.PageNo, DbType.Int32);
                    parameters.Add("PageSize", request.PageSize, DbType.Int32);
                    parameters.Add("SortColumn", request.SortColumn, DbType.Int32);
                    //parameters.Add("SortColumnName", request.SortColumnName, DbType.String);
                    parameters.Add("SortDirection", request.SortDirection, DbType.String);
                    parameters.Add("CompCode", CompCode.Trim(), DbType.String);
                    parameters.Add("DivCode", DivCode);
                    parameters.Add("BatchNo", BatchID.Trim(), DbType.String);
                    parameters.Add("BatchStatus", BatchStatus.Trim(), DbType.String);


                    List<ATTBatchProcessing> batchProcessings = (await connection.QueryAsync<ATTBatchProcessing>("ESEWATRANSACTION_GETACCOUNTVALIDATEDDATA", parameters, commandType: CommandType.StoredProcedure)).ToList();
                    return batchProcessings;

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public JsonResponse UpdateTransactionStatus(string CompCode, string BatchNo, string DivCode, string BankUserName, string BankPassword, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);
                    param.Add("@P_DivCode", DivCode);
                    param.Add("@P_BatchNO", BatchNo);
                    param.Add("@P_UserName", UserName);
                    param.Add("@P_IPAddress", IPAddress);
                    param.Add("@P_Entry_Date", DateTime.Now);
                    List<ATTEBatchProcessing> aTTEBatchProcessings = connection.Query<ATTEBatchProcessing>(sql: "ESEWATRANSACTIONSTATUS_GETVALIDATEDDATAFORUPDATE", param: param, null, commandType: CommandType.StoredProcedure).ToList();
                    if (aTTEBatchProcessings.Count > 0)
                    {
                       
                        try
                        {
                            List<ATTEBatchProcessing> aTTEBatchProcessingAPIReturn = new List<ATTEBatchProcessing>(); 
                            var res = EsewaAPIMethods.TransactionStatus(aTTEBatchProcessings);
                            if (res.IsSuccess) { aTTEBatchProcessingAPIReturn = (List<ATTEBatchProcessing>)res.ResponseData; }
                            else return res;

                            using (SqlTransaction tran = connection.BeginTransaction())
                            {
                                DataTable dt = new DataTable();

                                dt.Columns.Add("WarrantNo");
                                dt.Columns.Add("TransactionCode");
                                dt.Columns.Add("TransactionMessage");
                                dt.Columns.Add("TransactionDetail");
                                aTTEBatchProcessingAPIReturn.ForEach(x => dt.Rows.Add(x.sub_token, x.TransactionCode, x.TransactionMessage, x.TransactionDetail));
                                SqlCommand cmd = new SqlCommand("ESEWA_TRANSACTION_STATUS_UPDATE", connection);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Transaction = tran;
                                SqlParameter sqlParameter = cmd.Parameters.AddWithValue("@UDT", dt);
                                sqlParameter = cmd.Parameters.AddWithValue("@P_COMPCODE", CompCode);
                                sqlParameter = cmd.Parameters.AddWithValue("@P_DivCode", DivCode);
                                sqlParameter = cmd.Parameters.AddWithValue("@P_BATCHID", BatchNo);
                                sqlParameter = cmd.Parameters.AddWithValue("@P_IP_ADDRESS", IPAddress);
                                sqlParameter = cmd.Parameters.AddWithValue("@P_USERNAME", UserName);
                                sqlParameter = cmd.Parameters.AddWithValue("@P_DATE_NOW", DateTime.Now);
                                sqlParameter.Direction = ParameterDirection.Input;
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        if (reader.GetString(0) == "true")
                                        {
                                            jsonResponse.IsSuccess = true;
                                            jsonResponse.Message = reader.GetString(1);
                                        }
                                        else
                                        {
                                            jsonResponse.Message = reader.GetString(1);
                                        }

                                    }
                                }
                                if (jsonResponse.IsSuccess)
                                    tran.Commit();
                                else
                                    tran.Rollback();

                            }
                        }
                        catch (Exception ex)
                        {
                            //UserName = _loggedInUser.GetUserName();
                            DateTime Date = DateTime.Now;
                            //BatchNo = aTTAccountDetails.token;
                            //_logDetails.InsertLogDetails(BatchNo, ex.Message, UserName);
                            jsonResponse.Message = ex.Message;
                            jsonResponse.IsSuccess = false;
                            jsonResponse.HasError = true;

                            return jsonResponse;
                        }
                    }
                    else
                    {
                        jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.HasError = true;
                    jsonResponse.ResponseData = ex;
                }
                return jsonResponse;
            }
        }

    }
}
