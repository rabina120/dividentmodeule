

using Dapper;
using Entity.Certificate;
using Entity.Common;
using Entity.Reports;
using Interface.Certificate;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Certificate
{
    public class ClearPSLPostingRepo : IClearPSLPosting
    {
        IOptions<ReadConfig> _connectionString;

        public ClearPSLPostingRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }

        public JsonResponse GetClearPSLPostingCompanyData(string CompCode, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_COMPCODE", CompCode);
                    param.Add("@P_USERNAME", UserName);
                    param.Add("@P_IP_ADDRESS ", IP);
                    param.Add("@P_DATE_NOW", DateTime.Now);


                    List<ATTClearPSLPosting> ClearPSLPosting = connection.Query<ATTClearPSLPosting>("GET_HOLDERINFO_PSL_POSTING_CLEAR", param, commandType: CommandType.StoredProcedure)?.ToList();
                    if (ClearPSLPosting.Count > 0)
                    {
                        jsonResponse.ResponseData = ClearPSLPosting;
                        jsonResponse.Message = "Record Found";
                        jsonResponse.IsSuccess = true;
                    }



                    else
                    {
                        jsonResponse.Message = "Cannot Find Any Data";
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.Message = ex.Message;
                }
            }
            return jsonResponse;
        }
        public JsonResponse PostPSLClearPosting(List<ATTClearPSLPosting> aTTpSLClearPostings, ATTClearPSLPosting recorddetails, string SelectedAction, string UserName, string IP)
        {


            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        try
                        {
                            DataTable dt = new DataTable();


                            dt.Columns.Add("pslno");
                            dt.Columns.Add("CERTNO");


                            //aTTpSLClearPostings.ForEach(x => dt.Rows.Add(x.PSL_CLEAR_NO));
                            aTTpSLClearPostings.ForEach(x => dt.Rows.Add(x.PSL_CLEAR_NO, x.CertNo));
                            SqlCommand cmd = new SqlCommand("PSL_POSTING_CLEAR", connection);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Transaction = tran;
                            SqlParameter param = cmd.Parameters.AddWithValue("@udtPSL", dt);
                            param = cmd.Parameters.AddWithValue("@P_COMPCODE", recorddetails.compcode);
                            param = cmd.Parameters.AddWithValue("@P_USERNAME", UserName);
                            param = cmd.Parameters.AddWithValue("@P_REMARKS", recorddetails.clear_approved_remarks);
                            param = cmd.Parameters.AddWithValue("@P_POSTINGDATE", recorddetails.AppDate_Clear);
                            param = cmd.Parameters.AddWithValue("@P_SELECTEDACTION", SelectedAction);
                            param = cmd.Parameters.AddWithValue("@P_IP_ADDRESS", IP);
                            param = cmd.Parameters.AddWithValue("@P_DATE_NOW", DateTime.Now);
                            param.Direction = ParameterDirection.Input;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    if (reader.GetString(0) == "1")
                                    {
                                        response.IsSuccess = true;
                                        response.Message = "PSLClear Posted Sucessfully !!!";
                                    }
                                    else
                                    {
                                        response.Message = "Unposted!!!";
                                    }

                                }
                            }
                            if (response.IsSuccess)
                                tran.Commit();
                            else
                                tran.Rollback();
                        }
                        catch (Exception ex)
                        {
                            response.Message = ex.Message;
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                }
            }
            return response;
        }
        public JsonResponse GetSingleClearPSLData(string CompCode, string PSL_CLEAR_NO, int ShholderNo)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        try
                        {
                            DynamicParameters param = new DynamicParameters();
                            param.Add("@P_COMPCODE", CompCode);
                            param.Add("@P_PSLCLEARNO", PSL_CLEAR_NO);
                            param.Add("@P_SHHOLDERNO", ShholderNo);


                            List<ATTClearPSLPosting> aTTpSLClearPostings = connection.Query<ATTClearPSLPosting>("GET_CERTDETAILS_PSLPOSTING_CLEAR", param, tran, commandType: CommandType.StoredProcedure).ToList();

                            if (aTTpSLClearPostings.Count > 0)
                            {
                                jsonResponse.IsSuccess = true;
                                jsonResponse.ResponseData = aTTpSLClearPostings;
                            }
                            else
                            {
                                jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                            }

                        }
                        catch (Exception ex)
                        {
                            jsonResponse.Message = ex.Message;
                            tran.Rollback();
                        }
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.Message = ex.Message;
                }
            }
            return jsonResponse;
        }

        public JsonResponse ViewReport(string CompCode, string ReportType, string UserName, string IP)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {

                    connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_COMPCODE", CompCode);
                    param.Add("@P_REPORTTYPE", ReportType);
                    param.Add("@P_USERNAME", UserName);
                    param.Add("@P_IP_ADDRESS", IP);
                    param.Add("@P_ENTRY_DATE", DateTime.Now);
                    List<ATTClearPSLViewReport> pslReport = connection.Query<ATTClearPSLViewReport>(sql: "GET_PSL_POSTING_REPORT", param: param, commandType: CommandType.StoredProcedure)?.ToList();

                    if (pslReport.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = pslReport;

                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = "Cannot Find Any Data !!!";
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Message = ex.Message;
                }
            }
            return response;


        }


    }
}






