
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
    public class District : IDistrict
    {

        IOptions<ReadConfig> connectionString;
        public District(IOptions<ReadConfig> _connectionString)
        {
            this.connectionString = _connectionString;
        }
        #region All District List
        public JsonResponse GetAllDistrict()
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {

                try
                {
                    connection.Open();
                    List<ATTDistrict> districtsToReturn = connection.Query<ATTDistrict>(sql: "GET_All_DISTRICT", param: null, null, commandType: CommandType.StoredProcedure).ToList();

                    if (districtsToReturn != null)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = districtsToReturn;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = ATTMessages.NO_RECORDS_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
                return response;
            }
        }
        #endregion

        #region Selected District
        public JsonResponse GetDistrict(string distcode)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("P_DISTCODE", Convert.ToInt32(distcode));
                    List<ATTDistrict> districtsToReturn = connection.Query<ATTDistrict>(sql: "GET_DISTRICT", param: param, null, commandType: CommandType.StoredProcedure).ToList();

                    if (districtsToReturn.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = districtsToReturn;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = ATTMessages.NO_RECORDS_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
                return response;
            }
        }
        #endregion
    }
}
