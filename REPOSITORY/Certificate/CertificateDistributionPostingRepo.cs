


using Dapper;
using Entity.Certificate;
using Entity.Common;
using Entity.ShareHolder;
using Interface.Certificate;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Certificate
{
    public class CertificateDistributionPostingRepo : ICertificateDistributionPosting
    {
        IOptions<ReadConfig> connectionString;
        public CertificateDistributionPostingRepo(IOptions<ReadConfig> connectionString)
        {
            this.connectionString = connectionString;
        }

        public JsonResponse GetCertificateDistributionCompanyData(string CompCode, string startdate, string enddate, string UserName, string IP)
        {


            var today = DateTime.Now;
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                string fixdate = "1921-01-01";
                try
                {

                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@COMPCODE", CompCode);
                    param.Add("@STARTDATE", startdate == null ? " " : fixdate);
                    param.Add("@ENDDATE", enddate == null ? " " : DateTime.Now.ToString("yyyy-MM-dd"));
                    param.Add("@P_IP_ADDRESS", IP);
                    param.Add("@P_USERNAME", UserName);
                    param.Add("@P_DATE_NOW", DateTime.Now);

                    //List<ATTCERTIFICATE> paraCertificateDistribute = connection.Query<ATTCERTIFICATE>("GET_CERTIFICATE_DISTRIBUTION_POSTING", param, commandType: CommandType.StoredProcedure)?.AsList();
                    //ATTCERTIFICATE paraCertificateDistribute = SqlMapper.Query<ATTCERTIFICATE, ATTShHolder, ATTCERTIFICATE>(connection, sql: "GET_CERTIFICATE_DISTRIBUTION_POSTING",
                    //  (certificate, holder) =>
                    //  {
                    //      certificate.aTTShHolder = holder;
                    //      return certificate;
                    //  }, param: param, null, splitOn: "Spholder", commandType: CommandType.StoredProcedure).FirstOrDefault();

                    List<ATTCERTIFICATE> paraCertificateDistribute = SqlMapper.QueryAsync<ATTCERTIFICATE, ATTShHolder, ATTCERTIFICATE>(connection, "GET_CERTIFICATE_DISTRIBUTION_POSTING",
                       (certificate, holder) =>
                       {
                           certificate.aTTShHolder = holder;
                           return certificate;
                       }, param, null, splitOn: "Spholder", commandType: CommandType.StoredProcedure)?.Result.AsList();

                    if (paraCertificateDistribute.Count > 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = paraCertificateDistribute;
                    }
                    else
                    {
                        jsonResponse.Message = "Cannot Find Any Data !!!";
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

        public JsonResponse PostCertificateDistributionEntry(List<ATTCERTIFICATE> aTTCERTIFICATE, ATTCERTIFICATE recordDetails, string ActionType, string UserName, string IP)
        {

            JsonResponse jsonResponse = new JsonResponse();
            SqlTransaction trans;
            DynamicParameters param;

            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {

                try
                {
                    connection.Open();
                    using (trans = connection.BeginTransaction())
                    {
                        try
                        {




                            foreach (ATTCERTIFICATE aTTCERTIFICATe in aTTCERTIFICATE)
                            {
                                param = new DynamicParameters();
                                param.Add("@P_COMPCODE", recordDetails.compcode);
                                param.Add("@P_TXTPOSTING_REMARKS", recordDetails.App_remarks);
                                param.Add("@P_MSKPOSTINGDT", recordDetails.App_date);
                                param.Add("@P_ACTION", ActionType);
                                param.Add("@P_APPROVEDBY", UserName);
                                param.Add("@P_SHHOLDERNO", aTTCERTIFICATe.ShholderNo);
                                param.Add("@P_CERTNO", aTTCERTIFICATe.CertNo);
                                param.Add("@P_IP_ADDRESS", IP);
                                param.Add("@P_USERNAME", UserName);
                                param.Add("@P_DATE_NOW", DateTime.Now);


                                jsonResponse = connection.Query<JsonResponse>("SAVE_CERTIFICATE_DISTRIBUTION_POSTING", param, trans, commandType: CommandType.StoredProcedure).FirstOrDefault();

                            }

                            trans.Commit();
                            jsonResponse.Message = ATTMessages.SAVED_SUCCESS;
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            jsonResponse.IsSuccess = false;
                            jsonResponse.Message = ex.Message;
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


    }
}
