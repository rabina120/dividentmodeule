



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

namespace Repository.CDS
{
    public class RematerializePostingRepo : IRematerializePosting
    {
        IOptions<ReadConfig> connectionString;
        public RematerializePostingRepo(IOptions<ReadConfig> connectionString)
        {
            this.connectionString = connectionString;
        }

        public JsonResponse GetReMaterializeData(string CompCode)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_COMPCODE", CompCode);
                    param.Add("@TransType", "02");
                    List<CertificateDemateDetails> certificateDemateDetails = SqlMapper.QueryAsync<CertificateDemateDetails, ATTShHolder, CertificateDemateDetails>(connection, "GET_MATERIALIZE_DATA",
                       (certificate, holder) =>
                       {
                           certificate.aTTShHolder = holder;
                           return certificate;
                       }, param, null, splitOn: "ShHolder", commandType: CommandType.StoredProcedure)?.Result.AsList();

                    jsonResponse.IsSuccess = true;
                    jsonResponse.ResponseData = certificateDemateDetails;

                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.Message = ex.Message;
                }
            }
            return jsonResponse;

        }

        public JsonResponse PostReMaterializeEntry(List<CertificateDemateDetails> certificateDemate, CertificateDemateDetails recordDetails, string ActionType, string UserName)
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
                            foreach (CertificateDemateDetails certificateDemateDetails in certificateDemate)
                            {
                                param = new DynamicParameters();
                                param.Add("@CompCode", recordDetails.compcode);
                                param.Add("@Demate_Regno", certificateDemateDetails.demate_regno);
                                param.Add("@App_Remarks", recordDetails.App_remarks);
                                param.Add("@App_Date", recordDetails.App_date);
                                param.Add("@ActionType", ActionType);
                                param.Add("@UserName", UserName);
                                param.Add("@ISIN_NO", certificateDemateDetails.isin_no);
                                param.Add("@BHolderNo", certificateDemateDetails.shholderno);
                                param.Add("@CertNo", certificateDemateDetails.certno);
                                param.Add("@SrNoFrom", certificateDemateDetails.srnofrom);
                                param.Add("@SrNoTo", certificateDemateDetails.srnoto);
                                param.Add("@ShKitta", certificateDemateDetails.shkitta);
                                param.Add("@TrDate", certificateDemateDetails.tr_date);

                                connection.Query("REMATERIALIZE_DATA_POSTING", param, trans, commandType: CommandType.StoredProcedure);

                            }

                            trans.Commit();
                            jsonResponse.IsSuccess = true;

                            if (ActionType.Trim() == "A") jsonResponse.Message = ATTMessages.CERTIFICATE.REVERSE_POSTING_SUCCESS;
                            else if (ActionType.Trim() == "R") jsonResponse.Message = ATTMessages.CERTIFICATE.REVERSE_REJECT_POSTING_SUCCESS;
                            else if (ActionType.Trim() == "D") jsonResponse.Message = ATTMessages.CERTIFICATE.REVERSE_DELETE_POSTING_SUCCESS;

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
