using Dapper;
using Entity.Common;
using Entity.Esewa;
using Entity.Reports;
using Interface.Reports;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Reports
{
    public class EsewaReportsRepo : IEsewaReports
    {
        IOptions<ReadConfig> _connectionString;

        public EsewaReportsRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }

        public JsonResponse GenerateReport(string CompCode, string DivCode, string BatchNo, string ReportType)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
                {
                    string procedureName = GetReportProcedureName(ReportType, connection);

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@CompCode", CompCode);
                    param.Add("@DivCode", DivCode);
                    param.Add("@BatchNo", BatchNo);
                    param.Add("@ReportType", ReportType);
                    List<ATTBatchProcessing> batchProcessings = (connection.QueryAsync<ATTBatchProcessing>(procedureName, param, commandType: CommandType.StoredProcedure))?.Result.ToList();

                    jsonResponse.ResponseData = batchProcessings;
                    jsonResponse.TotalRecords = batchProcessings.Count();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return jsonResponse;
        }

        public string GetReportProcedureName(string ReportType, SqlConnection connection)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@RCode", ReportType);
            return connection.Query<string>("GET_ESEWA_REPORT_NAME", param, null, commandType: CommandType.StoredProcedure)?.FirstOrDefault();

        }

        public JsonResponse GetAllBatchList(string CompCode, string DivCode)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@CompCode", CompCode);
                    param.Add("@DivCode", DivCode);
                    List<ATTDividendBatch> aTTDividendBatches = connection.Query<ATTDividendBatch>("GET_ALL_BATCH_LIST", param, null, commandType: CommandType.StoredProcedure)?.ToList();

                    if (aTTDividendBatches.Count > 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = aTTDividendBatches;
                    }
                    else
                    {
                        jsonResponse.IsSuccess = false;
                        jsonResponse.Message = "No Batch Found !!!";
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return jsonResponse;
        }

        public JsonResponse GetAllReports()
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
                {
                    List<ATTEsewaReports> aTTEsewaReports = connection.Query<ATTEsewaReports>("GET_ALL_REPORTS", null, commandType: CommandType.StoredProcedure)?.ToList();
                    jsonResponse.ResponseData = aTTEsewaReports;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return jsonResponse;
        }

        public JsonResponse LoadEsewaReportByHolder(string CompCode, string DivCode, string CompEnName, string BatchNo, string Holder)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@CompCode", CompCode);
                    param.Add("@DivCode", DivCode);
                    param.Add("@BatchNo", BatchNo);
                    param.Add("@Holder", Holder);


                    List<ATTBatchProcessing> batchProcessings = (connection.Query<ATTBatchProcessing>("GET_INDIVIDUAL_REPORT", param, commandType: CommandType.StoredProcedure))?.ToList();

                    jsonResponse.ResponseData = batchProcessings;
                    jsonResponse.TotalRecords = batchProcessings.Count();
                    if (batchProcessings.Count() > 0) jsonResponse.IsSuccess = true;
                    else
                    {
                        jsonResponse.Message = "No Record Found";
                        jsonResponse.IsSuccess = false;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }


            return jsonResponse;
        }
    }
}
