
using Entity.Common;
using Interface.Security;
using Microsoft.Extensions.Options;

using System;
using System.Data.SqlClient;

namespace Repository.Security
{
    public class CheckDatabaseConnectionRepo : ICheckDatabaseConnection
    {
        IOptions<ReadConfig> connectionString;

        public CheckDatabaseConnectionRepo(IOptions<ReadConfig> _connectionString)
        {
            connectionString = _connectionString;
        }
        public JsonResponse CheckDatabaseConnection(string ConnectionString)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    response.IsSuccess = true;
                    response.Message = ATTMessages.DATABASE.CONNECTION_SUCCESS;
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
