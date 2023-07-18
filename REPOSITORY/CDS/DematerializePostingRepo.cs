

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
    public class DematerializePostingRepo : IDematerializePosting
    {
        IOptions<ReadConfig> connectionString;

        public DematerializePostingRepo(IOptions<ReadConfig> connectionString)
        {
            this.connectionString = connectionString;
        }


        public JsonResponse GetParaCompChildList(string CompCode)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@CompCode", CompCode);

                    List<ParaComp_Child> paraComps = connection.Query<ParaComp_Child>("GET_PARACOMP_CHILD", param, commandType: CommandType.StoredProcedure)?.AsList();
                    if (paraComps.Count > 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = paraComps;
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

        public JsonResponse GetDeMaterializeData(string CompCode, string FromDate, string ToDate, string RegNoFrom, string RegNoTo, string ISINNO, string CheckCA, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@P_COMPCODE", CompCode);
                        param.Add("@P_FromDate", FromDate);
                        param.Add("@P_ToDate", ToDate);
                        param.Add("@RegNoFrom", RegNoFrom);
                        param.Add("@RegNoTo", RegNoTo);
                        param.Add("@ISINNO", ISINNO);
                        param.Add("@CheckCA", CheckCA);
                        param.Add("@P_USERNAME", UserName);
                        param.Add("@P_IP_ADDRESS", IP);
                        param.Add("@EntryDate", DateTime.Now);
                        param.Add("@TransType", "01");
                        param.Add("@P_DATE_NOW", DateTime.Now);

                        List<CertificateDemateDetails> certificateDemateDetails = SqlMapper.QueryAsync<CertificateDemateDetails, ATTShHolder, CertificateDemateDetails>(connection, "GET_MATERIALIZE_DATA",
                            (certificate, holder) =>
                            {
                                certificate.aTTShHolder = holder;
                                return certificate;
                            }, param, tran, splitOn: "ShHolder", commandType: CommandType.StoredProcedure)?.Result.AsList();

                        if (certificateDemateDetails.Count > 0)
                        {
                            jsonResponse.IsSuccess = true;
                            jsonResponse.ResponseData = certificateDemateDetails;
                        }
                        else
                        {
                            jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                        }
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

        public JsonResponse PostDeMaterializeEntry(List<CertificateDemateDetails> certificateDemate, CertificateDemateDetails recordDetails, string ActionType, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            long? buyer_holderno;
            bool CheckAccount = false;
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param;
                    using (SqlTransaction trans = connection.BeginTransaction())
                    {
                        if (ActionType == "A")
                        {
                            foreach (CertificateDemateDetails certificateDemateDetails in certificateDemate)
                            {
                                JsonResponse responseFromFindHolderNo = new JsonResponse();
                                responseFromFindHolderNo = FindHolderNo(recordDetails.compcode, certificateDemateDetails.isin_no, connection, trans);
                                if (responseFromFindHolderNo.IsSuccess)
                                {
                                    buyer_holderno = (long)responseFromFindHolderNo.ResponseData;
                                }
                                else
                                {
                                    return responseFromFindHolderNo;
                                }
                                JsonResponse responseFromCertificateDemateRegNo = new JsonResponse();

                                responseFromCertificateDemateRegNo = GET_CERTIFICATE_DEMATE_BY_DREGNO(recordDetails.compcode, certificateDemateDetails.demate_regno, connection, trans);
                                CheckAccount = false;
                                if (responseFromCertificateDemateRegNo.IsSuccess)
                                {
                                    List<CertificateDemateDetails> cert = new List<CertificateDemateDetails>();
                                    cert = (List<CertificateDemateDetails>)responseFromCertificateDemateRegNo.ResponseData;
                                    if (cert.Count > 0)
                                    {
                                        foreach (CertificateDemateDetails certificate in cert)
                                        {
                                            JsonResponse responseForFindAccount = new JsonResponse();
                                            responseFromFindHolderNo = FindAccount(recordDetails.compcode, certificate.shholderno, certificate.bo_acct_no, connection, trans);
                                            if (!responseFromFindHolderNo.IsSuccess)
                                            {
                                                return responseFromFindHolderNo;
                                            }
                                            param = new DynamicParameters();
                                            param.Add("@compcode", recordDetails.compcode);
                                            param.Add("@CertNo", certificate.certno);
                                            param.Add("@SHolderNo", certificate.shholderno);
                                            param.Add("@BHolderNo", buyer_holderno);
                                            param.Add("@SrNoFrom", certificate.srnofrom);
                                            param.Add("@SrNoTo", certificate.srnoto);
                                            param.Add("@RegNo", certificate.demate_regno);
                                            param.Add("@mseqno", certificate.seq_no);
                                            param.Add("@mdrn_no", certificate.drn_no);
                                            param.Add("@TransferDt", recordDetails.App_date);
                                            param.Add("@Remarks", recordDetails.App_remarks);
                                            param.Add("@UserName", UserName);
                                            param.Add("@TranKitta", certificate.shkitta);
                                            param.Add("@P_IP_ADDRESS", IP);
                                            param.Add("@P_DATE_NOW", DateTime.Now);


                                            JsonResponse responseFromUpdateTransferDematePosting = new JsonResponse();
                                            responseFromUpdateTransferDematePosting = connection.Query<JsonResponse>("UPDATE_TRANSFER_DEMATE_POSTINGS", param, trans, commandType: CommandType.StoredProcedure).FirstOrDefault();
                                            if (!responseFromUpdateTransferDematePosting.IsSuccess)
                                            {
                                                responseFromUpdateTransferDematePosting.Message = "Error While Updating Certificates !!!";
                                                return responseFromUpdateTransferDematePosting;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    return responseFromCertificateDemateRegNo;
                                }

                                JsonResponse responseFromCertificateDematePosting = new JsonResponse();

                                if (!DEMATERIALIZE_DATA_POSTING(recordDetails.compcode, recordDetails.certno,  certificateDemateDetails.demate_regno, certificateDemateDetails.regno, recordDetails.App_remarks, recordDetails.App_date, ActionType, UserName, connection, trans, IP))
                                {
                                    jsonResponse.Message = "Transaction Failed !!!";
                                    return jsonResponse;
                                }
                            }
                            trans.Commit();
                            jsonResponse.IsSuccess = true;
                            jsonResponse.Message = "Record Have Been Posted";
                        }
                        else if (ActionType == "R")
                        {
                            foreach (CertificateDemateDetails certificateDemateDetails in certificateDemate)
                            {
                                if (!DEMATERIALIZE_DATA_POSTING(recordDetails.compcode, certificateDemateDetails.certno,certificateDemateDetails.demate_regno, certificateDemateDetails.regno, recordDetails.App_remarks, recordDetails.App_date, ActionType, UserName, connection, trans, IP))
                                {
                                    jsonResponse.Message = "Transaction Failed !!!";
                                    return jsonResponse;
                                }
                            }
                            trans.Commit();
                            jsonResponse.IsSuccess = true;
                            jsonResponse.Message = "Record Have Been Rejected";
                        }
                        else if (ActionType == "D")
                        {
                            foreach (CertificateDemateDetails certificateDemateDetails in certificateDemate)
                            {
                                if (!DEMATERIALIZE_DATA_POSTING(recordDetails.compcode, certificateDemateDetails.certno, certificateDemateDetails.demate_regno, certificateDemateDetails.regno, recordDetails.App_remarks, recordDetails.App_date, ActionType, UserName, connection, trans, IP))
                                {
                                    jsonResponse.Message = "Transaction Failed !!!";
                                    return jsonResponse;
                                }
                            }
                            trans.Commit();
                            jsonResponse.IsSuccess = true;
                            jsonResponse.Message = "Record Have Been Deleted";
                        }

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
        public JsonResponse FindHolderNo(string CompCode, string ISINNO, SqlConnection connection, SqlTransaction trans)
        {
            JsonResponse response = new JsonResponse();
            long? holderNo = null;
            try
            {

                DynamicParameters param = new DynamicParameters();
                param.Add("@CompCode", CompCode);
                param.Add("@ISINNO", ISINNO);
                holderNo = connection.Query<long>("GET_PARACOMPCHILD_HOLDERNO", param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                if (holderNo != null)
                {
                    response.IsSuccess = true;
                    response.ResponseData = holderNo;
                }
                else
                {
                    response.Message = "Cannot Create New Holder No ";
                    trans.Rollback();
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                trans.Rollback();
            }
            return response;
        }

        public JsonResponse GET_CERTIFICATE_DEMATE_BY_DREGNO(string CompCode, int? DRegNo, SqlConnection connection, SqlTransaction trans)
        {
            List<CertificateDemateDetails> certificateDemateDetails = new List<CertificateDemateDetails>();
            JsonResponse response = new JsonResponse();
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@CompCode", CompCode);
                param.Add("@DemateRegNo", DRegNo);
                certificateDemateDetails = connection.Query<CertificateDemateDetails>("GET_CERTIFICATE_DEMATE_BY_DREGNO", param, trans, commandType: CommandType.StoredProcedure)?.ToList();
                if (certificateDemateDetails.Count > 0)
                {
                    response.IsSuccess = true;
                    response.ResponseData = certificateDemateDetails;
                }
                else
                {
                    response.Message = "No Certificate Found !!!";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }


        public JsonResponse FindAccount(string CompCode, int? Shholderno, string bo_acct_no, SqlConnection connection, SqlTransaction trans)
        {
            JsonResponse response = new JsonResponse();
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@CompCode", CompCode);
                param.Add("@ShHolderNo", Shholderno);
                param.Add("@BoAccount", bo_acct_no);
                response = connection.Query<JsonResponse>("FIND_ACCOUNT_EXITS", param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                if (!response.IsSuccess)
                    response.Message = "Cannot Find Account !!!";
            }
            catch (Exception Ex)
            {
                response.Message = Ex.Message;
            }
            return response;
        }



        public bool DEMATERIALIZE_DATA_POSTING(string compcode, int? certNo, int? demate_regno, int? regno, string App_remarks, DateTime? App_date, string ActionType, string UserName, SqlConnection connection, SqlTransaction trans, string IP)
        {

            DynamicParameters param = new DynamicParameters();
            param.Add("@DemateRegno", demate_regno);
            param.Add("@CompCode", compcode);
            param.Add("@certNo", certNo);         
            param.Add("@Remarks", App_remarks);
            param.Add("@AppDate", App_date);
            param.Add("@ActionType", ActionType);
            param.Add("@UserName", UserName);
            param.Add("@P_IP_ADDRESS", IP);
            param.Add("@P_DATE_NOW", DateTime.Now);


            int result = connection.Query<int>("DEMATERIALIZE_DATA_POSTING", param, trans, commandType: CommandType.StoredProcedure).First();
            if (result == 0)
            {
                trans.Rollback();
                return false;
            }

            return true;
        }

        public JsonResponse GetSingleDeMaterializeData(string CompCode, string DemateRegno, string RegNo, string ISINNo, string Remarks, string DRNNo, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@COMPCODE", CompCode);
                        param.Add("@DemateRegNo", DemateRegno);
                        param.Add("@RegNo", RegNo);
                        param.Add("@ISINNo", ISINNo);
                        param.Add("@Remarks", Remarks);
                        param.Add("@DRNNO", DRNNo);
                        param.Add("@UserName", UserName);
                        param.Add("@EntryDate", DateTime.Now);
                        param.Add("@P_IP_ADDRESS", IPAddress);

                        List<CertificateDemateDetails> certificateDemateDetails = connection.Query<CertificateDemateDetails>("GET_SINGLE_MATERIALIZE_DATA", param, tran, commandType: CommandType.StoredProcedure).ToList();

                        if (certificateDemateDetails.Count > 0)
                        {
                            jsonResponse.IsSuccess = true;
                            jsonResponse.ResponseData = certificateDemateDetails;
                        }
                        else
                        {
                            jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                        }

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
    }
}
