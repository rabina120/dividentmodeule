
using Dapper;
using Entity.Common;
using Entity.DakhilTransfer;
using Entity.Parameter;
using Interface.DakhilTransfer;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.DakhilTransfer
{
    public class DakhilIndividualTransferEntryRepo : IDakhilIndividualTransferEntry
    {

        IOptions<ReadConfig> _connectionString;

        public DakhilIndividualTransferEntryRepo(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }

        public JsonResponse GetBuyerInformation(string CompCode, string BHolderNo, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_CompCode", CompCode);
                    parameters.Add("@P_HOLDERNO", BHolderNo);
                    parameters.Add("@P_USERNAME", UserName);
                    parameters.Add("@P_IP_ADDRESS", IPAddress);
                    parameters.Add("@P_DATE_NOW", DateTime.Now);
                    ATTDakhilBuyer dakhilBuyer = connection.Query<ATTDakhilBuyer>(sql: "GET_DAKHIL_BUYER_INFORMATION",
                        param: parameters, transaction: null, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                    if (dakhilBuyer != null)
                    {
                        if (dakhilBuyer.HolderLock == "Y")
                        {
                            response.IsSuccess = false;
                            response.Message = ATTMessages.SHARE_HOLDER.LOCK;
                        }
                        else
                        {
                            response.IsSuccess = true;
                            response.ResponseData = dakhilBuyer;
                        }
                        
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

        public JsonResponse GetBrokerList(string CompCode)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_CompCode", CompCode);
                    List<ATTBroker> brokerList = connection.Query<ATTBroker>(sql: "GET_ALL_BROKER_LIST",
                        param: parameters, transaction: null, commandType: CommandType.StoredProcedure)?.ToList();
                    if (brokerList.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = brokerList;
                    }
                    else
                    {
                        response.Message = "No Brokers Found !!!";
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

        public JsonResponse GetCertificateInformation(string CompCode, string CertificateNo, string SelectedAction, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_CompCode", CompCode);
                    parameters.Add("@P_CertificateNo", CertificateNo);
                    parameters.Add("@P_SelectedAction", SelectedAction);
                    parameters.Add("@P_USERNAME", UserName);
                    parameters.Add("@P_IP_ADDRESS", IP);
                    parameters.Add("@P_DATE_NOW", DateTime.Now);
                    ATTDakhilCertificate dakhilCertificate = connection.Query<ATTDakhilCertificate>(sql: "GET_DAKHIL_CERTIFICATE_DETAIL", param: parameters, transaction: null, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                    if (dakhilCertificate != null)
                    {
                        if (SelectedAction == "A")
                        {
                            switch (dakhilCertificate.CertStatus)
                            {
                                case 2:
                                    jsonResponse.Message = "Selected Certificate is Under Pledge!";
                                    break;
                                case 3:
                                    jsonResponse.Message = "Selected Certificate is Under Suspend.";
                                    break;
                                case 4:
                                    jsonResponse.Message = "Selected Certificate is Under Lost.";
                                    break;
                                case 5:
                                    jsonResponse.Message = "Selected Certificate is Already Dakhil Registered.";
                                    break;
                                case 6:
                                    jsonResponse.Message = "Selected Certificate is Already Splitted.";
                                    break;
                                case 7:
                                    jsonResponse.Message = "Selected Certificate is Already Consolidated.";
                                    break;
                                case 20:
                                    jsonResponse.Message = "Selected Certificate is On Demate Process.";
                                    break;
                                case 21:
                                    jsonResponse.Message = "Selected Certificate is Already Demated.";
                                    break;
                                default:
                                    jsonResponse.IsSuccess = true;
                                    jsonResponse.ResponseData = dakhilCertificate;
                                    break;
                            }
                        }
                        else if (SelectedAction == "U" || SelectedAction == "D")
                        {
                            switch (dakhilCertificate.CertStatus)
                            {
                                case 2:
                                    jsonResponse.Message = "Selected Certificate is Under Pledge!";
                                    break;
                                case 3:
                                    jsonResponse.Message = "Selected Certificate is Under Suspend.";
                                    break;
                                case 4:
                                    jsonResponse.Message = "Selected Certificate is Under Lost.";
                                    break;
                                default:
                                    jsonResponse.IsSuccess = true;
                                    jsonResponse.ResponseData = dakhilCertificate;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        jsonResponse.Message = ATTMessages.CERTIFICATE.NOT_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.HasError = true;
                    jsonResponse.ResponseData = ex;
                }
                return jsonResponse;
            }
        }

        public JsonResponse SaveDakhilTransfer(ATTDakhilTransfer DakhilTransferData, string SelectedAction, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@P_CompCode", DakhilTransferData.CompCode);
                        parameters.Add("@P_CertificateNo", DakhilTransferData.CertificateNo);
                        parameters.Add("@P_ShHolderNo", DakhilTransferData.ShHolderNo);
                        parameters.Add("@P_BHolderNo", DakhilTransferData.BHolderNo);
                        parameters.Add("@P_SrNoFrom", DakhilTransferData.SrNoFrom);
                        parameters.Add("@P_SrNoTo", DakhilTransferData.SrNoTo);
                        parameters.Add("@P_BrokerCode", DakhilTransferData.BrokerCode);
                        parameters.Add("@P_LetterNo", DakhilTransferData.LetterNo);
                        parameters.Add("@P_Charge", DakhilTransferData.Charge);
                        parameters.Add("@P_TradeType", DakhilTransferData.TradeType);
                        parameters.Add("@P_DakhilDate", DakhilTransferData.DakhilDate);
                        parameters.Add("@P_BHolderExist", DakhilTransferData.BHolderExist);
                        parameters.Add("@P_Remarks", DakhilTransferData.Remarks);
                        parameters.Add("@P_TranKitta", DakhilTransferData.TranKitta);
                        parameters.Add("@P_Transfered", 0);
                        parameters.Add("@P_UserName", UserName);
                        parameters.Add("@P_IP_ADDRESS", IP);
                        parameters.Add("@P_Date", DateTime.Now.ToString("yyyy-MM-dd"));
                        parameters.Add("@P_SelectedAction", SelectedAction);
                        parameters.Add("@P_DATE_NOW", DateTime.Now);

                        jsonResponse = connection.Query<JsonResponse>(sql: "POST_DAKHIL_TRANSFER", param: parameters,
                            transaction: tran, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                        if (jsonResponse.IsSuccess)
                            tran.Commit();
                        else tran.Rollback();

                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.HasError = true;
                    jsonResponse.ResponseData = ex;
                }
                return jsonResponse;
            }
        }
    }
}
