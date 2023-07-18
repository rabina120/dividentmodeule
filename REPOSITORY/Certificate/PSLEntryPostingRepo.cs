



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
    public class PSLEntryPostingRepo : IPSLEntryPosting

    {
        IOptions<ReadConfig> _connectionString;

        public PSLEntryPostingRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }

        public JsonResponse GetCertificate(string CompCode, string PSLNO, string ShholderNo)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_COMPCODE", CompCode);
                    param.Add("@P_PSLNO", PSLNO);
                    param.Add("@P_SHHOLDERNO", ShholderNo);


                    //param.Add("@TransType", "02");
                    List<ATTPSLEntryPosting> certificate = connection.Query<ATTPSLEntryPosting>("GET_CERTDETAILS_PSLPOSTING", param, commandType: CommandType.StoredProcedure)?.ToList();
                    if (certificate.Count > 0)
                    {
                        jsonResponse.ResponseData = certificate;
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

        public JsonResponse GetPSLEntryCompanyData(string CompCode, string UserName, string IP)
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

                    List<ATTPSLEntryPosting> PSLEntry = connection.Query<ATTPSLEntryPosting>("GET_HOLDERINFO_PSL_POSTING", param, commandType: CommandType.StoredProcedure)?.ToList();
                    if (PSLEntry.Count > 0)
                    {
                        jsonResponse.ResponseData = PSLEntry;
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

        public JsonResponse PostPSLEntryPosting(List<ATTPSLEntryPosting> aTTpSLEntryPostings, ATTPSLEntryPosting recorddetails, string SelectedAction, string UserName, string IP)
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

                            dt.Columns.Add("PSLNo");
                            dt.Columns.Add("CertNo");
                            //dt.Columns.Add("ShholderNo");
                            //dt.Columns.Add("FName() +' '+LName()");
                            //dt.Columns.Add("Status");
                            //dt.Columns.Add("EntryUser");
                            //dt.Columns.Add("EntryDate");
                            //dt.Columns.Add("Totalkitta");
                            //dt.Columns.Add("TransDate");
                            //dt.Columns.Add("Remark");

                            aTTpSLEntryPostings.ForEach(x => dt.Rows.Add(x.PSLNo, x.CertNo));
                            SqlCommand cmd = new SqlCommand("PSL_POSTING", connection);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Transaction = tran;
                            SqlParameter param = cmd.Parameters.AddWithValue("@udt", dt);
                            param = cmd.Parameters.AddWithValue("@P_COMPCODE", recorddetails.compcode);
                            param = cmd.Parameters.AddWithValue("@P_USERNAME", UserName);
                            param = cmd.Parameters.AddWithValue("@P_REMARKS", recorddetails.psl_approved_remarks);
                            param = cmd.Parameters.AddWithValue("@P_POSTINGDATE", recorddetails.AppDate_PSL);
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
                                        response.Message = "SUCCESSFULLY !!!";
                                    }
                                    else
                                    {
                                        response.Message = "UNPOSTED!!!";
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
                    List<ATTPSLViewReport> pslReport = connection.Query<ATTPSLViewReport>(sql: "GET_PSL_POSTING_REPORT", param: param, commandType: CommandType.StoredProcedure)?.ToList();

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
