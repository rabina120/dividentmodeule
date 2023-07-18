using Dapper;
using Entity.Common;
using Entity.Dividend;
using Entity.Esewa;
using Interface.Reports;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Reports
{
    public class EsewaStatusReportsRepo : IEsewaStatusReport
    {
        IOptions<ReadConfig> _connectionString;

        public EsewaStatusReportsRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }

        public JsonResponse GenerateReportData(string CompCode, string DivCode, string Batch, string ReportType, string ReportSubType, string exportTo, string username, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    if (exportTo == "P")
                    {

                        DynamicParameters param = new DynamicParameters();
                        param.Add("@P_CompCode", CompCode);
                        param.Add("@p_divcode", DivCode);
                        param.Add("@p_batch", Batch);
                        param.Add("@p_reporttype", ReportType);
                        param.Add("@p_reportsubtype", ReportSubType);
                        param.Add("@p_username", username);
                        param.Add("@p_ipaddress", IPAddress);
                        param.Add("@p_entry_date", DateTime.Now);
                        List<dynamic> aTTData = connection.Query<dynamic>(sql: "ESEWASTATUSREPORT_GETREPORTDATA", param: param, null, commandType: CommandType.StoredProcedure).ToList();
                        if (aTTData.Count != 0)
                        {
                            response.ResponseData = aTTData;
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.Message = ATTMessages.NO_RECORDS_FOUND;
                        }
                    }
                    else
                    {
                        SqlCommand cmd = new SqlCommand("ESEWASTATUSREPORT_GETREPORTDATA", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter excelParam = new SqlParameter();
                        excelParam = cmd.Parameters.AddWithValue("@P_CompCode", CompCode);
                        excelParam = cmd.Parameters.AddWithValue("@P_DivCode", DivCode);
                        excelParam = cmd.Parameters.AddWithValue("@p_batch", Batch);
                        excelParam = cmd.Parameters.AddWithValue("@p_reporttype", ReportType);
                        excelParam = cmd.Parameters.AddWithValue("@p_reportsubtype", ReportSubType);
                        excelParam = cmd.Parameters.AddWithValue("@P_USERNAME", username);
                        excelParam = cmd.Parameters.AddWithValue("@P_IPADDRESS", IPAddress);
                        excelParam = cmd.Parameters.AddWithValue("@P_ENTRY_DATE", DateTime.Now);

                        excelParam.Direction = ParameterDirection.Input;
                        DataSet ds = new DataSet("Data");
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = cmd;

                        da.Fill(ds);

                        if (ds.Tables[0].Rows.Count == 0)
                        {
                            response.IsSuccess = false;
                            response.Message = ATTMessages.NO_RECORDS_FOUND;
                        }
                        else
                        {
                            response.IsSuccess = true;
                            response.ResponseData = ds;
                        }
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

        public JsonResponse GetAllBatch(string CompCode, string DivCode)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);
                    param.Add("@p_divcode", DivCode);
                    List<ATTBatchDetail> aTTDividendTables = connection.Query<ATTBatchDetail>(sql: "ESEWASTATUSREPORT_GETALLBATCH", param: param, null, commandType: CommandType.StoredProcedure).ToList();
                    if (aTTDividendTables.Count != 0)
                    {
                        response.ResponseData = aTTDividendTables;
                        response.IsSuccess = true;
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

        public JsonResponse GetAllDividends(string CompCode)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);
                    List<ATTDividend> aTTDividendTables = connection.Query<ATTDividend>(sql: "ESEWASTATUSREPORT_GETALLDIVIDENDS", param: param, null, commandType: CommandType.StoredProcedure).ToList();
                    if (aTTDividendTables.Count > 0)
                    {
                        response.ResponseData = aTTDividendTables;
                        response.IsSuccess = true;
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
