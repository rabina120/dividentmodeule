using Dapper;
using Entity.Common;
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
    public class ConsolidateReportRepo : IConsolidateReport
    {
        IOptions<ReadConfig> _connectionString;
        public ConsolidateReportRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }

        public JsonResponse GenerateReport(ATTConsolidateReport ReportData, string ExportReportType, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", ReportData.CompCode);
                    param.Add("@P_ConsolidateFrom ", ReportData.ConsolidateDate);
                    param.Add("P_ConsolidateTo", ReportData.ConsolidateTo);
                    param.Add("@P_HolderNoFrom ", ReportData.HolderNoFrom);
                    param.Add("@P_HolderNoTo", ReportData.HolderNoTo);
                    param.Add("@P_SelectedAction", ReportData.DataType);
                    param.Add("@P_Username", UserName);
                    param.Add("@P_IP_ADDRESS", IP);
                    param.Add("@P_DATE_NOW", DateTime.Now);

                    if (ReportData.DataType == "P")
                    {
                        List<ATTConsolidateReport> certificate = connection.Query<ATTConsolidateReport>("GET_CONSOLIDATE_REPORT", param, commandType: CommandType.StoredProcedure)?.ToList();
                        if (certificate.Count > 0)
                        {
                            jsonResponse.ResponseData = certificate;
                            jsonResponse.Message = "Record Found";
                            jsonResponse.IsSuccess = true;
                        }
                        else
                        {
                            jsonResponse.Message = "Cannot";

                        }


                    }
                    else if (ReportData.DataType == "U")
                    {
                        List<ATTConsolidateReport> certificate = connection.Query<ATTConsolidateReport>("GET_CONSOLIDATE_REPORT", param, commandType: CommandType.StoredProcedure)?.ToList();
                        if (certificate.Count > 0)
                        {
                            jsonResponse.ResponseData = certificate;
                            jsonResponse.Message = "Record Found";
                            jsonResponse.IsSuccess = true;
                        }
                        else
                        {
                            jsonResponse.Message = "Cannot";

                        }
                    }
                    else
                    {
                        List<ATTConsolidateReport> certificate = connection.Query<ATTConsolidateReport>("GET_CONSOLIDATE_REPORT", param, commandType: CommandType.StoredProcedure)?.ToList();
                        if (certificate.Count > 0)
                        {
                            jsonResponse.ResponseData = certificate;
                            jsonResponse.Message = "Record Found";
                            jsonResponse.IsSuccess = true;
                        }
                        else
                        {
                            jsonResponse.Message = "Cannot";

                        }
                    }
                }

                catch (Exception ex)
                {
                    jsonResponse.Message = ex.Message;
                }
                return jsonResponse;
            }
        }



    }



}
