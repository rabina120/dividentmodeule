using Dapper;
using Entity.Common;
using Entity.Esewa;
using ENTITY.FundTransfer;
using Interface.ShareHolder;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.ShareHolder
{
    public class PaymentSchedulePostingRepo : IPaymentSchedulePosting
    {
        IOptions<ReadConfig> _connectionString;

        public PaymentSchedulePostingRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }

        public JsonResponse DividendBatchDetails(string CompCode, string DivCode)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@CompCode", CompCode);
                    param.Add("@DivCode", DivCode);

                    ATTDividendBatch aTTDividendBatch = connection.Query<ATTDividendBatch>("GET_DIVIDEND_BATCH", param, commandType: CommandType.StoredProcedure)?.FirstOrDefault();

                    if (aTTDividendBatch != null)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = aTTDividendBatch;
                    }
                    else
                    {
                        jsonResponse.IsSuccess = false;
                        jsonResponse.Message = "No Batch Found !!!";
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

        public JsonResponse ExportToExcel(string CompCode, string BatchNo)
        {
            JsonResponse response = new JsonResponse();
            var parameters = new DynamicParameters();

            using (var connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                try
                {
                    parameters.Add("CompCode", CompCode);
                    parameters.Add("BatchNo", BatchNo);

                    List<ATTExportBatchProcessing> batchProcessings = connection.Query<ATTExportBatchProcessing>("BATCH_PROCESSING_EXPORT", parameters, commandType: CommandType.StoredProcedure)?.ToList();

                    if (batchProcessings.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = batchProcessings;
                    }
                    else
                    {
                        response.Message = "No Records Found !!!";
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

        public async Task<List<ATTBatchProcessing>> GetBatchProcessingAsync(ATTDataListRequest request, string CompCode, string BatchNo, string DivCode = null, string PostedData = null, bool isAccountValidate = false, string ProcedureName = null)
        {
            string procedure = string.Empty;
            try
            {
                using (var connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
                {
                    connection.Open();
                    try
                    {
                        procedure = ProcedureName;
                        var parameters = new DynamicParameters();
                        parameters.Add("SearchValue", request.SearchValue.Trim(), DbType.String);
                        parameters.Add("PageNo", request.PageNo, DbType.Int32);
                        parameters.Add("PageSize", request.PageSize, DbType.Int32);
                        parameters.Add("SortColumn", request.SortColumn, DbType.Int32);
                        //parameters.Add("SortColumnName", request.SortColumnName, DbType.String);
                        parameters.Add("SortDirection", request.SortDirection, DbType.String);
                        parameters.Add("CompCode", CompCode.Trim(), DbType.String);
                        parameters.Add("DivCode", DivCode);
                        parameters.Add("BatchNo", BatchNo.Trim(), DbType.String);


                        List<ATTBatchProcessing> batchProcessings = (await connection.QueryAsync<ATTBatchProcessing>(procedure, parameters, commandType: CommandType.StoredProcedure)).ToList();
                        return batchProcessings;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }


        }

        public JsonResponse SaveBatchProcessingList(string CompCode, string BatchNo, string DivCode, string UserName)
        {
            JsonResponse response = new JsonResponse();
            using (var connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    using (SqlTransaction trans = connection.BeginTransaction())
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("@P_CompCode", CompCode);
                        parameters.Add("@P_BatchNo", BatchNo);
                        parameters.Add("@P_UserName", UserName);
                        parameters.Add("@P_DivCode", DivCode);

                        connection.Query("SaveBatchPaymentList", parameters, trans, commandType: CommandType.StoredProcedure);

                        trans.Commit();

                        response.IsSuccess = true;
                        response.Message = "Record Have Been Saved";
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
