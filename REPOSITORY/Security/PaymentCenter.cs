
using Dapper;
using Entity.Common;
using Interface.Security;
using Microsoft.Extensions.Options;


using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Security
{
    public class PaymentCenter : IPaymentCenter
    {
        IOptions<ReadConfig> connectionString;
        public PaymentCenter(IOptions<ReadConfig> _connectionString)
        {
            connectionString = _connectionString;
        }
        public JsonResponse GetPaymentCenter()
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    //DynamicParameters param = new DynamicParameters();
                    List<ATTPaymentCenter> aTTPaymentCenter = connection.Query<ATTPaymentCenter>(sql: "SELECT * FROM PAYMENT_CENTER ORDER BY CENTERID", param: null, commandType: null).ToList();

                    if (aTTPaymentCenter.Count > 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = aTTPaymentCenter;
                    }
                    else
                    {
                        jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                        jsonResponse.IsSuccess = false;
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.ResponseData = ex;
                    jsonResponse.HasError = true;

                }
                return jsonResponse;
            }
        }

        public JsonResponse GetPaymentCenterHolder(string compcode, string shholderno)
        {
            throw new NotImplementedException();
        }
    }
}
