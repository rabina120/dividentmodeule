using Dapper;
using Entity.Common;
using Interface.Esewa;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Repository.Esewa
{
    public class LogDetailRepo : ILogDetails
    {
        IOptions<ReadConfig> _connectionString;

        public LogDetailRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }

        public void InsertLogDetails(string BatchNo, string Message, string UserName)
        {
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction trans = connection.BeginTransaction())
                    {
                        try
                        {
                            DynamicParameters param = new DynamicParameters();
                            param.Add("@BatchNo", BatchNo);
                            param.Add("@Message", Message);
                            param.Add("@UserName", UserName);
                            param.Add("@CreatedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                            connection.Execute("INSERT_ESEWAERROR_LOG", param, trans, commandType: CommandType.StoredProcedure);
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();

                        }
                    }

                }
                catch (Exception ex)
                {

                }
            }

        }
    }
}
