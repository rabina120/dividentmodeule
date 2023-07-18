


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
    public class CertificateEntryRepo : ICertificateEntry
    {
        IOptions<ReadConfig> _connectionString;

        public CertificateEntryRepo(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }

        public JsonResponse GetShHolderInformation(string ShHolderNo, string CompCode, string SelectedAction, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_CompCode", CompCode);
                    parameters.Add("@P_HOLDERNO", ShHolderNo);
                    parameters.Add("@P_SELECTED_ACTION", SelectedAction);
                    parameters.Add("@P_USERNAME", UserName);
                    parameters.Add("@P_IPADDRESS", IPAddress);
                    parameters.Add("@P_ENTRYDATE", DateTime.Now);
                    ATTCertificateEntryShHolder shholder = connection.Query<ATTCertificateEntryShHolder>(sql: "CERTIFICATEENTRYREPO_GETSHHOLDERINFORMATION",
                        param: parameters, transaction: null, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                    if (shholder != null)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = shholder;

                        ATTCertificateEntryShHolder aTTCertificateEntryShHolder = connection.Query<ATTCertificateEntryShHolder>(sql: "CERTIFICATEENTRYREPO_GETSHHOLDERINFORMATION",
                            param: parameters, transaction: null, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                    }
                    else
                    {
                        response.Message = ATTMessages.SHARE_HOLDER.NOT_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.HasError = true;
                    response.ResponseData = ex;
                }
            }
            return response;
        }

        public JsonResponse SearchCertificate(string ShHolderNoFrom, string ShHolderNoTo, string CertificateNoFrom, string CertificateNoTo, string SerialNoFrom, string SerialNoTo, string ShareKitttaFrom, string ShareKitttaTo, string CompCode, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_ShHolderNoFrom", ShHolderNoFrom);
                    parameters.Add("@P_ShHolderNoTo", ShHolderNoTo);
                    parameters.Add("@P_CertificateNoFrom", CertificateNoFrom);
                    parameters.Add("@P_CertificateNoTo", CertificateNoTo);
                    parameters.Add("@P_SerialNoFrom", SerialNoFrom);
                    parameters.Add("@P_SerialNoTo", SerialNoTo);
                    parameters.Add("@P_ShareKitttaFrom", ShareKitttaFrom);
                    parameters.Add("@P_ShareKitttaTo", ShareKitttaTo);
                    parameters.Add("@P_CompCode", CompCode);
                    parameters.Add("@P_USERNAME", UserName);
                    parameters.Add("@P_IPADDRESS", IPAddress);
                    parameters.Add("@P_ENTRYDATE", DateTime.Now);
                    //string certificate = connection.Query<string>(sql: "CERTIFICATEENTRYREPO_SEARCHCERTIFICATE",
                    //    param: parameters, transaction: null, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    List<ATTCertificateEntryCertificate> certificate = connection.Query<ATTCertificateEntryCertificate>(sql: "CERTIFICATEENTRYREPO_SEARCHCERTIFICATE",
                        param: parameters, transaction: null, commandType: CommandType.StoredProcedure).ToList();
                    if (certificate != null)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = certificate;
                    }
                    else
                    {
                        response.Message = ATTMessages.SHARE_HOLDER.NOT_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.HasError = true;
                    response.ResponseData = ex;
                }
            }
            return response;
        }

        public JsonResponse SaveCertificate(string CompCode, string SelectedAction, string ShHolderNo, string CertificateNo, string ShareType, string ShareKitta,
            string CertificateIssuedDate, string CertificateType, string StartSerialNo, string EndSerialNo, string ShOwnerType, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {

                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@P_CompCode", CompCode);
                        parameters.Add("@P_USERNAME", UserName);
                        parameters.Add("@P_IPADDRESS", IPAddress);
                        parameters.Add("@P_SelectedAction", SelectedAction);
                        parameters.Add("@P_ShHolderNo", ShHolderNo);
                        parameters.Add("@P_CertificateNo", CertificateNo);
                        parameters.Add("@P_ShareType", ShareType);
                        parameters.Add("@P_ShareKitta", ShareKitta);
                        parameters.Add("@P_CertificateIssuedDate", CertificateIssuedDate);
                        parameters.Add("@P_CertificateType", CertificateType);
                        parameters.Add("@P_StartSerialNo", StartSerialNo);
                        parameters.Add("@P_EndSerialNo", EndSerialNo);
                        parameters.Add("@P_ShOwnerType", ShOwnerType);
                        parameters.Add("@P_ENTRYDATE", DateTime.Now);
                        //string certificate = connection.Query<string>(sql: "CERTIFICATEENTRYREPO_SEARCHCERTIFICATE",
                        //    param: parameters, transaction: null, commandType: CommandType.StoredProcedure).FirstOrDefault();
                        response = connection.Query<JsonResponse>(sql: "CERTIFICATEENTRYREPO_SAVECERTIFICATE",
                            param: parameters, transaction: transaction, commandType: CommandType.StoredProcedure).SingleOrDefault();
                        if (response.IsSuccess)
                        {
                            transaction.Commit();
                        }
                        else
                        {
                            transaction.Rollback();
                        }
                    }
                    catch (Exception ex)
                    {
                        response.IsSuccess = false;
                        response.HasError = true;
                        response.ResponseData = ex;
                    }


                }

            }
            return response;
        }
    }
}
