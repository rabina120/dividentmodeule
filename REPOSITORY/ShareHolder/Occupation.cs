
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
    public class Occupation : IOccupation
    {
        IOptions<ReadConfig> connectionString;
        public Occupation(IOptions<ReadConfig> _connectionString)
        {
            connectionString = _connectionString;
        }

        #region All Occupation List
        public JsonResponse GetAllOccupation(string shownertype)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_SHOWNERTYPE", (shownertype));

                    List<ATTOccupation> occupationsToReturn = connection.Query<ATTOccupation>(sql: "GET_ALl_OCCUPATION", param: param, null, commandType: CommandType.StoredProcedure).ToList();

                    if (occupationsToReturn.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = occupationsToReturn;
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

        #region SelectedOccupation
        public JsonResponse GetOccupation(string occupationId)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_OCCUPATIONID", Convert.ToInt32(occupationId));

                    List<ATTOccupation> occupationsToReturn = connection.Query<ATTOccupation>(sql: "GET_OCCUPATION", param: param, null, commandType: CommandType.StoredProcedure).ToList();

                    if (occupationsToReturn.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = occupationsToReturn;
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
