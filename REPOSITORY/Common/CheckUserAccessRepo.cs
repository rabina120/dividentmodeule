

using Dapper;
using Entity.Common;
using Interface.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Common
{
    public class CheckUserAccessRepo : ICheckUserAccess
    {
        IOptions<ReadConfig> _connectionString;

        public CheckUserAccessRepo(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }
        public bool CheckIfAccessible(string UserId, ControllerContext controllerContext)
        {
            bool response = false;
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                try
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_UserId", UserId);
                    string URL = "/" + controllerContext.RouteData.Values["area"].ToString() + "/" + controllerContext.RouteData.Values["controller"].ToString();
                    param.Add("@P_URL", URL.Trim());
                    response = connection.Query<bool>("CHECK_MENU_ACCESS", param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();

                }
                catch (Exception ex)
                {
                    response = false;
                }

            }
            return response;
        }
    }
}
