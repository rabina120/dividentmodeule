using Entity.Common;
using ENTITY.FundTransfer;
using ENTITY.FundTransfer.NPS;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace REPOSITORY.FundTransfer
{


    public class NPSHelper
    {
        public static class StatusCode
        {
            public const string SUCCESS = "100";
            public const string FAILURE = "500";
            public const string INVALID_ACCOUNT = "000";
            public const string PARTIAL_VALID_ACCOUNT = "200";
            public const string VALID_ACCOUNT = "100";

            public const string TRANSACTION_STARTED = "100";
            public const string TRANSACTION_SUCESS = "200";
            public const string TRANSACTION_INCOMPLETE = "300";
            public const string TRANSACTION_FAILED = "500";
        }
        public static string _configPath = Environment.CurrentDirectory + "\\Keys\\npsKeys.json";
        #region public methods 

        public static ATTEBatchProcessing AccountValidation(ATTEBatchProcessing aTTEBatchProcessing)
        {
            return InitiateAccountValidation(aTTEBatchProcessing);
        }
        public static List<ATTEBatchProcessing> AccountValidation(List<ATTEBatchProcessing> aTTEBatchProcessings)
        {
            string data = string.Empty;
            aTTEBatchProcessings.ForEach(accDetail => accDetail = InitiateAccountValidation(accDetail));
            return aTTEBatchProcessings;
        }

        private static ATTEBatchProcessing InitiateAccountValidation(ATTEBatchProcessing accDetail)
        {
            ATTNPSBankValidationResponse apiResponse = new ATTNPSBankValidationResponse();
            try
            {

                apiResponse = ValidateBank(accDetail.FullName, accDetail.BankNo, accDetail.SwiftCode);
                if (apiResponse.errors.Count == 0)
                {


                    accDetail.Percentage = apiResponse.data.NameMatchPercentage;
                    decimal validPercentage = 0;
                    decimal.TryParse(accDetail.Percentage, out validPercentage);
                    accDetail.ValidateDate = DateTime.Now;
                    if (validPercentage > (decimal)0)
                    {
                        accDetail.ValidateCode = StatusCode.PARTIAL_VALID_ACCOUNT;
                        accDetail.ValidateMessage = "Account Partially Validated";
                        if (validPercentage == (decimal)100)
                        {
                            accDetail.ValidAccount = true;
                            accDetail.ValidateCode = StatusCode.VALID_ACCOUNT;
                            accDetail.ValidateMessage = "Account Validated Sucessfully.";

                        }
                    }
                    else if (validPercentage == (decimal)0)
                    {
                        accDetail.ValidateCode = StatusCode.INVALID_ACCOUNT;
                        accDetail.ValidateMessage = "Account Doesnot Match.";

                    }

                }
                else
                {
                    var error = apiResponse.errors[0];
                    accDetail.ValidateCode = StatusCode.FAILURE + "-" + error.nps_code;
                    accDetail.ValidateMessage = error.error_message;
                }
            }

            catch (Exception e)
            {
                e.LogErrorToText();
                accDetail.ValidateCode = StatusCode.FAILURE;
                accDetail.ValidateMessage = "There was a Problem with the APi request.";
            }
            return accDetail;
        }

        public static List<ATTEBatchProcessing> TransactionProcessing(List<ATTEBatchProcessing> aTTEBatchProcessings, string SourceBank, string SourceAccountNum, string sourceAccountName)
        {
            JsonResponse jsonResponse = new JsonResponse();
            List<ATTEBatchProcessing> allResponses = new List<ATTEBatchProcessing>();
            var data = string.Empty;

            string processID = Guid.NewGuid().ToString();
            aTTEBatchProcessings.ForEach(x =>
            {
                x.Token = processID;
                x.sub_token = x.sub_token + processID;
            });
            List<ATTEBatchProcessing> batchResponse = new List<ATTEBatchProcessing>();

            List<ATTNPSBulkTransactionDetails> batchToProcess = new List<ATTNPSBulkTransactionDetails>();
            for (int item = 0; item < aTTEBatchProcessings.Count; item++)
            {

                batchToProcess.Add(new ATTNPSBulkTransactionDetails()
                {
                    Amount = string.Format("{0:0.00}", aTTEBatchProcessings[item].TotalAmt.ToString()),
                    DestinationAccName = aTTEBatchProcessings[item].FullName,
                    DestinationAccNo = aTTEBatchProcessings[item].BankNo,
                    DestinationBank = aTTEBatchProcessings[item].SwiftCode,
                    MerchantTxnId = aTTEBatchProcessings[item].sub_token,
                    DestinationCurrency = "NPR",
                    IsDestinationMobile = "n",
                    TransactionRemarks = aTTEBatchProcessings[item].TransactionRemarks,

                });

            }
            batchResponse.AddRange(aTTEBatchProcessings);

            if (batchToProcess.Count > 0)
            {

                var apiResponse = BulkFundTranferRequest(batchToProcess, SourceBank,
                    SourceAccountNum, sourceAccountName, processID);
                if (apiResponse.code!="0")
                {
                    batchResponse.ForEach(x =>
                    {
                        x.TransactionCode = StatusCode.TRANSACTION_INCOMPLETE;
                        x.TransactionMessage = "Transaction Status is Not Complete. One or more Transaction Had Errors";
                        x.TransactionDetail = JsonConvert.SerializeObject(apiResponse.errors).ToString();
                    });
                    return batchResponse;
                }
                string txnID = apiResponse.data.MerchantProcessId;
                var statusResponse = CheckBulkStatus(txnID);
                AppLogger.LogData(statusResponse, "NPS FUNDTRANSFER", "Transaction Response For ProcessID: " + txnID);
                if (statusResponse.code != "0")
                {
                    batchResponse.ForEach(x =>
                    {
                        x.TransactionCode = StatusCode.TRANSACTION_STARTED;
                        x.TransactionMessage = "Transaction Was Initiated Successfully.";
                        x.TransactionDetail = "Transaction Started on:" + DateTime.Now.ToString() + " SYS TIME";
                    });
                    return batchResponse;
                }
                batchResponse.ForEach(x =>
                {
                    var row = statusResponse.data.Find(row => row.MerchantTxnId == x.sub_token);

                    if (row != null)
                    {
                        x.TransactionCode = GetReturnStatusFromTransactionStatus(row.TransactionStatus);
                        x.TransactionMessage = row.TransactionStatus;
                        x.TransactionDate = DateTime.Parse(row.TransactionDate);
                        x.TransactionDetail = "Transaction Updated with Status: " + row.TransactionStatus + " on " + DateTime.Now.ToString();
                    }
                });

            }
            

            return batchResponse;
        }

        private static string GetReturnStatusFromTransactionStatus(string transactionStatus)
        {
            string status = StatusCode.TRANSACTION_INCOMPLETE;
            string normalizedStatus = transactionStatus.Normalize();
            switch (normalizedStatus)
            {
                case "SUCCESS":
                    status = StatusCode.TRANSACTION_SUCESS;
                    break;
                case "FAILURE":
                    status = StatusCode.TRANSACTION_FAILED;
                    break;
                default:
                    status = StatusCode.TRANSACTION_INCOMPLETE;
                    break;
            }
            return status;
        }
        #endregion
        #region config from file
        private static NPSAPIKeys ReadConfig()
        {
            NPSAPIKeys source = new NPSAPIKeys();
            if (File.Exists(_configPath))
            {

                using (StreamReader r = new StreamReader(_configPath))
                {
                    string json = r.ReadToEnd();
                    source = JsonConvert.DeserializeObject<NPSAPIKeys>(json);
                }
            }
            else
                throw new Exception("Fund Transfer Keys Not Configured");
            return source;
        }

        public static int MaxBatchSize()
        {
            string batchsizeStr = ReadConfig().FundTransferBatchSize;
            int batchsizeNum = 0;
            int.TryParse(batchsizeStr, out batchsizeNum);
            return batchsizeNum;
        }

        #endregion
        #region NPS API Requests

        //first code is for login 
        public static string Login()
        {
            ATTNPSLoginReq nps = new ATTNPSLoginReq()
            {
                MerchantId = ReadConfig().MerchantId,
                ApiUserName = ReadConfig().ApiUserName,

            };

            var finalsignature = signData(nps);
            var plainTextBytes = Encoding.UTF8.GetBytes(ReadConfig().AuthorizationPayload);
            string encodedText = Convert.ToBase64String(plainTextBytes);
            ATTNPSLoginReqWithSig npsrequest = new ATTNPSLoginReqWithSig()
            {
                MerchantId = nps.MerchantId,
                ApiUserName = nps.ApiUserName,
                timestamp = nps.timestamp,
                signature = finalsignature
            };
            string serializedrequest = JsonConvert.SerializeObject(npsrequest);
            try
            {
                WebClient newclient = new WebClient();
                newclient.Headers["Content-type"] = "application/json";
                newclient.Headers[HttpRequestHeader.Authorization] = "Basic" + " " + encodedText;
                string finalresponse = newclient.UploadString(ReadConfig().LoginUrl, serializedrequest);
                var data = JsonConvert.DeserializeObject<ATTNPSResponse>(finalresponse);
                ATTNPSLoginSuccess lognREsp = new();
                if (data.code == "0")
                {
                    lognREsp = JsonConvert.DeserializeObject<ATTNPSLoginSuccess>(finalresponse);
                }
                return lognREsp.data.AccessToken;

            }
            catch (Exception)
            {

                throw;
            }
        }

        //get bank list  (working fine)

        public static List<ATTBank> GetBanksFromProvider()
        {
            List<ATTBank> aTTBanks = new List<ATTBank>();
            var banks = GetBankList();
            if (banks.errors.Count > 0)
            {
                AppLogger.LogData(banks, "Get Bank From API", "NPS Helper");
                throw new Exception("Cannot get Data From Api");
            }
            else
            {
                foreach (var item in banks.data)
                {
                    aTTBanks.Add(NPSBankToBank(item));
                }
            }
            return aTTBanks;
        }

        private static ATTBank NPSBankToBank(ATTNPSBankListDetails aTTNPSBanks)
        {
            ATTBank bank = new ATTBank();
            bank.BankName = aTTNPSBanks.InstitutionName;
            bank.BanKCode = aTTNPSBanks.InstrumentCode;
            return bank;
        }

        public static ATTNPSBankList GetBankList()
        {
            string AccessToken = Login();
            ATTNPSLoginReq nps = new ATTNPSLoginReq()
            {
                MerchantId = ReadConfig().MerchantId,
                ApiUserName = ReadConfig().ApiUserName,
            };
            var finalsignature = signData(nps);

            ATTNPSLoginReqWithSig npsrequest = new ATTNPSLoginReqWithSig()
            {
                MerchantId = nps.MerchantId,
                ApiUserName = nps.ApiUserName,
                timestamp = nps.timestamp,
                signature = finalsignature
            };
            string serializedrequest = JsonConvert.SerializeObject(npsrequest);
            WebClient newclient = new WebClient();
            newclient.Headers["Content-type"] = "application/json";
            newclient.Headers[HttpRequestHeader.Authorization] = "Bearer" + " " + AccessToken;
            string finalresponse = newclient.UploadString(ReadConfig().BankUrl, serializedrequest);
            var data = JsonConvert.DeserializeObject<ATTNPSResponse>(finalresponse);
            var response = JsonConvert.DeserializeObject<ATTNPSBankList>(finalresponse);
            return response;
        }



        //validate bank  (working fine) 
        private static ATTNPSBankValidationResponse ValidateBank(string AccountName = "Diwakar Baskota", string AccountNumber = "1900000000000076", string BankCode = "FTTESTBANK")
        {
            string AccessToken = Login();
            ATTNPSAccountValidateReq nps = new ATTNPSAccountValidateReq()
            {
                AccountName = AccountName,
                AccountNumber = AccountNumber,
                ApiUserName = ReadConfig().ApiUserName,
                BankCode = BankCode,
                MerchantId = ReadConfig().MerchantId,
                timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff"),
            };
            var finalsignature = signData(nps);
            ATTNPSAccountValidateReq npsrequest = new ATTNPSAccountValidateReq()
            {
                AccountName = AccountName,
                AccountNumber = AccountNumber,
                ApiUserName = nps.ApiUserName,
                BankCode = nps.BankCode,
                MerchantId = nps.MerchantId,
                signature = finalsignature,
                timestamp = nps.timestamp,
            };
            string serializedrequest = JsonConvert.SerializeObject(npsrequest);
            WebClient newclient = new WebClient();
            newclient.Headers["Content-type"] = "application/json";
            newclient.Headers[HttpRequestHeader.Authorization] = "Bearer" + " " + AccessToken;
            string finalresponse = newclient.UploadString(ReadConfig().AccountValidationUrl, serializedrequest);
            var data = JsonConvert.DeserializeObject<ATTNPSResponse>(finalresponse);
            var response = JsonConvert.DeserializeObject<ATTNPSBankValidationResponse>(finalresponse);
            return response;
        }

        //fund transfer request single 
        private static ATTNPSFundTransferSuccessResponse FundTranferRequest()
        {
            string AccessToken = Login();
            ATTNPSFundTransferRequest nps = new ATTNPSFundTransferRequest()
            {
                Amount = "100",
                ApiUserName = ReadConfig().BulkFundTransferUrl,
                DestinationAccName = "SABIN DAWADI",
                DestinationAccNo = "19000000000000000005",
                DestinationBank = "FTTESTBANK",
                DestinationCurrency = "NPR",
                MerchantId = ReadConfig().MerchantId,
                MerchantProcessID = "abc123abcd",
                MerchantTxnId = "abc123abcd",
                SourceAccName = "DIWAKAR BASKOTA",
                SourceAccNo = "1900000000000076",
                SourceBank = "FTTESTBANK",
                SourceCurrency = "NPR",
                TimeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff"),
                TransactionRemarks = "fundTransfer UAT",
                TransactionRemarks2 = "",
                TransactionRemarks3 = "",
            };
            var finalsignature = signData(nps);

            ATTNPSFundTransferRequest npsrequest = new ATTNPSFundTransferRequest()
            {
                Amount = "100",
                ApiUserName = nps.ApiUserName,
                DestinationAccName = "SABIN DAWADI",
                DestinationAccNo = "19000000000000000005",
                DestinationBank = "FTTESTBANK",
                DestinationCurrency = "NPR",
                MerchantId = nps.MerchantId,
                MerchantProcessID = "abc123abcd", //Should be unique for every request
                MerchantTxnId = "abc123abcd",//Should be unique for every request
                Signature = finalsignature,
                SourceAccName = "DIWAKAR BASKOTA",
                SourceAccNo = "1900000000000076",
                SourceBank = "FTTESTBANK",
                SourceCurrency = "NPR",
                TimeStamp = nps.TimeStamp,
                TransactionRemarks = "fundTransfer UAT",
                TransactionRemarks2 = "",
                TransactionRemarks3 = "",
            };
            string serializedrequest = JsonConvert.SerializeObject(npsrequest);
            WebClient newclient = new WebClient();
            newclient.Headers["Content-type"] = "application/json";
            newclient.Headers[HttpRequestHeader.Authorization] = "Bearer" + " " + AccessToken;
            string finalresponse = newclient.UploadString(ReadConfig().FundTransferUrl, serializedrequest);
            var data = JsonConvert.DeserializeObject<ATTNPSResponse>(finalresponse);
            var response = JsonConvert.DeserializeObject<ATTNPSFundTransferSuccessResponse>(finalresponse);
            return response;
        }

        //check status

        private static ATTNPSCheckStatusResponse CheckStatus(string MerchantTxnId)
        {
            string AccessToken = Login();
            ATTNPSCheckStatusRequest nps = new ATTNPSCheckStatusRequest()
            {
                ApiUserName = ReadConfig().ApiUserName,
                MerchantId = ReadConfig().MerchantId,
                MerchantTxnId = MerchantTxnId,
                TimeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff"),
            };
            var finalsignature = signData(nps);

            ATTNPSCheckStatusRequest npsrequest = new ATTNPSCheckStatusRequest()
            {
                ApiUserName = nps.ApiUserName,
                MerchantId = nps.MerchantId,
                MerchantTxnId = MerchantTxnId,
                TimeStamp = nps.TimeStamp,
                Signature = finalsignature
            };
            string serializedrequest = JsonConvert.SerializeObject(npsrequest);
            WebClient newclient = new WebClient();
            newclient.Headers["Content-type"] = "application/json";
            newclient.Headers[HttpRequestHeader.Authorization] = "Bearer" + " " + AccessToken;
            string finalresponse = newclient.UploadString(ReadConfig().TransactionStatusUrl, serializedrequest);
            var data = JsonConvert.DeserializeObject<ATTNPSResponse>(finalresponse);
            var response = JsonConvert.DeserializeObject<ATTNPSCheckStatusResponse>(finalresponse);
            return response;
        }

        //bulk fund transfer 
        private static ATTNPSBulkFundTransferResponse BulkFundTranferRequest(List<ATTNPSBulkTransactionDetails> trandetailsList, string sourceBank, string sourceAccount, string sourceAccountName, string ProcessID)
        {
            string AccessToken = Login();



            ATTBulkFundTransferRequest nps = new ATTBulkFundTransferRequest()
            {
                ApiUserName = ReadConfig().ApiUserName,
                MerchantId = ReadConfig().MerchantId,
                MerchantProcessId = ProcessID,
                SourceAccName = sourceAccountName,
                SourceAccNo = sourceAccount,
                SourceBank = sourceBank,
                SourceCurrency = "NPR",
                TimeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff"),
                //TransactionDetail=trandetailsList
            };

            var finalsignature = signData(nps);

            ATTBulkFundTransferRequest npsrequest = new ATTBulkFundTransferRequest()
            {
                ApiUserName = nps.ApiUserName,
                MerchantId = nps.MerchantId,
                MerchantProcessId = nps.MerchantProcessId,
                Signature = finalsignature,
                SourceAccName = nps.SourceAccName,
                SourceAccNo = nps.SourceAccNo,
                SourceBank = nps.SourceBank,
                SourceCurrency = nps.SourceCurrency,
                TimeStamp = nps.TimeStamp,
                TransactionDetail = trandetailsList
            };
            string serializedrequest = JsonConvert.SerializeObject(npsrequest);
            WebClient newclient = new WebClient();
            newclient.Headers["Content-type"] = "application/json";
            newclient.Headers[HttpRequestHeader.Authorization] = "Bearer" + " " + AccessToken;
            string finalresponse = newclient.UploadString(ReadConfig().BulkFundTransferUrl, serializedrequest);
            var data = JsonConvert.DeserializeObject<ATTNPSResponse>(finalresponse);
            var response = JsonConvert.DeserializeObject<ATTNPSBulkFundTransferResponse>(finalresponse);
            return response;
        }

        // bulk status check
        private static ATTNPSBulkCheckStatusResponse CheckBulkStatus(string ProcessID)
        {
            string AccessToken = Login();
            ATTNPSBulkCheckStatusRequest nps = new ATTNPSBulkCheckStatusRequest()
            {
                ApiUserName = ReadConfig().ApiUserName,
                MerchantId = ReadConfig().MerchantId,
                MerchantProcessId = ProcessID,
                TimeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff"),
            };
            var finalsignature = signData(nps);

            ATTNPSBulkCheckStatusRequest npsrequest = new ATTNPSBulkCheckStatusRequest()
            {
                ApiUserName = nps.ApiUserName,
                MerchantId = nps.MerchantId,
                MerchantProcessId = ProcessID,
                TimeStamp = nps.TimeStamp,
                Signature = finalsignature
            };
            string serializedrequest = JsonConvert.SerializeObject(npsrequest);
            WebClient newclient = new WebClient();
            newclient.Headers["Content-type"] = "application/json";
            newclient.Headers[HttpRequestHeader.Authorization] = "Bearer" + " " + AccessToken;
            string finalresponse = newclient.UploadString(ReadConfig().BulkTransactionStatusUrl, serializedrequest);
            var data = JsonConvert.DeserializeObject<ATTNPSResponse>(finalresponse);
            var response = JsonConvert.DeserializeObject<ATTNPSBulkCheckStatusResponse>(finalresponse);
            return response;
        }


        //this methods are for signature generation using pkcs1 sha256

        private static string signData(object obj)
        {
            byte[] signature;
            var jsonObj = JsonConvert.SerializeObject(obj);
            string json = GetConcatValuesFromJson(jsonObj);
            using (TextReader strReader = new StringReader(ReadConfig().PrivateKey))
            {
                var pemReader = new PemReader(strReader);
                AsymmetricCipherKeyPair keyPair = (AsymmetricCipherKeyPair)pemReader.ReadObject();
                signature = ServerGenerateSignature(json, (Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters)keyPair.Private);
            }
            return Convert.ToBase64String(signature);
        }
        private static bool ClientValidateSignature(string sourceData, byte[] signature, RsaKeyParameters publicKey)
        {
            byte[] tmpSource = Encoding.ASCII.GetBytes(sourceData);

            ISigner signClientSide = SignerUtilities.GetSigner(PkcsObjectIdentifiers.Sha256WithRsaEncryption.Id);
            signClientSide.Init(false, publicKey);
            signClientSide.BlockUpdate(tmpSource, 0, tmpSource.Length);

            return signClientSide.VerifySignature(signature);
        }

        private static byte[] ServerGenerateSignature(string sourceData, RsaKeyParameters privateKey)
        {
            byte[] tmpSource = Encoding.ASCII.GetBytes(sourceData);

            ISigner sign = SignerUtilities.GetSigner(PkcsObjectIdentifiers.Sha256WithRsaEncryption.Id);
            sign.Init(true, privateKey);
            sign.BlockUpdate(tmpSource, 0, tmpSource.Length);
            return sign.GenerateSignature();
        }


        private static string GetConcatValuesFromJson(string item)
        {
            Dictionary<string, string> obj;
            try
            {
                obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(item);
            }
            catch
            {
                obj = null;
            }
            if (obj == null)
            {
                return item;
            }
            string signData = "";
            foreach (var prop in obj.Keys.OrderBy(x => x))
            {
                if (prop.ToLower() != "signature")
                {
                    try
                    {
                        signData += obj[prop];
                    }
                    catch
                    {
                        signData += "";
                    }
                }
            }
            return signData;
        }
        #endregion

    }
}

