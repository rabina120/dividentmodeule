
using Dapper;
using Entity.Common;
using Entity.DemateDividend;
using Interface.DividendProcessing;
using Microsoft.Extensions.Options;
using Repository.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.DividendProcessing
{
    public class UndoDematePaymentRepo : IUndoDematePayment
    {
        IOptions<ReadConfig> _connectionString;

        public UndoDematePaymentRepo(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }
        public JsonResponse GetDividendInformation(string CompCode, string DivCode, string shholderno, string based, string undoType, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                //List<ATTDemateDividend> cashDividendInformation;

                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_DivCode", DivCode.Trim());
                    param.Add("@P_COMPCODE", CompCode.Trim());
                    param.Add("@TableName1", "2");

                    string tableName = new TableReporsitory(_connectionString).GetTableName(param);

                    param = new DynamicParameters();
                    param.Add("@P_TABLENAME", tableName.Trim());
                    param.Add("@P_BOIDNO", shholderno.Trim());
                    param.Add("@P_COMPCODE", CompCode.Trim());
                    param.Add("@P_IP_ADDRESS", IP);
                    param.Add("@P_USERNAME", UserName);
                    param.Add("@P_DATE_NOW", DateTime.Now);
                    param.Add("@P_ACTION", based == "S" ? "S" : "W");

                    List<ATTDemateDividend>  cashDividendInformation = connection.Query<ATTDemateDividend>(sql: "GET_HOLDER_DEMATE_DIVIDEND_INFORMATION_for_undo", param: param, commandType: System.Data.CommandType.StoredProcedure)?.ToList();

                    if (cashDividendInformation.Count > 0)
                    {
                        if (cashDividendInformation.Count > 1)
                        {
                            jsonResponse.IsSuccess = false;
                            if (based == "S")
                            {
                                jsonResponse.Message = "Duplicate Boid found. Search by Warrant no";
                            }
                            else
                            {
                                jsonResponse.Message = "Duplicate warrant no found. Search by Boid";
                            }

                        }
                        else if (undoType == "Issue")
                        {
                            if (cashDividendInformation[0].WIssued == false)
                            {
                                jsonResponse.IsSuccess = false;
                                jsonResponse.Message = "Dividend Not Issued";
                            }
                            else if (cashDividendInformation[0].wissue_Approved == "N")
                            {
                                jsonResponse.IsSuccess = false;
                                jsonResponse.Message = "Dividend Issued But Unapproved";
                            }
                            else if (cashDividendInformation[0].WPaid == true && cashDividendInformation[0].wpaid_approved == "Y")
                            {
                                jsonResponse.IsSuccess = false;
                                jsonResponse.Message = "Undo Payment First";
                            }
                            else
                            {
                                jsonResponse.IsSuccess = true;
                                jsonResponse.ResponseData = cashDividendInformation[0];
                            }
                        }
                        else if (undoType == "Payment")
                        {
                            if (cashDividendInformation[0].WPaid == false)
                            {
                                jsonResponse.IsSuccess = false;
                                jsonResponse.Message = "Payment Not Issued";
                            }
                            else if (cashDividendInformation[0].wpaid_approved == "N")
                            {
                                jsonResponse.IsSuccess = false;
                                jsonResponse.Message = "Payment Issued But Unapproved";
                            }
                            else
                            {
                                jsonResponse.IsSuccess = true;
                                jsonResponse.ResponseData = cashDividendInformation[0];
                            }
                        }
                    }
                    else
                    {
                        jsonResponse.Message = ATTMessages.SHARE_HOLDER.NOT_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.ResponseData = ex;
                    jsonResponse.IsSuccess = false;
                    jsonResponse.HasError = true;
                }
                return jsonResponse;
            }
        }

        public JsonResponse SaveCashDividend(string DivCode, string CompCode, string undoType, string warrantno, string shholderno, string UserName, string IP)
        {
            JsonResponse jsonResponse2 = new JsonResponse();


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
                        param.Add("@TableName1", "2");
                        string tableName = new TableReporsitory(_connectionString).GetTableName(param);
                        if (tableName != null)
                        {
                            param = new DynamicParameters();
                            param.Add("@compcode", CompCode);
                            param.Add("@undoType", undoType);
                            param.Add("@warrantno", warrantno);
                            param.Add("@boid", shholderno);
                            param.Add("@TableName1", tableName);
                            param.Add("@username", UserName);
                            param.Add("@P_IP_ADDRESS", IP);
                            param.Add("@P_DATE_NOW", DateTime.Now);
                            jsonResponse2 = connection.Query<JsonResponse>("SAVE_DEMATE_DIVIDEND_UNDO_PAYMENT", param: param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                            if (jsonResponse2.IsSuccess)
                                trans.Commit();
                            else
                                trans.Rollback();
                        }
                        else
                        {
                            jsonResponse2.Message = ATTMessages.NO_TABLES_FOUND;
                        }
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse2.ResponseData = ex;
                    jsonResponse2.IsSuccess = false;
                    jsonResponse2.HasError = true;
                }
                return jsonResponse2;

            }
        }
    }
}
