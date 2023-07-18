using Dapper;
using Entity.CDS;
using Entity.Common;
using Entity.ShareHolder;
using Interface.CDS;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.CDS
{
    public class ReversalEntryRepo : IReversalEntry
    {
        IOptions<ReadConfig> _connectionString;

        public ReversalEntryRepo(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }

        public JsonResponse GenerateReport(string CompCode, string UserName, string DateFrom, string DateTo)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
                {


                    DynamicParameters param = new DynamicParameters();
                    param.Add("@CompCode", CompCode);
                    param.Add("@UserName", UserName);
                    param.Add("@DateFrom", DateFrom);
                    param.Add("@DateTo", DateTo);
                    List<ATTReversalCertificate> reversalCertificates = (connection.QueryAsync<ATTReversalCertificate>("GENERATE_REPORT_REVERSAL_CERTIFIACTE", param, commandType: CommandType.StoredProcedure))?.Result.ToList();
                    if (reversalCertificates.Count > 0)
                    {
                        jsonResponse.ResponseData = reversalCertificates;
                        jsonResponse.TotalRecords = reversalCertificates.Count();
                    }
                    else
                    {
                        jsonResponse.Message = "No Records Found !!!";
                    }

                }
            }
            catch (Exception ex)
            {
                jsonResponse.Message = ex.Message;
            }
            return jsonResponse;
        }

        public JsonResponse GetDataFromDRN(string CompCode, string UserName, string ShholderNo, string DRNNO, string IP)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_COMPCODE", CompCode);
                    parameters.Add("@P_USERNAME", UserName);
                    parameters.Add("@P_SHHOLDERNO", ShholderNo);
                    parameters.Add("@P_DRNNO", DRNNO);
                    parameters.Add("@P_IP_ADDRESS", IP);
                    parameters.Add("@P_DATE_NOW", DateTime.Now);

                    List<CertificateDemateDetails> certificateDemateDetails = SqlMapper.Query<CertificateDemateDetails, ATTShHolder, CertificateDemateDetails>(connection, sql: "GET_DATA_FROM_DRN_CERTIFICATE_REVERSAL_ENTRY",
                        (cert, holder) =>
                        {
                            cert.aTTShHolder = holder;
                            return cert;
                        },
                        param: parameters, null, splitOn: "SpCert", commandType: CommandType.StoredProcedure)?.ToList();


                    //List<ATTCertificate> certificates = connection.Query<ATTCertificate>("GET_DATA_FROM_DRN_CERTIFICATE_REVERSAL_ENTRY", parameters, null, commandType: CommandType.StoredProcedure).ToList();
                    if (certificateDemateDetails.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = certificateDemateDetails;
                    }
                    else
                    {
                        response.Message = "Cannot Find Any Data !!!";
                    }

                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                }

            }
            return response;
        }

        public JsonResponse GetReversalCertificateList(string CompCode, string UserName, string SelectedAction, string CertNo = null)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                try
                {
                    DynamicParameters parameters = new DynamicParameters();

                    List<CertificateDemateDetails> certificates = connection.Query<CertificateDemateDetails>("", parameters, null, commandType: CommandType.StoredProcedure).ToList();
                    if (certificates.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = certificates;
                    }
                    else
                    {
                        response.Message = "Cannot Find Any Data !!!";
                    }

                }
                catch (Exception ex)
                {
                    response.ResponseData = ex;
                    response.IsSuccess = false;
                    response.HasError = true;
                }

            }
            return response;
        }

        public JsonResponse GetShholderInformation(string CompCode, string UserName, string ShholderNo, string IP)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_COMPCODE", CompCode);
                    parameters.Add("@P_SHHOLDERNO", ShholderNo);
                    parameters.Add("@P_USERNAME", UserName);
                    parameters.Add("@P_IP_ADDRESS", IP);
                    parameters.Add("@P_DATE_NOW", DateTime.Now);

                    ATTShHolder aTTShHolder = connection.Query<ATTShHolder>("GET_SHHOLDER_INFORMATION_FOR_REVERSAL", parameters, null, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    if (aTTShHolder != null)
                    {
                        if (aTTShHolder.HolderLock == "N" || aTTShHolder.HolderLock == null)
                        {
                            response.IsSuccess = true;
                            response.ResponseData = aTTShHolder;
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = ATTMessages.SHARE_HOLDER.LOCK;
                        }
                    }
                    else
                    {
                        response.Message = ATTMessages.SHARE_HOLDER.NOT_FOUND;
                    }

                }
                catch (Exception ex)
                {
                    response.ResponseData = ex;
                    response.IsSuccess = false;
                    response.HasError = true;
                }

            }
            return response;
        }

        public JsonResponse SaveReversalCertificate(string CompCode, string UserName, string SelectedAction, string Remarks, string ApprovalDate, List<CertificateDemateDetails> certificates, string DRNNO, string ShHolderNo, string IP)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                try
                {
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        DataTable dt = new DataTable();
                        dt.Columns.Add("seqno");
                        dt.Columns.Add("certno");
                        dt.Columns.Add("shkitta");
                        dt.Columns.Add("srnofrom");
                        dt.Columns.Add("srnoto");
                        dt.Columns.Add("bo_acct_no");
                        dt.Columns.Add("app_date");
                        dt.Columns.Add("regno");
                        dt.Columns.Add("isin_no");
                        dt.Columns.Add("shholderNo");
                        int mseqno = 1;
                        certificates.ForEach(certificate =>
                        {
                            dt.Rows.Add(mseqno++, certificate.certno, certificate.shkitta, certificate.srnofrom, certificate.srnoto,
                                certificate.bo_acct_no, certificate.App_date.ToString("yyyy-MM-dd").Substring(0, 10), certificate.regno, certificate.isin_no,
                                certificate.aTTShHolder.ShholderNo);

                        });

                        SqlCommand cmd = new SqlCommand("Reversal_entry", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Transaction = transaction;
                        SqlParameter param = cmd.Parameters.AddWithValue("@udt", dt);
                        param = cmd.Parameters.AddWithValue("@P_Remarks", Remarks);
                        param = cmd.Parameters.AddWithValue("@P_USERNAME", UserName);
                        param = cmd.Parameters.AddWithValue("@P_CompCode", CompCode);
                        param = cmd.Parameters.AddWithValue("@P_ActionType", SelectedAction);
                        param = cmd.Parameters.AddWithValue("@P_SHHOLDERNO", ShHolderNo);
                        param = cmd.Parameters.AddWithValue("@P_DRNNO", DRNNO);
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
                                    response.Message = ATTMessages.CERTIFICATE.REVERSE_ENTRY_SUCCESS;

                                }
                                else
                                {
                                    response.IsSuccess = false;
                                    response.Message = ATTMessages.CERTIFICATE.REVERSE_FAILED;
                                }

                            }
                        }
                        if (response.IsSuccess)
                            transaction.Commit();
                        else
                            transaction.Rollback();

                    }
                }
                catch (Exception ex)
                {
                    response.ResponseData = ex;
                    response.IsSuccess = false;
                    response.HasError = true;
                }
            }
            return response;
        }
    }
}
