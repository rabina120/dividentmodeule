
using Dapper;
using Entity.Common;
using Entity.Dividend;
using Entity.ShareHolder;
using Interface.DividendProcessing;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.DividendProcessing
{
    public class CashDividendPaymentEntry : ICashDividendPaymentEntry
    {
        IOptions<ReadConfig> _connectionString;

        public CashDividendPaymentEntry(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }

        public JsonResponse GetDividendPaymentInformation(string CompCode, string DivCode, string shholderno, string a, string action)
        {
            //string a is for shholder or warrant no and ac is to check if it is undo or add
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                ATTCashDividend cashDividendInformation;
                try
                {
                    connection.Open();

                    string tableName = connection.Query<string>(sql: "SELECT tablename1 FROM DIVTABLE WHERE  DIVCODE = " + DivCode + " and DIVCALCULATED = 1 and COMPCODE='" + CompCode + "' order by divcode", param: null, commandType: null).FirstOrDefault();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_TABLENAME", tableName);
                    param.Add("@P_SHHOLDERNO", shholderno);
                    if (a == "S")
                    {
                        param.Add("@P_ACTION", "S");
                        cashDividendInformation = SqlMapper.Query<ATTCashDividend, ATTShHolder, ATTCashDividend>(connection, sql: "GET_HOLDER_DIVIDEND_INFORMATION",
                       (cashDividendInformation, shHolder) =>
                       {
                           cashDividendInformation.attShholder = shHolder;
                           return cashDividendInformation;
                       },
                       param: param, null, splitOn: "ShholderBind", commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                    }
                    else
                    {
                        param.Add("@P_ACTION", "W");
                        cashDividendInformation = SqlMapper.Query<ATTCashDividend, ATTShHolder, ATTCashDividend>(connection, sql: "GET_HOLDER_DIVIDEND_INFORMATION",
                      (cashDividendInformation, shHolder) =>
                      {
                          cashDividendInformation.attShholder = shHolder;
                          return cashDividendInformation;
                      },
                      param: param, null, splitOn: "ShholderBind", commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                    }


                    if (cashDividendInformation != null)
                    {
                        if (action == "A")
                        {
                            if (cashDividendInformation.Wissue_Approved == "Y" && cashDividendInformation.wissue_status == "POSTED" && cashDividendInformation.WPaid == false)
                            {
                                jsonResponse.IsSuccess = true;
                                jsonResponse.ResponseData = cashDividendInformation;
                            }
                            else if (cashDividendInformation.WPaid == true && cashDividendInformation.wpaid_approved == "N")
                            {
                                jsonResponse.IsSuccess = false;
                                jsonResponse.Message = "Dividend Already Paid But Unposted";
                            }
                            else if (cashDividendInformation.WPaid == true && cashDividendInformation.wpaid_approved == "Y")
                            {
                                jsonResponse.IsSuccess = false;
                                jsonResponse.Message = "Dividend Already Paid And Posted";
                            }
                            else if (cashDividendInformation.WIssued == false)
                            {
                                jsonResponse.IsSuccess = false;
                                jsonResponse.Message = "Dividend Is Not Issued Yet.";
                            }
                            else
                            {
                                jsonResponse.IsSuccess = false;
                                jsonResponse.Message = "Cannot Find The Record Check if Ithas been Issued Yet.!!";
                            }
                        }
                        else if (action == "D")
                        {
                            if (cashDividendInformation.WPaid == true && cashDividendInformation.wpaid_approved == "Y" && cashDividendInformation.wpaid_status == "POSTED")
                            {
                                jsonResponse.IsSuccess = false;
                                jsonResponse.Message = "Dividend Already Paid And Approved!!";
                            }
                            else if (cashDividendInformation.WPaid == true && cashDividendInformation.wpaid_approved == "N")
                            {
                                jsonResponse.IsSuccess = true;
                                jsonResponse.ResponseData = cashDividendInformation;
                            }
                            else
                            {
                                jsonResponse.IsSuccess = false;
                                jsonResponse.Message = "Cannot Find The Record!!";
                            }
                        }
                    }
                    else
                    {
                        jsonResponse.IsSuccess = false;
                        jsonResponse.Message = "Cannot Find The Record!!";
                    }

                }
                catch (Exception ex)
                {
                    jsonResponse.Message = ex.Message;
                }
                return jsonResponse;
            }
        }

        //public JsonResponse GetMaxSeqno(string tablename, string centerid)
        //{
        //    JsonResponse jsonResponse = new JsonResponse();
        //    using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
        //    {
        //        connection.Open();
        //        try
        //        {

        //            int maxSeqNo = connection.Query<int>(sql: "SELECT max(seqno) FROM " + tablename + " WHERE centerid = " + centerid, param: null, commandType: null).FirstOrDefault();
        //            if (maxSeqNo == 0)
        //            {
        //                maxSeqNo = maxSeqNo + 1;
        //            }
        //            jsonResponse.IsSuccess = true;
        //            jsonResponse.ResponseData = maxSeqNo;

        //        }
        //        catch (Exception ex)
        //        {
        //            jsonResponse.Message = ex.Message;
        //        }
        //        return jsonResponse;
        //    }
        //}

        public JsonResponse SaveCashDividendPayment(string tablename, string wamtpaiddt, string batchno, string compcode, string shholderno, string selectedAction, string username)
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
                        ", wissue_Approved= 'N', wissue_status= 'UNPOSTED' ,dwiby='" + username + "'" +
                        "where compcode='" + compcode + "' and shholderno='" + shholderno + "' and wissued =1 and wissue_approved = 'Y'";
                    }
                    else
                    {
                        sql = "Update " + tablename + " set wpaid = 0,wpaid_approved=Null,wpaid_status=null,WPaidBy=null, WAmtPaidDt=null,batchno=null where compcode =" + compcode + " and shholderno=" + shholderno + " and wpaid = 1 and wpaid_approved = 'N'";
                    }

                    connection.Query(sql: sql, param: null, commandType: null).FirstOrDefault();
                    jsonResponse2.IsSuccess = true;

                    jsonResponse2.Message = selectedAction == "A" ? "Cash Dividend Paid Added Successfully." : "Cash Dividend UnPaid Added Successfully.";
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
