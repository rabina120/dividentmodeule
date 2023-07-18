
using Dapper;
using Entity.Common;
using Interface.Security;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Security
{
    public class RoleDefineRepo : IRoleDefine
    {
        IOptions<ReadConfig> _connectionString;

        public RoleDefineRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }

        public JsonResponse SaveRole(string RoleName, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction trans = connection.BeginTransaction())
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("RoleName", RoleName.Trim());
                        param.Add("UserName", UserName);
                        param.Add("EntryDate", DateTime.Now);
                        param.Add("IPAddress", IPAddress);
                        jsonResponse = connection.Query<JsonResponse>("SAVE_ROLE", param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                        if (jsonResponse.IsSuccess)
                            trans.Commit();
                        else
                        {
                            trans.Rollback();
                            jsonResponse.Message = ATTMessages.CANNOT_SAVE;
                        }
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.ResponseData = ex;
                    jsonResponse.HasError = true;
                }
            }
            return jsonResponse;
        }
    }
}
