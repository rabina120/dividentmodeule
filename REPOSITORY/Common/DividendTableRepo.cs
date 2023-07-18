


using Dapper;
using Entity.Common;
using Interface.Common;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Common
{
    public class DividendTableRepo : IDividendTable
    {
        private readonly IOptions<ReadConfig> _connectionString;
        public DividendTableRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }
        public JsonResponse GetAllDividendTableLists(string CompCode)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);

                    List<ATTDividendTable> dividendTablesToReturn = connection.Query<ATTDividendTable>(sql: "GET_ALL_DIVIDEND_TABLE_LIST", param: param, null, commandType: CommandType.StoredProcedure).ToList();

                    if (dividendTablesToReturn.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = dividendTablesToReturn;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = ATTMessages.NO_TABLES_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    response.ResponseData = ex;
                    response.IsSuccess = false;
                    response.HasError = true;

                }
                return response;
            }
        }
    }
}
