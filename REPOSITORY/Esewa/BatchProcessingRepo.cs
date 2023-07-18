using Dapper;
using Entity.Common;
using Entity.Esewa;
using Interface.Esewa;
using Interface.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Repository.Esewa.EsewaHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace Repository.Esewa
{
    public class BatchProcessingRepo : IBatchProcessing
    {
        IOptions<ReadConfig> _connectionString;

        private IConfiguration _configuration;
        private IAudit _audit;
        string _keys;
        public BatchProcessingRepo(IOptions<ReadConfig> connectionString, IConfiguration configuration, IAudit audit)
        {
            _connectionString = connectionString;
            _configuration = configuration;
            _keys = "EsewaKeys";
            _audit = audit;
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
                    var batchlists = connection.Query<int>(sql: "GetAllActiveBatch", param: param, null, commandType: CommandType.StoredProcedure).ToList();
                    if (batchlists.Count < 1)
                    {
                        response = new JsonResponse();
                        response.IsSuccess = false;
                        response.Message = "No Active Batch <br/> Create New Batch";
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
        public JsonResponse CheckBatchStatus(string CompCode, string DivCode, string BatchID, string UserName, string IPAddress)
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
                    param.Add("@P_BatchID", BatchID);
                    param.Add("@P_UserName", UserName);
                    param.Add("@P_IPAddress", IPAddress);
                    param.Add("@P_Entry_Date", DateTime.Now);
                    response = connection.Query<JsonResponse>(sql: "CheckBatchStatus", param: param, null, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    if (response == null)
                    {
                        response = new JsonResponse();
                        response.IsSuccess = true;
                        response.ResponseData = "BC";
                        response.ResponseData2 = 1;
                    }
                    else
                    {
                        response.ResponseData = response.ResponseData.ToString().Trim();
                        if (response.ResponseData.ToString().Contains("."))
                        {
                            response.CallBack = response.ResponseData.ToString().Split(".")[1];
                            response.ResponseData = response.ResponseData.ToString().Split(".")[0];
                            response.IsValid = true;
                        }
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
        public JsonResponse CreateBatch(string CompCode, string DivCode, string UserName, string IPAddress)
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
                        param.Add("@P_DivCode", DivCode);
                        param.Add("@P_UserName", UserName);
                        param.Add("@P_IPAddress", IPAddress);
                        param.Add("@P_Entry_Date", DateTime.Now);

                        response = connection.Query<JsonResponse>(sql: "CreateBatch", param: param, transaction, commandType: CommandType.StoredProcedure).FirstOrDefault();
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
        public JsonResponse GetBankDetailsEsewa()
        {

            return EsewaAPIMethods.GetBankDetailsEsewa();

        }
        public JsonResponse UpdateBankDetailsFromEsewa(string username, string ipaddress)
        {

            var bank = EsewaAPIMethods.GetBankDetailsEsewa();
            var response = new JsonResponse();
            response = UpdateBankDetails((ATTBanks)bank.ResponseData, username, ipaddress);
            return response;

        }
        public JsonResponse UpdateBankDetails(ATTBanks banks, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            try
            {
                using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
                {
                    connection.Open();
                    using (SqlTransaction trans = connection.BeginTransaction())
                    {

                        if (banks.banks.Count > 0)
                        {
                            DataTable dataTableSuccess = new DataTable();
                            dataTableSuccess.Columns.Add("Bank_Code", typeof(string));
                            dataTableSuccess.Columns.Add("Bank_Name", typeof(string));
                            dataTableSuccess.Columns.Add("Bank_Regex", typeof(string));
                            dataTableSuccess.Columns.Add("Account_Validation", typeof(bool));


                            banks.banks.ForEach(att =>
                            {
                                dataTableSuccess.Rows.Add(att.Bank_Code, att.Bank_Name,
                                  att.Bank_Regex, att.Account_Validation);

                            });
                            using (SqlCommand cmd = new SqlCommand("SAVE_API_BANK_DETAILS"))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Connection = connection;
                                cmd.Transaction = trans;
                                cmd.Parameters.AddWithValue("@APIBANKDETAILS", dataTableSuccess);
                                cmd.Parameters.AddWithValue("@UserName", UserName);
                                cmd.Parameters.AddWithValue("@IPAddress", IPAddress);
                                cmd.Parameters.AddWithValue("@Entry_Date", DateTime.Now);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        if (reader.GetInt32(0) == 1)
                                        {
                                            response.IsSuccess = true;
                                            response.Message = reader.GetString(1);
                                        }
                                        else
                                        {
                                            response.Message = reader.GetString(1);
                                        }
                                    }
                                }
                                if (response.IsSuccess) trans.Commit();

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ResponseData = ex;
                response.IsSuccess = false;
            }
            return response;
        }
        public JsonResponse UpdateBankDetailsToSystem(string swiftcode, string bankcode, string UserName, string IPAddress)
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
                        param.Add("@bankcode", bankcode);
                        param.Add("@swiftcode", swiftcode);
                        param.Add("@UserName", UserName);
                        param.Add("@iPAddress", IPAddress);
                        param.Add("@Entry_Date", DateTime.Now);

                        response = connection.Query<JsonResponse>(sql: "SAVE_API_BANK_DETAILS_WITH_NCHL_CODE", param: param, transaction, commandType: CommandType.StoredProcedure).FirstOrDefault();
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
        public JsonResponse UploadCDSData(DataTable CdsTable, string UserName, string IPAddress, string CompCode, string DivCode, string BatchID)
        {
            JsonResponse jsonResponse = new JsonResponse();


            using (SqlConnection con = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    foreach (DataRow drow in CdsTable.Rows)
                    {
                        drow["fullname"] = Regex.Replace(drow["fullname"].ToString(), @"\s+", "").ToString();
                        drow["faname"] = Regex.Replace(drow["faname"].ToString(), @"\s+", "").ToString();
                        drow["grfaname"] = Regex.Replace(drow["grfaname"].ToString(), @"\s+", "").ToString();
                        drow["bankname"] = Regex.Replace(drow["bankname"].ToString(), @"\s+", "").ToString();
                    }
                    con.Open();
                    using (SqlTransaction trans = con.BeginTransaction())
                    {
                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con, SqlBulkCopyOptions.KeepIdentity, trans))
                        {
                            //Set the database table name.
                            sqlBulkCopy.DestinationTableName = "dbo.CDSImport";


                            //[OPTIONAL]: Map the Excel columns with that of the database table

                            //-- ExcelDataReader -- SQL

                            sqlBulkCopy.ColumnMappings.Add("boid", "boid");
                            sqlBulkCopy.ColumnMappings.Add("fullname", "name");
                            sqlBulkCopy.ColumnMappings.Add("faname", "fathername");
                            sqlBulkCopy.ColumnMappings.Add("grfaname", "grandfathername");
                            sqlBulkCopy.ColumnMappings.Add("address", "address");
                            sqlBulkCopy.ColumnMappings.Add("bankcode", "bankcode");
                            sqlBulkCopy.ColumnMappings.Add("bankname", "bankname");
                            sqlBulkCopy.ColumnMappings.Add("bankaccno", "bankno");
                            sqlBulkCopy.ColumnMappings.Add("citno", "citizenshipno");

                            sqlBulkCopy.ColumnMappings.Add("compcode", "compcode");
                            sqlBulkCopy.ColumnMappings.Add("divcode", "divcode");
                            sqlBulkCopy.ColumnMappings.Add("batchid", "batchid");
                            sqlBulkCopy.ColumnMappings.Add("entryuser", "entryuser");
                            sqlBulkCopy.ColumnMappings.Add("entrydate", "entrydate");
                            sqlBulkCopy.BatchSize = 10000;
                            sqlBulkCopy.BulkCopyTimeout = 60;
                            sqlBulkCopy.WriteToServer(CdsTable);
                            DynamicParameters param = new DynamicParameters();
                            param.Add("@P_CompCode", CompCode.Substring(0, 3));
                            param.Add("@P_UserName", UserName);
                            param.Add("@P_DivCode", DivCode);
                            param.Add("@P_BatchID", BatchID);
                            param.Add("@P_IPAddress", IPAddress);
                            param.Add("@P_Entry_Date", DateTime.Now);
                            jsonResponse = con.Query<JsonResponse>(sql: "UPDATE_BATCH_DETAIL_AFTER_CDS_IMPORT", param: param, trans, commandType: CommandType.StoredProcedure).FirstOrDefault();
                            if (jsonResponse.IsSuccess)
                            {
                                trans.Commit();
                                jsonResponse.Message = "CDS Data Imported Successfully .";
                            }
                            else
                            {
                                trans.Rollback();

                            }


                            con.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.HasError = true;
                    jsonResponse.ResponseData = ex;
                    jsonResponse.IsSuccess = false;
                    jsonResponse.Message = ex.Message;
                }
            }
            return jsonResponse;
        }

        public JsonResponse ValidateCDSData(string CompCode, string DivCode, string BatchID, string UserName, string IPAddress)
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
                        param.Add("@P_DivCode", DivCode);
                        param.Add("@P_BatchID", BatchID);
                        param.Add("@P_UserName", UserName);
                        param.Add("@P_IPAddress", IPAddress);
                        param.Add("@P_Entry_Date", DateTime.Now);
                        response = connection.Query<JsonResponse>(sql: "ValidateCDSData", param: param, transaction, commandType: CommandType.StoredProcedure).FirstOrDefault();
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
        #region old code
        //public JsonResponse ValidateAccountDetailsBatchOld(string DivCode, string CompCode, string BatchID, string BankUserName, string BankPassword, string UserName, string IPAddress)
        //{
        //    JsonResponse jsonResponse = new JsonResponse();
        //    JsonResponse response = new JsonResponse();
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
        //            List<ATTEBatchProcessing> aTTEBatchProcessings = connection.Query<ATTEBatchProcessing>(sql: "VALIDATE_ACCOUNT_DETAILS_GET_BATCHPROCESSING_DATA_1", parameters, null, commandType: CommandType.StoredProcedure).ToList();
        //            if (aTTEBatchProcessings.Count > 0)
        //            {

        //                var clientPrivateKey = _configuration.GetSection(_keys).GetSection("ClientPrivateKey").Value;
        //                var clientPublicKey = _configuration.GetSection(_keys).GetSection("ClientPublicKey").Value;
        //                var esewaPublicKey = _configuration.GetSection(_keys).GetSection("EsewaPublicKey").Value;
        //                var esewaPublicKeySig = _configuration.GetSection(_keys).GetSection("EsewaPublicKeySig").Value;
        //                var clinetId = _configuration.GetSection(_keys).GetSection("ClientId").Value;
        //                int divider = 1000; //block size for account validation
        //                int block = (int)Math.Ceiling(((double)aTTEBatchProcessings.Count) / divider);
        //                for (int currentblock = 1; currentblock <= block; currentblock++)
        //                {
        //                    List<ATTEBatchProcessing> BatchToProcess = new List<ATTEBatchProcessing>();
        //                    var data = string.Empty;
        //                    List<ATTTransctionDetails> aTTransctionDetails = new List<ATTTransctionDetails>();
        //                    for (int item = (currentblock - 1) * divider; item < currentblock * divider; item++)
        //                    {
        //                        if (item < aTTEBatchProcessings.Count)
        //                        {
        //                            BatchToProcess.Add(aTTEBatchProcessings[item]);
        //                            aTTransctionDetails.Add(
        //                              new ATTTransctionDetails()
        //                              {
        //                                  payee_bank_code = aTTEBatchProcessings[item].SwiftCode,
        //                                  sub_token = aTTEBatchProcessings[item].sub_token,
        //                                  payee_account_name = aTTEBatchProcessings[item].FullName,
        //                                  payee_account_number = aTTEBatchProcessings[item].BankNo
        //                              });
        //                        }
        //                        else
        //                        {
        //                            item = currentblock * divider;
        //                        }

        //                    }
        //                    //foreach (ATTEBatchProcessing batchProcessing in aTTEBatchProcessings)

        //                    //{
        //                    //    aTTransctionDetails.Add(
        //                    //          new ATTTransctionDetails()
        //                    //          {
        //                    //              payee_bank_code = batchProcessing.SwiftCode,
        //                    //              sub_token = batchProcessing.sub_token,
        //                    //              payee_account_name = batchProcessing.FullName,
        //                    //              payee_account_number = batchProcessing.BankNo
        //                    //          });
        //                    //}
        //                    ATTAccountDetails aTTAccountDetails = new ATTAccountDetails()
        //                    {
        //                        token = aTTEBatchProcessings[0].Token,
        //                        transaction_details = aTTransctionDetails
        //                    };

        //                    data = JsonConvert.SerializeObject(aTTAccountDetails);
        //                    //BankUserName = "accounts@f1soft.com";
        //                    //BankPassword = "corporate@222";
        //                    BankUserName = _configuration.GetSection(_keys).GetSection("UserName").Value;
        //                    BankPassword = _configuration.GetSection(_keys).GetSection("Password").Value;
        //                    var alphaNumeric = AlphaNumericString.getAlphaNumericString(32);
        //                    byte[] secrect = EncryptDecryptSecreteKey.getSecretKey(alphaNumeric);

        //                    string base64EncodedSecretKey = EncryptDecryptSecreteKey.keyToString(secrect);
        //                    byte[] encryptedSecretKey = RSAEncryptDecrypt.encrypt(base64EncodedSecretKey, esewaPublicKey);
        //                    string finalSecretKey = EncryptDecryptSecreteKey.base64Encode(encryptedSecretKey);
        //                    string encryptedData = EncryptDecryptSecreteKey.encrypt(data, secrect);
        //                    string signature = SignaturePayload.generateSignature(encryptedData, clientPrivateKey);
        //                    var response1 = CallEsewaApiAccountValidation(encryptedData, signature, finalSecretKey, clinetId, BankUserName, BankPassword);
        //                    Regex reg = new Regex("^((?!\\<(|\\/)[a-z][a-z0-9]*>).)*$");
        //                    Match match = reg.Match(response1.Content);
        //                    List<ATTEsewaNewResponse> aTTEsewaResponse = new List<ATTEsewaNewResponse>();
        //                    ATTEncryptedDetails output = new ATTEncryptedDetails();
        //                    if (match.Success) { output = JsonConvert.DeserializeObject<ATTEncryptedDetails>(response1.Content); }
        //                    else { output.error = true; output.message = "Invalid Credentials Supplied!!"; }

        //                    if (!output.error)
        //                    {
        //                        bool valid = SignaturePayload.verifySignature(output.data, esewaPublicKeySig, EncryptDecryptSecreteKey.base64Decode(output.signature));
        //                        if (valid)
        //                        {
        //                            byte[] decodedSecretKey = EncryptDecryptSecreteKey.base64Decode(output.secret_key);
        //                            string plainSecretKey = RSAEncryptDecrypt.decrypt(decodedSecretKey, clientPrivateKey);
        //                            byte[] outPutSecetKey = EncryptDecryptSecreteKey.getSecretKey(plainSecretKey);
        //                            string finalOutput = EncryptDecryptSecreteKey.decrypt(output.data, outPutSecetKey);
        //                            aTTEsewaResponse = JsonConvert.DeserializeObject<List<ATTEsewaNewResponse>>(finalOutput);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        jsonResponse.IsSuccess = !output.error;
        //                        jsonResponse.Message = output.message;
        //                    }
        //                    //new method of updating response
        //                    foreach (var res in aTTEsewaResponse)
        //                    {
        //                        BatchToProcess.Where(X =>
        //                                                   X.BankNo == res.payee_account_number
        //                                                   && X.FullName == res.payee_bank_code
        //                                                   && X.SwiftCode == res.payee_bank_code).ToList()
        //                                                   .ForEach(v =>
        //                                                   {
        //                                                       v.ValidateMessage = res.message;
        //                                                       v.ValidAccount = res.error;
        //                                                       v.ValidateCode = res.code;
        //                                                       v.Percentage = res.percentage;
        //                                                   }
        //                                                   );


        //                    }
        //                    /* old method of updating response
        //                    foreach (var udtData in aTTEBatchProcessings.Join(aTTEsewaResponse,
        //                                b => b.BankNo, e => e.payee_account_number,
        //                                (b, e) => new { b, e })
        //                                .Where(combine =>
        //                                {
        //                                    return (combine.b.BankNo == combine.e.payee_account_number) && (combine.b.BankName == combine.e.payee_account_name);
        //                                }))
        //                    {
        //                        udtData.b.ValidAccount = !udtData.e.error;
        //                        udtData.b.ValidateCode = udtData.e.code;
        //                        udtData.b.ValidateMessage = udtData.e.message;
        //                    }
        //                    */
        //                    ///update in database
        //                    using (SqlTransaction tran = connection.BeginTransaction())
        //                    {
        //                        DataTable dt = new DataTable();

        //                        dt.Columns.Add("WarrantNo");
        //                        dt.Columns.Add("ValidAccount");
        //                        dt.Columns.Add("ValidCode");
        //                        dt.Columns.Add("ValidMessage");
        //                        dt.Columns.Add("ValidPercentage");
        //                        BatchToProcess.ForEach(x => dt.Rows.Add(x.WarrantNo, x.ValidAccount, x.ValidateCode, x.ValidateMessage, x.Percentage));
        //                        SqlCommand cmd = new SqlCommand("ESEWA_ACCOUNT_VALIDATION", connection);
        //                        cmd.CommandType = CommandType.StoredProcedure;
        //                        cmd.Transaction = tran;
        //                        SqlParameter sqlParameter = cmd.Parameters.AddWithValue("@UDT", dt);
        //                        sqlParameter = cmd.Parameters.AddWithValue("@P_COMPCODE", CompCode);
        //                        sqlParameter = cmd.Parameters.AddWithValue("@P_DivCode", DivCode);
        //                        sqlParameter = cmd.Parameters.AddWithValue("@P_BATCHID", BatchID);
        //                        sqlParameter = cmd.Parameters.AddWithValue("@P_IP_ADDRESS", IPAddress);
        //                        sqlParameter = cmd.Parameters.AddWithValue("@P_USERNAME", UserName);
        //                        sqlParameter = cmd.Parameters.AddWithValue("@P_DATE_NOW", DateTime.Now);
        //                        sqlParameter.Direction = ParameterDirection.Input;
        //                        using (SqlDataReader reader = cmd.ExecuteReader())
        //                        {
        //                            if (reader.Read())
        //                            {
        //                                if (reader.GetString(0) == "true")
        //                                {
        //                                    response.IsSuccess = true;
        //                                    response.Message = reader.GetString(1);
        //                                }
        //                                else
        //                                {
        //                                    response.Message = reader.GetString(1);
        //                                }

        //                            }
        //                        }
        //                        if (response.IsSuccess)
        //                            tran.Commit();
        //                        else
        //                            tran.Rollback();

        //                    }
        //                    currentblock++;
        //                }

        //            }
        //            else
        //            {
        //                response = CloseBatch(DivCode, CompCode, BatchID, UserName, IPAddress, "NO DATA AVAILABLE FOR THE CURRENT DIVIDEND AND THE CURRENT BATCH !!! ");
        //                response.IsSuccess = false;
        //                response.Message = "NO DATA AVAILABLE FOR THE CURRENT DIVIDEND AND THE CURRENT BATCH !!! <br/> BATCH WILL BE CLOSED";

        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            response.IsSuccess = false;
        //            response.HasError = true;
        //            response.ResponseData = ex;
        //        }
        //        return response;
        //    }
        //}
        public JsonResponse TransactionProcessing(string DivCode, string CompCode, string BatchID, string BankUserName, string BankPassword, string SourceBankName, string SourceBankNumber, string UserName, string IPAddress)
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

                        JsonResponse res = EsewaAPIMethods.TransactionProcessing(aTTEBatchProcessings, SourceBankName, SourceBankNumber);
                        if (!res.IsSuccess) { return res; }
                        else { aTTEBatchProcessings = (List<ATTEBatchProcessing>)res.ResponseData; }

                        using (SqlTransaction tran = connection.BeginTransaction())
                        {
                            DataTable dt = new DataTable();

                            dt.Columns.Add("WarrantNo");
                            dt.Columns.Add("TransactionCode");
                            dt.Columns.Add("TransactionMessage");
                            dt.Columns.Add("TransactionDetail");
                            aTTEBatchProcessings.ForEach(x => dt.Rows.Add(x.WarrantNo, x.TransactionCode, x.TransactionMessage, x.TransactionDetail));
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
                                    if (reader.GetString(0) == "true")
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
        public JsonResponse ValidateAccountDetails(string DivCode, string CompCode, string BatchID, string BankUserName, string BankPassword, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            JsonResponse response = new JsonResponse();
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
                    List<ATTEBatchProcessing> aTTEBatchProcessings = connection.Query<ATTEBatchProcessing>(sql: "VALIDATE_ACCOUNT_DETAILS_GET_BATCHPROCESSING_DATA", parameters, null, commandType: CommandType.StoredProcedure).ToList();
                    if (aTTEBatchProcessings.Count > 0)
                    {
                        JsonResponse res = EsewaAPIMethods.AccountValidation(aTTEBatchProcessings, BankUserName, BankPassword);
                        if (!res.IsSuccess) { return res; }
                        else { aTTEBatchProcessings = (List<ATTEBatchProcessing>)res.ResponseData; }
                        using (SqlTransaction tran = connection.BeginTransaction())
                        {
                            DataTable dt = new DataTable();

                            dt.Columns.Add("WarrantNo");
                            dt.Columns.Add("ValidAccount");
                            dt.Columns.Add("ValidCode");
                            dt.Columns.Add("ValidMessage");
                            aTTEBatchProcessings.ForEach(x => dt.Rows.Add(x.WarrantNo, x.ValidAccount, x.ValidateCode, x.ValidateMessage));
                            SqlCommand cmd = new SqlCommand("ESEWA_ACCOUNT_VALIDATION", connection);
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
                                    if (reader.GetString(0) == "true")
                                    {
                                        response.IsSuccess = true;
                                        response.Message = reader.GetString(1);
                                    }
                                    else
                                    {
                                        response.Message = reader.GetString(1);
                                    }

                                }
                            }
                            if (response.IsSuccess)
                                tran.Commit();
                            else
                                tran.Rollback();

                        }
                    }
                    else
                    {
                        response = CloseBatch(DivCode, CompCode, BatchID, UserName, IPAddress, "NO DATA AVAILABLE FOR THE CURRENT DIVIDEND AND THE CURRENT BATCH !!! ");
                        response.IsSuccess = false;
                        response.Message = "NO DATA AVAILABLE FOR THE CURRENT DIVIDEND AND THE CURRENT BATCH !!! <br/> BATCH WILL BE CLOSED";

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
        #endregion

        public JsonResponse ValidateAccountDetailsBatch(string DivCode, string CompCode, string BatchID, string BankUserName, string BankPassword, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                List<ATTEBatchProcessing> aTTEBatchProcessings = new List<ATTEBatchProcessing>();
                try
                {
                    connection.Open();
                    bool DataHasCount = true;
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_COMPCODE", CompCode);
                    parameters.Add("@P_DIVCODE", DivCode);
                    parameters.Add("@P_BATCHID", BatchID);
                    parameters.Add("@P_USERNAME", UserName);
                    parameters.Add("@P_IP_ADDRESS", IPAddress);
                    parameters.Add("@P_ENTRY_DATE", DateTime.Now);


                    aTTEBatchProcessings = connection.Query<ATTEBatchProcessing>(sql: "VALIDATE_ACCOUNT_DETAILS_GET_BATCHPROCESSING_DATA", parameters, null, commandType: CommandType.StoredProcedure).ToList();

                    List<ATTEBatchProcessing> APIResponse = new List<ATTEBatchProcessing>();
                    if (aTTEBatchProcessings.Count > 0)
                    {
                        foreach (var BatchData in aTTEBatchProcessings)
                        {

                            JsonResponse response1 = EsewaAPIMethods.AccountValidation(BatchData, BankUserName, BankPassword);
                            if (!response1.IsSuccess) { return response; }
                            else
                            {
                                var BatchDataRes = (ATTEBatchProcessing)response1.ResponseData;
                                BatchData.ValidAccount = BatchDataRes.ValidAccount;
                                BatchData.ValidateCode = BatchDataRes.ValidateCode ?? "0";
                                BatchData.ValidateMessage = BatchDataRes.ValidateMessage ?? "No Response";
                                BatchData.Percentage = BatchDataRes.Percentage;
                                APIResponse.Add(BatchDataRes);
                            }
                        }
                        ///update in database
                        using (SqlTransaction tran = connection.BeginTransaction())
                        {
                            DataTable dt = new DataTable();

                            dt.Columns.Add("WarrantNo");
                            dt.Columns.Add("ValidAccount");
                            dt.Columns.Add("ValidCode");
                            dt.Columns.Add("ValidMessage");
                            dt.Columns.Add("ValidPercentage");
                            APIResponse.ForEach(x => dt.Rows.Add(x.WarrantNo, x.ValidAccount, x.ValidateCode, x.ValidateMessage, x.Percentage == "" ? "0" : x.Percentage));



                            SqlCommand cmd = new SqlCommand("ESEWA_ACCOUNT_VALIDATION", connection);
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
                            using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                            {
                                if (reader.Read())
                                {
                                    if (reader.GetString(0).ToLower() == "true")
                                    {
                                        response.IsSuccess = true;
                                        response.Message = reader.GetString(1);

                                    }
                                    else
                                    {

                                        response.Message = reader.GetString(1);
                                    }

                                }

                            }
                            if (response.IsSuccess)
                            {
                                tran.Commit();
                            }
                            else
                            {
                                tran.Rollback();
                            }
                        }

                    }
                    else
                    {

                        response = CloseBatch(DivCode, CompCode, BatchID, UserName, IPAddress, "NO DATA AVAILABLE FOR THE CURRENT DIVIDEND AND THE CURRENT BATCH !!! ");
                        response.IsSuccess = false;
                        response.Message = "NO DATA AVAILABLE FOR THE CURRENT DIVIDEND AND THE CURRENT BATCH !!! <br/> BATCH WILL BE CLOSED";

                    }




                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.HasError = true;
                    response.ResponseData = ex;
                    response.Message = ex.Message;
                    UndoFailedAccountValidation(aTTEBatchProcessings, CompCode, DivCode, BatchID, IPAddress, UserName);
                    AppLogger.LogErrorToText(ex, "Account Validation for Batch :" + BatchID + " Compcode: " + CompCode);
                }
                return response;
            }
        }

        public JsonResponse UndoFailedAccountValidation(List<ATTEBatchProcessing> failedBatch, string CompCode, string DivCode, string BatchID, string IPAddress, string UserName)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                using (SqlTransaction tran = connection.BeginTransaction())
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Column1");
                    failedBatch.ForEach(x => dt.Rows.Add(x.WarrantNo));
                    SqlCommand cmd = new SqlCommand("ESEWA_ACCOUNT_VALIDATION_ERROR", connection);
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
                                response.IsSuccess = true;
                                response.Message = reader.GetString(1);
                            }
                            else
                            {
                                response.Message = reader.GetString(1);
                            }

                        }
                    }
                    if (response.IsSuccess)
                        tran.Commit();
                    else
                        tran.Rollback();

                }
                return response;
            }
        }

        public JsonResponse GetBanks(string username, string ipaddress, string controllerName)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@controllername", controllerName);
                    param.Add("@P_UserName", username);
                    param.Add("@P_IPAddress", ipaddress);
                    param.Add("@P_Entry_Date", DateTime.Now);

                    var bank = connection.Query<APIBanksDetailFromSystem>(sql: "GET_APIBANKS", param: param, null, commandType: CommandType.StoredProcedure).ToList();
                    if (bank.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = bank;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = "No Bank List Found in System!!";
                    }

                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.HasError = true;
                    response.ResponseData = ex;
                    AppLogger.LogErrorToText(ex, "Get Esewa Bank Details from System");
                }
                return response;
            }


        }
    }
}
