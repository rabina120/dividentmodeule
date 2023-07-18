

using Dapper;
using Entity.Common;
using Entity.Dividend;
using Interface.Parameter;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Parameter
{
    public class DividendParameterSetUp : IDividendParameterSetUp
    {
        IOptions<ReadConfig> connectionString;
        public DividendParameterSetUp(IOptions<ReadConfig> _connectionString)
        {
            connectionString = _connectionString;
        }

        public JsonResponse DeleteParameterSetup(string UserName, string CompCode, string DivCode, string ActionType, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction trans = connection.BeginTransaction())
                    {
                        using (SqlCommand cmd = new SqlCommand("DELETE_DIV_DETAIL"))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Connection = connection;
                            cmd.Transaction = trans;
                            cmd.Parameters.AddWithValue("@CompCode", CompCode);
                            cmd.Parameters.AddWithValue("@ActionType", ActionType);
                            cmd.Parameters.AddWithValue("@DivCode", DivCode);
                            cmd.Parameters.AddWithValue("@UserName", UserName);
                            cmd.Parameters.AddWithValue("@P_ENTRY_DATE", DateTime.Now);
                            cmd.Parameters.AddWithValue("@P_IP_ADDRESS", IPAddress);



                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {

                                if (reader.Read())
                                {
                                    //a = reader.Read();
                                    response.Message = reader.GetString(0);
                                    response.IsSuccess = Convert.ToBoolean(reader.GetString(1));

                                }
                            }


                            trans.Commit();
                        }

                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
                return response;
            }

        }

        public JsonResponse GetDividendCode(string CompCode)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    string sqlQuery = "SELECT max(DivCode)+1 as Divcode from DivTable where compcode = '" + CompCode + "'";
                    int? data = connection.Query<int?>(sql: sqlQuery, param: null, null, commandType: null)?.FirstOrDefault();
                    if (data == null)
                    {
                        data = 1;
                    }
                    response.ResponseData = ((1000 + data).ToString()).Substring(1, 3);
                    if (response.ResponseData != null)
                    {
                        response.IsSuccess = true;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
                return response;
            }

        }
        public string GetDividendCodeString(string CompCode)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    string sqlQuery = "SELECT max(DivCode)+1 as Divcode from DivTable where compcode = '" + CompCode + "'";
                    int? data = connection.Query<int?>(sql: sqlQuery, param: null, null, commandType: null)?.FirstOrDefault();
                    if (data == null)
                    {
                        data = 1;
                    }
                    response.ResponseData = ((1000 + data).ToString()).Substring(1, 3);
                    if (response.ResponseData != null)
                    {
                        response.IsSuccess = true;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
                return response.ResponseData.ToString();
            }

        }

        public JsonResponse SaveParameterSetup(ATTDividend dividend, string ActionType, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            string DivCode = GetDividendCodeString(dividend.compcode);
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        string detailPart = dividend.compcode + "_" + DivCode + dividend.AgmNo;
                        string description = null;

                        string tableName1 = null, tableName2 = null;
                        if (ActionType.Trim() == "1")
                        {
                            tableName1 = "DivMaster" + detailPart;
                            tableName2 = "DivMasterCDS" + detailPart;
                            description = "FIN" + dividend.compcode + "_" + DivCode + dividend.AgmNo;

                        }
                        else if (ActionType.Trim() == "2")
                        {
                            tableName1 = "Shbonus" + detailPart;
                            tableName2 = "ShbonusCDS" + detailPart;
                            description = "BON" + dividend.compcode + "_" + DivCode + dividend.AgmNo;
                        }
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@CompCode", dividend.compcode);
                        param.Add("@DivCode", DivCode);
                        param.Add("@Description", description);
                        param.Add("@Agm", dividend.AgmNo);
                        param.Add("@DivType", ActionType);
                        param.Add("@FiscalYr", dividend.FiscalYr);
                        param.Add("@BonusShPer", dividend.BonusShPer);
                        param.Add("@taxper", dividend.taxper);
                        param.Add("@DDeclareDt", dividend.DDeclareDt);
                        param.Add("@tableName1", tableName1 ?? tableName1);
                        param.Add("@tableName2", tableName2 ?? tableName2);
                        param.Add("@UserName", UserName);
                        param.Add("@@EntryDate", dividend.DDeclareDt);
                        param.Add("@P_ENTRY_DATE", DateTime.Now);
                        param.Add("@P_IP_ADDRESS", IPAddress);

                        response = connection.Query<JsonResponse>("Save_Dividend_Parameter_Setup", param, tran, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                        if (response.IsSuccess)
                        {
                            tran.Commit();
                            response.Message = "Dividend Setup with DivCode: " + DivCode;
                        }
                        else
                        {
                            tran.Rollback();
                            response.Message = ATTMessages.CANNOT_SAVE;
                        }

                        //AuditRepo audit = new AuditRepo(connectionString);
                        //audit.auditSave(dividend.username, "DIVIDEND Parameter Save of "+ dividend.compcode + dividend.Divcode, "DIVIDEND Parameter Save");

                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
                return response;
            }
        }

        public JsonResponse GetDividivendDetails(string CompCode, string DivCode, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@CompCode", CompCode);
                    param.Add("@DivCode", DivCode);
                    param.Add("@UserName", UserName);
                    param.Add("@P_ENTRY_DATE", DateTime.Now);
                    param.Add("@P_IP_ADDRESS", DateTime.Now);
                    ATTDividend aTTDividend = connection.Query<ATTDividend>("Get_Dividend_Details", param, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                    jsonResponse.IsSuccess = true;
                    if (aTTDividend != null)
                    {
                        jsonResponse.ResponseData = aTTDividend;
                    }

                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.HasError = true;
                    jsonResponse.ResponseData = ex;
                }
                return jsonResponse;
            }
        }
    }
}
