using Dapper;
using Entity.Common;
using Interface.ShareHolder;
using Microsoft.Extensions.Options;
using Repository.Common;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.ShareHolder
{
    public class PaymentScheduleEntryRepo : IPaymentScheduleEntry
    {
        IOptions<ReadConfig> _connectionString;

        public PaymentScheduleEntryRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }

        public JsonResponse SavePaymentSchedule(string CompCode, string DivCode, string ShareType, string UserName)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    using (SqlTransaction trans = connection.BeginTransaction())
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@P_DivCode", DivCode);
                        param.Add("@P_COMPCODE", CompCode);
                        param.Add("@TableName1", ShareType);
                        string tableName = new TableReporsitory(_connectionString).GetTableName(param);

                        if (tableName != null)
                        {
                            param = new DynamicParameters();
                            param.Add("@CompCode", CompCode.Trim());
                            bool isComplete = (bool)(connection.Query<bool>("IsPaymentComplete", param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault());

                            if (isComplete)
                            {
                                param.Add("@DivCode", DivCode.Trim());
                                param.Add("@ShareType", ShareType);

                                int? batchno = (connection.Query<int>("GET_BATCH_ID", param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault());

                                batchno = batchno == null ? 1 : batchno + 1;


                                param.Add("@UserName", UserName.Trim());
                                param.Add("@TableName", tableName);
                                param.Add("@CreateData", DateTime.Now.ToString("yyyy-MM-dd"));
                                param.Add("@BatchId", batchno);
                                param.Add("@Token", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
                                connection.Query("SAVE_DIVIDEND_BATCH", param, trans, commandType: CommandType.StoredProcedure);
                                string token = param.Get<string>("@Token");


                                param.Add("@Token", token);

                                if (ShareType == "1")
                                {
                                    connection.Execute("DELETE_DATA_FROM_TEMP", null, trans, commandType: CommandType.StoredProcedure);
                                    param.Add("@ShareType", "1");
                                    DynamicParameters parameters = new DynamicParameters();
                                    parameters.Add("@P_COMPCODE", CompCode);
                                    //connection.Query(sql: "SAVE_TEMP_DATA", parameters, trans, commandType: CommandType.StoredProcedure);


                                }
                                else if (ShareType == "2")
                                {
                                    param.Add("@ShareType", "2");
                                }

                                int? affectedRow = connection.Query<int?>("SAVE_BATCH_PAYMENT", param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                                if (affectedRow > 0)
                                {
                                    trans.Commit();
                                    jsonResponse.IsSuccess = true;
                                    jsonResponse.Message = "Payment Batch Created";
                                }
                                else
                                {
                                    trans.Rollback();
                                    jsonResponse.IsSuccess = false;
                                    jsonResponse.Message = "No Record For New Batch";
                                }

                            }
                            else
                            {
                                jsonResponse.IsSuccess = false;
                                jsonResponse.Message = "Previous Transaction is Not Complete";
                            }
                        }
                        else
                        {
                            jsonResponse.IsSuccess = false;
                            jsonResponse.Message = "Table Not Found";
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
