

using Dapper;
using Entity.Certificate;
using Entity.Common;
using Interface.Certificate;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Certificate
{
    public class ClearPSLEntryRepo : IClearPSLEntry
    {
        IOptions<ReadConfig> connectionString;
        public ClearPSLEntryRepo(IOptions<ReadConfig> _connectionString)
        {
            this.connectionString = _connectionString;
        }

        public JsonResponse GetPSLInformation(string CompCode, string ShholderNo, string SelectedAction, string PSLNo, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_COMPCODE", CompCode);
                    param.Add("@P_SHHOLDERNO", ShholderNo);
                    param.Add("@P_SELECTEDACTION", SelectedAction);
                    param.Add("@P_pslno", PSLNo);
                    param.Add("@P_Username", UserName);
                    param.Add("@P_IP_ADDRESS", IP);
                    param.Add("@P_DATE_NOW", DateTime.Now);



                    List<ATTPSLEntryClear> psl = connection.Query<ATTPSLEntryClear>("GET_HOLDERINFO_PSL_CLEAR", param, commandType: CommandType.StoredProcedure)?.ToList();

                    if (psl.Count > 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.Message = "record found";
                        jsonResponse.ResponseData = psl;
                    }
                    else
                    {
                        jsonResponse.Message = "No Such Holder Exists !!!";
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



        public JsonResponse SearchHolderPSL(string CompCode, string ShholderNo, string SelectedAction, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_COMPCODE", CompCode);
                    param.Add("@P_SHHOLDERNO", ShholderNo);
                    param.Add("@P_SELECTEDACTION", SelectedAction);
                    param.Add("@P_Username", UserName);
                    param.Add("@P_IP_ADDRESS", IP);
                    param.Add("@P_DATE_NOW", DateTime.Now);




                    List<ATTPSLEntryClear> psl = connection.Query<ATTPSLEntryClear>("Get_Holdersearch_PSL_Clear", param, commandType: CommandType.StoredProcedure)?.ToList();

                    if (psl.Count > 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.Message = "record found";
                        jsonResponse.ResponseData = psl;
                    }
                    else
                    {
                        jsonResponse.Message = "No Such Holder Exists !!!";
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
        public JsonResponse SavePSLClearEntry(List<ATTPSLEntryClear> PSLEntry, string CompCode, int ShholderNo, int pslno, int PSL_clear_No, string Charge, string ClearedDt, string ClearRemark, string UserName, string Issuedup, string SelectedAction, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        try
                        {
                            DataTable dt = new DataTable();
                            dt.Columns.Add("SeqNo");
                            dt.Columns.Add("CertNo");

                            int i = 1;
                            PSLEntry.ForEach(x => dt.Rows.Add(i++, x.CertNo));

                            SqlCommand cmd = new SqlCommand("PSL_CLEAR_BATCHENTRY", connection);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Transaction = tran;
                            SqlParameter param = cmd.Parameters.AddWithValue("@udt", dt);
                            param = cmd.Parameters.AddWithValue("@P_COMPCODE", CompCode);
                            param = cmd.Parameters.AddWithValue("@P_SHHOLDERNO", ShholderNo);
                            param = cmd.Parameters.AddWithValue("@P_PSLNO ", pslno);
                            param = cmd.Parameters.AddWithValue("@P_PSLCLEARNO", PSL_clear_No);
                            param = cmd.Parameters.AddWithValue("@P_CLEAR_CHARGE", Charge);

                            param = cmd.Parameters.AddWithValue("@P_CLEARDATE", ClearedDt);
                            param = cmd.Parameters.AddWithValue("@P_REMARK", ClearRemark);
                            param = cmd.Parameters.AddWithValue("@P_USERNAME", UserName);
                            param = cmd.Parameters.AddWithValue("@P_DUPISSUE", Issuedup);
                            param = cmd.Parameters.AddWithValue("@P_SELECTEDACTION", SelectedAction);
                            param = cmd.Parameters.AddWithValue("@P_IP", IP);
                            param = cmd.Parameters.AddWithValue("@P_DATE_NOW", DateTime.Now);
                            param.Direction = ParameterDirection.Input;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    if (reader.GetInt32(0) == 1)
                                    {
                                        jsonResponse.IsSuccess = true;

                                    }
                                    else
                                    {
                                        jsonResponse.IsSuccess = false;
                                    }
                                    jsonResponse.Message = reader.GetString(1);
                                }
                            }
                            if (jsonResponse.IsSuccess)
                            {
                                jsonResponse.Message = ATTMessages.SAVED_SUCCESS;
                                tran.Commit();
                            }

                            else
                                tran.Rollback();
                        }
                        catch (Exception ex)
                        {
                            jsonResponse.IsSuccess = false;
                            jsonResponse.Message = ex.Message;
                        }
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.Message = ex.Message;
                }
                return jsonResponse;
            }
        }

        public JsonResponse SearchHolderPSL(string compCode, string shholderNo, string SelectedAction)
        {
            throw new NotImplementedException();
        }

        public JsonResponse GetPSLInformation(string compCode, string shholderNo, string selectedAction, string PSLNo)
        {
            throw new NotImplementedException();
        }
    }
}


