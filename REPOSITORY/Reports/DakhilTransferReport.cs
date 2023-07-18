using Dapper;
using Entity.Common;
using INTERFACE.Reports;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace REPOSITORY.Reports
{
    public class DakhilTransferReport : IDakhilTransferReport
    {
        IOptions<ReadConfig> _connectionString;
        public DakhilTransferReport(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }

        public JsonResponse GetReportDataForExcel(string Compcode, string SelectedAction, string ReportType, string FromDate, string ToDate, string RegnoFrom, string RegnoTo, string TranKittaFrom, string TranKittaTo, string BHolderNoFrom, string BHolderNoTo, string SHolderNoFrom, string SHolderNoTo, string Broker, string username, string Ipaddress)
        {
            JsonResponse response = new JsonResponse();
            try
            {
                using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
                {
                    SqlCommand cmd = new SqlCommand("GET_DAKHIL_REPORT", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter excelParam = new SqlParameter();
                    excelParam = cmd.Parameters.AddWithValue("@COMPCODE", Compcode);
                    excelParam = cmd.Parameters.AddWithValue("@SELECTEDACTION", SelectedAction);
                    excelParam = cmd.Parameters.AddWithValue("@REPORTTYPE", ReportType);
                    excelParam = cmd.Parameters.AddWithValue("@SHAREKITTAFROM", TranKittaFrom);
                    excelParam = cmd.Parameters.AddWithValue("@SHAREKITTATO", TranKittaTo);
                    excelParam = cmd.Parameters.AddWithValue("@FROMDATE", FromDate);
                    excelParam = cmd.Parameters.AddWithValue("@TODATE", ToDate);
                    excelParam = cmd.Parameters.AddWithValue("@REGNOFROM", RegnoFrom);
                    excelParam = cmd.Parameters.AddWithValue("@REGNOTO", RegnoTo);
                    excelParam = cmd.Parameters.AddWithValue("@BHOLDERNOFROM", BHolderNoFrom);
                    excelParam = cmd.Parameters.AddWithValue("@BHOLDERNOTO", BHolderNoTo);
                    excelParam = cmd.Parameters.AddWithValue("@SHOLDERNOFROM", SHolderNoFrom);
                    excelParam = cmd.Parameters.AddWithValue("@SHOLDERNOTO", SHolderNoTo);
                    excelParam = cmd.Parameters.AddWithValue("@BROKER", Broker);
                    excelParam = cmd.Parameters.AddWithValue("@USERNAME", username);
                    excelParam = cmd.Parameters.AddWithValue("@IPADDRESS", Ipaddress);
                    excelParam = cmd.Parameters.AddWithValue("@DATENOW", DateTime.Now);


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
                response.HasError=true;
                response.Message = ex.Message;
                response.ResponseData = ex;
            }
            return response;
        }

        public JsonResponse GetReportDataForPDF(string Compcode, string SelectedAction, string ReportType, string FromDate, string ToDate, string RegnoFrom, string RegnoTo, string TranKittaFrom, string TranKittaTo, string BHolderNoFrom, string BHolderNoTo, string SHolderNoFrom, string SHolderNoTo, string Broker, string username, string Ipaddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@COMPCODE", Compcode);
                        param.Add("@SELECTEDACTION", SelectedAction);
                        param.Add("@REPORTTYPE", ReportType);
                        param.Add("@SHAREKITTAFROM",TranKittaFrom  );
                        param.Add("@SHAREKITTATO", TranKittaTo );
                        param.Add("@FROMDATE",FromDate  );
                        param.Add("@TODATE", ToDate );
                        param.Add("@REGNOFROM",  RegnoFrom);
                        param.Add("@REGNOTO", RegnoTo );
                        param.Add("@BHOLDERNOFROM", BHolderNoFrom );
                        param.Add("@BHOLDERNOTO",BHolderNoTo  );
                        param.Add("@SHOLDERNOFROM", SHolderNoFrom);
                        param.Add("@SHOLDERNOTO", SHolderNoTo);
                        param.Add("@BROKER", Broker );
                        param.Add("@USERNAME", username );
                        param.Add("@IPADDRESS",Ipaddress  );
                        param.Add("@DATENOW", DateTime.Now);
                        List<dynamic> reportData = connection.Query<dynamic>("GET_DAKHIL_REPORT", param, transaction, commandType: CommandType.StoredProcedure).ToList();
                        if (reportData.Count > 0)
                        {
                            jsonResponse.IsSuccess = true;
                            jsonResponse.ResponseData = reportData;
                            transaction.Commit();
                        }
                        else
                        {
                            jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                            transaction.Rollback();
                        }

                    }

                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.Message = ex.Message;
                }

            }
            return jsonResponse;
        }
    }
}
