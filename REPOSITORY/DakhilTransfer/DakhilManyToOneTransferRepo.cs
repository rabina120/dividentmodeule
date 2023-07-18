

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
    public class DakhilManyToOneTransferRepo : IDakhilManyToOneTransferEntry
    {
        IOptions<ReadConfig> _connectionString;

        public DakhilManyToOneTransferRepo(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }
        public JsonResponse GetBuyerInformation(string CompCode, string BHolderNo, string UserName, string IP)
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
                    parameters.Add("@P_UserName", UserName);
                    parameters.Add("@P_IP_ADDRESS", IP);
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
                        response.Message = "No Holder Found !!!";
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
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

        public JsonResponse GetSellerCertificateInformation(string CompCode, string CertificateNo, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_CompCode", CompCode);
                    parameters.Add("@P_CertificateNo", CertificateNo);
                    parameters.Add("@P_USERNAME", UserName);
                    parameters.Add("@P_IP_ADDRESS", IPAddress);
                    parameters.Add("@P_ENTRY_DATE", DateTime.Now);
                    ATTDakhilSellerInformation sellerInformation = connection.Query<ATTDakhilSellerInformation>(sql: "GET_SELLER_CERTIFICATE_INFORMATION_DAKHIL_TRANSFER",
                        param: parameters, transaction: null, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                    if (sellerInformation != null)
                    {
                        switch (sellerInformation.CertStatus)
                        {
                            case 2:
                                response.Message = "This Certificate is Under Pledge.";
                                break;
                            case 3:
                                response.Message = "This Certificate is Under Suspend.";
                                break;
                            case 4:
                                response.Message = "This Certificate is Under Lost.";
                                break;
                            case 5:
                                response.Message = "This Certificate is Already Dakhil Registered.";
                                break;
                            case 6:
                                response.Message = "This Certificate is Splitted.";
                                break;
                            case 7:
                                response.Message = "This Certificate is Consolidated.";
                                break;
                            case 20:
                                response.Message = "This Certifcate is Under Demate Process.";
                                break;
                            case 21:
                                response.Message = "This Certificate is Already Demated.";
                                break;
                            default:
                                response.IsSuccess = true;
                                response.ResponseData = sellerInformation;
                                break;
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

        public JsonResponse GetMaxRegNo(string CompCode)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_CompCode", CompCode);
                    int? maxRegNo = connection.Query<int>(sql: "GET_MAX_REGNO_DAKHIL",
                        param: parameters, transaction: null, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                    response.IsSuccess = true;
                    response.ResponseData = maxRegNo;
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

        public JsonResponse SaveBatchDakhilTransfer(string CompCode, string BuyerHolderNo, List<ATTDakhilSellerInformation> sellers, int? Letter, int? TradeType,
            string Broker, string DakhilDate, string UserName, string IP)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        DataTable dt = new DataTable();

                        dt.Columns.Add("CertNo");
                        dt.Columns.Add("ShKitta");
                        dt.Columns.Add("ShHolderNo");
                        dt.Columns.Add("SrNoFrom");
                        dt.Columns.Add("SrNoTo");
                        dt.Columns.Add("RegNo");
                        dt.Columns.Add("TranNo");

                        sellers.ForEach(x => dt.Rows.Add(x.CertNo, x.ShKitta, x.ShHolderNo, x.SrNoFrom, x.SrNoTo, x.RegNo, x.TranNo));
                        SqlCommand cmd = new SqlCommand("POST_BATCH_DAKHIL_TRANSFER", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Transaction = tran;
                        SqlParameter param = cmd.Parameters.AddWithValue("@udt", dt);
                        param = cmd.Parameters.AddWithValue("@P_COMPCODE", CompCode);
                        param = cmd.Parameters.AddWithValue("@P_LETTER", Letter);
                        param = cmd.Parameters.AddWithValue("@P_TRADETYPE", TradeType);
                        param = cmd.Parameters.AddWithValue("@P_BROKER", Broker);
                        param = cmd.Parameters.AddWithValue("@P_DAKHILDATE", DakhilDate);
                        param = cmd.Parameters.AddWithValue("@P_BUYERHOLDERNO", BuyerHolderNo);
                        param = cmd.Parameters.AddWithValue("@P_USERNAME", UserName);
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
                                    response.Message = "Dakhil Transfer Successfully !!!";
                                }
                                else
                                {
                                    response.Message = "Failed To Dakhil Transfer !!!";
                                }

                            }
                        }
                        if (response.IsSuccess)
                            tran.Commit();
                        else
                            tran.Rollback();

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
    }
}
