
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
    public class CashDemateDividendIssueEntry : ICashDemateDividendIssueEntry
    {
        IOptions<ReadConfig> _connectionString;

        public CashDemateDividendIssueEntry(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }

        public JsonResponse GetDemateDividendInformation(string CompCode, string DivCode, string BoidNo, string a, string action, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                //ATTDemateDividend cashDividendInformation = new ATTDemateDividend();

                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_DivCode", DivCode);
                    param.Add("@P_COMPCODE", CompCode);
                    param.Add("@TableName1", "2");
                    string tableName = new TableReporsitory(_connectionString).GetTableName(param);


                    param = new DynamicParameters();
                    param.Add("@P_TABLENAME", tableName);
                    param.Add("@P_BOIDNO", BoidNo);
                    param.Add("@P_COMPCODE", CompCode);
                    param.Add("@P_IP_ADDRESS", IP);
                    param.Add("@P_USERNAME", UserName);
                    param.Add("@P_DATE_NOW", DateTime.Now);
                    param.Add("@P_ACTION", a == "S" ? "S" : "W");

                    List<ATTDemateDividend> cashDividendInformation = connection.Query<ATTDemateDividend>(sql: "GET_HOLDER_DEMATE_DIVIDEND_INFORMATION", param: param, commandType: System.Data.CommandType.StoredProcedure)?.ToList();


                    if (cashDividendInformation.Count > 0 )
                    {
                        if(cashDividendInformation.Count > 1)
                        {
                            jsonResponse.IsSuccess = false;
                            if(a == "S")
                            {
                                jsonResponse.Message = "Duplicate holder found. Search by Warrant no";
                            }
                            else
                            {
                                jsonResponse.Message = "Duplicate holder found. Search by BOID no";
                            }
                            
                        }
                        else if (cashDividendInformation[0].WIssued == true && cashDividendInformation[0].wissue_status == "POSTED" && cashDividendInformation[0].wissue_Approved == "Y")
                        {
                            jsonResponse.IsSuccess = false;
                            jsonResponse.Message = "Dividend already issued and Approved";
                        }
                        else if (cashDividendInformation[0].WIssued == true && cashDividendInformation[0].wissue_Approved == "N")
                        {
                            if (action == "A")
                            {
                                jsonResponse.IsSuccess = false;
                                jsonResponse.Message = "Dividend already issued but Unposted";
                            }
                            else
                            {
                                jsonResponse.IsSuccess = true;
                                jsonResponse.ResponseData = cashDividendInformation[0];
                            }

                        }
                        else if (cashDividendInformation[0].WIssued == false)
                        {
                            if (action == "A")
                            {
                                jsonResponse.IsSuccess = true;
                                jsonResponse.ResponseData = cashDividendInformation[0];

                            }
                            else
                            {
                                jsonResponse.IsSuccess = false;
                                jsonResponse.Message = "Demate Dividend Is Not Issued Yet.";
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
                    jsonResponse.ResponseData = ex;
                    jsonResponse.IsSuccess = false;
                    jsonResponse.HasError = true;

                }
                return jsonResponse;
            }
        }

        public JsonResponse GetMaxDemateSeqno(string tablename, string centerid)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    int? maxSeqNo = connection.Query<int?>(sql: "SELECT max(seqno) FROM " + tablename + " WHERE centerid = '" + centerid + "'", param: null, commandType: null).FirstOrDefault();
                    if (maxSeqNo >= 1)
                    {
                        maxSeqNo = maxSeqNo + 1;
                    }
                    else
                    {
                        maxSeqNo = 1;
                    }

                    jsonResponse.IsSuccess = true;
                    jsonResponse.ResponseData = maxSeqNo;

                }
                catch (Exception ex)
                {
                    jsonResponse.Message = ex.Message;
                }
                return jsonResponse;
            }
        }

        public JsonResponse SaveCashDemateDividend(string DivCode, string CompCode, string centerid, string remarks, string bankName, string accountNo, string compcode, string warrantNo, string boidno, string selectedAction, string wissueddate, string creditedDt, string UserName, string IsPaidBy, string IP)
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
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@P_DivCode", DivCode);
                        param.Add("@P_COMPCODE", CompCode);
                        param.Add("@TableName1", "2");
                        string tableName = new TableReporsitory(_connectionString).GetTableName(param);
                        if (tableName != null)
                        {
                            if (selectedAction == "A")
                            {
                                jsonResponse = GetMaxDemateSeqno(tableName, centerid);
                                maxSeqno = Convert.ToInt32(jsonResponse.ResponseData);

                            }

                            param = new DynamicParameters();
                            param.Add("@compcode", compcode);
                            param.Add("@TableName2", tableName);
                            param.Add("@Bo_idno", boidno);
                            param.Add("@Action", selectedAction);
                            param.Add("@IsPaidBy", IsPaidBy);
                            param.Add("@warrantNo", warrantNo);
                            param.Add("@P_IP_ADDRESS", IP);
                            param.Add("@P_USERNAME", UserName);
                            param.Add("@P_DATE_NOW", DateTime.Now);
                            if (selectedAction == "A")
                            {
                                param.Add("@centerid", centerid);
                                param.Add("@maxSeqno", maxSeqno);
                                param.Add("@wissueddate", wissueddate);
                                param.Add("@creditedDt", creditedDt);
                                param.Add("@remarks", remarks);
                                param.Add("@bankName", bankName);
                                param.Add("@accountNo", accountNo);

                            }


                            jsonResponse2 = connection.Query<JsonResponse>(sql: "SAVE_CASH_DEMATE_ISSUE_ENTRY", param: param, trans, commandType: CommandType.StoredProcedure).FirstOrDefault();

                            if (jsonResponse2.IsSuccess)
                                trans.Commit();
                        }
                        else
                        {
                            jsonResponse2.Message = "Failed to Get Table Name";
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

