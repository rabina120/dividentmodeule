
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

    public class DividentPaymentPosting : IDividentPaymentEntryPosting
    {
        IOptions<ReadConfig> connectionString;
        public DividentPaymentPosting(IOptions<ReadConfig> _connectionString)
        {
            this.connectionString = _connectionString;
        }

        public JsonResponse GetDividentPaymentForApproval(string CompCode, string FromDate, string ToDate, string Divcode, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();

            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_DivCode", Divcode.Trim());
                    param.Add("@P_COMPCODE", CompCode.Trim());
                    param.Add("@TableName1", "1");
                    string tableName = new TableReporsitory(connectionString).GetTableName(param);
                    //ATTDividend aTTDividend = connection.Query<ATTDividend>(sql: "Select * from DivTable where compcode ='" + CompCode + "' and divcode='" + Divcode + "' and Divcalculated=1", null, commandType: null)?.FirstOrDefault();
                    if (tableName != null)
                    {
                        param = new DynamicParameters();
                        param.Add("@CompCode", CompCode.Trim());
                        param.Add("@P_FromDate", FromDate);
                        param.Add("@P_ToDate", ToDate);
                        param.Add("@TableName1", tableName.Trim());
                        param.Add("@P_IP_ADDRESS", IP);
                        param.Add("@P_USERNAME", UserName);
                        param.Add("@P_DATE_NOW", DateTime.Now);

                        List<ATTDividentPaymentEntry> aTTDividentPaymentEntry = SqlMapper.Query<ATTDividentPaymentEntry, ATTShHolder, ATTDividentPaymentEntry>(connection, sql: "Get_Cash_Dividend_ForApproval",
                                (attCashDividend, attShHolder) =>
                                {
                                    attCashDividend.attShholder = attShHolder;
                                    return attCashDividend;
                                },
                                param: param, null, splitOn: "SpHolder", commandType: CommandType.StoredProcedure)?.ToList();

                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = aTTDividentPaymentEntry;
                        if (aTTDividentPaymentEntry.Count > 0)
                        {
                            jsonResponse.Message = tableName;
                        }
                        else
                        {
                            jsonResponse.Message = "Record Not Found";
                        }
                    }
                    else
                        jsonResponse.Message = "Table could not be found";
                }
                catch (Exception ex)
                {
                    jsonResponse.ResponseData = ex;
                    jsonResponse.IsSuccess = false;
                    jsonResponse.HasError = true;
                }
            }
            return jsonResponse;
        }

        public JsonResponse PostDividentPaymentRequest(List<ATTDividentPaymentEntry> aTTDividentPaymentEntrys, ATTDividentPaymentEntry RecordDetails, string ActionType, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();

            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    using (SqlTransaction trans = connection.BeginTransaction())
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@P_DivCode", RecordDetails.DivCode.Trim());
                        param.Add("@P_COMPCODE", RecordDetails.compcode.Trim());
                        param.Add("@TableName1", "1");
                        string tableName = new TableReporsitory(connectionString).GetTableName(param);

                        if (tableName != null)
                        {
                            DataTable dt = new DataTable();

                            dt.Columns.Add("shholderNo");
                            dt.Columns.Add("WarrantNo");

                            aTTDividentPaymentEntrys.ForEach(x => dt.Rows.Add(x.ShHolderNo, x.WarrantNo));

                            SqlCommand cmd = new SqlCommand("Cash_Dividend_Payment_Posting", connection);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Transaction = trans;

                            SqlParameter parameters = cmd.Parameters.AddWithValue("@UDT", dt);
                            parameters = cmd.Parameters.AddWithValue("@P_COMPCODE", RecordDetails.compcode.Trim());
                            parameters = cmd.Parameters.AddWithValue("@TableName1", tableName.Trim());
                            parameters = cmd.Parameters.AddWithValue("@mdate", RecordDetails.wissue_app_date);
                            parameters = cmd.Parameters.AddWithValue("@GUserName", UserName);
                            parameters = cmd.Parameters.AddWithValue("@remarks", RecordDetails.wissue_auth_remarks);

                            parameters = cmd.Parameters.AddWithValue("@P_Action", ActionType);
                            parameters = cmd.Parameters.AddWithValue("@P_IP_ADDRESS", IP);
                            parameters = cmd.Parameters.AddWithValue("@P_USERNAME", UserName);
                            parameters = cmd.Parameters.AddWithValue("@P_DATE_NOW", DateTime.Now);
                            parameters.Direction = ParameterDirection.Input;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    if (reader.GetString(0) == "true")
                                    {
                                        jsonResponse.IsSuccess = true;
                                        string msg = ActionType == "A" ? "Approved" : "Rejected";
                                        jsonResponse.Message = "Record/s " + msg + " Successfully !!!";
                                    }
                                    else
                                    {
                                        string msg = ActionType == "A" ? "Approve" : "Reject";
                                        jsonResponse.Message = "Failed To " + msg + "Record/s !!!";
                                    }

                                }
                            }
                            if (jsonResponse.IsSuccess)
                                trans.Commit();
                            else
                                trans.Rollback();
                        }
                        else
                        {
                            jsonResponse.Message = "Failed to Get Table Name";
                        }
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.ResponseData = ex;
                    jsonResponse.IsSuccess = false;
                    jsonResponse.HasError = true;
                }
            }
            return jsonResponse;
        }
    }
}

