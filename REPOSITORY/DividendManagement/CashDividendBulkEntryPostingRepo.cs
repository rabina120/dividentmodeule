
using Dapper;
using Entity.Common;
using Entity.Dividend;

using Interface.DividendManagement;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.DividendManagement
{
    public class CashDividendBulkEntryPostingRepo : ICashDividendBulkEntryPosting
    {
        IOptions<ReadConfig> _connectionString;

        public CashDividendBulkEntryPostingRepo(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }

        public JsonResponse GenerateData(string CompCode, string BonusType, string ShareType, string DivCode, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("Get_Dividend_Data_Bulk_Entry_Posting", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter excelParam = new SqlParameter();
                    excelParam = cmd.Parameters.AddWithValue("@CompCode", CompCode);
                    excelParam = cmd.Parameters.AddWithValue("@ShareType", ShareType);
                    excelParam = cmd.Parameters.AddWithValue("@BonusType", BonusType);
                    excelParam = cmd.Parameters.AddWithValue("@DivCode", DivCode);
                    excelParam = cmd.Parameters.AddWithValue("@P_USERNAME", UserName);
                    excelParam = cmd.Parameters.AddWithValue("@P_IP_ADDRESS", IPAddress);
                    excelParam = cmd.Parameters.AddWithValue("@P_ENTRY_DATE", DateTime.Now);

                    excelParam.Direction = ParameterDirection.Input;
                    DataSet ds = new DataSet("Excel");
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;

                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        jsonResponse.IsSuccess = false;
                        jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                    }
                    else
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = ds;
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@compcode", CompCode);
                        parameters.Add("@divcode", DivCode);
                        parameters.Add("@ShareType", ShareType);
                        string tablename = connection.Query<string>(sql: "Get_Dividend_TableName_Bulk_Entry_Posting", parameters, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                        jsonResponse.ResponseData2 = tablename;
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

        public JsonResponse GetDividendList(string CompCode, string BonusType, string ShareType)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@CompCode", CompCode);
                    param.Add("@ShareType", ShareType);
                    param.Add("@BonusType", BonusType);
                    List<ATTDividend> aTTDividendTables = connection.Query<ATTDividend>(sql: "Get_Dividend_List_Bulk_Entry_Posting", param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                    if (aTTDividendTables.Count > 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = aTTDividendTables;
                    }
                    else
                    {
                        jsonResponse.IsSuccess = false;
                        jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
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
        public JsonResponse GetSelectedDividendDetails(string CompCode, string BonusType, string ShareType, string DivCode)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@CompCode", CompCode);
                    param.Add("@ShareType", ShareType);
                    param.Add("@BonusType", BonusType);
                    param.Add("@DivCode", DivCode);
                    ATTSummaryBulkEntryPosting summaryData = connection.Query<ATTSummaryBulkEntryPosting>(sql: "Get_Dividend_Summary_Bulk_Entry_Posting", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                    if (summaryData != null)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = summaryData;
                    }
                    else
                    {
                        jsonResponse.IsSuccess = false;
                        jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
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
        public JsonResponse Save(string CompCode, string BonusType, string ShareType, string DivCode, string AcceptOrReject, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@CompCode", CompCode);
                        param.Add("@ShareType", ShareType);
                        param.Add("@BonusType", BonusType);
                        param.Add("@DivCode", DivCode);
                        param.Add("@AcceptOrReject", AcceptOrReject);
                        param.Add("@UserName", UserName);
                        param.Add("@IPAddress", IPAddress);
                        param.Add("@EntryDate", DateTime.Now);
                        jsonResponse = connection.Query<JsonResponse>(sql: "Save_Dividend_Bulk_Entry_Posting", param, transaction, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                        if (jsonResponse.IsSuccess)
                            transaction.Commit();
                        else
                            transaction.Rollback();
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
    }
}

