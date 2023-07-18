
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
    public class ShOwnerSubType : IShOwnerSubType
    {
        IOptions<ReadConfig> connectionString;
        public ShOwnerSubType(IOptions<ReadConfig> _connectionString)
        {
            connectionString = _connectionString;
        }
        #region Get ALl ShownerSub type
        public JsonResponse GetAllShOwnerSubType(string shownertype)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_SHOWNERTYPE", (shownertype));

                    List<ATTShOwnerSubType> shOwnerSubTypesToReturn = connection.Query<ATTShOwnerSubType>(sql: "GET_ALL_SHOWNERSUBTYPE", param: param, null, commandType: CommandType.StoredProcedure).ToList();

                    if (shOwnerSubTypesToReturn.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = shOwnerSubTypesToReturn;
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

        #endregion

        #region selected showner type

        public JsonResponse GetShownerSubType(string shownertype, string shownersubtype)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_SHOWNERTYPE", (shownertype));
                    param.Add("@P_SHOWNERSUBTYPE", (shownersubtype));
                    List<ATTShOwnerSubType> shOwnerSubTypesToReturn = connection.Query<ATTShOwnerSubType>(sql: "GET_SHOWNERSUBTYPE", param: param, null, commandType: CommandType.StoredProcedure).ToList();

                    if (shOwnerSubTypesToReturn.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = shOwnerSubTypesToReturn;
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
        #endregion
    }
}
