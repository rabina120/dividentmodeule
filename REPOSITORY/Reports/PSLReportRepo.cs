using Dapper;
using Entity.Certificate;
using Entity.Common;
using Entity.Reports;
using Interface.Reports;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;


namespace Repository.Reports
{
    public class PSLReportRepo : IPSLReport
    {
        IOptions<ReadConfig> connectionstring;
        public PSLReportRepo(IOptions<ReadConfig> _connectionstring)
        {
            this.connectionstring = _connectionstring;
        }

        public JsonResponse GenerateReport(ATTPSLReport aTTPSLReport, string Username)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionstring.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_COMPCODE", aTTPSLReport.CompCode);
                    param.Add("@P_PCODE", aTTPSLReport.PCode);
                    param.Add("@P_TRANTYPE", aTTPSLReport.TranType);
                    param.Add("@P_DATEFROM", aTTPSLReport.PSLDateFrom);
                    param.Add("@P_DATETO", aTTPSLReport.PSLDateTo);
                    param.Add("@P_HOLDERNOFROM", aTTPSLReport.HolderNoFrom);
                    param.Add("@P_HOLDERNOTO", aTTPSLReport.HolderNoTo);
                    param.Add("@P_CERTNOFROM", aTTPSLReport.CertNoFrom);
                    param.Add("@P_CERTNOTO", aTTPSLReport.CertNoTo);
                    param.Add("@P_APPSTATUS", aTTPSLReport.DataType);
                    param.Add("@P_ORDERBY", aTTPSLReport.OrderBy);
                    param.Add("@P_SHHOWNERTYPE", aTTPSLReport.ShareType);
                    param.Add("@P_USERNAME", aTTPSLReport.UserName);
                    param.Add("@P_REPORTTYPE", aTTPSLReport.ReportType);
                    List<ATTPSLReport> certificateDetails = new List<ATTPSLReport>();
                    if (aTTPSLReport.ReportType == "U")
                    {
                        certificateDetails = SqlMapper.QueryAsync<ATTPSLReport, ATTCERTIFICATE, ATTPSLReport>(connection, "GET_PSL_REPORT",
                            (holder, certificate) =>
                            {
                                holder.Certificate = certificate;
                                return holder;

                            }, param, null, commandType: CommandType.StoredProcedure)?.Result.AsList();
                        if (certificateDetails.Count > 0)
                        {




                        }
                    }
                }
                catch (Exception ex)
                {




                }

            }

            return response;


        }

        //public JsonResponse GenerateReport(ATTPSLReport ReportData, string ExportReportType)
        //{
        //    throw new NotImplementedException();
        //}

        public JsonResponse GenerateReport(string CompCode, string PCode, string TranType, string PSLDateFrom, string PSLDateTo, string HolderNoFrom, string HolderNoTo, string CertNoFrom, string CertNoTo, string AppStatus, string OrderBy, string ShareType, string ReportType, string UserName, string IpAddress, string EntryDateTime)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionstring.Value.DefaultConnection)))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("GET_PSL_REPORT", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    connection.Open();

                    param = cmd.Parameters.AddWithValue("@P_COMPCODE", CompCode);
                    param = cmd.Parameters.AddWithValue("@P_PCODE", PCode);
                    param = cmd.Parameters.AddWithValue("@P_TRANTYPE", TranType);
                    param = cmd.Parameters.AddWithValue("@P_DATEFROM", PSLDateFrom);
                    param = cmd.Parameters.AddWithValue("@P_DATETO", PSLDateTo);
                    param = cmd.Parameters.AddWithValue("@P_HOLDERNOFROM", HolderNoFrom);
                    param = cmd.Parameters.AddWithValue("@P_HOLDERNOTO", HolderNoTo);
                    param = cmd.Parameters.AddWithValue("@P_CERTNOFROM", CertNoFrom);
                    param = cmd.Parameters.AddWithValue("@P_CERTNOTO", CertNoTo);
                    param = cmd.Parameters.AddWithValue("@P_APPSTATUS", AppStatus);
                    param = cmd.Parameters.AddWithValue("@P_ORDERBY", OrderBy);
                    param = cmd.Parameters.AddWithValue("@P_SHHOWNERTYPE", ShareType);
                    param = cmd.Parameters.AddWithValue("@P_USERNAME", UserName);
                    param = cmd.Parameters.AddWithValue("@P_REPORTTYPE", ReportType);
                    param = cmd.Parameters.AddWithValue("@P_IP_ADDRESS", IpAddress);
                    param = cmd.Parameters.AddWithValue("@P_ENTRY_DATE", Convert.ToDateTime(EntryDateTime));


                    param.Direction = ParameterDirection.Input;
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
                catch (Exception ex)
                {
                    response.HasError = true;
                    response.Message = ex.Message;
                    response.ResponseData = ex;

                }

            }

            return response;
        }

        public JsonResponse GenerateReportPdf(string CompCode, string PCode, string TranType, string PSLDateFrom, string PSLDateTo, string HolderNoFrom, string HolderNoTo, string CertNoFrom, string CertNoTo, string AppStatus, string OrderBy, string ShareType, string ReportType, string UserName, string IpAddress, string EntryDateTime)
        {
            JsonResponse jsonResponse = new JsonResponse();



            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionstring.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@P_COMPCODE", CompCode);
                        param.Add("@P_PCODE", PCode);
                        param.Add("@P_TRANTYPE", TranType);
                        param.Add("@P_DATEFROM", PSLDateFrom);
                        param.Add("@P_DATETO", PSLDateTo);
                        param.Add("@P_HOLDERNOFROM", HolderNoFrom);
                        param.Add("@P_HOLDERNOTO", HolderNoTo);
                        param.Add("@P_CERTNOFROM", CertNoFrom);
                        param.Add("@P_CERTNOTO", CertNoTo);
                        param.Add("@P_APPSTATUS", AppStatus);
                        param.Add("@P_ORDERBY", OrderBy);
                        param.Add("@P_SHHOWNERTYPE", ShareType);
                        param.Add("@P_USERNAME", UserName);
                        param.Add("@P_REPORTTYPE", ReportType);
                        param.Add("@P_IP_ADDRESS", IpAddress);
                        param.Add("@P_ENTRY_DATE", Convert.ToDateTime(EntryDateTime));

                        List<dynamic> reportData = connection.Query<dynamic>("GET_PSL_REPORT", param, transaction, commandType: CommandType.StoredProcedure).ToList();
                        if (reportData.Count > 0)
                        {
                            jsonResponse.IsSuccess = true;
                            jsonResponse.ResponseData = reportData;
                            transaction.Commit();
                        }
                        else
                        {
                            jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                            transaction.Rollback();
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
        public JsonResponse GetAllPledgeAt()
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionstring.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    //DynamicParameters param = new DynamicParameters();
                    List<ATTPSLEntry> aTTPaymentCenter = connection.Query<ATTPSLEntry>("GET_ALL_PLEDGE_CENTER", param: null, commandType: null).ToList();

                    if (aTTPaymentCenter.Count > 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = aTTPaymentCenter;
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.ResponseData = ex.Message;

                }
                return jsonResponse;
            }
        }
    }
}
