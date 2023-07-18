using Dapper;
using Entity.Common;
using Interface.Reports;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Reports
{
    public class DailyReport : IDailyReport
    {
        IOptions<ReadConfig> _connectionString;

        public DailyReport(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }
        public JsonResponse GenerateReport(string UserName, string CompCode, string DailyReportDate, string IPAddress)
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
                        param.Add("UserName", UserName);
                        param.Add("DailyReportDate", DailyReportDate);
                        param.Add("IPAddress", IPAddress);
                        param.Add("EntryDate", DateTime.Now);
                        List<dynamic> aTTAuditTrials = connection.Query<dynamic>("Generate_Daily_Report", param, transaction, commandType: CommandType.StoredProcedure).ToList();
                        if (aTTAuditTrials.Count > 0)
                        {
                            jsonResponse.IsSuccess = true;
                            jsonResponse.ResponseData = aTTAuditTrials;
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
