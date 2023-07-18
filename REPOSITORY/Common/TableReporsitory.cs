

using Dapper;
using Entity.Common;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Common
{
    public class TableReporsitory
    {
        IOptions<ReadConfig> _connectionString;

        public TableReporsitory(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }

        public string GetTableName(DynamicParameters parameters)
        {
            string tableName = null;
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {

                tableName = connection.Query<string>("GET_TABLE_NAME", param: parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();

            }
            return tableName;
        }
    }
}
