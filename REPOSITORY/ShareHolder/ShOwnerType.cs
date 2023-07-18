
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
    public class ShOwnerType : IShOwnerType
    {
        IOptions<ReadConfig> connectionString;
        public ShOwnerType(IOptions<ReadConfig> _connectionString)
        {
            connectionString = _connectionString;
        }
        public JsonResponse GetAllShOwnerType()
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    List<ATTShOwnerType> shOwnerTypesToReturn = connection.Query<ATTShOwnerType>(sql: "GET_ALL_SHOWNERTYPE", param: null, null, commandType: CommandType.StoredProcedure).ToList();

                    if (shOwnerTypesToReturn.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = shOwnerTypesToReturn;
                    }
                    else
                    {
                        response.Message = ATTMessages.NO_RECORDS_FOUND;
                        response.IsSuccess = false;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.HasError = true;
                    response.ResponseData = ex;
                }
                return response;
            }
        }

        public JsonResponse GetShownerType(string shownertype)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {

                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_SHOWNERTYPE", (shownertype));

                    List<ATTShOwnerType> shOwnerTypesToReturn = connection.Query<ATTShOwnerType>(sql: "GET_SHOWNERTYPE", param: param, null, commandType: CommandType.StoredProcedure).ToList();

                    if (shOwnerTypesToReturn.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = shOwnerTypesToReturn;
                    }
                    else
                    {
                        response.Message = ATTMessages.NO_RECORDS_FOUND;
                        response.IsSuccess = false;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.HasError = true;
                    response.ResponseData = ex;
                }
                return response;
            }
        }
    }
}
