
using Dapper;
using Entity.Common;
using Entity.Parameter;
using Interface.Parameter;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Parameter
{
    public class PaymentTypeRepo : IPaymentType
    {
        IOptions<ReadConfig> connectionstring;

        public PaymentTypeRepo(IOptions<ReadConfig> _connectionstring)
        {
            this.connectionstring = _connectionstring;
        }
        public JsonResponse GetPaymentCode()
        {
            JsonResponse jsonResponse = new JsonResponse();

            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionstring.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    string sqlQuery = "GetPaymentCode";
                    var data = connection.Query<int>(sql: sqlQuery, param: null, null, commandType: null)?.FirstOrDefault();
                    jsonResponse.Message = ((1000 + data).ToString()).Substring(1, 3);
                    if (jsonResponse.Message != null)
                    {
                        jsonResponse.IsSuccess = true;
                    }

                }
                catch (Exception ex)
                {
                    jsonResponse.Message = ex.Message;
                }
            }

            return jsonResponse;
        }

        public JsonResponse GetPaymentDetails(string CenterId, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();

            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionstring.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@CenterId", CenterId.ToString().PadLeft(3, '0'));
                    param.Add("@UserName", UserName);
                    param.Add("@IPAddress", IPAddress);
                    param.Add("@ENTRYDATE", DateTime.Now);
                    ATTPamentType aTTPamentType = connection.Query<ATTPamentType>("GET_PAYMENT_DETAILS", param, commandType: CommandType.StoredProcedure)?.FirstOrDefault();

                    if (aTTPamentType != null)
                    {
                        jsonResponse.ResponseData = aTTPamentType;
                        jsonResponse.IsSuccess = true;
                    }
                    else
                    {
                        jsonResponse.IsSuccess = false;
                        jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.Message = ex.Message;
                    jsonResponse.HasError = true;
                    jsonResponse.ResponseData = ex;
                }

            }

            return jsonResponse;
        }

        public JsonResponse SavePaymentDetails(ATTPamentType aTTPamentType, string ActionType, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            SqlTransaction trans;
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionstring.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (trans = connection.BeginTransaction())
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@CenterId", aTTPamentType.CenterId);
                        parameters.Add("@CenterName", aTTPamentType.CenterName);
                        parameters.Add("@Address", aTTPamentType.Address);
                        parameters.Add("@NepaliName", aTTPamentType.NepaliName);
                        parameters.Add("@NepaliAddress", aTTPamentType.NepaliAddress);
                        parameters.Add("@TelNo", aTTPamentType.TelNo);
                        parameters.Add("@ActionType", ActionType);
                        parameters.Add("@ENTRYDATE", DateTime.Now);
                        parameters.Add("@UserName", UserName);
                        parameters.Add("@IPAddress", IPAddress);
                        jsonResponse = connection.Query<JsonResponse>("INSERT_UPDATE_PAYMENT", parameters, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                        if (jsonResponse.IsSuccess)
                        {
                            trans.Commit();
                        }
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
                    jsonResponse.Message = ex.Message;
                    jsonResponse.HasError = true;
                    jsonResponse.ResponseData = ex;
                }

            }
            return jsonResponse;
        }
    }
}
