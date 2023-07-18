

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
    public class CERTIFICATEREPO : ICERTIFICATE
    {

        IOptions<ReadConfig> connectionString;
        public CERTIFICATEREPO(IOptions<ReadConfig> _connectionString)
        {
            this.connectionString = _connectionString;
        }
        public JsonResponse GetCertInformation(string CertNo, string compcode)
        {
            JsonResponse jsonResponse = new JsonResponse();
            ATTCERTIFICATE objatt = new ATTCERTIFICATE();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@CertNo", CertNo);
                    param.Add("@CompCode", compcode);
                    ATTCERTIFICATE lstcertificate = connection.Query<ATTCERTIFICATE>("GetCertDetail", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    if (lstcertificate != null)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.Message = "Certificate found";
                        jsonResponse.ResponseData = lstcertificate;
                    }
                    else
                    {
                        jsonResponse.Message = "No such Certificate found.";
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

        public JsonResponse LoadCertificateTable(string CertNo, string compcode)
        {
            JsonResponse jsonResponse = new JsonResponse();

            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@CertNo", CertNo);
                    param.Add("@CompCode", compcode);
                    List<ATTDAKHILTRANSFER> lstDAKHIL = connection.Query<ATTDAKHILTRANSFER>("LoadCertificateTable", param, commandType: CommandType.StoredProcedure)?.ToList();

                    if (lstDAKHIL.Count > 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.Message = "Certificate found";
                        jsonResponse.ResponseData = lstDAKHIL;
                    }
                    else
                    {
                        jsonResponse.Message = "No Certificate Records found.";
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
