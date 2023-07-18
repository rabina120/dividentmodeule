

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
    public class CERTIFICATEDISTRIBUTIONENTRYREPO : ICERTIFICATEDISTRIBUTIONENTRY
    {
        IOptions<ReadConfig> connectionString;

        public CERTIFICATEDISTRIBUTIONENTRYREPO(IOptions<ReadConfig> _connectionString)
        {
            this.connectionString = _connectionString;
        }

        ///LOAD HOLDER INFORMATION
        public JsonResponse GET_SHHOLDER_DISTRIBUTE(string CompCode, string ShholderNo, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@COMPCODE", CompCode);
                    param.Add("@SHHOLDERNO", ShholderNo);
                    param.Add("@P_IP_ADDRESS", IP);
                    param.Add("@P_USERNAME", UserName);
                    param.Add("@P_DATE_NOW", DateTime.Now);

                    ATTShHolder shHolder = connection.Query<ATTShHolder>("GET_SHHOLDER_DISTRIBUTE", param, commandType: CommandType.StoredProcedure)?.FirstOrDefault();

                    if (shHolder != null)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = shHolder;
                    }
                    else
                    {
                        jsonResponse.Message = "No Such Holder Exists !!!";
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


        ///save certificate ACTION 'S' ---DELETE ACTION 'D'
        public JsonResponse SaveDistributionCertificate(List<ATTCertDet> certificateList, string compCode, string certno, string selectedAction, string DistDate, string Username, string IP)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        try
                        {
                            DynamicParameters parameters = new DynamicParameters();
                            parameters.Add("@COMPCODE", compCode);
                            parameters.Add("@USERNAME", Username);
                            parameters.Add("@CERTNO", certno);
                            parameters.Add("@ACTION", selectedAction);
                            parameters.Add("@DISTDATE", DistDate);
                            parameters.Add("@P_IP_ADDRESS", IP);
                            parameters.Add("@P_DATE_NOW", DateTime.Now);

                            int srnno = 1;
                            foreach (ATTCertDet cert in certificateList)
                            {
                                parameters.Add("@CERTNO", cert.CertNo);
                                connection.Query("SAVE_CERT_DISTRIBUTION", parameters, tran, commandType: CommandType.StoredProcedure);
                                srnno++;
                            }
                            response.IsSuccess = true;

                            if (selectedAction == "A")
                            {
                                response.Message = "Certificate Have Been Distributed";
                            }
                            else
                            {
                                response.Message = "Certificate Have Been Deleted";
                            }
                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            response.Message = ex.Message;
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                }
            }
            return response;
        }

        ///load certificates FOR ADD 'A' DELETE 'D'
        public JsonResponse GET_SHHOLDER_CERTDISTRIBUTE(string CompCode, int ShholderNo, string Action)
        {
            JsonResponse jsonResponse = new JsonResponse();
            ATTCERTIFICATE objatt = new ATTCERTIFICATE();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@COMPCODE", CompCode);
                    param.Add("@SHHOLDERNO", ShholderNo);
                    param.Add("@Action", Action);
                    List<ATTCERTIFICATE> lstcert = connection.Query<ATTCERTIFICATE>("GET_SHHOLDER_CERTDISTRIBUTE", param, commandType: CommandType.StoredProcedure)?.ToList();

                    if (lstcert.Count > 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.Message = "Certificate found";
                        jsonResponse.ResponseData = lstcert;
                    }
                    else
                    {
                        jsonResponse.Message = "No Cerificates to Distribute.";
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.Message = ex.Message;
                }
                return jsonResponse;
            }
        }
    }
}





