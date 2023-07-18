
using Dapper;
using Entity.Common;
using Entity.ShareHolder;
using Interface.ShareHolder;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.ShareHolder
{

    public class CertDet : ICertDet
    {

        IOptions<ReadConfig> connectionString;
        public CertDet(IOptions<ReadConfig> _connectionString)
        {
            this.connectionString = _connectionString;
        }
        public JsonResponse GetCertDet(int shholderno, string compcode)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {

                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("P_COMPCODE", compcode);
                    param.Add("P_SHHOLDERNO", shholderno);
                    var sql = "SELECT * from certdet where  ShHolderNo = " + shholderno + " and compcode=" + compcode+"and certstatus in (1,2,3,4,5,20,21)";
                    List<ATTCertDet> certDetToReturn = connection.Query<ATTCertDet>(sql: sql, param: param, null, commandType: null)?.ToList();

                    foreach (ATTCertDet aTTCertDet in certDetToReturn)
                    {
                        if (aTTCertDet.CertStatus >= 2 && aTTCertDet.CertStatus <= 4)
                        {

                            var sql2 = "SELECT PSLNo,TranDt,EntryUser,remark as remarks FROM CERTSPL where certno = " + aTTCertDet.CertNo + "and status = 'A' and compcode = " + compcode;
                            ATTCertDet aTTCertDetPSL = connection.Query<ATTCertDet>(sql: sql2, param: null, null, commandType: null)?.FirstOrDefault();
                            aTTCertDet.pslno = aTTCertDetPSL.pslno;
                            aTTCertDet.TranDt = aTTCertDetPSL.TranDt;
                            aTTCertDet.entryuser = aTTCertDetPSL.entryuser;
                            aTTCertDet.Remarks = aTTCertDetPSL.Remarks;
                        }

                        if (aTTCertDet.DupliNo > 0)
                        {
                            var sql3 = "SELECT * From certSPL_clear WHERE certno= " + aTTCertDet.CertNo + " and  TranType=4  and compcode='" + compcode + "'order by ClearedDt ";
                            ATTCertDet aTTCertDetDupliNo = connection.Query<ATTCertDet>(sql: sql3, param: null, null, commandType: null)?.FirstOrDefault();
                            aTTCertDet.Remarks = aTTCertDet.Remarks??""+"Dup_Cer: " + aTTCertDetDupliNo.ClearedDt;
                        }
                        if(aTTCertDet.CertStatus ==20 || aTTCertDet.CertStatus==21)
                        {
                            aTTCertDet.Remarks = aTTCertDet.bo_acct_no;
                        }
                    }
                    if (certDetToReturn != null)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = certDetToReturn;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
                return response;
            }
        }

        public JsonResponse GetCertStatuses()
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {

                try
                {
                    connection.Open();

                    var sql = "SELECT * from CertStatus ";
                    List<ATTCertStatus> certStatusToReturn = connection.Query<ATTCertStatus>(sql: sql, param: null, null, commandType: null)?.ToList();


                    if (certStatusToReturn != null)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = certStatusToReturn;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = ATTMessages.NO_RECORDS_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
                return response;
            }
        }

        public JsonResponse UpdateCertificate(int shholderno, List<ATTCertDet> lisOfCertificates)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {

                try
                {
                    connection.Open();
                    foreach (ATTCertDet certDet in lisOfCertificates)
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("SHHOLDER_NO", shholderno);
                        param.Add("CERTNO", Convert.ToInt32(certDet.CertNo));
                        param.Add("CERTSTATUS", Convert.ToInt32(certDet.CertStatus));
                        response = connection.Query<JsonResponse>(sql: "UPDATE_HOLDER_CERTIFICATE", param: param, null, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
                return response;
            }
        }
    }
}
