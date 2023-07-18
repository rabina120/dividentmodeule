using Dapper;
using Entity.Common;
using Entity.Esewa;
using Interface.Esewa;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Esewa
{
    public class AccountValidationRepo : IAccountValidation
    {
        IOptions<ReadConfig> _connectionString;

        public AccountValidationRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }

        public ATTAccountDetails ProcessAccountValidation(string CompCode, string DivCode, string BatchNo)
        {
            ATTAccountDetails aTTAccountDetails = new ATTAccountDetails();
            try
            {
                using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@CompCode", CompCode);
                    parameters.Add("@BatchNo", BatchNo);
                    parameters.Add("@DivCode", DivCode);
                    connection.Open();
                    aTTAccountDetails.token = connection.Query<string>("GET_TOKEN_FROM_BATCHNO", parameters, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                    aTTAccountDetails.transaction_details = connection.Query<ATTTransctionDetails>("GET_ACCOUNT_TO_VALID", parameters, commandType: CommandType.StoredProcedure)?.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return aTTAccountDetails;
        }

        public void SaveBankDetails(ATTBanks aTTBanks)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
                {
                    connection.Open();
                    using (SqlTransaction trans = connection.BeginTransaction())
                    {

                        try
                        {
                            if (aTTBanks.banks.Count > 0)
                            {
                                DataTable dataTableSuccess = new DataTable();
                                dataTableSuccess.Columns.Add("Bank_Code", typeof(string));
                                dataTableSuccess.Columns.Add("Bank_Name", typeof(string));
                                dataTableSuccess.Columns.Add("Bank_Regex", typeof(string));
                                dataTableSuccess.Columns.Add("Account_Validation", typeof(bool));


                                aTTBanks.banks.ForEach(att =>
                                {
                                    dataTableSuccess.Rows.Add(att.Bank_Code, att.Bank_Name,
                                      att.Bank_Regex, att.Account_Validation);

                                });
                                using (SqlCommand cmd = new SqlCommand("SAVE_API_BANK_DETAILS"))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Connection = connection;
                                    cmd.Transaction = trans;

                                    cmd.Parameters.AddWithValue("@APIBANKDETAILS", dataTableSuccess);
                                    //cmd.Parameters.AddWithValue("@DataTableError", dataTableError);

                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {

                                        if (reader.Read())
                                        {

                                        }
                                    }

                                }
                            }


                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();

                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public JsonResponse SaveErrorRemarks(ATTEncryptedDetails output = null, ATTAccountValidationResponse response = null)
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
                                param.Add("@Token", "");
                                param.Add("@Details", output.details);
                                param.Add("@Error", output.error);
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

        public JsonResponse SaveOutputRemarks(List<ATTAccountValidationResponse> response, string CompCode, string BatchNo, string Token)
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
                            if (response.Count > 0)
                            {
                                DataTable dataTableSuccess = new DataTable();
                                dataTableSuccess.Columns.Add("payee_account_number", typeof(string));
                                dataTableSuccess.Columns.Add("payee_account_name", typeof(string));
                                dataTableSuccess.Columns.Add("payee_bank_code", typeof(string));
                                dataTableSuccess.Columns.Add("code", typeof(string));
                                dataTableSuccess.Columns.Add("message", typeof(string));
                                dataTableSuccess.Columns.Add("error", typeof(bool));
                                dataTableSuccess.Columns.Add("percentage", typeof(double));
                                dataTableSuccess.Columns.Add("sub_token", typeof(int));

                                response.ForEach(att =>
                                {
                                    dataTableSuccess.Rows.Add(att.payee_account_number, att.payee_account_name,
                                      att.payee_bank_code, att.code,
                                      att.message, !att.error, att.percentage, att.sub_token);

                                });
                                using (SqlCommand cmd = new SqlCommand("SAVE_VALID_ACCOUNT"))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Connection = connection;
                                    cmd.Transaction = trans;

                                    cmd.Parameters.AddWithValue("@DataTableSuccess", dataTableSuccess);
                                    cmd.Parameters.AddWithValue("@CompCode", CompCode);
                                    cmd.Parameters.AddWithValue("@BatchNo", BatchNo);
                                    cmd.Parameters.AddWithValue("@Token", Token);
                                    //cmd.Parameters.AddWithValue("@DataTableError", dataTableError);

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


                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            jsonResponse.Message = ex.Message;
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
