
using Dapper;
using Entity.Common;
using Entity.Dividend;
using Entity.ShareHolder;
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
    public class DividentPaymentEntry : IDividentPaymentEntry
    {
        IOptions<ReadConfig> _connectionString;

        public DividentPaymentEntry(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }
        public JsonResponse GetDividendInformation(string CompCode, string DivCode, string shholderno, string a, string action, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                List<ATTCashDividend> cashDividendInformation;

                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_DivCode", DivCode.Trim());
                    param.Add("@P_COMPCODE", CompCode.Trim());
                    param.Add("@TableName1", "1");
                    string tableName = new TableReporsitory(_connectionString).GetTableName(param);

                    param = new DynamicParameters();
                    param.Add("@P_TABLENAME", tableName.Trim());
                    param.Add("@P_SHHOLDERNO", shholderno.Trim());
                    param.Add("@P_COMPCODE", CompCode.Trim());
                    param.Add("@P_USERNAME", UserName);
                    param.Add("@P_ACTION", a == "S" ? "S" : "W");
                    param.Add("@P_IP_ADDRESS", IPAddress);
                    param.Add("@P_ENTRY_DATE", DateTime.Now);

                    cashDividendInformation = SqlMapper.Query<ATTCashDividend, ATTShHolder, ATTCashDividend>(connection, sql: "GET_HOLDER_DIVIDENDPAYMENT_INFORMATION",
                   (cashDividendInformation, shHolder) =>
                   {
                       cashDividendInformation.attShholder = shHolder;
                       return cashDividendInformation;
                   },
                   param: param, null, splitOn: "ShholderBind", commandType: CommandType.StoredProcedure)?.ToList();


                    if (cashDividendInformation.Count >0 )
                    {

                        if (cashDividendInformation.Count > 1)
                        {
                            jsonResponse.IsSuccess = false;
                            if (a == "S")
                            {
                                jsonResponse.Message = "Duplicate holder found. Search by Warrant no";
                            }
                            else
                            {
                                jsonResponse.Message = "Duplicate holder found. Search by holder no";
                            }

                        }
                        else if (cashDividendInformation[0].attShholder.HolderLock != "Y" || cashDividendInformation[0].attShholder.HolderLock == null)
                        {
                            if (cashDividendInformation[0].WPaid == true && cashDividendInformation[0].wpaid_status == "POSTED" && cashDividendInformation[0].wpaid_approved == "Y")
                            {
                                jsonResponse.IsSuccess = false;
                                jsonResponse.Message = ATTMessages.DIVIDEND.PAID_POSTED_APPROVED;
                            }
                            else if (cashDividendInformation[0].WPaid == true && cashDividendInformation[0].wpaid_approved == "N")
                            {
                                if (action == "A")
                                {
                                    jsonResponse.IsSuccess = false;
                                    jsonResponse.Message = ATTMessages.DIVIDEND.PAID_UNPOSTED_UNAPPROVED;
                                }
                                else
                                {
                                    jsonResponse.IsSuccess = true;
                                    jsonResponse.ResponseData = cashDividendInformation[0];
                                }

                            }

                            else
                            {
                                if (action == "A")
                                {
                                    jsonResponse.IsSuccess = true;
                                    jsonResponse.ResponseData = cashDividendInformation[0];
                                }
                                else
                                {
                                    jsonResponse.IsSuccess = false;
                                    jsonResponse.Message = ATTMessages.DIVIDEND.NOT_ISSUED;

                                }
                            }
                        }
                        else
                        {
                            jsonResponse.IsSuccess = false;
                            jsonResponse.Message = ATTMessages.SHARE_HOLDER.LOCK;
                        }



                    }
                    else
                    {
                        jsonResponse.IsSuccess = false;
                        jsonResponse.Message = ATTMessages.SHARE_HOLDER.NOT_FOUND + " or " + ATTMessages.DIVIDEND.NOT_ISSUED;
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

        public JsonResponse GetMaxSeqno(string tablename, string centerid)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_TableName", tablename.Trim());
                    param.Add("@P_CenterId", centerid.Trim());


                    int maxSeqNo = connection.Query<int>(sql: "GET_MAX_SEQ_NO", param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    maxSeqNo = maxSeqNo + 1;

                    jsonResponse.IsSuccess = true;
                    jsonResponse.ResponseData = maxSeqNo;

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

        public JsonResponse SaveDividentPaymentEntry(string DivCode, string CompCode, string bankName, string accountNo, string centerid, string PayUser, string telno, string cashOrCheque,string warrantNo, string shholderno, string selectedAction,string creditedDt, string wissueddate,string IssueRemarks, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            JsonResponse jsonResponse2 = new JsonResponse();

            int maxSeqno = 0;


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
                            param.Add("@P_DivCode", DivCode);
                            param.Add("@P_COMPCODE", CompCode);
                            param.Add("@TableName1", "1");
                            string tableName = new TableReporsitory(_connectionString).GetTableName(param);
                            if (tableName != null)
                            {
                                jsonResponse = GetMaxSeqno(tableName, centerid);
                                maxSeqno = Convert.ToInt32(jsonResponse.ResponseData);

                                param = new DynamicParameters();
                                param.Add("@compcode", CompCode);
                                param.Add("@shholderno", shholderno);
                                param.Add("@warrantNo", warrantNo);
                                param.Add("@bankName", bankName);
                                param.Add("@accountNo", accountNo);
                                param.Add("@TableName1", tableName);
                                param.Add("@cashOrCheque", cashOrCheque);
                                param.Add("@centerid", centerid);
                                param.Add("@maxSeqno", maxSeqno);
                                param.Add("@username", PayUser);

                                param.Add("@wissueddate", wissueddate);
                                param.Add("@creditedDt", creditedDt);
                                param.Add("@IssueRemarks", IssueRemarks);
                                param.Add("@telno", telno=null??"00");
                                param.Add("@Action", selectedAction);
                                param.Add("@P_ENTRY_DATE", DateTime.Now);
                                param.Add("@P_IP_ADDRESS", IPAddress);

                                jsonResponse2 = connection.Query<JsonResponse>("SAVE_DIVIDEND_PAYMENT_ENTRY", param: param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();

                                trans.Commit();

                            }
                            else
                            {
                                jsonResponse2.Message = "Failed to Get Table Name";
                            }
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            jsonResponse2.IsSuccess = false;
                            jsonResponse2.Message = ex.Message;
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
