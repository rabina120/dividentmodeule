using Dapper;
using Entity.Common;
using Entity.Dividend;
using ENTITY.Dividend;
using Interface.Reports;
using Microsoft.Extensions.Options;
using Repository.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Reports

{
    public class CashDividendReport : ICashDividendReport
    {
        IOptions<ReadConfig> _connectionString;
        public CashDividendReport(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }
        public JsonResponse GenerateDataForReport(string CompCode, string DivCode, string SelectedReportType, string undoType, string seqNoFrom, string seqNoTo, string KittaFrom, string KittaTo, string DateFrom, string DateTo, string PaymentType, string Posted, string PaymentCenter, string BatchNo, bool WithBankDetails, string ShareType, int? Occupation, string ExportFileType, string UserName, string IPAddress, string SelectedReportName)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_DivCode", DivCode);
                    param.Add("@P_COMPCODE", CompCode);
                    param.Add("@TableName1", ShareType == "P" ? 1 : 2);

                    string tableName = new TableReporsitory(_connectionString).GetTableName(param);
                    if (tableName != null)
                    {

                        if (SelectedReportType == "SRIP")
                        {
                            DynamicParameters dynamicParametersForSummary = new DynamicParameters();


                            dynamicParametersForSummary.Add("@P_TABLENAME", tableName);
                            dynamicParametersForSummary.Add("@P_SHARETYPE", ShareType);
                            ATTSummaryReportDividend aTTSummaryReport = new ATTSummaryReportDividend();
                            aTTSummaryReport.aTTTotalDividendWarrants = connection.Query<ATTTotalDividendWarrants>(sql: "GET_TOTAL_DIVIDEND_WARRANTS_SUMMARY", param: dynamicParametersForSummary, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                            aTTSummaryReport.aTTIssuedDividendWarrants = connection.Query<ATTIssuedDividendWarrants>(sql: "GET_ISSUED_POSTED_AND_UNPOSTED_DIVIDEND_WARRANTS_SUMMARY", param: dynamicParametersForSummary, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                            aTTSummaryReport.aTTIssuedPostedDividendWarrants = connection.Query<ATTIssuedPostedDividendWarrants>(sql: "GET_ISSUED_POSTED_DIVIDEND_WARRANTS_SUMMARY", param: dynamicParametersForSummary, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                            aTTSummaryReport.aTTIssuedUnpostedDividendWarrants = connection.Query<ATTIssuedUnpostedDividendWarrants>(sql: "GET_ISSUED_UNPOSTED_DIVIDEND_WARRANTS_SUMMARY", param: dynamicParametersForSummary, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                            aTTSummaryReport.aTTUnIssuedDividendWarrants = connection.Query<ATTUnIssuedDividendWarrants>(sql: "GET_UNISSUED_DIVIDEND_WARRANTS_SUMMARY", param: dynamicParametersForSummary, commandType: CommandType.StoredProcedure)?.FirstOrDefault();

                            aTTSummaryReport.aTTPaidPostedDividendWarrants = connection.Query<ATTPaidPostedDividendWarrents>(sql: "GET_PAID_POSTED_DIVIDEND_WARRANTS_SUMMARY", param: dynamicParametersForSummary, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                            aTTSummaryReport.aTTPaidUnpostedDividendWarrants = connection.Query<ATTPaidUnpostedDividendWarrents>(sql: "GET_PAID_UNPOSTED_DIVIDEND_WARRANTS_SUMMARY", param: dynamicParametersForSummary, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                            aTTSummaryReport.aTTUnpaidDividendWarrants = connection.Query<ATTUnpaidDividendWarrents>(sql: "GET_UNPAID_DIVIDEND_WARRANTS_SUMMARY", param: dynamicParametersForSummary, commandType: CommandType.StoredProcedure)?.FirstOrDefault();

                            response.IsSuccess = true;
                            response.ResponseData = aTTSummaryReport;
                        }
                        else
                        {
                            if (ExportFileType == "E")
                            {
                                SqlCommand cmd = new SqlCommand("GET_CASH_DIVIDEND_DATA_FOR_REPORTSCBNEW", connection);
                                cmd.CommandType = CommandType.StoredProcedure;
                                SqlParameter excelParam = new SqlParameter();
                                excelParam = cmd.Parameters.AddWithValue("@P_TableName", tableName);
                                excelParam = cmd.Parameters.AddWithValue("@P_CompCode", CompCode);
                                excelParam = cmd.Parameters.AddWithValue("@P_DivCode", DivCode);
                                excelParam = cmd.Parameters.AddWithValue("@P_SelectedReportType", SelectedReportType);
                                excelParam = cmd.Parameters.AddWithValue("@P_UndoType", undoType);
                                excelParam = cmd.Parameters.AddWithValue("@P_SeqNoFrom", seqNoFrom);
                                excelParam = cmd.Parameters.AddWithValue("@P_SeqNoTo", seqNoTo);
                                excelParam = cmd.Parameters.AddWithValue("@P_KittaFrom", KittaFrom);
                                excelParam = cmd.Parameters.AddWithValue("@P_KittaTo", KittaTo);
                                excelParam = cmd.Parameters.AddWithValue("@P_DateFrom", DateFrom);
                                excelParam = cmd.Parameters.AddWithValue("@P_DateTo", DateTo);
                                excelParam = cmd.Parameters.AddWithValue("@P_PaymentType", PaymentType);
                                excelParam = cmd.Parameters.AddWithValue("@P_Posted", Posted);
                                excelParam = cmd.Parameters.AddWithValue("@P_PaymentCenter", PaymentCenter);
                                excelParam = cmd.Parameters.AddWithValue("@P_BatchNo", BatchNo);
                                excelParam = cmd.Parameters.AddWithValue("@P_ShareType", ShareType);
                                excelParam = cmd.Parameters.AddWithValue("@P_WithBankDetails", WithBankDetails ? '1' : '0');
                                excelParam = cmd.Parameters.AddWithValue("@P_USERNAME", UserName);
                                excelParam = cmd.Parameters.AddWithValue("@P_IP_ADDRESS", IPAddress);
                                excelParam = cmd.Parameters.AddWithValue("@P_ENTRY_DATE", DateTime.Now);
                                excelParam = cmd.Parameters.AddWithValue("@P_SELECTED_REPORT_NAME", SelectedReportName);
                                excelParam = cmd.Parameters.AddWithValue("@P_occupation", Occupation);

                                excelParam.Direction = ParameterDirection.Input;
                                DataSet ds = new DataSet("Data");
                                SqlDataAdapter da = new SqlDataAdapter();
                                da.SelectCommand = cmd;

                                da.Fill(ds);

                                if (ds.Tables[0].Rows.Count == 0)
                                {
                                    response.IsSuccess = false;
                                    response.Message = ATTMessages.NO_RECORDS_FOUND;
                                }
                                else
                                {
                                    response.IsSuccess = true;
                                    response.ResponseData = ds;
                                }

                            }
                            else
                            {
                                DynamicParameters reportParameters = new DynamicParameters();
                                reportParameters.Add("@P_TableName", tableName);
                                reportParameters.Add("@P_CompCode", CompCode);
                                reportParameters.Add("@P_DivCode", DivCode);
                                reportParameters.Add("@P_SelectedReportType", SelectedReportType);
                                reportParameters.Add("@P_UndoType", undoType);
                                reportParameters.Add("@P_SeqNoFrom", seqNoFrom);
                                reportParameters.Add("@P_SeqNoTo", seqNoTo);
                                reportParameters.Add("@P_KittaFrom", KittaFrom);
                                reportParameters.Add("@P_KittaTo", KittaTo);
                                reportParameters.Add("@P_DateFrom", DateFrom);
                                reportParameters.Add("@P_DateTo", DateTo);
                                reportParameters.Add("@P_PaymentType", PaymentType);
                                reportParameters.Add("@P_Posted", Posted.ToUpper());
                                reportParameters.Add("@P_PaymentCenter", PaymentCenter);
                                reportParameters.Add("@P_BatchNo", BatchNo);
                                reportParameters.Add("@P_ShareType", ShareType);
                                reportParameters.Add("@P_WithBankDetails", WithBankDetails ? '1' : '0');
                                reportParameters.Add("@P_USERNAME", UserName);
                                reportParameters.Add("@P_IP_ADDRESS", IPAddress);
                                reportParameters.Add("@P_ENTRY_DATE", DateTime.Now);
                                reportParameters.Add("@P_SELECTED_REPORT_NAME", SelectedReportName);
                                reportParameters.Add("@P_occupation", Occupation);

                                List<dynamic> cashDividendInformation = connection.Query<dynamic>(sql: "GET_CASH_DIVIDEND_DATA_FOR_REPORTSCBNEW",
                                        param: reportParameters, null, commandType: CommandType.StoredProcedure)?.ToList();
                                if (cashDividendInformation.Count > 0) { response.IsSuccess = true; response.ResponseData = cashDividendInformation; }
                                else
                                {
                                    response.Message = ATTMessages.NO_RECORDS_FOUND;
                                }
                            }

                        }
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = ATTMessages.NO_TABLES_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.HasError = true;
                    response.ResponseData = ex;
                }
                return response;
            }
        }
    }
}
