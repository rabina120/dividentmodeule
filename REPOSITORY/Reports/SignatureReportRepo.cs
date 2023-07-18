
using Dapper;
using Entity.Common;
using Interface.Reports;

using Microsoft.Extensions.Options;

using System;
using System.Data.SqlClient;

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
                    DynamicParameters parameters = new DynamicParameters();



                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                }
            }
            return response;
        }
    }
}
