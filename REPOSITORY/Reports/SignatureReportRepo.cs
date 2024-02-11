
using Dapper;
using Entity.Common;
using Interface.Reports;

using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace Repository.Reports
{
    public class SignatureReportRepo : ISignatureReport
    {
        IOptions<ReadConfig> _connectionString;

        public SignatureReportRepo(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }

        public JsonResponse GenerateReport(string CompCode, string UserName, string SelectedAction, string IPAddress, string DateFrom = null, string DateTo = null, string HolderFrom = null, string HolderTo = null)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@COMPCODE", CompCode);
                        parameters.Add("@USERNAME", UserName);
                        parameters.Add("@SELECTEDACTION", SelectedAction);
                        parameters.Add("@IPADDRESS", IPAddress);
                        parameters.Add("@DATEFROM", DateFrom);
                        parameters.Add("@DATETO", DateTo);
                        parameters.Add("@HOLDERFROM", HolderFrom);
                        parameters.Add("@HOLDERTO", HolderTo);
                        List<dynamic> reportData = connection.Query<dynamic>("SIGNATURE_REPORT", parameters, transaction, commandType: CommandType.StoredProcedure).ToList();
                        if (reportData.Count > 0)
                        {
                            response.IsSuccess = true;
                            response.ResponseData = reportData;
                            transaction.Commit();
                        }
                        else
                        {
                            response.Message = ATTMessages.NO_RECORDS_FOUND;
                            transaction.Rollback();
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Message = ex.Message;
                }
            }
            return response;
        }
    }
}
