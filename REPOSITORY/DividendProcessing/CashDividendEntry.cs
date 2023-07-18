
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
using System.Globalization;
using System.Linq;

namespace Repository.DividendProcessing
{
    public class CashDividendEntry : ICashDividendEntry
    {
        IOptions<ReadConfig> _connectionString;

        public CashDividendEntry(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }

        public JsonResponse BulkIssue(string CompCode, string DivCode, string DivType, string IssueDate, bool isIssue, bool isPay, string IssueRemarks, DataTable dataTable, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction trans = connection.BeginTransaction())
                    {

                        DataTable tableToSend = new DataTable();
                        tableToSend.Columns.Add("shholderno", type: typeof(String));
                        tableToSend.Columns.Add("warrantno", type: typeof(String));
                        tableToSend.Columns.Add("bankname", type: typeof(String));
                        tableToSend.Columns.Add("bankadd", type: typeof(String));
                        tableToSend.Columns.Add("bankaccno", type: typeof(String));
                        tableToSend.Columns.Add("crediteddt", type: typeof(DateTime));
                        CultureInfo datetimeprovider = CultureInfo.InvariantCulture;
                        foreach (DataRow row in dataTable.Rows)
                        {
                            var id = DivType == "01" ? row["shholderno"] : row["boid"];
                            tableToSend.Rows.Add(id == null ? "" : id.ToString(),
                                row["warrantno"].ToString(), row["bankname"].ToString(), row["bankadd"] == null ? "" : row["bankadd"].ToString(), row["bankaccno"].ToString()
                                , row["crediteddt"].ToString());
                        }
                        jsonResponse = BulkCopy(connection, trans, tableToSend);
                        if (jsonResponse.IsSuccess)
                        {

                            DynamicParameters param = new DynamicParameters();
                            param.Add("@Compcode", CompCode);
                            param.Add("@DivCode", DivCode);
                            param.Add("@DivType", DivType);
                            param.Add("@IsIssue", isIssue);
                            param.Add("@IsPay", isPay);
                            param.Add("@IssueDate", IssueDate);
                            param.Add("@Issueremarks", IssueRemarks);
                            param.Add("@Username", UserName);
                            param.Add("@IPaddress", IPAddress);


                            jsonResponse = connection.Query<JsonResponse>(sql: "Dividend_BulkIssue", param: param, transaction: trans, commandType: CommandType.StoredProcedure).FirstOrDefault();
                            if (jsonResponse.IsSuccess)
                            {
                                trans.Commit();
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.HasError = true;
                    jsonResponse.Message = ex.Message;
                    jsonResponse.ResponseData = ex;
                }
            }
            return jsonResponse;
        }

        public JsonResponse GetDividendInformation(string CompCode, string DivCode, string shholderno, string a, string action, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                //ATTCashDividend cashDividendInformation;

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
                    param.Add("@P_IP_ADDRESS", IP);
                    param.Add("@P_USERNAME", UserName);
                    param.Add("@P_DATE_NOW", DateTime.Now);
                    param.Add("@P_ACTION", a == "S" ? "S" : "W");

                    List<ATTCashDividend> cashDividendInformation = SqlMapper.Query<ATTCashDividend, ATTShHolder, ATTCashDividend>(connection, sql: "GET_HOLDER_DIVIDEND_INFORMATION",
                       (cashDividendInformation, shHolder) =>
                       {
                           cashDividendInformation.attShholder = shHolder;
                           return cashDividendInformation;
                       },
                       param: param, null, splitOn: "ShholderBind", commandType: CommandType.StoredProcedure)?.ToList();

                    if (cashDividendInformation.Count > 0)
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
                            if (cashDividendInformation[0].WIssued == true && cashDividendInformation[0].wissue_status == "POSTED" && cashDividendInformation[0].Wissue_Approved == "Y")
                            {
                                jsonResponse.IsSuccess = false;
                                jsonResponse.Message = ATTMessages.DIVIDEND.ISSUED_POSTED_APPROVED;
                            }
                            else if (cashDividendInformation[0].WIssued == true && cashDividendInformation[0].Wissue_Approved == "N")
                            {
                                if (action == "A")
                                {
                                    jsonResponse.IsSuccess = false;
                                    jsonResponse.Message = ATTMessages.DIVIDEND.ISSUED_UNPOSTED_UNAPPROVED;
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
                    maxSeqNo++;
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

        public JsonResponse SaveCashDividend(string DivCode, string CompCode, string centerid, string bankName, string accountNo, string remarks, string telno, string cashOrCheque, string UserName, string warrantNo, string shholderno, string selectedAction, string creditedDt, string wissueddate, string IsPaidBy, string IP)
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
                        param.Add("@TableName1", "1");
                        string tableName = new TableReporsitory(_connectionString).GetTableName(param);
                        if (tableName != null)
                        {
                            jsonResponse = GetMaxSeqno(tableName, centerid);
                            maxSeqno = Convert.ToInt32(jsonResponse.ResponseData);
                            param = new DynamicParameters();
                            param.Add("@compcode", CompCode);
                            param.Add("@warrantNo", warrantNo);
                            param.Add("@shholderno", shholderno);
                            param.Add("@TableName1", tableName);
                            param.Add("@cashOrCheque", cashOrCheque);
                            param.Add("@centerid", centerid);
                            param.Add("@maxSeqno", maxSeqno);
                            param.Add("@username", UserName);
                            param.Add("@creditedDt", creditedDt);
                            param.Add("@wissueddate", wissueddate);
                            param.Add("@bankName", bankName);
                            param.Add("@accountNo", accountNo);
                            param.Add("@telno", telno = null ?? "00");
                            param.Add("@remarks", remarks);
                            param.Add("@Action", selectedAction);
                            param.Add("@IsPaidBy", IsPaidBy);
                            param.Add("@P_IP_ADDRESS", IP);
                            param.Add("@P_DATE_NOW", DateTime.Now);
                            jsonResponse2 = connection.Query<JsonResponse>("SAVE_CASH_DIVIDEND_ISSUE_ENTRY", param: param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                            if (jsonResponse.IsSuccess)
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

        public JsonResponse BulkCopy(SqlConnection conn, SqlTransaction transaction, DataTable dataTable)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity, transaction))
                {

                    sqlBulkCopy.DestinationTableName = "dbo.Temp_BulkIssueUpload";
                    sqlBulkCopy.ColumnMappings.Add("shholderno", "shholderno");
                    sqlBulkCopy.ColumnMappings.Add("warrantno", "warrantno");
                    sqlBulkCopy.ColumnMappings.Add("bankname", "bankname");
                    sqlBulkCopy.ColumnMappings.Add("bankadd", "bankadd");
                    sqlBulkCopy.ColumnMappings.Add("bankaccno", "bankaccno");
                    sqlBulkCopy.ColumnMappings.Add("crediteddt", "crediteddt");
                    sqlBulkCopy.WriteToServer(dataTable);
                    jsonResponse.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                jsonResponse.Message = ex.Message;
            }

            return jsonResponse;

        }
    }
}
