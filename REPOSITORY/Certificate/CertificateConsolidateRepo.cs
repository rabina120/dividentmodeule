



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

    public class CertificateConsolidateRepo : ICertificateConsolidate
    {
        IOptions<ReadConfig> _connectionString;

        public CertificateConsolidateRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }

        public JsonResponse GetCertificateDetails(string CompCode, string ShholderNo, string CertificateNo, string SelectedAction, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@SHHOLDERNO", ShholderNo);
                    param.Add("@COMPCODE", CompCode);
                    param.Add("@CERTNO", CertificateNo);
                    param.Add("@P_USERNAME", UserName);
                    param.Add("@P_IP_ADDRESS", IP);
                    param.Add("@P_DATE_NOW", DateTime.Now);

                    if (SelectedAction == "A")
                    {

                        List<ATTCertificateConsolidate> certificate = connection.Query<ATTCertificateConsolidate>("GETCERIFICATE_FOR_CONSOLIDATE", param, commandType: CommandType.StoredProcedure)?.ToList();
                        if (certificate.Count == 1)
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
                                    break;
                                case 7:
                                    jsonResponse.IsSuccess = false;
                                    jsonResponse.Message = "Selected Certificate is already Consolidated.";
                                    break;
                                case 8:
                                    jsonResponse.IsSuccess = false;
                                    jsonResponse.Message = "Selected Certificate is not Found";
                                    break;
                                case 9:
                                    jsonResponse.IsSuccess = false;
                                    jsonResponse.Message = "Selected Certificate is not Found.";
                                    break;
                                case 10:
                                    jsonResponse.IsSuccess = false;
                                    jsonResponse.Message = "Selected Certificate is in Split Process.";
                                    break;
                                case 11:
                                    jsonResponse.IsSuccess = false;
                                    jsonResponse.Message = "Selected Certificate is in Consolidate Process.";
                                    break;
                                default:
                                    jsonResponse.IsSuccess = true;
                                    jsonResponse.ResponseData = certificate;
                                    break;
                            }
                        }
                        else
                        {
                            jsonResponse.Message = "Cannot Find Certificate ";
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

        public JsonResponse GetShholderDetailsByShHolderNo(string CompCode, string ShholderNo, string SelectedAction, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_COMPCODE", CompCode);
                    param.Add("@P_SHHOLDERNO", ShholderNo);
                    param.Add("@P_USERNAME", UserName);
                    param.Add("@P_IP_ADDRESS", IP);
                    param.Add("@P_DATE_NOW", DateTime.Now);


                    if (SelectedAction == "A")
                    {
                        List<ATTCertificateConsolidate> certificate = connection.Query<ATTCertificateConsolidate>("GET_SHHOLDER_INFORMATION", param, commandType: CommandType.StoredProcedure)?.ToList();
                        if (certificate.Count > 0)
                        {
                            jsonResponse.ResponseData = certificate;
                            jsonResponse.Message = "Record Found";
                            jsonResponse.IsSuccess = true;
                        }
                        else
                        {
                            jsonResponse.Message = "Cannot Find Sholder !!!";

                        }
                    }
                    else
                    {
                        List<ATTCertificateConsolidate> certificate = connection.Query<ATTCertificateConsolidate>("GET_SHHOLDER_INFORMATION", param, commandType: CommandType.StoredProcedure)?.ToList();
                        if (certificate.Count > 0)
                        {
                            jsonResponse.ResponseData = certificate;
                            jsonResponse.Message = "Record Found";
                            jsonResponse.IsSuccess = true;
                        }
                        else
                        {
                            jsonResponse.Message = "Cannot Find Sholder !!!";

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

        public JsonResponse SaveCertificateConsolidate(string CompCode, string ShholderNo, List<ATTCertificateConsolidate> aTTCertificateConsolidate, string CertificateNo, string Splitdate, string remarks, string SelectedAction, string UserName, string IP)
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

                            if (SelectedAction == "A")
                            {

                                DynamicParameters param = new DynamicParameters();
                                param.Add("@CompCode", CompCode);
                                int? maxisplit = connection.Query<int?>("[GET_CERTIFICATE_CONSO_MAX_SPLIT]", param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                                param.Add("@shholderno", ShholderNo);


                                maxisplit = maxisplit == null ? 1 : ++maxisplit;



                                param.Add("@entryDate", DateTime.Now.ToString("yyyy-MM-dd"));
                                param.Add("@username", UserName);
                                param.Add("@splitdate", Splitdate);
                                param.Add("@SPLITNO", maxisplit);
                                param.Add("@P_IP_ADDRESS", IP);
                                param.Add("@P_DATE_NOW", DateTime.Now);

                                param.Add("@remarks", remarks);
                                int? seqno = 1;


                                foreach (ATTCertificateConsolidate certificate in aTTCertificateConsolidate)
                                {

                                    param.Add("@splitcertno", certificate.CertNo);
                                    param.Add("@splitsrnofrom", certificate.SrNoFrom);
                                    param.Add("@splitsrnoto", certificate.SrNoTo);
                                    param.Add("@SplitKitta", certificate.ShKitta);
                                    param.Add("@shownertype", certificate.ShOwnerType);
                                    param.Add("@sharetype", certificate.ShareType);
                                    param.Add("@Seqno", seqno);
                                    JsonResponse response = connection.Query<JsonResponse>("SAVE_CERTIFICATE_CONSOLIDATE", param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                                    seqno++;
                                    if (!response.IsSuccess)
                                    {
                                        trans.Rollback();

                                        return response;

                                    }
                                    else
                                    {
                                        trans.Commit();
                                    }
                                    jsonResponse.IsSuccess = response.IsSuccess;
                                    response.Message = response.Message; ;
                                }

                            }
                            else
                            {
                                jsonResponse.Message = "Cannot";
                            }

                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            jsonResponse.IsSuccess = false;
                            jsonResponse.Message = ex.Message;
                            return jsonResponse;
                        }
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



