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
    public class CertificateSplitRepo : ICertificateSplit
    {
        IOptions<ReadConfig> _connectionString;

        public CertificateSplitRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }

        public JsonResponse GetCertificateDetailsByCertificateNo(string CompCode, string CertificateNo, string ActionType, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@CompCode", CompCode);
                    param.Add("@CertificateNo", CertificateNo);
                    param.Add("@P_IP_ADDRESS", IP);
                    param.Add("@P_USERNAME", UserName);
                    param.Add("@P_DATE_NOW", DateTime.Now);

                    if (ActionType == "A")
                    {
                        List<ATTCertificateSplit> certificate = connection.Query<ATTCertificateSplit>("GET_CERTIFICATE_BY_CERTNO", param, commandType: CommandType.StoredProcedure)?.ToList();

                        if (certificate.Count > 0)
                        {
                            switch (certificate[0].CertStatus)
                            {
                                case 2:
                                    jsonResponse.IsSuccess = false;
                                    jsonResponse.Message = "Selected Certificate is Under Pledge.";
                                    break;
                                case 3:
                                    jsonResponse.IsSuccess = false;
                                    jsonResponse.Message = "Selected Certificate is Under Suspend.";
                                    break;
                                case 4:
                                    jsonResponse.IsSuccess = false;
                                    jsonResponse.Message = "Selected Certificate is Under Lost.";
                                    break;
                                case 5:
                                    jsonResponse.IsSuccess = false;
                                    jsonResponse.Message = "Selected Certificate is already Dakhil Registered.";
                                    break;
                                case 6:
                                    jsonResponse.IsSuccess = false;
                                    jsonResponse.Message = "Selected Certificate is already Splitted.";
                                    ATTCertificateSplit cert= certificate.Find(x => x.CertStatus == 1);
                                    if(cert!=null)
                                    {
                                        List<ATTCertificateSplit> certlist = new List<ATTCertificateSplit>();
                                        certlist.Add(certificate.Find(x => x.CertStatus == 1));
                                        jsonResponse.IsSuccess = true;
                                        jsonResponse.ResponseData = certlist;
                                    }
                                    break;
                                case 7:
                                    jsonResponse.IsSuccess = false;
                                    jsonResponse.Message = "Selected Certificate is already Consolidated.";
                                    break;
                                case 10:
                                    jsonResponse.IsSuccess = false;
                                    jsonResponse.Message = "Certificate Under Split Process.";
                                    break;
                                default:
                                    jsonResponse.IsSuccess = true;
                                    jsonResponse.ResponseData = certificate;
                                    break;
                            }
                        }
                        else
                        {
                            jsonResponse.Message = "Cannot Find Certificate !!!";
                        }

                    }
                    else
                    {

                        param.Add("@Part", 1);
                        List<ATTCertificateSplit> certificate = connection.Query<ATTCertificateSplit>("GET_CERTIFICATE_BY_CERTNO_UNDO_SPLIT", param, commandType: CommandType.StoredProcedure)?.ToList();

                        if (certificate.Count > 0)
                        {
                            param.Add("@Part", 2);
                            certificate[0].aTTCertificateSplitDetails = connection.Query<ATTCertificateSplitDetail>("GET_CERTIFICATE_BY_CERTNO_UNDO_SPLIT", param, commandType: CommandType.StoredProcedure)?.ToList();
                            if (certificate[0].aTTCertificateSplitDetails.Count > 0)
                            {
                                jsonResponse.IsSuccess = true;
                                jsonResponse.ResponseData = certificate[0];
                            }
                            else
                            {
                                jsonResponse.IsSuccess = false;
                                jsonResponse.Message = ATTMessages.CERTIFICATE.NOT_FOUND;
                            }

                        }
                        else
                        {
                            jsonResponse.Message = ATTMessages.CERTIFICATE.NOT_FOUND;
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

        public JsonResponse CheckCertificateNo(string CompCode, int? CertificateNo)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@CompCode", CompCode);
                    param.Add("@CertificateNo", CertificateNo);

                    jsonResponse = connection.Query<JsonResponse>("CHECK_CERTIFICATE_No", param, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                    if (CertificateNo == null)
                    {
                        jsonResponse.Message = (Convert.ToInt64(jsonResponse.Message) + 1).ToString();
                    }
                    if (jsonResponse.Message == null)
                    {
                        jsonResponse.Message = (1).ToString();
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.Message = ex.Message;
                    return jsonResponse;
                }
            }
            return jsonResponse;
        }

        public JsonResponse SaveCertificateSplit(string CompCode, string CertificateNo, string srnofrom, string srnoto, List<ATTCertificateSplit> aTTCertificates, string shholderno, string Splitdate, int shownertype, int sharetype, string remarks, string SelectedAction, string PageNo, string SplitNo, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                using (SqlTransaction trans = connection.BeginTransaction())
                {
                    try
                    {
                        if (SelectedAction == "A")
                        {
                            DynamicParameters param = new DynamicParameters();
                            param.Add("@CompCode", CompCode);
                            param.Add("@CertificateNo", CertificateNo);

                            int? maxisplit = connection.Query<int?>("GET_CERTIFICATE_MAX_SPLIT", param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                            maxisplit = maxisplit == null ? 1 : ++maxisplit;

                            param.Add("@entryDate", DateTime.Now.ToString("yyyy-MM-dd"));
                            param.Add("@username", UserName);
                            param.Add("@splitdate", Splitdate);
                            param.Add("@maxsplitno", maxisplit);
                            param.Add("@shholderno", shholderno);
                            param.Add("@shownertype", shownertype);
                            param.Add("@sharetype", sharetype);
                            param.Add("@remarks", remarks);
                            param.Add("@P_IP_ADDRESS", IP);
                            param.Add("@P_DATE_NOW", DateTime.Now);
                            int? seqno = 1;


                            foreach (ATTCertificateSplit certificate in aTTCertificates)
                            {
                                param.Add("@splitcertno", certificate.CertNo);
                                param.Add("@splitsrnofrom", certificate.SrNoFrom);
                                param.Add("@splitsrnoto", certificate.SrNoTo);
                                param.Add("@SplitKitta", certificate.ShKitta);
                                param.Add("@Seqno", seqno);
                                JsonResponse response = connection.Query<JsonResponse>("Certificate_split_entry", param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                                seqno++;
                                if (!response.IsSuccess)
                                {
                                    trans.Rollback();
                                    return response;
                                }
                            }
                            DynamicParameters parameters = new DynamicParameters();

                            parameters.Add("@CompCode", CompCode);
                            parameters.Add("@CertificateNo", CertificateNo);
                            parameters.Add("@Shholderno", shholderno);
                            parameters.Add("@P_UserName", UserName);
                            parameters.Add("@P_IP_ADDRESS", IP);
                            parameters.Add("@P_DATE_NOW", DateTime.Now);


                            JsonResponse responseFromServer = connection.Query<JsonResponse>("Update_Splitted_Certificate", parameters, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                            if (responseFromServer.IsSuccess)
                                trans.Commit();
                            else
                                trans.Rollback();

                            return responseFromServer;
                        }

                        else
                        {
                            DynamicParameters parameters = new DynamicParameters();

                            parameters.Add("@CompCode", CompCode);
                            parameters.Add("@CertificateNo", CertificateNo);
                            parameters.Add("@SplitNo", SplitNo);
                            parameters.Add("@P_UserName", UserName);
                            parameters.Add("@P_IP_ADDRESS", IP);
                            parameters.Add("@P_DATE_NOW", DateTime.Now);

                            JsonResponse responseFromServer = connection.Query<JsonResponse>("UNDO_CERTIFICATE_SPLIT_NEW", parameters, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                            if (responseFromServer.IsSuccess)
                                trans.Commit();
                            else
                                trans.Rollback();

                            return responseFromServer;
                        }

                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        jsonResponse.IsSuccess = false;
                        jsonResponse.Message = ex.Message;

                    }
                }
            }
            return jsonResponse;
        }

        public JsonResponse CreateReport(string CompCode, string CertificateNo, string UserName)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@CompCode", CompCode);
                    param.Add("@CertificateNo", CertificateNo);
                    //int? MaxSplitNo = connection.Query<int>("GET_MAX_SPLIT_NO_REPORT", param, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                    //param.Add("@PageNo", MaxSplitNo);
                    List<ATTCertificateSplit> certificates = connection.Query<ATTCertificateSplit>("GET_CERTIFICATE_SPLIT_FOR_REPORT", param, commandType: CommandType.StoredProcedure)?.ToList();
                    if (certificates.Count > 0)
                    {
                        jsonResponse.Message = "Generating Certificate Split Report.";
                        jsonResponse.ResponseData = certificates;
                        jsonResponse.IsSuccess = true;
                    }
                    else
                    {
                        jsonResponse.Message = "Cannot Find Any Data.";
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.Message = ex.Message;
                    return jsonResponse;
                }
            }
            return jsonResponse;
        }
    }
}
