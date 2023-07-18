



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
    public class CertificateConsolidatePostingRepo : ICertificateConsolidatePosting
    {
        IOptions<ReadConfig> _connectionString;

        public CertificateConsolidatePostingRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }



        public JsonResponse GetCertificateConsolidateCompanyData(string CompCode, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_COMPCODE", CompCode);
                    param.Add("@TransType", "02");
                    param.Add("@P_USERNAME", UserName);
                    param.Add("@P_IP_ADDRESS", IP);
                    param.Add("@P_DATE_NOW", DateTime.Now);
                    List<ATTCertificateConsolidatePosting> certificate = connection.Query<ATTCertificateConsolidatePosting>("GET_CERTIFICATE_CON_DATA", param, commandType: CommandType.StoredProcedure)?.ToList();
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



        public JsonResponse GetCertificate(string CompCode, string SplitNo, string ShholderNo, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@COMPCODE", CompCode);
                    param.Add("@split_no", SplitNo);
                    param.Add("@shholderno", ShholderNo);
                    param.Add("@P_USERNAME", UserName);
                    param.Add("@P_IP_ADDRESS", IP);
                    param.Add("@P_DATE_NOW", DateTime.Now);

                    //param.Add("@TransType", "02");
                    List<ATTCertificateConsolidatePosting> certificate = connection.Query<ATTCertificateConsolidatePosting>("Get_Certificate_Informattion", param, commandType: CommandType.StoredProcedure)?.ToList();
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


        public JsonResponse PostCertificateConsolidateEntry(List<ATTCertificateConsolidatePosting> aTTCertificateConsolidatePostings, ATTCertificateConsolidatePosting recorddetails, string SelectedAction, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();

            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction trans = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (ATTCertificateConsolidatePosting aTTCertificateConsolidatePosting in aTTCertificateConsolidatePostings)
                            {
                                DynamicParameters param = new DynamicParameters();
                                param.Add("@COMPCODE", recorddetails.compcode);
                                param.Add("@SPLIT_NO", aTTCertificateConsolidatePosting.split_no);
                                param.Add("@SHHOLDERNO", aTTCertificateConsolidatePosting.ShholderNo);
                                param.Add("@APPROVED_BY", UserName);
                                param.Add("@APP_DATE", recorddetails.App_date);
                                param.Add("@APP_REMARKS", recorddetails.App_remarks);
                                param.Add("@ACTION", SelectedAction);
                                param.Add("@P_USERNAME", UserName);
                                param.Add("@P_IP_ADDRESS", IP);
                                param.Add("@P_DATE_NOW", DateTime.Now);

                                jsonResponse = connection.Query<JsonResponse>("POST_CONSOLIDATE_DATA", param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                            }
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            jsonResponse.Message = ex.Message;
                        }
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.Message = ex.Message;
                }
            }
            return jsonResponse;
        }
    }
}

