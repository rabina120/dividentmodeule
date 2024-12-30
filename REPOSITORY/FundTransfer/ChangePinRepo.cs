using Dapper;
using Entity.Common;
using Interface.Security;
using INTERFACE.FundTransfer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using System.Data;
using System;
using System.Linq;

namespace REPOSITORY.FundTransfer
{
    public class ChangePinRepo : IChangePin
    {
        private readonly IOptions<ReadConfig> _connectionString;
        private readonly IConfiguration _configuration;
        private readonly IAudit _audit;

        public ChangePinRepo(IOptions<ReadConfig> connectionString, IConfiguration configuration, IAudit audit)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _audit = audit ?? throw new ArgumentNullException(nameof(audit));
        }

        public JsonResponse ChangePin(string OldPin, string NewPin, string NewCurrentPin, string UserName, string IPAddress)
        {
            if (string.IsNullOrEmpty(OldPin) || string.IsNullOrEmpty(NewPin) || string.IsNullOrEmpty(NewCurrentPin))
            {
                return new JsonResponse
                {
                    IsSuccess = false,
                    HasError = true,
                };
            }

            JsonResponse response = new JsonResponse();

            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@OldPin", OldPin);
                        param.Add("@NewPin", NewPin);
                        param.Add("@NewCurrentPin", NewCurrentPin);
                        param.Add("@P_UserName", UserName);
                        param.Add("@P_IPAddress", IPAddress);
                        response = connection.Query<JsonResponse>("FT_UPDATEPIN", param, transaction, commandType: CommandType.StoredProcedure).FirstOrDefault();
                        if (response == null)
                        {
                            response = new JsonResponse
                            {
                                IsSuccess = false,
                                HasError = true,
                            };
                            transaction.Rollback();
                        }
                        else if (response.IsSuccess)
                        {
                            transaction.Commit();
                        }
                        else
                        {
                            transaction.Rollback();
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.HasError = true;
                    response.ResponseData = ex.Message;
                }
            }
            return response;
        }
    }
}
