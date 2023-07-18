
using Dapper;
using Entity.Common;
using Interface.Security;
using Microsoft.Extensions.Options;

using System;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Security
{
    public class AuditRepo : IAudit
    {
        IOptions<ReadConfig> connectionString;

        public AuditRepo(IOptions<ReadConfig> _connectionString)
        {
            connectionString = _connectionString;
        }

        public void saveLogAsync(string xAction, string rFile, string username, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {

                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_UserName", username);
                    parameters.Add("@P_Action", xAction);
                    parameters.Add("@P_File", rFile);
                    parameters.Add("@P_IP_Address", IPAddress);
                    parameters.Add("@P_Date_Now", DateTime.Now);
                    response = connection.Query<JsonResponse>(sql: "save_log", parameters, transaction: null, commandType: System.Data.CommandType.StoredProcedure)?.FirstOrDefault();

                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                }

            }

        }
        public void auditSave(string username, string xAction, string rFile)
        {
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                string time = DateTime.Now.ToString("h:mm:ss tt");
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                try
                {
                    connection.Open();
                    string sql = "INSERT into AUDITTRIAL(username,entrytime,reffile,remarks,actiondate) VALUES('" + username + "','" + time + "','" + rFile + "','" + xAction + "',cast('" + date + "' as smalldatetime) )";
                    connection.Query(sql: sql, null, commandType: null);

                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        }

        public JsonResponse auditSave(string username, string xAction, string rFile, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {

                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_UserName", username);
                    parameters.Add("@P_Action", xAction);
                    parameters.Add("@P_File", rFile);
                    parameters.Add("@P_IP_Address", IPAddress);
                    parameters.Add("@P_Date_Now", DateTime.Now);
                    response = connection.Query<JsonResponse>(sql: "AUDITTRIAL_INSERT", parameters, transaction: null, commandType: System.Data.CommandType.StoredProcedure)?.FirstOrDefault();

                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                }

            }
            return response;
        }

        public JsonResponse errorSave(string username, string rFile, string IPAddress, Exception exception)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {

                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_UserName", username);
                    parameters.Add("@P_Error", exception.Message);
                    parameters.Add("@P_ErrorCode", exception.Message.Substring(0, 3));
                    parameters.Add("@P_File", rFile);
                    parameters.Add("@P_IP_Address", IPAddress);
                    parameters.Add("@P_Date_Now", DateTime.Now);
                    response = connection.Query<JsonResponse>(sql: "ERRORTRAP_INSERT", parameters, transaction: null, commandType: System.Data.CommandType.StoredProcedure)?.FirstOrDefault();
                    response.Message = exception.Message;
                    response.IsSuccess = false;
                    response.HasError = true;

                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                    response.IsSuccess = false;
                    response.HasError = true;

                }

            }
            return response;
        }
    }
}
