using Entity.Common;
using Entity.Security;
using RestSharp;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using ENTITY.FundTransfer;
using ENTITY.FundTransfer.Esewa;

namespace REPOSITORY.FundTransfer.EsewaHelper
{
    public static class EsewaAPIMethods
    {



        private static EsewaAPIKeys ReadConfig()
        {
            using (StreamReader r = new StreamReader(Environment.CurrentDirectory + "\\Keys\\esewaKeys.json"))
            {
                string json = r.ReadToEnd();
                var source = System.Text.Json.JsonSerializer.Deserialize<EsewaAPIKeys>(json);
                return source;
            }
        }

        public static JsonResponse GetBankDetailsEsewa()
        {

            string EsewaUrl = ReadConfig().Url;
            var UserName = ReadConfig().UserName;
            var Password = ReadConfig().Password;
            var client = new RestClient($"{EsewaUrl}/api/banks");
            var request = new RestRequest(Method.GET);
            var byteArray = Encoding.ASCII.GetBytes($"{UserName}:{Password}");
            var clientAuthrizationHeader = new AuthenticationHeaderValue("Basic",
                                                          Convert.ToBase64String(byteArray));

            var auth = clientAuthrizationHeader.ToString();

            request.AddHeader("Authorization", auth);
            request.AddHeader("Content-Type", "application/json");
            IRestResponse response = client.Execute(request);
            ATTEsewaBanks aTTBanks = JsonConvert.DeserializeObject<ATTEsewaBanks>(response.Content);
            JsonResponse jsonResponse = new JsonResponse();
            if (aTTBanks != null)
            {
                aTTBanks.banks.RemoveAt(0);
                jsonResponse.ResponseData = aTTBanks;
                jsonResponse.IsSuccess = true;
                jsonResponse.Message = "Bank Loaded!!";
            }
            else
            {
                jsonResponse.Message = "FAILED TO CALL BANKS API.";
            }
            return jsonResponse;

        }

        public static JsonResponse AccountValidation(ATTEBatchProcessing aTTEBatchProcessings, string username, string password)
        {
            JsonResponse jsonResponse = new JsonResponse();

            try
            {

                var clientPrivateKey = ReadConfig().ClientKeys.ClientPrivateKey;
                var clientPrivateKeySig = ReadConfig().SignatureKeys.ClientPrivateKey;
                var clientPublicKey = ReadConfig().ClientKeys.ClientPublicKey;
                var esewaPublicKey = ReadConfig().EsewaKeys.PublicKey;
                var esewaPublicKeySig = ReadConfig().EsewaKeys.SignatureKey;
                var BankUserName = ReadConfig().UserName;
                var BankPassword = ReadConfig().Password;

                //if (username != BankUserName) return new JsonResponse() { Message = "Invalid Username" };
                //if (password != BankPassword) return new JsonResponse() { Message = "Invalid Password" };

                var clinetId = ReadConfig().ClientID;




                string data = string.Empty;

                List<ATTTransctionDetails> aTTransctionDetails = new List<ATTTransctionDetails>();

                aTTransctionDetails.Add(
                      new ATTTransctionDetails()
                      {
                          payee_bank_code = aTTEBatchProcessings.SwiftCode,
                          sub_token = aTTEBatchProcessings.Token,
                          payee_account_name = aTTEBatchProcessings.FullName,
                          payee_account_number = aTTEBatchProcessings.BankNo
                      });

                ATTAccountDetails aTTAccountDetails = new ATTAccountDetails()
                {
                    token = aTTEBatchProcessings.sub_token,
                    transaction_details = aTTransctionDetails
                };

                data = JsonConvert.SerializeObject(aTTAccountDetails);
                data.LogData("AccountValidation Request for Batch " + aTTEBatchProcessings.BatchID + " Compcode: " + aTTEBatchProcessings.Compcode);
                //BankUserName = "accounts@f1soft.com";
                //BankPassword = "corporate@222";
                // BankUserName = _configuration.GetSection(_keys).GetSection("UserName").Value;
                //BankPassword = _configuration.GetSection(_keys).GetSection("Password").Value;
                var alphaNumeric = AlphaNumericString.getAlphaNumericString(32);
                byte[] secrect = EncryptDecryptSecreteKey.getSecretKey(alphaNumeric);

                string base64EncodedSecretKey = EncryptDecryptSecreteKey.keyToString(secrect);
                byte[] encryptedSecretKey = RSAEncryptDecrypt.encrypt(base64EncodedSecretKey, esewaPublicKey);
                string finalSecretKey = EncryptDecryptSecreteKey.base64Encode(encryptedSecretKey);
                string encryptedData = EncryptDecryptSecreteKey.encrypt(data, secrect);
                string signature = SignaturePayload.generateSignature(encryptedData, clientPrivateKeySig);
                var response1 = CallEsewaApiAccountValidation(encryptedData, signature, finalSecretKey, clinetId, BankUserName, BankPassword);
                Regex reg = new Regex("^((?!\\<(|\\/)[a-z][a-z0-9]*>).)*$");
                JsonConvert.SerializeObject(response1.Content).LogData("AccountValidation Rest API Response Content Object for Batch " + aTTEBatchProcessings.BatchID + " Compcode: " + aTTEBatchProcessings.Compcode);
                JsonConvert.SerializeObject(response1.StatusCode).LogData("AccountValidation Rest API Response Status Code Object for Batch " + aTTEBatchProcessings.BatchID + " Compcode: " + aTTEBatchProcessings.Compcode);
                JsonConvert.SerializeObject(response1.ResponseStatus).LogData("AccountValidation Rest API Response Status Object for Batch " + aTTEBatchProcessings.BatchID + " Compcode: " + aTTEBatchProcessings.Compcode);
                Match match = reg.Match(response1.Content);

                List<ATTAccountValidationResponse> aTTEsewaResponse = new List<ATTAccountValidationResponse>();
                ATTEsewaResponse esewaResponse = new ATTEsewaResponse();
                ATTEncryptedDetails output = new ATTEncryptedDetails();
                if (match.Success)
                {
                    if (response1.IsSuccessful)
                    {
                        output = JsonConvert.DeserializeObject<ATTEncryptedDetails>(response1.Content);
                    }
                    else
                    {
                        jsonResponse.IsSuccess = true;
                        ATTEsewaResponse response = new ATTEsewaResponse();
                        aTTEBatchProcessings.ValidateMessage = response1.ErrorMessage;
                        aTTEBatchProcessings.ValidAccount = false;
                        aTTEBatchProcessings.ValidateCode = response1.StatusCode.ToString();
                        JsonConvert.SerializeObject(aTTEsewaResponse).LogData("AccountValidation Error  Response for Batch " + aTTEBatchProcessings.BatchID + " Compcode: " + aTTEBatchProcessings.Compcode);

                        jsonResponse.ResponseData = aTTEBatchProcessings;
                        return jsonResponse;
                    }
                }
                else
                {
                    if (response1.StatusCode == 0)
                    {
                        jsonResponse.IsSuccess = true;
                        ATTEsewaResponse response = new ATTEsewaResponse();
                        aTTEBatchProcessings.ValidateMessage = response1.ErrorMessage;
                        aTTEBatchProcessings.ValidAccount = false;
                        aTTEBatchProcessings.ValidateCode = response1.StatusCode.ToString();
                        jsonResponse.ResponseData = aTTEBatchProcessings;
                        return jsonResponse;
                    }
                    else
                    {
                        output.error = true; output.message = "Invalid Credentials Supplied!!";

                        JsonConvert.SerializeObject(output).LogData("AccountValidation Response Match Error  for Batch " + aTTEBatchProcessings.BatchID + " Compcode: " + aTTEBatchProcessings.Compcode);
                    }
                }

                if (!output.error)
                {
                    bool valid = SignaturePayload.verifySignature(output.data, esewaPublicKeySig, EncryptDecryptSecreteKey.base64Decode(output.signature));
                    jsonResponse.IsSuccess = true;
                    if (valid)
                    {
                        byte[] decodedSecretKey = EncryptDecryptSecreteKey.base64Decode(output.secret_key);
                        string plainSecretKey = RSAEncryptDecrypt.decrypt(decodedSecretKey, clientPrivateKey);
                        byte[] outPutSecetKey = EncryptDecryptSecreteKey.getSecretKey(plainSecretKey);
                        string finalOutput = EncryptDecryptSecreteKey.decrypt(output.data, outPutSecetKey);
                        if (finalOutput.StartsWith("["))
                        {

                            aTTEsewaResponse = JsonConvert.DeserializeObject<List<ATTAccountValidationResponse>>(finalOutput);
                        }
                        else
                        {
                            esewaResponse = JsonConvert.DeserializeObject<ATTEsewaResponse>(finalOutput);

                            aTTEsewaResponse.Add(new ATTAccountValidationResponse()
                            {
                                message = esewaResponse.Message,
                                code = esewaResponse.Code,
                                error = true

                            });
                        }
                    }
                }
                else
                {
                    jsonResponse.IsSuccess = true;
                    jsonResponse.Message = "Account Validation Failed";
                    aTTEBatchProcessings.ValidateMessage = jsonResponse.Message;
                    aTTEBatchProcessings.ValidAccount = false;
                    aTTEBatchProcessings.ValidateCode = "0";
                    aTTEBatchProcessings.Percentage = "0";
                    jsonResponse.ResponseData = aTTEBatchProcessings;
                    jsonResponse.ResponseData2 = output.data;
                    JsonConvert.SerializeObject(output).LogData("AccountValidation Response Output Error  for Batch " + aTTEBatchProcessings.BatchID + " Compcode: " + aTTEBatchProcessings.Compcode);

                    return jsonResponse;
                }


                //new method of updating response
                aTTEBatchProcessings.ValidateMessage = aTTEsewaResponse[0].message;
                aTTEBatchProcessings.ValidAccount = !aTTEsewaResponse[0].error;
                aTTEBatchProcessings.ValidateCode = aTTEsewaResponse[0].code;
                aTTEBatchProcessings.Percentage = aTTEsewaResponse[0].percentage.ToString();
                jsonResponse.ResponseData = aTTEBatchProcessings;
                JsonConvert.SerializeObject(aTTEsewaResponse).LogData("AccountValidation Response for Batch " + aTTEBatchProcessings.BatchID + " Compcode: " + aTTEBatchProcessings.Compcode);

                return jsonResponse;
            }
            catch (Exception ex)
            {
                jsonResponse.IsSuccess = true;
                aTTEBatchProcessings.ValidateMessage = "Internal Error";
                aTTEBatchProcessings.ValidAccount = false;
                aTTEBatchProcessings.ValidateCode = "0";
                aTTEBatchProcessings.Percentage = "0";
                jsonResponse.ResponseData = aTTEBatchProcessings;
                ex.LogErrorToText("Account Validation/EsewaApiMethods");
                JsonConvert.SerializeObject(ex).LogData("AccountValidation Exception Caught on Batch: " + aTTEBatchProcessings.BatchID + " Compcode: " + aTTEBatchProcessings.Compcode);
                return jsonResponse;

            }
        }
        public static JsonResponse AccountValidation(List<ATTEBatchProcessing> aTTEBatchProcessings, string username, string password)
        {
            var clientPrivateKey = ReadConfig().ClientKeys.ClientPrivateKey;
            var clientPrivateKeySig = ReadConfig().SignatureKeys.ClientPrivateKey;
            var clientPublicKey = ReadConfig().ClientKeys.ClientPublicKey;
            var esewaPublicKey = ReadConfig().EsewaKeys.PublicKey;
            var esewaPublicKeySig = ReadConfig().EsewaKeys.SignatureKey;
            var BankUserName = ReadConfig().UserName;
            var BankPassword = ReadConfig().Password;

            //if (username != BankUserName) return new JsonResponse() { Message = "Invalid Username" };
            //if (password != BankPassword) return new JsonResponse() { Message = "Invalid Password" };

            var clinetId = ReadConfig().ClientID;

            JsonResponse jsonResponse = new JsonResponse();

            //check for failure data
            if (aTTEBatchProcessings.Count == 1 && aTTEBatchProcessings[0].ValidAccount == true)
            {

                jsonResponse.IsSuccess = false;
                jsonResponse.Message = aTTEBatchProcessings[0].ValidateMessage;
                jsonResponse.HasError = true;
                return jsonResponse;

            }

            string data = string.Empty;

            List<ATTTransctionDetails> aTTransctionDetails = new List<ATTTransctionDetails>();
            foreach (ATTEBatchProcessing batchProcessing in aTTEBatchProcessings)

            {
                aTTransctionDetails.Add(
                      new ATTTransctionDetails()
                      {
                          payee_bank_code = batchProcessing.SwiftCode,
                          sub_token = batchProcessing.sub_token,
                          payee_account_name = batchProcessing.FullName,
                          payee_account_number = batchProcessing.BankNo
                      });
            }

            ATTAccountDetails aTTAccountDetails = new ATTAccountDetails()
            {
                token = aTTEBatchProcessings[0].Token,
                transaction_details = aTTransctionDetails
            };

            data = JsonConvert.SerializeObject(aTTAccountDetails);
            //BankUserName = "accounts@f1soft.com";
            //BankPassword = "corporate@222";
            // BankUserName = _configuration.GetSection(_keys).GetSection("UserName").Value;
            //BankPassword = _configuration.GetSection(_keys).GetSection("Password").Value;
            var alphaNumeric = AlphaNumericString.getAlphaNumericString(32);
            byte[] secrect = EncryptDecryptSecreteKey.getSecretKey(alphaNumeric);

            string base64EncodedSecretKey = EncryptDecryptSecreteKey.keyToString(secrect);
            byte[] encryptedSecretKey = RSAEncryptDecrypt.encrypt(base64EncodedSecretKey, esewaPublicKey);
            string finalSecretKey = EncryptDecryptSecreteKey.base64Encode(encryptedSecretKey);
            string encryptedData = EncryptDecryptSecreteKey.encrypt(data, secrect);
            string signature = SignaturePayload.generateSignature(encryptedData, clientPrivateKeySig);
            var response1 = CallEsewaApiAccountValidation(encryptedData, signature, finalSecretKey, clinetId, BankUserName, BankPassword);
            Regex reg = new Regex("^((?!\\<(|\\/)[a-z][a-z0-9]*>).)*$");
            Match match = reg.Match(response1.Content);
            List<ATTEsewaNewResponse> aTTEsewaResponse = new List<ATTEsewaNewResponse>();
            ATTEncryptedDetails output = new ATTEncryptedDetails();
            if (match.Success)
            {
                if (response1.IsSuccessful)
                {
                    output = JsonConvert.DeserializeObject<ATTEncryptedDetails>(response1.Content);
                }
                else
                {
                    throw new Exception(response1.ErrorMessage);
                }
            }
            else { output.error = true; output.message = "Invalid Credentials Supplied!!"; }

            if (!output.error)
            {
                bool valid = SignaturePayload.verifySignature(output.data, esewaPublicKeySig, EncryptDecryptSecreteKey.base64Decode(output.signature));
                jsonResponse.IsSuccess = true;
                if (valid)
                {
                    byte[] decodedSecretKey = EncryptDecryptSecreteKey.base64Decode(output.secret_key);
                    string plainSecretKey = RSAEncryptDecrypt.decrypt(decodedSecretKey, clientPrivateKey);
                    byte[] outPutSecetKey = EncryptDecryptSecreteKey.getSecretKey(plainSecretKey);
                    string finalOutput = EncryptDecryptSecreteKey.decrypt(output.data, outPutSecetKey);
                    aTTEsewaResponse = JsonConvert.DeserializeObject<List<ATTEsewaNewResponse>>(finalOutput);
                }
            }
            else
            {
                jsonResponse.IsSuccess = !output.error;
                jsonResponse.Message = "Account Validation Failed";
                jsonResponse.ResponseData2 = output.data;
                return jsonResponse;
            }
            //new method of updating response
            foreach (var res in aTTEsewaResponse)
            {
                aTTEBatchProcessings.Where(X =>
                                           X.BankNo == res.payee_account_number
                                           && Regex.Replace(X.FullName, @"\s+", " ") == res.payee_account_name
                                           && X.SwiftCode == res.payee_bank_code).ToList()
                                           .ForEach(v =>
                                           {
                                               v.ValidateMessage = res.message;
                                               v.ValidAccount = !res.error;
                                               v.ValidateCode = res.code;
                                               v.Percentage = res.percentage;
                                           }
                                           );


            }
            aTTEBatchProcessings.Where(X =>
                                        X.ValidateCode == null).ToList()
                                        .ForEach(v =>
                                        {
                                            v.ValidateCode = "0";
                                            v.ValidAccount = false;
                                            v.ValidateMessage = "No Response From API";
                                        }
                                        );


            jsonResponse.ResponseData = aTTEBatchProcessings;
            return jsonResponse;
        }
        private static IRestResponse CallEsewaApiAccountValidation(string data, string signature, string finalSecretKey, string ClientId, string UserName, string Password)
        {
            string EsewaUrl = ReadConfig().Url;
            var client = new RestClient($"{EsewaUrl}/api/pki/account/validation");
            client.Timeout = 60000;
            var request = new RestRequest(Method.POST);
            //var userName = "pcsapiuser@corporate.com";
            //var password = "Test@1234";
            var byteArray = Encoding.ASCII.GetBytes($"{UserName}:{Password}");
            var clientAuthrizationHeader = new AuthenticationHeaderValue("Basic",
                                                          Convert.ToBase64String(byteArray));

            var auth = clientAuthrizationHeader.ToString();

            request.AddHeader("Authorization", auth);
            request.AddHeader("Content-Type", "application/json");


            ATTEncryptedDetails aTTEncryptedDetails = new ATTEncryptedDetails()
            {
                data = data,
                signature = signature,
                secret_key = finalSecretKey,
                client_id = ClientId
            };
            var body = JsonConvert.SerializeObject(aTTEncryptedDetails);

            request.AddParameter("application/json", body, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);


            return response;
        }
        public static JsonResponse TransactionProcessingBatch(List<ATTEBatchProcessing> aTTEBatchProcessings, string username, string password, string SourceBank, string SourceAccountNum)
        {
            JsonResponse jsonResponse = new JsonResponse();
            var clientPrivateKey = ReadConfig().ClientKeys.ClientPrivateKey;
            var clientPrivateKeySig = ReadConfig().SignatureKeys.ClientPrivateKey;
            var clientPublicKey = ReadConfig().ClientKeys.ClientPublicKey;
            var esewaPublicKey = ReadConfig().EsewaKeys.PublicKey;
            var esewaPublicKeySig = ReadConfig().EsewaKeys.SignatureKey;
            var BankUserName = ReadConfig().UserName;
            var BankPassword = ReadConfig().Password;
            var clinetId = ReadConfig().ClientID;
            var data = string.Empty;
            //if (username != BankUserName) return new JsonResponse() { Message = "Invalid Username" };
            //if (password != BankPassword) return new JsonResponse() { Message = "Invalid Password" };
            ATTAccountDetails aTTAccount = new ATTAccountDetails();
            List<ATTTransctionDetails> transctionDetails = new List<ATTTransctionDetails>();
            aTTAccount = new ATTAccountDetails();
            aTTAccount.token = aTTEBatchProcessings[0].Token;
            foreach (ATTEBatchProcessing aTTEBatchProcessing in aTTEBatchProcessings)
            {
                ATTTransctionDetails aTTTransctionDetails = new ATTTransctionDetails();
                aTTTransctionDetails.payee_account_name = aTTEBatchProcessing.FullName;
                aTTTransctionDetails.payee_account_number = aTTEBatchProcessing.BankNo;
                aTTTransctionDetails.payee_bank_code = aTTEBatchProcessing.SwiftCode;
                aTTTransctionDetails.sub_token = aTTEBatchProcessing.sub_token;
                aTTTransctionDetails.amount = aTTEBatchProcessing.WarrantAmt;
                //aTTTransctionDetails.source_account_number = SourceBankNumber;
                //aTTTransctionDetails.source_bank_code = SourceBankName;
                aTTTransctionDetails.source_account_number = SourceAccountNum;
                aTTTransctionDetails.source_bank_code = SourceBank;
                aTTTransctionDetails.note = aTTEBatchProcessing.TransactionRemarks;
                transctionDetails.Add(aTTTransctionDetails);

            }
            aTTAccount.transaction_details = transctionDetails;
            string serializedObject = JsonConvert.SerializeObject(aTTAccount);
            try
            {

                var alphaNumeric = AlphaNumericString.getAlphaNumericString(32);
                byte[] secrect = EncryptDecryptSecreteKey.getSecretKey(alphaNumeric);

                string base64EncodedSecretKey = EncryptDecryptSecreteKey.keyToString(secrect);
                byte[] encryptedSecretKey = RSAEncryptDecrypt.encrypt(base64EncodedSecretKey, esewaPublicKey);
                string finalSecretKey = EncryptDecryptSecreteKey.base64Encode(encryptedSecretKey);
                string encryptedData = EncryptDecryptSecreteKey.encrypt(serializedObject, secrect);
                string signature = SignaturePayload.generateSignature(encryptedData, clientPrivateKeySig);
                ATTEncryptedDetails output = new ATTEncryptedDetails();

                IRestResponse response = CallEsewaApiTransactionProcessing(encryptedData, signature, finalSecretKey, clinetId, BankUserName, BankPassword);
                if (response.IsSuccessful)
                {
                    output = JsonConvert.DeserializeObject<ATTEncryptedDetails>(response.Content);

                }
                else
                {

                    output.error = true;
                    output.message = response.Content;
                    output.code = "0";
                }
                if (!output.error)
                {
                    bool valid = SignaturePayload.verifySignature(output.data, esewaPublicKeySig, EncryptDecryptSecreteKey.base64Decode(output.signature));
                    if (valid)
                    {
                        byte[] decodedSecretKey = EncryptDecryptSecreteKey.base64Decode(output.secret_key);
                        string plainSecretKey = RSAEncryptDecrypt.decrypt(decodedSecretKey, clientPrivateKey);
                        byte[] outPutSecetKey = EncryptDecryptSecreteKey.getSecretKey(plainSecretKey);
                        string finalOutput = EncryptDecryptSecreteKey.decrypt(output.data, outPutSecetKey);

                        ATTEsewaResponse aTTEsewaResponse = JsonConvert.DeserializeObject<ATTEsewaResponse>(finalOutput);
                        if (!aTTEsewaResponse.Error)
                        {
                            if (aTTEBatchProcessings[0].Token == aTTEsewaResponse.Token)
                            {
                                foreach (var transaction in aTTEsewaResponse.transaction_details)
                                {
                                    int INDEX = aTTEBatchProcessings.FindIndex(x => x.sub_token == transaction.sub_token);
                                    if (INDEX >= 0)
                                    {
                                        aTTEBatchProcessings[INDEX].TransactionCode = aTTEsewaResponse.Code;
                                        aTTEBatchProcessings[INDEX].TransactionMessage = transaction.message;
                                        aTTEBatchProcessings[INDEX].TransactionDetail = transaction.status;
                                    }
                                }

                            }
                            //jsonResponse = _transctionProcessing.SaveOutputRemarks(aTTEsewaResponse, CompCode, BatchNo, aTTAccount.token);
                        }
                        else
                        {
                            jsonResponse.IsSuccess = false;
                            jsonResponse.Message = aTTEsewaResponse.Message;

                        }
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = aTTEBatchProcessings;
                    }
                    else
                    {
                        output.message = "SIGNATURE IS NOT VALID.";
                        //jsonResponse = _transctionProcessing.SaveErrorRemarks(aTTAccount.token, output, null);
                        jsonResponse.IsSuccess = false;
                        jsonResponse.Message = output.message;
                        return jsonResponse;
                    }
                }
                else
                {
                    //jsonResponse = _transctionProcessing.SaveErrorRemarks(aTTAccount.token, output, null);
                    JsonConvert.SerializeObject(output).LogData("EsewaTransaction", DateTime.Now.ToString());


                }


            }
            catch (Exception ex)
            {
                //UserName = _loggedInUser.GetUserName();
                DateTime Date = DateTime.Now;
                //_logDetails.InsertLogDetails(BatchNo, ex.Message, UserName);
                jsonResponse.Message = ex.Message;
                jsonResponse.IsSuccess = false;
                jsonResponse.HasError = true;

            }

            return jsonResponse;
        }

        public static JsonResponse TransactionProcessing(List<ATTEBatchProcessing> aTTEBatchProcessings, string SourceBank, string SourceAccountNum)
        {
            JsonResponse jsonResponse = new JsonResponse();
            var clientPrivateKey = ReadConfig().ClientKeys.ClientPrivateKey;
            var clientPrivateKeySig = ReadConfig().SignatureKeys.ClientPrivateKey;
            var clientPublicKey = ReadConfig().ClientKeys.ClientPublicKey;
            var esewaPublicKey = ReadConfig().EsewaKeys.PublicKey;
            var esewaPublicKeySig = ReadConfig().EsewaKeys.SignatureKey;
            var BankUserName = ReadConfig().UserName;
            var BankPassword = ReadConfig().Password;
            var clinetId = ReadConfig().ClientID;
            var data = string.Empty;
            List<ATTEBatchProcessing> batchResponse = new List<ATTEBatchProcessing>();
            //if (username != BankUserName) return new JsonResponse() { Message = "Invalid Username" };
            //if (password != BankPassword) return new JsonResponse() { Message = "Invalid Password" };
            ATTAccountDetails aTTAccount = new ATTAccountDetails();
            foreach (ATTEBatchProcessing aTTEBatchProcessing in aTTEBatchProcessings)
            {
                List<ATTTransctionDetails> transctionDetails = new List<ATTTransctionDetails>();
                aTTAccount = new ATTAccountDetails();
                aTTAccount.token = aTTEBatchProcessing.sub_token;
                ATTTransctionDetails aTTTransctionDetails = new ATTTransctionDetails();
                aTTTransctionDetails.payee_account_name = aTTEBatchProcessing.FullName;
                aTTTransctionDetails.payee_account_number = aTTEBatchProcessing.BankNo;
                aTTTransctionDetails.payee_bank_code = aTTEBatchProcessing.SwiftCode;
                aTTTransctionDetails.sub_token = aTTEBatchProcessing.WarrantNo;
                aTTTransctionDetails.amount = aTTEBatchProcessing.WarrantAmt;
                //aTTTransctionDetails.source_account_number = SourceBankNumber;
                //aTTTransctionDetails.source_bank_code = SourceBankName;
                aTTTransctionDetails.source_account_number = SourceAccountNum;
                aTTTransctionDetails.source_bank_code = SourceBank;
                aTTTransctionDetails.note = aTTEBatchProcessing.TransactionRemarks;
                transctionDetails.Add(aTTTransctionDetails);

                aTTAccount.transaction_details = transctionDetails;

                string serializedObject = JsonConvert.SerializeObject(aTTAccount);
                serializedObject.LogData("Transaction Processing Request for Batch " + aTTEBatchProcessing.BatchID + " Compcode: " + aTTEBatchProcessing.Compcode);

                try
                {

                    var alphaNumeric = AlphaNumericString.getAlphaNumericString(32);
                    byte[] secrect = EncryptDecryptSecreteKey.getSecretKey(alphaNumeric);

                    string base64EncodedSecretKey = EncryptDecryptSecreteKey.keyToString(secrect);
                    byte[] encryptedSecretKey = RSAEncryptDecrypt.encrypt(base64EncodedSecretKey, esewaPublicKey);
                    string finalSecretKey = EncryptDecryptSecreteKey.base64Encode(encryptedSecretKey);
                    string encryptedData = EncryptDecryptSecreteKey.encrypt(serializedObject, secrect);
                    string signature = SignaturePayload.generateSignature(encryptedData, clientPrivateKeySig);
                    ATTEncryptedDetails output = new ATTEncryptedDetails();

                    IRestResponse response = CallEsewaApiTransactionProcessing(encryptedData, signature, finalSecretKey, clinetId, BankUserName, BankPassword);
                    if (response.IsSuccessful)
                    {
                        output = JsonConvert.DeserializeObject<ATTEncryptedDetails>(response.Content);
                        if (!output.error)
                        {
                            bool valid = SignaturePayload.verifySignature(output.data, esewaPublicKeySig, EncryptDecryptSecreteKey.base64Decode(output.signature));
                            if (valid)
                            {
                                byte[] decodedSecretKey = EncryptDecryptSecreteKey.base64Decode(output.secret_key);
                                string plainSecretKey = RSAEncryptDecrypt.decrypt(decodedSecretKey, clientPrivateKey);
                                byte[] outPutSecetKey = EncryptDecryptSecreteKey.getSecretKey(plainSecretKey);
                                string finalOutput = EncryptDecryptSecreteKey.decrypt(output.data, outPutSecetKey);

                                ATTEsewaResponse aTTEsewaResponse = JsonConvert.DeserializeObject<ATTEsewaResponse>(finalOutput);
                                finalOutput.LogData("Transaction Processing Response for Batch " + aTTEBatchProcessing.BatchID + " Compcode: " + aTTEBatchProcessing.Compcode);

                                if (!aTTEsewaResponse.Error)
                                {
                                    if (aTTEBatchProcessing.sub_token == aTTEsewaResponse.Token && aTTEBatchProcessing.WarrantNo == aTTEsewaResponse.transaction_details[0].sub_token)
                                    {
                                        aTTEBatchProcessing.TransactionCode = aTTEsewaResponse.Code;
                                        aTTEBatchProcessing.TransactionMessage = aTTEsewaResponse.transaction_details[0].status;
                                        aTTEBatchProcessing.TransactionDetail = aTTEsewaResponse.transaction_details[0].message;
                                    }
                                    else
                                    {
                                        aTTEBatchProcessing.TransactionCode = "11";
                                        aTTEBatchProcessing.TransactionMessage = "Transaction response didnot match with request.";
                                        aTTEBatchProcessing.TransactionDetail = "Check app_log";
                                    }
                                    //jsonResponse = _transctionProcessing.SaveOutputRemarks(aTTEsewaResponse, CompCode, BatchNo, aTTAccount.token);
                                }
                                else
                                {
                                    aTTEBatchProcessing.TransactionCode = aTTEsewaResponse.Code;
                                    aTTEBatchProcessing.TransactionMessage = aTTEsewaResponse.Message;
                                    aTTEBatchProcessing.TransactionDetail = aTTEsewaResponse.Details;
                                }


                            }
                            else
                            {
                                output.message = "SIGNATURE IS NOT VALID.";
                                aTTEBatchProcessing.TransactionCode = "11";
                                aTTEBatchProcessing.TransactionMessage = output.message;
                                aTTEBatchProcessing.TransactionDetail = output.details;
                            }
                        }
                        else
                        {
                            //jsonResponse = _transctionProcessing.SaveErrorRemarks(aTTAccount.token, output, null);
                            aTTEBatchProcessing.TransactionCode = output.code;
                            aTTEBatchProcessing.TransactionMessage = output.message;
                            aTTEBatchProcessing.TransactionDetail = output.details;
                        }
                    }
                    else
                    {
                        output.error = true;
                        output.message = response.Content;
                        output.code = "0";
                        aTTEBatchProcessing.TransactionCode = "10";
                        aTTEBatchProcessing.TransactionMessage = response.ErrorMessage;
                        aTTEBatchProcessing.TransactionDetail = response.Content;
                    }



                }
                catch (Exception ex)
                {
                    aTTEBatchProcessing.TransactionCode = "00";
                    aTTEBatchProcessing.TransactionMessage = ex.Message;
                    aTTEBatchProcessing.TransactionDetail = "An Exception occured!!";
                    ex.LogErrorToText("Transaction Processing Error for Batch " + aTTEBatchProcessing.BatchID + " Compcode: " + aTTEBatchProcessing.Compcode);
                }
                JsonConvert.SerializeObject(aTTEBatchProcessing).LogData("Transaction Processing for Batch " + aTTEBatchProcessing.BatchID + " Compcode: " + aTTEBatchProcessing.Compcode);

                batchResponse.Add(aTTEBatchProcessing);
            }
            jsonResponse.IsSuccess = true;
            jsonResponse.ResponseData = batchResponse;
            return jsonResponse;
        }
        private static IRestResponse CallEsewaApiTransactionProcessing(string data, string signature, string finalSecretKey, string ClientId, string UserName, string Password)
        {
            string EsewaUrl = ReadConfig().Url;

            var byteArray = Encoding.ASCII.GetBytes($"{UserName}:{Password}");
            var clientAuthrizationHeader = new AuthenticationHeaderValue("Basic",
                                                          Convert.ToBase64String(byteArray));

            var auth = clientAuthrizationHeader.ToString();
            var client = new RestClient($"{EsewaUrl}/api/pki/transactions");
            client.Timeout = 60000;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", auth);
            request.AddHeader("Content-Type", "application/json");

            ATTEncryptedDetails aTTEncryptedDetails = new ATTEncryptedDetails()
            {
                data = data,
                signature = signature,
                secret_key = finalSecretKey,
                client_id = ClientId
            };
            var Body = JsonConvert.SerializeObject(aTTEncryptedDetails);
            request.AddParameter("application/json", Body, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            return response;


        }
        public static JsonResponse TransactionStatus(List<ATTEBatchProcessing> aTTEBatchProcessings)
        {
            JsonResponse jsonResponse = new JsonResponse();
            var clientPrivateKey = ReadConfig().ClientKeys.ClientPrivateKey;
            var clientPrivateKeySig = ReadConfig().SignatureKeys.ClientPrivateKey;
            var esewaPublicKey = ReadConfig().EsewaKeys.PublicKey;
            var esewaPublicKeySig = ReadConfig().EsewaKeys.SignatureKey;
            var BankUserName = ReadConfig().UserName;
            var BankPassword = ReadConfig().Password;
            var clientid = ReadConfig().ClientID;
            List<ATTTransctionDetails> aTTTransctionDetails = new List<ATTTransctionDetails>();
            foreach (ATTEBatchProcessing batchProcessing in aTTEBatchProcessings)
            {
                ATTAccountDetails accountDetails = new ATTAccountDetails();
                accountDetails.token = aTTEBatchProcessings[0].sub_token;
                aTTTransctionDetails = new List<ATTTransctionDetails>();
                aTTTransctionDetails.Add(new ATTTransctionDetails()
                {
                    sub_token = batchProcessing.WarrantNo
                });
                accountDetails.transaction_details = aTTTransctionDetails;

                string serializedObject = JsonConvert.SerializeObject(accountDetails);

                var alphaNumeric = AlphaNumericString.getAlphaNumericString(32);
                byte[] secrect = EncryptDecryptSecreteKey.getSecretKey(alphaNumeric);

                string base64EncodedSecretKey = EncryptDecryptSecreteKey.keyToString(secrect);
                byte[] encryptedSecretKey = RSAEncryptDecrypt.encrypt(base64EncodedSecretKey, esewaPublicKey);
                string finalSecretKey = EncryptDecryptSecreteKey.base64Encode(encryptedSecretKey);
                string encryptedData = EncryptDecryptSecreteKey.encrypt(serializedObject, secrect);
                string signature = SignaturePayload.generateSignature(encryptedData, clientPrivateKeySig);

                IRestResponse response = CallEsewaApiTransactionStatus(encryptedData, signature, finalSecretKey, clientid, BankUserName, BankPassword);
                ATTEncryptedDetails output = new ATTEncryptedDetails();
                if (response.IsSuccessful)
                {
                    Regex reg = new Regex("^((?!\\<(|\\/)[a-z][a-z0-9]*>).)*$");
                    Match match = reg.Match(response.Content);
                    if (match.Success) { output = JsonConvert.DeserializeObject<ATTEncryptedDetails>(response.Content); }
                    else { output.error = true; output.message = "Invalid Credentials Supplied!!"; }
                }
                else
                { output.error = true; output.message = "No Response From eSewa API"; }


                if (!output.error)
                {
                    bool valid = SignaturePayload.verifySignature(output.data, esewaPublicKeySig, EncryptDecryptSecreteKey.base64Decode(output.signature));
                    jsonResponse.IsSuccess = valid;
                    if (valid)
                    {
                        byte[] decodedSecretKey = EncryptDecryptSecreteKey.base64Decode(output.secret_key);
                        string plainSecretKey = RSAEncryptDecrypt.decrypt(decodedSecretKey, clientPrivateKey);
                        byte[] outPutSecetKey = EncryptDecryptSecreteKey.getSecretKey(plainSecretKey);
                        string finalOutput = EncryptDecryptSecreteKey.decrypt(output.data, outPutSecetKey);

                        ATTEsewaResponse aTTEsewaResponse = JsonConvert.DeserializeObject<ATTEsewaResponse>(finalOutput);
                        if (!aTTEsewaResponse.Error)
                        {
                            batchProcessing.TransactionCode = aTTEsewaResponse.transaction_details[0].code;
                            batchProcessing.TransactionMessage = aTTEsewaResponse.transaction_details[0].status;
                            batchProcessing.TransactionDetail = aTTEsewaResponse.transaction_details[0].message;
                            //jsonResponse = _transactionStatusQuery.SaveSubTokenOutputRemarks(aTTEsewaResponse, CompCode, BatchNo, aTTAccountDetails.token);
                        }
                        else
                        {
                            //jsonResponse = _transactionStatusQuery.SaveErrorRemarks(null, aTTEsewaResponse, aTTAccountDetails.token);
                            jsonResponse.IsSuccess = !output.error;
                            jsonResponse.Message = output.message;
                        }
                    }
                    else
                    {
                        output.message = "Signation is Not Valid";
                        //jsonResponse = _transactionStatusQuery.SaveErrorRemarks(output, null, aTTAccountDetails.token);
                        jsonResponse.IsSuccess = false;
                        jsonResponse.Message = output.message;
                    }
                }
                else
                {
                    batchProcessing.UpdatedTransactionCode = output.code;
                    batchProcessing.UpdatedTransactionMessage = output.message;
                    batchProcessing.UpdatedTransactionDetail = output.details;
                }
            }
            if (jsonResponse.IsSuccess) { jsonResponse.ResponseData = aTTEBatchProcessings; }
            return jsonResponse;
        }
        private static IRestResponse CallEsewaApiTransactionStatus(string data, string signature, string finalSecretKey, string ClientId, string UserName, string Password)
        {
            string EsewaUrl = ReadConfig().Url;

            var byteArray = Encoding.ASCII.GetBytes($"{UserName}:{Password}");
            var clientAuthrizationHeader = new AuthenticationHeaderValue("Basic",
                                                          Convert.ToBase64String(byteArray));

            var auth = clientAuthrizationHeader.ToString();
            var client = new RestClient($"{EsewaUrl}/api/pki/transactions/status");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", auth);
            request.AddHeader("Content-Type", "application/json");

            ATTEncryptedDetails aTTEncryptedDetails = new ATTEncryptedDetails()
            {
                data = data,
                signature = signature,
                secret_key = finalSecretKey,
                client_id = ClientId
            };
            var Body = JsonConvert.SerializeObject(aTTEncryptedDetails);
            request.AddParameter("application/json", Body, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            return response;

        }

    }
}
