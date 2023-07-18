
using Dapper;
using Entity.Common;
using Entity.Dividend;
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
    public class DemateDividentPaymentEntry : IDemateDividentPaymentEntry
    {
        IOptions<ReadConfig> _connectionString;
        public DemateDividentPaymentEntry(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }
        public JsonResponse GetDemateDividendInformation(string CompCode, string DivCode, string BoidNo, string a, string action,string Username,string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                List<ATTDemateDividentPaymentEntry> demateDividendInformation;

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
                    param.Add("@P_USERNAME", Username);
                    param.Add("@P_IP_ADDRESS", IPAddress);
                    param.Add("@P_DATE_NOW", DateTime.Now);
                    if (a == "S")
                    {
                        param.Add("@P_ACTION", "S");

                        demateDividendInformation = connection.Query<ATTDemateDividentPaymentEntry>(sql: "GET_HOLDER_DEMATE_DIVIDEND_INFORMATION", param: param, commandType: System.Data.CommandType.StoredProcedure)?.ToList();
                    }
                    else
                    {
                        param.Add("@P_ACTION", "W");
                        demateDividendInformation = connection.Query<ATTDemateDividentPaymentEntry>(sql: "GET_HOLDER_DEMATE_DIVIDEND_INFORMATION", param: param, commandType: System.Data.CommandType.StoredProcedure)?.ToList();
                    }


                    if (demateDividendInformation.Count > 0)
                    {
                        if (demateDividendInformation.Count > 1)
                        {
                            jsonResponse.IsSuccess = false;
                            if (a == "S")
                            {
                                jsonResponse.Message = "Duplicate holder found. Search by Warrant no";
                            }
                            else
                            {
                                jsonResponse.Message = "Duplicate holder found. Search by boid no";
                            }

                        }
                        else if (demateDividendInformation[0].WPaid == true && demateDividendInformation[0].wpaid_status == "POSTED" && demateDividendInformation[0].wpaid_approved == "Y")
                        {
                            jsonResponse.IsSuccess = false;
                            jsonResponse.Message = "Dividend already paid and Approved";
                        }
                        else if (demateDividendInformation[0].WPaid == true && demateDividendInformation[0].wpaid_approved == "N")
                        {
                            if (action == "A")
                            {
                                jsonResponse.IsSuccess = false;
                                jsonResponse.Message = "Dividend already paid but Unposted";
                            }
                            else
                            {
                                jsonResponse.IsSuccess = true;
                                jsonResponse.ResponseData = demateDividendInformation[0];
                            }

                        }
                        else if (demateDividendInformation[0].WPaid == false || demateDividendInformation[0].WPaid==null)
                        {
                            if (action == "A")
                            {
                                jsonResponse.IsSuccess = true;
                                jsonResponse.ResponseData = demateDividendInformation[0];

                            }
                            else
                            {
                                if (action == "A")
                                {
                                    jsonResponse.IsSuccess = true;
                                    jsonResponse.ResponseData = demateDividendInformation[0];
                                }
                                else
                                {
                                    jsonResponse.IsSuccess = false;
                                    jsonResponse.Message = ATTMessages.DIVIDEND.NOT_ISSUED;

                                }
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
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_TableName", tablename.Trim());
                    param.Add("@P_CenterId", centerid.Trim());

                    int? maxSeqNo = connection.Query<int?>(sql: "GET_MAX_SEQ_NO", param: param, commandType: CommandType.StoredProcedure).FirstOrDefault();
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

                    jsonResponse.ResponseData = ex;
                    jsonResponse.IsSuccess = false;
                    jsonResponse.HasError = true;

                }
                return jsonResponse;
            }
        }

        public JsonResponse SaveDemateDividendPaymentEntry(string DivCode, string CompCode, string bankName, string accountNo, string centerid, string Payment, string PayUser, string remarks, string warrantNo, string boidno, string selectedAction,string creditedDt, string wissueddate, string username,string ipaddress)
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
                            jsonResponse = GetMaxDemateSeqno(tableName, centerid);
                            if(jsonResponse.IsSuccess)maxSeqno = Convert.ToInt32(jsonResponse.ResponseData);

                            param = new DynamicParameters();
                            param.Add("@compcode", CompCode);
                            param.Add("@Bo_idno", boidno);
                            param.Add("@warrantNo", warrantNo);
                            param.Add("@bankName", bankName);
                            param.Add("@accountNo", accountNo);
                            param.Add("@TableName2", tableName);
                            param.Add("@centerid", centerid);
                            param.Add("@maxSeqno", maxSeqno);
                            param.Add("@username", username);

                            param.Add("@wissueddate", wissueddate);
                            param.Add("@creditedDt", creditedDt);

                            param.Add("@Payment", Payment);
                            param.Add("@PayUser", PayUser);
                            param.Add("@remarks", remarks);
                            param.Add("@Action", selectedAction);
                            param.Add("@ipaddress", ipaddress);
                            param.Add("@p_date_now", DateTime.Now);

                            jsonResponse2 = connection.Query<JsonResponse>(sql: "SAVE_DEMATE_DIVIDENT_PAYMENT_ENTRY", param: param, trans, commandType: CommandType.StoredProcedure).FirstOrDefault();

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
