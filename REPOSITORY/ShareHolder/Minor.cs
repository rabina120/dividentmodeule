
using Dapper;
using Entity.Common;
using Entity.ShareHolder;
using Interface.ShareHolder;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.ShareHolder
{
    public class Minor : IMinor
    {
        IOptions<ReadConfig> connectionString;
        public Minor(IOptions<ReadConfig> _connectionString)
        {
            this.connectionString = _connectionString;
        }
        #region selected minor details

        public JsonResponse GetMinorDetails(string holderno)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("P_SHHOLDERNO", Convert.ToInt32(holderno));

                    List<ATTMinor> minorToReturn = connection.Query<ATTMinor>(sql: "GET_MINOR_DETAIL", param, null, commandType: CommandType.StoredProcedure).ToList();

                    if (minorToReturn.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = minorToReturn;
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;

                }
                return response;
            }
        }
        #endregion
    }
}
