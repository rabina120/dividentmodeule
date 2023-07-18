
using Dapper;
using Entity.Common;
using Entity.ShareHolder;
using Interface.Signature;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Signature
{
    public class Signature : ISignature
    {
        IOptions<ReadConfig> connectionString;
        public Signature(IOptions<ReadConfig> _connectionString)
        {
            connectionString = _connectionString;
        }

        public JsonResponse GetSignature(string compcode, string holderno)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {

                try
                {
                    connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_COMPCODE", (compcode));
                    param.Add("@P_HOLDERNO", (holderno));

                    ATTSignature signaturesToReturn = connection.Query<ATTSignature>(sql: "GET_SIGNATURE", param: param, null, commandType: CommandType.StoredProcedure)?.FirstOrDefault();

                    if (signaturesToReturn != null)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = signaturesToReturn;
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
