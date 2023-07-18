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
    public class BrokerSetupRepo : IBrokerSetup
    {
        IOptions<ReadConfig> connectionString;
        public BrokerSetupRepo(IOptions<ReadConfig> _connectionString)
        {
            this.connectionString = _connectionString;
        }

        public JsonResponse GetBrokerCode()
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    string sqlQuery = "GETBROKERCODE";
                    var data = connection.Query<int>(sql: sqlQuery, param: null, null, commandType: null)?.FirstOrDefault();
                    jsonResponse.Message = jsonResponse.Message = ((1000 + data).ToString()).Substring(1, 3);
                    if (jsonResponse.Message != null)
                    {
                        jsonResponse.IsSuccess = true;
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.ResponseData = ex;
                    jsonResponse.HasError = true;

                }
            }
            return jsonResponse;
        }

        public JsonResponse GetBrokerDetail(string Bcode, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {

                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@bcode", Bcode.ToString().PadLeft(3, '0'));
                    param.Add("@P_USERNAME", UserName);
                    param.Add("@P_IP_ADDRESS", IPAddress);
                    param.Add("@P_ENTRY_DATE", DateTime.Now);
                    ATTBroker aTTBroker = connection.Query<ATTBroker>("GET_BROKER_DETAIL", param, null, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                    if (aTTBroker != null)
                    {
                        jsonResponse.ResponseData = aTTBroker;
                        jsonResponse.IsSuccess = true;
                    }
                    else
                    {
                        jsonResponse.IsSuccess = false;
                        jsonResponse.Message = ATTMessages.CANNOT_SAVE;
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.ResponseData = ex;
                    jsonResponse.HasError = true;

                }
            }
            return jsonResponse;
        }

        public JsonResponse SaveBrokerDetails(ATTBroker aTTBroker, string ActionType, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            SqlTransaction trans;
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (trans = connection.BeginTransaction())
                    {
                        try
                        {
                            DynamicParameters parameters = new DynamicParameters();
                            parameters.Add("@Bcode", aTTBroker.Bcode);
                            parameters.Add("@Name", aTTBroker.Name);
                            parameters.Add("@Address", aTTBroker.Address);
                            parameters.Add("@Npname", aTTBroker.Npname);
                            parameters.Add("@Npadd", aTTBroker.Npadd);
                            parameters.Add("@TelNo", aTTBroker.Telno);
                            parameters.Add("@Contactperson1", aTTBroker.Contactperson1);
                            parameters.Add("@ActionType", ActionType);
                            parameters.Add("@EntryDate", DateTime.Now);
                            parameters.Add("@UserName", UserName);
                            parameters.Add("@IPAddress", IPAddress);
                            jsonResponse = connection.Query<JsonResponse>("INSERT_UPDATE_BROKER", parameters, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                            if (jsonResponse.IsSuccess)
                            {
                                trans.Commit();
                            }
                        }
                        catch (Exception ex)
                        {

                            jsonResponse.Message = ex.Message;
                            trans.Rollback();

                        }
                    }

                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.ResponseData = ex;
                    jsonResponse.HasError = true;

                }

            }
            return jsonResponse;
        }
    }
}
