using Dapper;
using Entity.Common;
using Entity.Esewa;
using Interface.Esewa;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Esewa
{
    public class TransactionStatusQueryRepo : ITransactionStatusQuery
    {
        IOptions<ReadConfig> _connectionString;

        public TransactionStatusQueryRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }

        public ATTAccountDetails GetBatchTokenNo(string CompCode, string BatchNo, string DivCode)
        {
            string TokenNo = string.Empty;

            ATTAccountDetails aTTAccountDetails = new ATTAccountDetails();
            //aTTAccountDetails.token = "sdald28";

            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                DynamicParameters param = new DynamicParameters();
                param.Add("@CompCode", CompCode);
                param.Add("@BatchNo", BatchNo);
                param.Add("@DivCode", DivCode);
                aTTAccountDetails.token = connection.Query<string>("GET_TOKEN_FROM_BATCHNO", param, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                return aTTAccountDetails;
            }

        }

        public JsonResponse SaveErrorRemarks(ATTEncryptedDetails output, ATTEsewaResponse esewaResponse, string BatchId)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    using (SqlTransaction trans = connection.BeginTransaction())
                    {
                        try
                        {
                            DynamicParameters param = new DynamicParameters();
                            if (output != null)
                            {
                                param.Add("@Code", output.code);
                                param.Add("@Message", output.message);
                                param.Add("@Token", BatchId);
                                param.Add("@Details", output.details);
                                param.Add("@Error", output.error);
                                param.Add("@CreatedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                                jsonResponse = connection.Query<JsonResponse>("SAVE_ESEWA_API_ERROR", param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                            }
                            else
                            {
                                param.Add("@Code", esewaResponse.Code);
                                param.Add("@Message", esewaResponse.Message);
                                param.Add("@Token", BatchId);
                                param.Add("@Details", esewaResponse.Details);
                                param.Add("@Error", esewaResponse.Error);
                                param.Add("@CreatedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                                jsonResponse = connection.Query<JsonResponse>("SAVE_ESEWA_API_ERROR", param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();


                            }
                            trans.Commit();

                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            jsonResponse.Message = ex.Message;
                            jsonResponse.IsSuccess = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.Message = ex.Message;
                    jsonResponse.IsSuccess = false;

                }
            }
            return jsonResponse;
        }


        public string GetTableForEsewa(string BatchId, string Type, SqlConnection connection)
        {
            string TableName = string.Empty;

            DynamicParameters param = new DynamicParameters();
            param.Add("@BatchId", BatchId);
            param.Add("@Type", Type);
            TableName = connection.Query<string>("GET_ESEWA_TABLE_NAME", param, commandType: CommandType.StoredProcedure)?.FirstOrDefault();

            return TableName;
        }

        public JsonResponse SaveSubTokenOutputRemarks(ATTEsewaResponse aTTEsewaResponse, string CompCode, string BatchNo, string Token)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    using (SqlTransaction trans = connection.BeginTransaction())
                    {
                        try
                        {
                            if (aTTEsewaResponse.transaction_details.Count > 0)
                            {
                                DataTable dataTable = new DataTable();
                                dataTable.Columns.Add("sub_token", typeof(int));
                                dataTable.Columns.Add("code", typeof(string));
                                dataTable.Columns.Add("message", typeof(string));
                                dataTable.Columns.Add("status", typeof(string));

                                aTTEsewaResponse.transaction_details.ForEach(att => dataTable.Rows.Add(Convert.ToInt64(att.sub_token), att.code, att.message, att.status));

                                using (SqlCommand cmd = new SqlCommand("SAVE_QUERY_DATA_ESEWA"))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Connection = connection;
                                    cmd.Transaction = trans;
                                    cmd.Parameters.AddWithValue("@Subtoken", dataTable);
                                    cmd.Parameters.AddWithValue("@CompCode", CompCode);
                                    cmd.Parameters.AddWithValue("@BatchNo", BatchNo);
                                    cmd.Parameters.AddWithValue("@Token", Token);
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {

                                        if (reader.Read())
                                        {
                                            jsonResponse.Message = reader.GetString(0);
                                            jsonResponse.IsSuccess = Convert.ToBoolean(reader.GetString(1));

                                        }
                                    }

                                }
                            }
                            DynamicParameters param = new DynamicParameters();
                            param.Add("@Code", aTTEsewaResponse.Code);
                            param.Add("@Message", aTTEsewaResponse.Message);
                            param.Add("@Token", aTTEsewaResponse.Token);
                            param.Add("@Details", aTTEsewaResponse.Details);
                            param.Add("@Error", aTTEsewaResponse.Error);
                            param.Add("@CreatedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                            jsonResponse = connection.Query<JsonResponse>("SAVE_ESEWA_API_Details", param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();


                            if (!aTTEsewaResponse.Error)
                            {
                                param = new DynamicParameters();
                                param.Add("@Token", aTTEsewaResponse.Token);
                                connection.Query("SET_TRANSACTION_COMPLETED", param, trans, commandType: CommandType.StoredProcedure);

                            }

                            trans.Commit();


                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            jsonResponse.Message = ex.Message;
                            jsonResponse.IsSuccess = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.Message = ex.Message;
                }
            }
            return jsonResponse;
        }

    }
}
