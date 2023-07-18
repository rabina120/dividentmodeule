
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
    public class DemateDividentPaymentPosting : IDemateDividentPaymentPosting
    {
        IOptions<ReadConfig> connectionString;
        public DemateDividentPaymentPosting(IOptions<ReadConfig> _connectionString)
        {
            this.connectionString = _connectionString;
        }

        public JsonResponse GetDemateDividentForApproval(string CompCode, string FromDate, string ToDate, string Divcode, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_DivCode", Divcode);
                    param.Add("@P_COMPCODE", CompCode);
                    param.Add("@TableName1", "2");
                    string tableName = new TableReporsitory(connectionString).GetTableName(param);
                    if (tableName != null)
                    {
                        //ATTDividend aTTDividend = connection.Query<ATTDividend>(sql: "Select * from DivTable where compcode = @CompCode and divcode='" + Divcode + "' and Divcalculated=1", null, commandType: null)?.FirstOrDefault();

                        param = new DynamicParameters();
                        param.Add("@CompCode", CompCode);
                        param.Add("@P_FromDate", FromDate);
                        param.Add("@P_ToDate", ToDate);
                        param.Add("@TableName2", tableName);
                        param.Add("@P_IP_ADDRESS", IP);
                        param.Add("@P_USERNAME", UserName);
                        param.Add("@P_DATE_NOW", DateTime.Now);

                        List<ATTDemateDividentPaymentPosting> aTTDemateDividentPaymentPosting = SqlMapper.Query<ATTDemateDividentPaymentPosting>(connection, sql: "Get_DematePosting_ForApproval", param: param, null, commandType: CommandType.StoredProcedure)?.ToList();
                        if (aTTDemateDividentPaymentPosting.Count > 0)
                        {
                            jsonResponse.ResponseData = aTTDemateDividentPaymentPosting;
                            jsonResponse.IsSuccess = true;
                        }
                        else
                        {

                            jsonResponse.Message = "Record Not Found";
                        }

                    }
                    else
                    {
                        jsonResponse.Message = "Cannot Find The Table Name!!";
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

        public JsonResponse PostDemateDividentPaymentPosting(List<ATTDemateDividentPaymentPosting> attDemateDividentPaymentPosting, string PostingRemarks, string PostingDate, string ActionType, string CompCode, string Divcode, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();

            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@P_DivCode", Divcode);
                        param.Add("@P_COMPCODE", CompCode);
                        param.Add("@TableName1", "2");
                        string tableName = new TableReporsitory(connectionString).GetTableName(param);

                        DataTable dt = new DataTable();

                        dt.Columns.Add("BOIDNo");
                        dt.Columns.Add("WarrantNo");


                        attDemateDividentPaymentPosting.ForEach(x => dt.Rows.Add(x.BO_idno, x.warrantno));

                        SqlCommand cmd = new SqlCommand("CASH_DEMATE_POSTING", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Transaction = tran;

                        SqlParameter parameters = cmd.Parameters.AddWithValue("@UDT", dt);
                        parameters = cmd.Parameters.AddWithValue("@P_COMPCODE", CompCode);
                        parameters = cmd.Parameters.AddWithValue("@TableName2", tableName);
                        parameters = cmd.Parameters.AddWithValue("@mdate", PostingDate);
                        parameters = cmd.Parameters.AddWithValue("@GUserName", UserName);
                        parameters = cmd.Parameters.AddWithValue("@remarks", PostingRemarks);
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
                                    jsonResponse.Message = ATTMessages.POSTING_SUCESS;
                                }
                                else
                                {
                                    string msg = ActionType == "A" ? "Approve" : "Reject";
                                    jsonResponse.Message = ATTMessages.POSTING_FAILED;
                                }

                            }
                        }
                        if (jsonResponse.IsSuccess)
                            tran.Commit();
                        else
                            tran.Rollback();

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
