


using Dapper;
using Entity.Certificate;
using Entity.Common;
using Interface.Certificate;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Certificate
{
    public class CertificateListRepo : ICertificateList
    {
        IOptions<ReadConfig> connectionString;
        public CertificateListRepo(IOptions<ReadConfig> _connectionString)
        {
            connectionString = _connectionString;
        }
        public JsonResponse AllCertificateList(ATTDuplicateCertificate ReportData, string OrderBy, string ShareOwnerType)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new
                SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand("GET_ALLCERTIFICATE_LIST", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter excelParam = new SqlParameter();
                    excelParam = cmd.Parameters.AddWithValue("@P_COMPCODE", ReportData.CompCode);
                    excelParam = cmd.Parameters.AddWithValue("@P_HOLDERNOFROM", ReportData.HolderNoFrom == 0? (object)null : ReportData.HolderNoFrom);
                    excelParam = cmd.Parameters.AddWithValue("@P_HOLDERNOTO", ReportData.HolderNoTo == 0 ? (object)null : ReportData.HolderNoTo);
                    excelParam = cmd.Parameters.AddWithValue("@P_CERTNOFROM", ReportData.CertNoFrom == 0 ? (object)null : ReportData.CertNoFrom);
                    excelParam = cmd.Parameters.AddWithValue("@P_CERTNOTO", ReportData.CertNoTo == 0 ? (object)null : ReportData.CertNoTo );
                    excelParam = cmd.Parameters.AddWithValue("@P_SHKITTAFROM", ReportData.ShareKittaFrom);
                    excelParam = cmd.Parameters.AddWithValue("@P_SHKITTATO", ReportData.ShareKittaTo);
                    excelParam = cmd.Parameters.AddWithValue("@P_SRNOFROM", ReportData.srnoFrom == 0 ? (object)null : ReportData.srnoFrom);
                    excelParam = cmd.Parameters.AddWithValue("@P_SRNOTO", ReportData.srnoTo == 0 ? (object)null : ReportData.srnoTo);
                    excelParam = cmd.Parameters.AddWithValue("@P_ORDERBY", OrderBy);
                    excelParam = cmd.Parameters.AddWithValue("@P_SHOWNERTYPE", ShareOwnerType);
                    excelParam.Direction = ParameterDirection.Input;
                    DataSet ds = new DataSet("Data");
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

        public JsonResponse AllCertificateListForPDF(ATTDuplicateCertificate ReportData, string OrderBy, string ShareOwnerType)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new
                SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();

                    param.Add("@P_COMPCODE", ReportData.CompCode);
                    param.Add("@P_HOLDERNOFROM", ReportData.HolderNoFrom ==0? (object)null : ReportData.HolderNoFrom);
                    param.Add("@P_HOLDERNOTO", ReportData.HolderNoTo == 0 ? (object)null : ReportData.HolderNoTo);
                    param.Add("@P_CERTNOFROM", ReportData.CertNoFrom == 0 ? (object)null : ReportData.CertNoFrom);
                    param.Add("@P_CERTNOTO", ReportData.CertNoTo == 0 ? (object)null : ReportData.CertNoTo);
                    param.Add("@P_SHKITTAFROM", ReportData.ShareKittaFrom );
                    param.Add("@P_SHKITTATO", ReportData.ShareKittaTo);
                    param.Add("@P_SRNOFROM", ReportData.srnoFrom == 0 ? (object)null : ReportData.srnoFrom);
                    param.Add("@P_SRNOTO", ReportData.srnoTo == 0 ? (object)null : ReportData.srnoTo);
                    param.Add("@P_ORDERBY", OrderBy);
                    param.Add("@P_SHOWNERTYPE", ShareOwnerType);

                    List<dynamic> certificate = connection.Query<dynamic>("GET_ALLCERTIFICATE_LIST", param, commandType: CommandType.StoredProcedure)?.ToList();
                    if (certificate.Count > 0)
                    {
                        
                        jsonResponse.ResponseData = certificate;
                        jsonResponse.IsSuccess = true;
                    }
                    else
                    {
                        jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
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

        public JsonResponse DisplayCertificateList(string CompCode, string UserName, string OrderBy, string Listtype, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new
                SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand("GETDUPLICATECERTIFICATELIST", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter excelParam = new SqlParameter();
                    excelParam = cmd.Parameters.AddWithValue("@COMPCODE", CompCode);
                    excelParam = cmd.Parameters.AddWithValue("@ORDERBY", OrderBy);
                    excelParam = cmd.Parameters.AddWithValue("@LISTTYPE", Listtype);
                    excelParam = cmd.Parameters.AddWithValue("@USERNAME", UserName);
                    excelParam = cmd.Parameters.AddWithValue("@IP", IP);
                    excelParam = cmd.Parameters.AddWithValue("@DATE_NOW", DateTime.Now);
                    excelParam.Direction = ParameterDirection.Input;
                    DataSet ds = new DataSet("Data");
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
                    }
                    //DynamicParameters param = new DynamicParameters();
                    //param.Add("@COMPCODE", CompCode);
                    //param.Add("@ORDERBY", OrderBy);
                    //param.Add("@USERNAME", UserName);
                    //param.Add("@LISTTYPE", Listtype);
                    //param.Add("@IP", IP);
                    //param.Add("@DATE_NOW", DateTime.Now);
                    //List<dynamic> certificate = connection.Query<dynamic>("GETDUPLICATECERTIFICATELIST", param, commandType: CommandType.StoredProcedure)?.ToList();
                    //if (certificate.Count > 0)
                    //{
                    //    jsonResponse.ResponseData = certificate;
                    //    jsonResponse.IsSuccess = true;
                    //}
                    //else
                    //{
                    //    jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                    //}
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.Message = ex.Message;
                }
            }
            return jsonResponse;
        }

        public JsonResponse DistributedUnDistributedList(ATTDuplicateCertificate ReportData, string OrderBy, string Listtype, string sharetype, string SelectedAction)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new
                SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("GET_DISTRIBUTED_UNDISTRIBUTED_LIST", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter excelParam = new SqlParameter();
                    excelParam = cmd.Parameters.AddWithValue("@P_COMPCODE", ReportData.CompCode);
                    excelParam = cmd.Parameters.AddWithValue("@P_HOLDERNOFROM", ReportData.HolderNoFrom);
                    excelParam = cmd.Parameters.AddWithValue("@P_HOLDERNOTO", ReportData.HolderNoTo);
                    excelParam = cmd.Parameters.AddWithValue("@P_CERTNOFROM", ReportData.CertNoFrom);
                    excelParam = cmd.Parameters.AddWithValue("@P_CERTNOTO", ReportData.CertNoTo);
                    excelParam = cmd.Parameters.AddWithValue("@P_DATEFROM", ReportData.FromDate);
                    excelParam = cmd.Parameters.AddWithValue("@P_DATETO", ReportData.ToDate);
                    excelParam = cmd.Parameters.AddWithValue("@P_ORDERBY", OrderBy);
                    excelParam = cmd.Parameters.AddWithValue("@P_TYPE", SelectedAction);
                    excelParam = cmd.Parameters.AddWithValue("@P_SHARETYPE", sharetype);
                    excelParam = cmd.Parameters.AddWithValue("@P_STATUS", Listtype);
                    excelParam.Direction = ParameterDirection.Input;
                    DataSet ds = new DataSet("Data");
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

        public JsonResponse DistributedUnDistributedListForPDF(ATTDuplicateCertificate ReportData, string OrderBy, string Listtype, string sharetype, string SelectedAction)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new
                SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();

                    param.Add("@P_COMPCODE", ReportData.CompCode);
                    param.Add("@P_HOLDERNOFROM", ReportData.HolderNoFrom);
                    param.Add("@P_HOLDERNOTO", ReportData.HolderNoTo);
                    param.Add("@P_CERTNOFROM", ReportData.CertNoFrom);
                    param.Add("@P_CERTNOTO", ReportData.CertNoTo);
                    param.Add("@P_DATEFROM", ReportData.FromDate);
                    param.Add("@P_DATETO", ReportData.ToDate);
                    param.Add("@P_ORDERBY", OrderBy);
                    param.Add("@P_TYPE", SelectedAction);
                    param.Add("@P_SHARETYPE", sharetype);
                    param.Add("@P_STATUS", Listtype);

                    List<dynamic> certificate = connection.Query<dynamic>("GET_DISTRIBUTED_UNDISTRIBUTED_LIST", param, commandType: CommandType.StoredProcedure)?.ToList();
                    if (certificate.Count > 0)
                    {
                        jsonResponse.ResponseData = certificate;
                        jsonResponse.IsSuccess = true;
                    }
                    else
                    {
                        jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
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

        public JsonResponse ExporttoExcel(string CompCode, string UserName, string OrderBy)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new
                SqlConnection(connectionString.Value.DefaultConnection))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@COMPCODE", CompCode);
                    param.Add("@ORDERBY", OrderBy);
                    param.Add("@USERNAME", UserName);

                    List<ATTDuplicateCertificate> certificate = connection.Query<ATTDuplicateCertificate>("GETDUPLICATECERTIFICATELIST", param, commandType: CommandType.StoredProcedure)?.ToList();
                    if (certificate.Count > 0)
                    {
                        jsonResponse.ResponseData = certificate;
                        jsonResponse.IsSuccess = true;
                    }
                    else
                    {
                        jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
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
    }
}
