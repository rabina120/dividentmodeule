using Dapper;
using Entity.CDS;
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
    public class DemateRemateReportRepo : IDemateRemateReport
    {
        IOptions<ReadConfig> _connectionString;
        public DemateRemateReportRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }

        public JsonResponse GenerateReportExcel(ATTReportTypeForDemateRemate aTTReportType, string ExportReportType, string Username)
        {
            JsonResponse jsonResponse = new JsonResponse();

            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("GENERATE_DATA_FOR_DEMATE_REMATE_REPORT", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    connection.Open();

                    param = cmd.Parameters.AddWithValue("@CompCode", aTTReportType.CompCode);
                    param = cmd.Parameters.AddWithValue("@ReportType", aTTReportType.ReportType);
                    param = cmd.Parameters.AddWithValue("@DataType", aTTReportType.DataType);
                    param = cmd.Parameters.AddWithValue("@OrderBy", aTTReportType.OrderBy);
                    param = cmd.Parameters.AddWithValue("@SecondaryReportType", aTTReportType.SecondaryReportType);
                    param = cmd.Parameters.AddWithValue("@ISINNo", aTTReportType.ISINNo);
                    param = cmd.Parameters.AddWithValue("@DP", aTTReportType.DP);
                    param = cmd.Parameters.AddWithValue("@DemateType", aTTReportType.DemateType);
                    param = cmd.Parameters.AddWithValue("@CertDetails", aTTReportType.CertDetails);
                    param = cmd.Parameters.AddWithValue("@DemateReqFrom", aTTReportType.DemateReqFrom);
                    param = cmd.Parameters.AddWithValue("@DemateReqTo", aTTReportType.DemateReqTo);
                    param = cmd.Parameters.AddWithValue("@EntryDateFrom", aTTReportType.EntryDateFrom);
                    param = cmd.Parameters.AddWithValue("@EntryDateTo", aTTReportType.EntryDateTo);
                    param = cmd.Parameters.AddWithValue("@RegNoFrom", aTTReportType.RegNoFrom);
                    param = cmd.Parameters.AddWithValue("@RegNoTo", aTTReportType.RegNoTo);
                    param = cmd.Parameters.AddWithValue("@HolderNoFrom", aTTReportType.HolderNoFrom);
                    param = cmd.Parameters.AddWithValue("@HolderNoTo", aTTReportType.HolderNoTo);
                    param = cmd.Parameters.AddWithValue("@CertNoFrom", aTTReportType.CertNoFrom);
                    param = cmd.Parameters.AddWithValue("@CertNoTo", aTTReportType.CertNoTo);
                    param = cmd.Parameters.AddWithValue("@UserName", Username);

                    param.Direction = ParameterDirection.Input;
                    DataSet ds = new DataSet("Data");
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;

                    da.Fill(ds);

                    
                    if (ds.Tables.Count == 0 )
                    {
                        jsonResponse.IsSuccess = false;
                        jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                    }
                    else
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = ds;
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

        public JsonResponse GenerateReport(ATTReportTypeForDemateRemate aTTReportType, string ExportReportType, string Username)
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
                        param.Add("@CompCode", aTTReportType.CompCode);
                        param.Add("@ReportType", aTTReportType.ReportType);
                        param.Add("@DataType", aTTReportType.DataType);
                        param.Add("@OrderBy", aTTReportType.OrderBy);
                        param.Add("@SecondaryReportType", aTTReportType.SecondaryReportType);
                        param.Add("@ISINNo", aTTReportType.ISINNo);
                        param.Add("@DP", aTTReportType.DP);
                        param.Add("@DemateType", aTTReportType.DemateType);
                        param.Add("@CertDetails", aTTReportType.CertDetails);
                        param.Add("@DemateReqFrom", aTTReportType.DemateReqFrom);
                        param.Add("@DemateReqTo", aTTReportType.DemateReqTo);
                        param.Add("@EntryDateFrom", aTTReportType.EntryDateFrom);
                        param.Add("@EntryDateTo", aTTReportType.EntryDateTo);
                        param.Add("@RegNoFrom", aTTReportType.RegNoFrom);
                        param.Add("@RegNoTo", aTTReportType.RegNoTo);
                        param.Add("@HolderNoFrom", aTTReportType.HolderNoFrom);
                        param.Add("@HolderNoTo", aTTReportType.HolderNoTo);
                        param.Add("@CertNoFrom", aTTReportType.CertNoFrom);
                        param.Add("@CertNoTo", aTTReportType.CertNoTo);
                        param.Add("@UserName", Username);

                        List<dynamic> reportData = connection.Query<dynamic>("GENERATE_DATA_FOR_DEMATE_REMATE_REPORT", param, transaction, commandType: CommandType.StoredProcedure).ToList();
                        
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
                    jsonResponse.Message = ex.Message;
                }
                return jsonResponse;
            }
        }

        public JsonResponse GetAllParaCompChild(string CompCode)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@CompCode", CompCode);

                    List<ParaComp_Child> paraComps = connection.Query<ParaComp_Child>("GET_PARACOMP_CHILD", param, commandType: CommandType.StoredProcedure)?.ToList();
                    if (paraComps.Count > 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = paraComps;
                    }
                    else
                    {
                        jsonResponse.Message = "ISINO For Company Not Found";
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

        public JsonResponse GetDataFromCertificateDetail(string CompCode)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);

                    List<ATTCertificateDetail> certDetailsToReturn = connection.Query<ATTCertificateDetail>(sql: "GET_DATA_FROM_CERTIFICATE_DETAIL", param: param, commandType: CommandType.StoredProcedure)?.ToList();
                    if (certDetailsToReturn.Count > 0)
                    {
                        jsonResponse.ResponseData = certDetailsToReturn;
                        jsonResponse.IsSuccess = true;
                    }
                    else
                    {
                        jsonResponse.IsSuccess = false;
                        jsonResponse.Message = "No Certificate Detail List For Company Found !!!<br/> Are You Sure You Are Choosing the Right Company? <br/>";
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
