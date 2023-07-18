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
    public class TransctionProcessingRepo : ITransctionProcessing
    {
        IOptions<ReadConfig> _connectionString;

        public TransctionProcessingRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }

        public ATTAccountDetails ProcessTransction(string CompCode, string BatchNo, string DivCode)
        {
            ATTAccountDetails aTTAccountDetails = new ATTAccountDetails();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@CompCode", CompCode);
                parameters.Add("@BatchNo", BatchNo);
                parameters.Add("@DivCode", DivCode);
                aTTAccountDetails.token = connection.Query<string>("GET_TOKEN_FROM_BATCHNO", parameters, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                aTTAccountDetails.transaction_details = connection.Query<ATTTransctionDetails>("GET_PROCESSING_DATA", parameters, commandType: CommandType.StoredProcedure)?.ToList();
            }

            return aTTAccountDetails;
        }

        public JsonResponse SaveOutputRemarks(ATTEsewaResponse EsewaData, string CompCode, string BatchNo, string Token)
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
                            if (EsewaData.transaction_details.Count > 0)
                            {
                                DataTable dataTable = new DataTable();
                                dataTable.Columns.Add("sub_token", typeof(int));
                                dataTable.Columns.Add("code", typeof(string));
                                dataTable.Columns.Add("message", typeof(string));
                                dataTable.Columns.Add("status", typeof(string));

                                EsewaData.transaction_details.ForEach(att => dataTable.Rows.Add(Convert.ToInt64(att.sub_token), att.code, att.message, att.status));
                                using (SqlCommand cmd = new SqlCommand("SAVE_PROCESSING_DATA_ESEWA"))
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
                                            //a = reader.Read();
                                            jsonResponse.Message = reader.GetString(0);
                                            jsonResponse.IsSuccess = Convert.ToBoolean(reader.GetString(1));

                                        }
                                    }

                                }
                            }
                            DynamicParameters param = new DynamicParameters();
                            param.Add("@Code", EsewaData.Code);
                            param.Add("@Message", EsewaData.Message);
                            param.Add("@Token", EsewaData.Token);
                            param.Add("@Details", EsewaData.Details);
                            param.Add("@Error", EsewaData.Error);
                            param.Add("@CreatedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                            jsonResponse = connection.Query<JsonResponse>("SAVE_ESEWA_API_Details", param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
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



        public JsonResponse SaveErrorRemarks(string BatchId, ATTEncryptedDetails output, ATTEsewaResponse esewaResponse)
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
    }
}
