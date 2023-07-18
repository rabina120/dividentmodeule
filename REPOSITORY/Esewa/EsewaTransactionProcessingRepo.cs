using Dapper;
using Entity.Common;
using Entity.Dividend;
using Entity.Esewa;
using Interface.Esewa;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Repository.Esewa.EsewaHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Esewa
{
    public class EsewaTransactionProcessingRepo : IEsewaTransactionProcessing
    {
        IOptions<ReadConfig> _connectionString;
        private IConfiguration _configuration;
        string _keys;
        public EsewaTransactionProcessingRepo(IOptions<ReadConfig> connectionString, IConfiguration configuration)
        {
            _connectionString = connectionString;
            _configuration = configuration;
            _keys = "EsewaKeys";

        }
        public JsonResponse GetAllActiveBatch(string CompCode, string DivCode, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);
                    param.Add("@P_DivCode", DivCode);
                    param.Add("@P_UserName", UserName);
                    param.Add("@P_IPAddress", IPAddress);
                    param.Add("@P_Entry_Date", DateTime.Now);
                    var batchlists = connection.Query<int>(sql: "GetAllActiveBatchTP", param: param, null, commandType: CommandType.StoredProcedure).ToList();
                    if (batchlists.Count < 1)
                    {
                        response = new JsonResponse();
                        response.IsSuccess = false;
                        response.Message = "No Active Batch For Transaction";
                        response.IsValid = true;

                    }
                    else
                    {
                        response.IsSuccess = true;
                        response.ResponseData = batchlists;
                    }

                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.HasError = true;
                    response.ResponseData = ex;
                }
                return response;
            }
        }
        public JsonResponse CheckBatchStatus(string CompCode, string DivCode,string BatchID, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);
                    param.Add("@P_DivCode", DivCode);
                    param.Add("@P_batchid", BatchID);
                    param.Add("@P_UserName", UserName);
                    param.Add("@P_IPAddress", IPAddress);
                    param.Add("@P_Entry_Date", DateTime.Now);
                    response = connection.Query<JsonResponse>(sql: "CheckBatchStatusTP", param: param, null, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    if (response == null)
                    {
                    response = new JsonResponse();
                        throw new Exception("Batch is Not Ready For Trnasaction");
                    }
                    else
                    {
                        if (response.ResponseData.ToString().Contains(".")) response.IsValid = true;
                        response.ResponseData = response.ResponseData.ToString().Trim().Split('.')[0];
                    }

                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.HasError = true;
                    response.ResponseData = ex;
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        public JsonResponse TransactionProcessing(string DivCode, string CompCode, string BatchID, string BankID, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_COMPCODE", CompCode);
                    parameters.Add("@P_DIVCODE", DivCode);
                    parameters.Add("@P_BATCHID", BatchID);
                    parameters.Add("@P_USERNAME", UserName);
                    parameters.Add("@P_IP_ADDRESS", IPAddress);
                    parameters.Add("@P_ENTRY_DATE", DateTime.Now);
                    List<ATTEBatchProcessing> aTTEBatchProcessings = connection.Query<ATTEBatchProcessing>(sql: "GET_BATCHPROCESSING_DATA_TRANSACTION_PROCESSING", parameters, null, commandType: CommandType.StoredProcedure).ToList();
                    if (aTTEBatchProcessings.Count > 0)
                    {
                        List<ATTSourceBank> sourceBank = new List<ATTSourceBank>();
                        JsonResponse resBank=GetSourceBanks(CompCode, UserName, IPAddress, BankID);
                        if (resBank.IsSuccess) sourceBank = (List<ATTSourceBank>)resBank.ResponseData;
                        else throw new Exception("Cannot Load Source Bank Data!");
                        List<ATTEBatchProcessing> esewaReturnData = new List<ATTEBatchProcessing>();
                        JsonResponse res = EsewaAPIMethods.TransactionProcessing(aTTEBatchProcessings, sourceBank[0].SwiftCode, sourceBank[0].AccountNo);
                        if (!res.IsSuccess) { return res; }
                        else {  esewaReturnData = (List<ATTEBatchProcessing>)res.ResponseData; }

                        using (SqlTransaction tran = connection.BeginTransaction())
                        {
                            DataTable dt = new DataTable();

                            dt.Columns.Add("WarrantNo");
                            dt.Columns.Add("TransactionCode");
                            dt.Columns.Add("TransactionMessage");
                            dt.Columns.Add("TransactionDetail");
                            esewaReturnData.ForEach(x => dt.Rows.Add(x.sub_token, x.TransactionCode, x.TransactionMessage, x.TransactionDetail));
                            SqlCommand cmd = new SqlCommand("ESEWA_TRANSACTION_PROCESSING", connection);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Transaction = tran;
                            SqlParameter sqlParameter = cmd.Parameters.AddWithValue("@UDT", dt);
                            sqlParameter = cmd.Parameters.AddWithValue("@P_COMPCODE", CompCode);
                            sqlParameter = cmd.Parameters.AddWithValue("@P_DivCode", DivCode);
                            sqlParameter = cmd.Parameters.AddWithValue("@P_BATCHID", BatchID);
                            sqlParameter = cmd.Parameters.AddWithValue("@P_IP_ADDRESS", IPAddress);
                            sqlParameter = cmd.Parameters.AddWithValue("@P_USERNAME", UserName);
                            sqlParameter = cmd.Parameters.AddWithValue("@P_DATE_NOW", DateTime.Now);
                            sqlParameter.Direction = ParameterDirection.Input;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    if (reader.GetString(0).ToLower() == "true")
                                    {
                                        jsonResponse.IsSuccess = true;
                                        jsonResponse.Message = reader.GetString(1);
                                    }
                                    else
                                    {
                                        jsonResponse.Message = reader.GetString(1);
                                    }

                                }
                            }
                            if (jsonResponse.IsSuccess)
                                tran.Commit();
                            else
                                tran.Rollback();

                        }
                    }
                    else
                    {
                        jsonResponse = CloseBatch(DivCode, CompCode, BatchID, UserName, IPAddress, "NO DATA AVAILABLE FOR THE CURRENT DIVIDEND AND THE CURRENT BATCH !!! ");
                        jsonResponse.IsSuccess = false;
                        jsonResponse.Message = "NO DATA AVAILABLE FOR THE CURRENT DIVIDEND AND THE CURRENT BATCH !!! <br/> BATCH WILL BE CLOSED";
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

        public JsonResponse CloseBatch(string DivCode, string CompCode, string BatchID, string UserName, string IPAddress, string Remarks)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {


                        DynamicParameters param = new DynamicParameters();
                        param.Add("@P_CompCode", CompCode);
                        param.Add("@P_BATCHID", BatchID);
                        param.Add("@P_DivCode", DivCode);
                        param.Add("@P_UserName", UserName);
                        param.Add("@P_IPAddress", IPAddress);
                        param.Add("@P_Remarks", Remarks);
                        param.Add("@P_Entry_Date", DateTime.Now);

                        response = connection.Query<JsonResponse>(sql: "CloseBatch", param: param, transaction, commandType: CommandType.StoredProcedure).FirstOrDefault();
                        if (response.IsSuccess)
                            transaction.Commit();
                        else
                            transaction.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.HasError = true;
                    response.ResponseData = ex;
                }
                return response;
            }
        }

        

        public JsonResponse GetDividendList(string CompCode, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);
                    param.Add("@P_UserName", UserName);
                    param.Add("@P_IPAddress", IPAddress);
                    param.Add("@P_Entry_Date", DateTime.Now);
                    List<ATTDividend> aTTDividends = connection.Query<ATTDividend>(sql: "ESEWATRANSACTIONPROCESSING_GETDIVIDENDLIST", param: param, null, commandType: CommandType.StoredProcedure).ToList();
                    if (aTTDividends.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = aTTDividends;
                    }
                    else
                    {
                        response.Message = ATTMessages.NO_TABLES_FOUND;
                    }

                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.HasError = true;
                    response.ResponseData = ex;
                }
                return response;
            }
        }

        public JsonResponse GetSourceBanks(string CompCode,string UserName,string IPAddress,string BankID=null)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);
                    param.Add("@P_bankid", BankID);
                    param.Add("@P_UserName", UserName);
                    param.Add("@P_IPAddress", IPAddress);
                    param.Add("@P_Entry_Date", DateTime.Now);
                    List<ATTSourceBank> aTTDividends = connection.Query<ATTSourceBank>(sql: "ESEWATRANSACTIONPROCESSING_GETSOURCEBANK", param: param, null, commandType: CommandType.StoredProcedure).ToList();
                    if (aTTDividends.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = aTTDividends;
                    }
                    else
                    {
                        response.Message = ATTMessages.NO_TABLES_FOUND;
                    }

                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.HasError = true;
                    response.ResponseData = ex;
                }
                return response;
            }
        }

        #region oldcode
        //public JsonResponse TransactionProcessingTask(string DivCode, string CompCode, string BatchID, string BankUserName, string BankPassword, string SourceBankName, string SourceBankNumber, string UserName, string IPAddress)
        //{
        //    JsonResponse jsonResponse = new JsonResponse();
        //    using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
        //    {
        //        try
        //        {
        //            connection.Open();

        //            DynamicParameters parameters = new DynamicParameters();
        //            parameters.Add("@P_COMPCODE", CompCode);
        //            parameters.Add("@P_DIVCODE", DivCode);
        //            parameters.Add("@P_BATCHID", BatchID);
        //            parameters.Add("@P_USERNAME", UserName);
        //            parameters.Add("@P_IP_ADDRESS", IPAddress);
        //            parameters.Add("@P_ENTRY_DATE", DateTime.Now);
        //            List<ATTEBatchProcessing> aTTEBatchProcessings = connection.Query<ATTEBatchProcessing>(sql: "GET_BATCHPROCESSING_DATA_TRANSACTION_PROCESSING", parameters, null, commandType: CommandType.StoredProcedure).ToList();
        //            if (aTTEBatchProcessings.Count > 0)
        //            {
        //                List<Task> ListOfTasks = new List<Task>();
        //                for (int i = 0; i < aTTEBatchProcessings.Count; i = i++)
        //                {

        //                    Task task1 = Task.Run(() => { jsonResponse = ProcessTransactionIndividual(CompCode, DivCode, BatchID, IPAddress, UserName, aTTEBatchProcessings[i], BankUserName, BankPassword); });
        //                    ListOfTasks.Add(task1);
        //                    if (aTTEBatchProcessings.Count > i + 1)
        //                    {
        //                        Task task2 = Task.Run(() => { jsonResponse = ProcessTransactionIndividual(CompCode, DivCode, BatchID, IPAddress, UserName, aTTEBatchProcessings[i + 1], BankUserName, BankPassword); });
        //                        ListOfTasks.Add(task2);
        //                        i++;
        //                    }

        //                    Task.WaitAll(ListOfTasks.ToArray());
        //                }

        //            }
        //            else
        //            {
        //                jsonResponse = CloseBatch(DivCode, CompCode, BatchID, UserName, IPAddress, "NO DATA AVAILABLE FOR THE CURRENT DIVIDEND AND THE CURRENT BATCH !!! ");
        //                jsonResponse.IsSuccess = false;
        //                jsonResponse.Message = "NO DATA AVAILABLE FOR THE CURRENT DIVIDEND AND THE CURRENT BATCH !!! <br/> BATCH WILL BE CLOSED";
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            jsonResponse.IsSuccess = false;
        //            jsonResponse.HasError = true;
        //            jsonResponse.ResponseData = ex;
        //        }
        //        return jsonResponse;
        //    }
        //}
        //public JsonResponse TransactionProcessingT(string DivCode, string CompCode, string BatchID, string BankUserName, string BankPassword, string SourceBankName, string SourceBankNumber, string UserName, string IPAddress)
        //{
        //    JsonResponse jsonResponse = new JsonResponse();
        //    using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
        //    {
        //        try
        //        {
        //            connection.Open();

        //            DynamicParameters parameters = new DynamicParameters();
        //            parameters.Add("@P_COMPCODE", CompCode);
        //            parameters.Add("@P_DIVCODE", DivCode);
        //            parameters.Add("@P_BATCHID", BatchID);
        //            parameters.Add("@P_USERNAME", UserName);
        //            parameters.Add("@P_IP_ADDRESS", IPAddress);
        //            parameters.Add("@P_ENTRY_DATE", DateTime.Now);
        //            List<ATTEBatchProcessing> aTTEBatchProcessings = connection.Query<ATTEBatchProcessing>(sql: "GET_BATCHPROCESSING_DATA_TRANSACTION_PROCESSING", parameters, null, commandType: CommandType.StoredProcedure).ToList();
        //            if (aTTEBatchProcessings.Count > 0)
        //            {
        //                List<Task> ListOfTasks = new List<Task>();
        //                for (int i = 0; i < aTTEBatchProcessings.Count; i = i++)
        //                {

        //                    Task task1 = Task.Run(() => { jsonResponse = ProcessTransactionIndividual(CompCode, DivCode, BatchID, IPAddress, UserName, aTTEBatchProcessings[i], BankUserName, BankPassword); });
        //                    ListOfTasks.Add(task1);
        //                    if (aTTEBatchProcessings.Count > i + 1)
        //                    {
        //                        Task task2 = Task.Run(() => { jsonResponse = ProcessTransactionIndividual(CompCode, DivCode, BatchID, IPAddress, UserName, aTTEBatchProcessings[i + 1], BankUserName, BankPassword); });
        //                        ListOfTasks.Add(task2);
        //                        i++;
        //                    }

        //                    Task.WaitAll(ListOfTasks.ToArray());
        //                }

        //            }
        //            else
        //            {
        //                jsonResponse = CloseBatch(DivCode, CompCode, BatchID, UserName, IPAddress, "NO DATA AVAILABLE FOR THE CURRENT DIVIDEND AND THE CURRENT BATCH !!! ");
        //                jsonResponse.IsSuccess = false;
        //                jsonResponse.Message = "NO DATA AVAILABLE FOR THE CURRENT DIVIDEND AND THE CURRENT BATCH !!! <br/> BATCH WILL BE CLOSED";
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            jsonResponse.IsSuccess = false;
        //            jsonResponse.HasError = true;
        //            jsonResponse.ResponseData = ex;
        //        }
        //        return jsonResponse;
        //    }
        //}


        //    public JsonResponse ProcessTransactionIndividual(string CompCode, string DivCode, string BatchID, string IPAddress, string UserName, ATTEBatchProcessing aTTEBatchProcessing, string BankUserName, string BankPassword)
        //    {
        //        JsonResponse jsonResponse = new JsonResponse();
        //        var clientPrivateKey = _configuration.GetSection(_keys).GetSection("ClientPrivateKey").Value;
        //        var clientPublicKey = _configuration.GetSection(_keys).GetSection("ClientPublicKey").Value;
        //        var esewaPublicKey = _configuration.GetSection(_keys).GetSection("EsewaPublicKey").Value;
        //        var esewaPublicKeySig = _configuration.GetSection(_keys).GetSection("EsewaPublicKeySig").Value;
        //        var clinetId = _configuration.GetSection(_keys).GetSection("ClientId").Value;
        //        var data = string.Empty;
        //        ATTAccountDetails aTTAccount = new ATTAccountDetails();
        //        List<ATTTransctionDetails> transctionDetails = new List<ATTTransctionDetails>();
        //        aTTAccount.token = aTTEBatchProcessing.Token;
        //        ATTTransctionDetails aTTTransctionDetails = new ATTTransctionDetails();
        //        aTTTransctionDetails.payee_account_name = aTTEBatchProcessing.FullName;
        //        aTTTransctionDetails.payee_account_number = aTTEBatchProcessing.BankNo;
        //        aTTTransctionDetails.payee_bank_code = aTTEBatchProcessing.SwiftCode;
        //        aTTTransctionDetails.sub_token = aTTEBatchProcessing.sub_token;
        //        aTTTransctionDetails.amount = aTTEBatchProcessing.WarrantAmt;
        //        //aTTTransctionDetails.source_account_number = SourceBankNumber;
        //        //aTTTransctionDetails.source_bank_code = SourceBankName;
        //        aTTTransctionDetails.source_account_number = _configuration.GetSection(_keys).GetSection("BankAccNo").Value;
        //        aTTTransctionDetails.source_bank_code = _configuration.GetSection(_keys).GetSection("BankCode").Value;
        //        aTTTransctionDetails.note = aTTEBatchProcessing.TransactionRemarks;
        //        transctionDetails.Add(aTTTransctionDetails);

        //        aTTAccount.transaction_details = transctionDetails;

        //        string serializedObject = JsonConvert.SerializeObject(aTTAccount);
        //        try
        //        {

        //            var alphaNumeric = AlphaNumericString.getAlphaNumericString(32);
        //            byte[] secrect = EncryptDecryptSecreteKey.getSecretKey(alphaNumeric);

        //            string base64EncodedSecretKey = EncryptDecryptSecreteKey.keyToString(secrect);
        //            byte[] encryptedSecretKey = RSAEncryptDecrypt.encrypt(base64EncodedSecretKey, esewaPublicKey);
        //            string finalSecretKey = EncryptDecryptSecreteKey.base64Encode(encryptedSecretKey);
        //            string encryptedData = EncryptDecryptSecreteKey.encrypt(serializedObject, secrect);
        //            string signature = SignaturePayload.generateSignature(encryptedData, clientPrivateKey);
        //            ATTEncryptedDetails output = new ATTEncryptedDetails();
        //            IRestResponse response = CallEsewaApiTransactionProcessing(encryptedData, signature, finalSecretKey, clinetId, BankUserName, BankPassword);
        //            if (response.IsSuccessful)
        //            {
        //                output = JsonConvert.DeserializeObject<ATTEncryptedDetails>(response.Content);

        //            }
        //            else
        //            {

        //                output.error = true;
        //                output.message = response.Content;
        //                output.code = "0";
        //            }
        //            if (!output.error)
        //            {
        //                bool valid = SignaturePayload.verifySignature(output.data, esewaPublicKeySig, EncryptDecryptSecreteKey.base64Decode(output.signature));
        //                if (valid)
        //                {
        //                    byte[] decodedSecretKey = EncryptDecryptSecreteKey.base64Decode(output.secret_key);
        //                    string plainSecretKey = RSAEncryptDecrypt.decrypt(decodedSecretKey, clientPrivateKey);
        //                    byte[] outPutSecetKey = EncryptDecryptSecreteKey.getSecretKey(plainSecretKey);
        //                    string finalOutput = EncryptDecryptSecreteKey.decrypt(output.data, outPutSecetKey);

        //                    ATTEsewaResponse aTTEsewaResponse = JsonConvert.DeserializeObject<ATTEsewaResponse>(finalOutput);
        //                    if (!aTTEsewaResponse.Error)
        //                    {
        //                        if (aTTEBatchProcessing.Token == aTTEsewaResponse.Token && aTTEBatchProcessing.sub_token == aTTEsewaResponse.transaction_details[0].sub_token)
        //                        {
        //                            aTTEBatchProcessing.TransactionCode = aTTEsewaResponse.Code;
        //                            aTTEBatchProcessing.TransactionMessage = aTTEsewaResponse.transaction_details[0].message;
        //                            aTTEBatchProcessing.TransactionDetail = aTTEsewaResponse.transaction_details[0].status;
        //                        }
        //                        //jsonResponse = _transctionProcessing.SaveOutputRemarks(aTTEsewaResponse, CompCode, BatchNo, aTTAccount.token);
        //                    }
        //                    else
        //                    {
        //                        aTTEBatchProcessing.TransactionCode = aTTEsewaResponse.Code;
        //                        aTTEBatchProcessing.TransactionMessage = aTTEsewaResponse.Message;
        //                        aTTEBatchProcessing.TransactionDetail = aTTEsewaResponse.Details;
        //                    }
        //                    //else
        //                    //{
        //                    //    //jsonResponse = _transctionProcessing.SaveErrorRemarks(aTTEsewaResponse.Token, null, aTTEsewaResponse);
        //                    //    jsonResponse.IsSuccess = !aTTEsewaResponse.Error;
        //                    //    jsonResponse.Message = aTTEsewaResponse.Message;
        //                    //}
        //                }
        //                else
        //                {
        //                    output.message = "SIGNATURE IS NOT VALID.";
        //                    //jsonResponse = _transctionProcessing.SaveErrorRemarks(aTTAccount.token, output, null);
        //                    jsonResponse.IsSuccess = false;
        //                    jsonResponse.Message = output.message;
        //                    return jsonResponse;
        //                }
        //            }
        //            else
        //            {
        //                //jsonResponse = _transctionProcessing.SaveErrorRemarks(aTTAccount.token, output, null);
        //                jsonResponse.IsSuccess = !output.error;
        //                jsonResponse.Message = output.message;
        //                return jsonResponse;

        //            }


        //        }
        //        catch (Exception ex)
        //        {
        //            //UserName = _loggedInUser.GetUserName();
        //            DateTime Date = DateTime.Now;
        //            //_logDetails.InsertLogDetails(BatchNo, ex.Message, UserName);
        //            jsonResponse.Message = ex.Message;
        //            jsonResponse.IsSuccess = false;
        //            jsonResponse.HasError = true;
        //            return jsonResponse;
        //        }
        //        using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
        //        {
        //            using (SqlTransaction tran = connection.BeginTransaction())
        //            {


        //                SqlCommand cmd = new SqlCommand("ESEWA_TRANSACTION_PROCESSING_INDIVIDUAL", connection);
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Transaction = tran;
        //                SqlParameter sqlParameter = cmd.Parameters.AddWithValue("@P_COMPCODE", CompCode);
        //                sqlParameter = cmd.Parameters.AddWithValue("@P_DivCode", DivCode);
        //                sqlParameter = cmd.Parameters.AddWithValue("@P_BATCHID", BatchID);
        //                sqlParameter = cmd.Parameters.AddWithValue("@P_IP_ADDRESS", IPAddress);
        //                sqlParameter = cmd.Parameters.AddWithValue("@P_USERNAME", UserName);
        //                sqlParameter = cmd.Parameters.AddWithValue("@P_DATE_NOW", DateTime.Now);
        //                sqlParameter = cmd.Parameters.AddWithValue("@P_WarrantNo", aTTEBatchProcessing.WarrantNo);
        //                sqlParameter = cmd.Parameters.AddWithValue("@P_TransactionCode", aTTEBatchProcessing.TransactionCode);
        //                sqlParameter = cmd.Parameters.AddWithValue("@P_TransactionMessage", aTTEBatchProcessing.TransactionMessage);
        //                sqlParameter = cmd.Parameters.AddWithValue("@P_TransactionDetail", aTTEBatchProcessing.TransactionDetail);
        //                sqlParameter.Direction = ParameterDirection.Input;
        //                using (SqlDataReader reader = cmd.ExecuteReader())
        //                {
        //                    if (reader.Read())
        //                    {
        //                        if (reader.GetString(0).ToLower() == "true")
        //                        {
        //                            jsonResponse.IsSuccess = true;
        //                            jsonResponse.Message = reader.GetString(1);
        //                        }
        //                        else
        //                        {
        //                            jsonResponse.Message = reader.GetString(1);
        //                        }

        //                    }
        //                }
        //                if (jsonResponse.IsSuccess)
        //                    tran.Commit();
        //                else
        //                    tran.Rollback();

        //            }
        //        }
        //        return jsonResponse;
        //    }
        #endregion
    }
}
