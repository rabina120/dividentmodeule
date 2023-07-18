

using Dapper;
using Entity.Common;
using Entity.DemateDividend;
using Interface.DividendProcessing;
using Microsoft.Extensions.Options;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.DividendProcessing
{
    public class CashDemateDividendPaymentEntry : ICashDemateDividendPaymentEntry
    {
        IOptions<ReadConfig> _connectionString;

        public CashDemateDividendPaymentEntry(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }

        public JsonResponse GetDemateDividendPaymentInformation(string CompCode, string DivCode, string shholderno, string a, string action)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {

                ATTDemateDividend cashDividendInformation;

                try
                {
                    connection.Open();
                    string tableName = connection.Query<string>(sql: "SELECT tablename2 FROM DIVTABLE WHERE  DIVCODE = " + DivCode + " and DIVCALCULATED = 1 and COMPCODE='" + CompCode + "'", param: null, commandType: null).FirstOrDefault();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_TABLENAME", tableName);
                    param.Add("@P_SHHOLDERNO", shholderno);
                    if (a == "S")
                    {
                        param.Add("@P_ACTION", "S");

                        cashDividendInformation = connection.Query<ATTDemateDividend>(sql: "GET_HOLDER_DEMATE_DIVIDEND_INFORMATION", param: param, commandType: System.Data.CommandType.StoredProcedure)?.FirstOrDefault();
                    }
                    else
                    {
                        param.Add("@P_ACTION", "W");
                        cashDividendInformation = connection.Query<ATTDemateDividend>(sql: "GET_HOLDER_DEMATE_DIVIDEND_INFORMATION", param: param, commandType: System.Data.CommandType.StoredProcedure)?.FirstOrDefault();
                    }


                    if (cashDividendInformation != null)
                    {
                        if (action == "A")
                        {
                            if (cashDividendInformation.WIssued == true && cashDividendInformation.wissue_status == "POSTED" && cashDividendInformation.wissue_Approved == "Y" && cashDividendInformation.WPaid == false)
                            {
                                jsonResponse.IsSuccess = true;
                                jsonResponse.ResponseData = cashDividendInformation;
                            }
                            else if (cashDividendInformation.WPaid == true && cashDividendInformation.wissue_Approved == "N")
                            {
                                jsonResponse.IsSuccess = false;
                                jsonResponse.Message = "Dividend already paid but Unposted";



                            }
                            else if (cashDividendInformation.WPaid == true && cashDividendInformation.wpaid_approved == "Y")
                            {
                                jsonResponse.IsSuccess = false;
                                jsonResponse.Message = "Demate Dividend Is Already Paid And Posted.";
                            }
                            else
                            {
                                jsonResponse.IsSuccess = false;
                                jsonResponse.Message = "Demate Payment Information Not Found.";
                            }


                        }
                        else if (action == "D")
                        {
                            if (cashDividendInformation.WPaid == true && cashDividendInformation.wpaid_approved == "Y" && cashDividendInformation.wpaid_status == "POSTED")
                            {
                                jsonResponse.IsSuccess = false;
                                jsonResponse.Message = "Dividend Already Paid And Apporved";
                            }
                            else if (cashDividendInformation.WPaid == true && cashDividendInformation.wpaid_approved == "N")
                            {
                                jsonResponse.IsSuccess = true;
                                jsonResponse.ResponseData = cashDividendInformation;
                            }
                            else
                            {
                                jsonResponse.IsSuccess = false;
                                jsonResponse.Message = "Dividend Payment Information Cannot be Found.";
                            }
                        }

                    }
                    else
                    {
                        jsonResponse.IsSuccess = false;
                        jsonResponse.Message = "Cannot Find The Holder!!";
                    }

                }
                catch (Exception ex)
                {
                    jsonResponse.Message = ex.Message;
                }
                return jsonResponse;
            }
        }

        public JsonResponse GetMaxDematePaymentSeqno(string tablename, string centerid)
        {
            throw new NotImplementedException();
        }

        public JsonResponse SaveCashDemateDividendPayment(string tablename, string wamtpaiddt, string batchno, string compcode, string shholderno, string selectedAction, string username)
        {
            JsonResponse jsonResponse2 = new JsonResponse();

            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {


                try
                {
                    connection.Open();

                    string sql = "";
                    if (selectedAction == "A")
                    {
                        sql = "Update " + tablename + " set wpaid = 1 , wpaid_approved ='N', wpaid_status= 'UNPOSTED'," +
                            " WPaidBy = '" + username + "', WAmtPaidDt = '" + wamtpaiddt + "' , batchno =" + batchno +

                        "where compcode='" + compcode + "' and Bo_idno='" + shholderno + "' and wissued =1 and wissue_approved = 'Y'";
                    }
                    else
                    {
                        sql = "Update " + tablename + " set wpaid = 0,wpaid_approved=Null,wpaid_status=null,WPaidBy=null, WAmtPaidDt=null,batchno=null where compcode =" + compcode + " and Bo_idno=" + shholderno + " and wpaid = 1 and wpaid_approved = 'N'";
                    }

                    connection.Query(sql: sql, param: null, commandType: null).FirstOrDefault();
                    jsonResponse2.IsSuccess = true;

                    jsonResponse2.Message = selectedAction == "A" ? "Demate Dividend Paid Added Successfully." : "Demate Dividend UnPaid Added Successfully.";
                }
                catch (Exception ex)
                {
                    jsonResponse2.Message = ex.Message;
                }
                return jsonResponse2;

            }
        }
    }
}
