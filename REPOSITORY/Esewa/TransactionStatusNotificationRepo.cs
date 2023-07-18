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
    public class TransactionStatusNotificationRepo : ITransactionStatusNotification
    {
        IOptions<ReadConfig> _connectionString;

        public TransactionStatusNotificationRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }



        public void SaveData(string data)
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
                            param.Add("@data", data);

                            jsonResponse = connection.Query<JsonResponse>("SaveNotification", param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();

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
        }

        public void SaveErrorRemarks(ATTEncryptedDetails output, ATTEsewaResponse aTTEsewaResponse)
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
                                param.Add("@Token", null);
                                param.Add("@Details", output.details);
                                param.Add("@Error", output.error);
                                param.Add("@CreatedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                                jsonResponse = connection.Query<JsonResponse>("SAVE_ESEWA_API_ERROR", param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                            }
                            else
                            {
                                param.Add("@Code", aTTEsewaResponse.Code);
                                param.Add("@Message", aTTEsewaResponse.Message);
                                param.Add("@Token", aTTEsewaResponse.Token);
                                param.Add("@Details", aTTEsewaResponse.Details);
                                param.Add("@Error", aTTEsewaResponse.Error);
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

        }

        public JsonResponse SaveTransactionNotification(ATTEsewaResponse aTTEsewaResponse)
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

                                using (SqlCommand cmd = new SqlCommand("INSERT_NOTIFICATION_DATA"))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Connection = connection;
                                    cmd.Transaction = trans;
                                    cmd.Parameters.AddWithValue("@Subtoken", dataTable);
                                    cmd.Parameters.AddWithValue("@Token", aTTEsewaResponse.Token);

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
