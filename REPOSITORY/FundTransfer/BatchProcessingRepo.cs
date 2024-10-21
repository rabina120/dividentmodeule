using Dapper;
using Entity.Common;
using ENTITY.FundTransfer;
using Interface.Security;
using INTERFACE.FundTransfer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace REPOSITORY.FundTransfer
{
    public class BatchProcessingRepo : IBatchProcessing
    {
        IOptions<ReadConfig> _connectionString;

        private IConfiguration _configuration;
        private IAudit _audit;
        public BatchProcessingRepo(IOptions<ReadConfig> connectionString, IConfiguration configuration, IAudit audit)
        {
            _connectionString = connectionString;
            _configuration = configuration;
            _audit = audit;
        }
        #region batch status processing 
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
                    var batchlists = connection.Query<int>(sql: "FT_BATCHPROCESSING_GET_ALL_BATCH", param: param, null, commandType: CommandType.StoredProcedure).ToList();
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
                    response = connection.Query<JsonResponse>(sql: "FT_BATCHPROCESSING_CHECK_BATCH_STATUS", param: param, null, commandType: CommandType.StoredProcedure).FirstOrDefault();
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

                        response = connection.Query<JsonResponse>(sql: "FT_BATCH_CREATE_BATCH", param: param, transaction, commandType: CommandType.StoredProcedure).FirstOrDefault();
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

                        response = connection.Query<JsonResponse>(sql: "FT_BATCH_CLOSE_BATCH", param: param, transaction, commandType: CommandType.StoredProcedure).FirstOrDefault();
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
        #endregion
        #region banks
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

                    var bank = connection.Query<APIBanksDetailFromSystem>(sql: "FT_GET_ALL_BANKS", param: param, null, commandType: CommandType.StoredProcedure).ToList();
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
                    ex.LogErrorToText("Get Esewa Bank Details from System");
                }
                return response;
            }


        }
        public JsonResponse GetBankDetailsFromAPI()
        {
            JsonResponse jsonResponse = new JsonResponse
            {
                ResponseData = NPSHelper.GetBanksFromProvider()
            };
            if (jsonResponse.ResponseData != null)
            {
                jsonResponse.IsSuccess = true;
            }
            return jsonResponse;

        }
        public JsonResponse UpdateBankDetailsFromAPI(string username, string ipaddress)
        {

            var bank = NPSHelper.GetBanksFromProvider();

            var response = new JsonResponse();
            response = UpdateBankDetails(bank, username, ipaddress);
            return response;

        }
        public JsonResponse UpdateBankDetails(List<ATTBank> banks, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            response.Message = ATTMessages.CANNOT_SAVE;
            try
            {
                using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
                {
                    connection.Open();
                    using (SqlTransaction trans = connection.BeginTransaction())
                    {

                        if (banks.Count > 0)
                        {
                            DataTable dataTableSuccess = new DataTable();
                            dataTableSuccess.Columns.Add("Bank_Code", typeof(string));
                            dataTableSuccess.Columns.Add("Bank_Name", typeof(string));



                            banks.ForEach(att =>
                            {
                                dataTableSuccess.Rows.Add(att.BanKCode, att.BankName);

                            });
                            using (SqlCommand cmd = new SqlCommand("FT_SAVE_BANK_LIST"))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Connection = connection;
                                cmd.Transaction = trans;
                                cmd.Parameters.AddWithValue("@FT_UDT_BANK_LIST", dataTableSuccess);
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
        #endregion
        #region create cds data for batch 
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
                        response = connection.Query<JsonResponse>(sql: "FT_BATCH_CREATE_ADD_BATCH_DATA", param: param, transaction, commandType: CommandType.StoredProcedure).FirstOrDefault();
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
        #endregion
        #region account validation
        public JsonResponse ValidateAccountDetailsAsync(string DivCode, string CompCode, string BatchID, string BankUserName, string BankPassword, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();

            List<ATTEBatchProcessing> aTTEBatchProcessings = new List<ATTEBatchProcessing>();
            aTTEBatchProcessings = GetDataForAccountValidation(DivCode, CompCode, BatchID, UserName, IPAddress);

            var AccValidationTask = StartAccountValidation(aTTEBatchProcessings, DivCode, CompCode, BatchID, UserName, IPAddress);
            var AccValidationResponse = AccValidationTask.Result;
            if (AccValidationTask.IsCompleted)
            {
                response = UpdateBatchAfterAccountValidation(DivCode, CompCode, BatchID, UserName, IPAddress);
            }

            return response;

        }
        private async Task<JsonResponse> StartAccountValidation(List<ATTEBatchProcessing> aTTEBatchProcessings, string DivCode, string CompCode, string BatchID, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            int maxRunningThread = 100;
            int responseTimeoutSec = 60;
            var semaphore = new SemaphoreSlim(maxRunningThread);
            List<Task<ATTEBatchProcessing>> accountValidationTasks = new List<Task<ATTEBatchProcessing>>();
            foreach (var batchRow in aTTEBatchProcessings)
            {
                var task = Task.Run(async () =>
                {

                    await semaphore.WaitAsync(); // Wait until semaphore is available
                    try
                    {
                        await Task.Delay(100);
                        return NPSHelper.AccountValidation(batchRow);

                    }
                    catch (Exception ex)
                    {

                        batchRow.ValidAccount = false;
                        batchRow.ValidateMessage = "API Account Validation Failed";
                        batchRow.ValidateCode = 500.ToString();
                        return batchRow;

                    }
                    finally
                    {
                        semaphore.Release(); // Release semaphore when task is done
                    }
                });

                accountValidationTasks.Add(task);
            }
            foreach (var task in accountValidationTasks)
            {
                var responseFromApi = await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(responseTimeoutSec)));
                ATTEBatchProcessing batchRow = new ATTEBatchProcessing();
                if (responseFromApi == task)
                {
                    batchRow = task.Result;

                }
                else
                {
                    batchRow.ValidAccount = false;
                    batchRow.ValidateMessage = "TimeOut Error Occured.";
                    batchRow.ValidateCode = 500.ToString();

                }
                if (task.IsCompleted)
                    response = await ProcessAccountValidationResponse(batchRow, DivCode, CompCode, BatchID, UserName, IPAddress);
            }
            response.IsSuccess = true;
            response.Message = "Account Validation Started.";
            return response;
        }
        public JsonResponse ValidateAccountDetailsBatch(string DivCode, string CompCode, string BatchID, string BankUserName, string BankPassword, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            List<ATTEBatchProcessing> aTTEBatchProcessings = new List<ATTEBatchProcessing>();
            aTTEBatchProcessings = GetDataForAccountValidation(DivCode, CompCode, BatchID, UserName, IPAddress);
            if (aTTEBatchProcessings.Count > 0)
            {

                List<ATTEBatchProcessing> APIResponse = new List<ATTEBatchProcessing>();
                APIResponse = NPSHelper.AccountValidation(aTTEBatchProcessings);

                response = ProcessAccountValidationResponse(APIResponse, DivCode, CompCode, BatchID, UserName, IPAddress);
            }
            else
            {
                response.Message = "No Accounts To Validate";
            }
            return response;
        }
        private List<ATTEBatchProcessing> GetDataForAccountValidation(string DivCode, string CompCode, string BatchID, string UserName, string IPAddress)
        {
            List<ATTEBatchProcessing> aTTEBatchProcessings = new List<ATTEBatchProcessing>();
            try
            {
                using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {

                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@P_COMPCODE", CompCode);
                        parameters.Add("@P_DIVCODE", DivCode);
                        parameters.Add("@P_BATCHID", BatchID);
                        parameters.Add("@P_USERNAME", UserName);
                        parameters.Add("@P_IP_ADDRESS", IPAddress);
                        parameters.Add("@P_ENTRY_DATE", DateTime.Now);


                        aTTEBatchProcessings = connection.Query<ATTEBatchProcessing>(sql: "FT_ACCOUNT_VALIDATION_GET_BATCHPROCESSING_DATA", parameters, transaction, commandType: CommandType.StoredProcedure).ToList();
                        transaction.Commit();
                    }

                }
            }
            catch (Exception ex)
            {
                AppLogger.LogErrorToText(ex);
            }
            return aTTEBatchProcessings;
        }
        private async Task<JsonResponse> ProcessAccountValidationResponse(ATTEBatchProcessing aTTEBatchProcessings, string DivCode, string CompCode, string BatchID, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    ///update in database
                    bool _processFailed = false;
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        try
                        {
                            string sql = "FT_ACCOUNT_VALIDATION_SAVE_UPDATE_ACCOUNT_VALIDATION_INDIVIDUAL";
                            DynamicParameters parameters = new DynamicParameters();
                            parameters.Add("@P_COMPCODE", CompCode);
                            parameters.Add("@P_DIVCODE", DivCode);
                            parameters.Add("@P_BATCHID", BatchID);
                            parameters.Add("@P_USERNAME", UserName);
                            parameters.Add("@P_IP_ADDRESS", IPAddress);
                            parameters.Add("@P_DATE_NOW", DateTime.Now);
                            parameters.Add("@P_WARRANTNO", aTTEBatchProcessings.WarrantNo);
                            parameters.Add("@P_VALIDCODE", aTTEBatchProcessings.ValidateCode);
                            parameters.Add("@P_VALIDMESSAGE", aTTEBatchProcessings.ValidateMessage);
                            parameters.Add("@P_VALIDPERCENTAGE", aTTEBatchProcessings.Percentage, DbType.Decimal);
                            await connection.QueryAsync(sql, parameters, tran, commandType: CommandType.StoredProcedure);
                            response.IsSuccess = true;
                            response.Message = ATTMessages.SAVED_SUCCESS;

                            if (response.IsSuccess)
                            {
                                _processFailed = false;
                            }
                            else
                            {
                                _processFailed = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            _processFailed = false;
                        }
                        if (!_processFailed)
                        {
                            tran.Commit();
                        }
                        else
                        {
                            tran.Rollback();
                        }

                    }

                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.HasError = true;
                    response.ResponseData = ex;
                    response.Message = ex.Message;
                    //UndoFailedAccountValidation(aTTEBatchProcessings, CompCode, DivCode, BatchID, IPAddress, UserName);
                    ex.LogErrorToText("Account Validation for Batch :" + BatchID + " Compcode: " + CompCode);
                }
            }

            return response;
        }

        private JsonResponse UpdateBatchAfterAccountValidation(string DivCode, string CompCode, string BatchID, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_COMPCODE", CompCode);
                    parameters.Add("@P_DIVCODE", DivCode);
                    parameters.Add("@P_BATCHID", BatchID);
                    parameters.Add("@P_USERNAME", UserName);
                    parameters.Add("@P_IP_ADDRESS", IPAddress);
                    parameters.Add("@P_DATE_NOW", DateTime.Now);
                    response = connection.Query<JsonResponse>(sql: "FT_ACCOUNT_VALIDATION_UPDATE_BATCH_AFTER_VALIDATION", parameters, transaction, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    transaction.Commit();
                }

            }
            return response;
        }
        private JsonResponse ProcessAccountValidationResponse(List<ATTEBatchProcessing> aTTEBatchProcessings, string DivCode, string CompCode, string BatchID, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    bool _processFailed = true;
                    if (aTTEBatchProcessings.Count > 0)
                    {
                        ///update in database

                        using (SqlTransaction tran = connection.BeginTransaction())
                        {
                            try
                            {

                                DataTable dt = new DataTable();

                                dt.Columns.Add("WarrantNo");
                                dt.Columns.Add("ValidAccount");
                                dt.Columns.Add("ValidCode");
                                dt.Columns.Add("ValidMessage");
                                dt.Columns.Add("ValidPercentage", typeof(decimal));
                                aTTEBatchProcessings.ForEach(x => dt.Rows.Add(x.WarrantNo, x.ValidAccount, x.ValidateCode, x.ValidateMessage, x.Percentage));



                                SqlCommand cmd = new SqlCommand("FT_ACCOUNT_VALIDATION_SAVE_UPDATE_ACCOUNT_VALIDATION", connection);
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
                                    _processFailed = false;
                                }
                                else
                                {
                                    _processFailed = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                _processFailed = false;
                            }
                            if (!_processFailed)
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
                    ex.LogErrorToText("Account Validation for Batch :" + BatchID + " Compcode: " + CompCode);
                }
            }

            return response;
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
                    SqlCommand cmd = new SqlCommand("FT_ACCOUNT_VALIDATION_UNDO_ON_VALIDATION_ERROR", connection);
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

        #endregion
        #region transaction processing 
        private JsonResponse GetDataForTransactionProcessing(string DivCode, string CompCode, string BatchID, string UserName, string IPAddress)
        {
            List<ATTEBatchProcessing> aTTEBatchProcessings = new List<ATTEBatchProcessing>();
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
                    aTTEBatchProcessings = connection.Query<ATTEBatchProcessing>(sql: "FT_TRANSACTIONPROCESSING_GET_DATA_FOR_TRANSACTION", parameters, null, commandType: CommandType.StoredProcedure).ToList();
                    jsonResponse.IsSuccess = aTTEBatchProcessings.Count > 0;
                    if (jsonResponse.IsSuccess)
                        jsonResponse.ResponseData = aTTEBatchProcessings;
                    else jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.HasError = true;
                    jsonResponse.ResponseData = ex;
                }
            }
            aTTEBatchProcessings.LogData("TransactionProcessing", "Get Trasaction Processing Data");

            return jsonResponse;
        }
        private JsonResponse SaveTransactionProcessingData(List<ATTEBatchProcessing> aTTEBatchProcessings, string DivCode, string CompCode, string BatchID, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            aTTEBatchProcessings.LogData("TransactionProcessing", "Saving Transaction Processing Data");

            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                using (SqlTransaction tran = connection.BeginTransaction())
                {
                    try
                    {

                        DataTable dt = new DataTable();

                        dt.Columns.Add("WarrantNo");
                        dt.Columns.Add("TransactionCode");
                        dt.Columns.Add("TransactionMessage");
                        dt.Columns.Add("TransactionDetail");
                        string token = aTTEBatchProcessings[0].Token;
                        aTTEBatchProcessings.ForEach(x => dt.Rows.Add(x.WarrantNo, x.TransactionCode, x.TransactionMessage, x.TransactionDetail));
                        SqlCommand cmd = new SqlCommand("FT_TRANSACTIONPROCESSING_SAVE_UPDATE_TRANSACTION_REPONSE", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Transaction = tran;
                        SqlParameter sqlParameter = cmd.Parameters.AddWithValue("@UDT", dt);
                        sqlParameter = cmd.Parameters.AddWithValue("@P_COMPCODE", CompCode);
                        sqlParameter = cmd.Parameters.AddWithValue("@P_DivCode", DivCode);
                        sqlParameter = cmd.Parameters.AddWithValue("@P_BATCHID", BatchID);
                        sqlParameter = cmd.Parameters.AddWithValue("@P_IP_ADDRESS", IPAddress);
                        sqlParameter = cmd.Parameters.AddWithValue("@P_USERNAME", UserName);
                        sqlParameter = cmd.Parameters.AddWithValue("@P_TOKEN", token);
                        sqlParameter = cmd.Parameters.AddWithValue("@P_DATE_NOW", DateTime.Now);
                        sqlParameter.Direction = ParameterDirection.Input;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                jsonResponse.IsSuccess = true;
                                jsonResponse.Message= reader.GetString(0);
                            }
                        }
                        tran.Commit();
                      

                    }
                    catch (Exception ex)
                    {
                        jsonResponse.IsSuccess = false;
                        jsonResponse.ResponseData = ex;
                        tran.Rollback();
                        jsonResponse.HasError = true;
                        AppLogger.LogErrorToText(ex);
                    }
                }
                

            }
            return jsonResponse;
        }
        private JsonResponse UpdateBatchAfterTransactionProcessing(List<ATTEBatchProcessing> aTTEBatchProcessings, string DivCode, string CompCode, string BatchID, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            aTTEBatchProcessings.LogData("TransactionProcessing", "Updating Transaction Processing Batch Information");
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))

            {
                connection.Open();
                using (SqlTransaction tran = connection.BeginTransaction())
                {
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@P_COMPCODE", CompCode);
                        parameters.Add("@P_DIVCODE", DivCode);
                        parameters.Add("@P_BATCHID", BatchID);
                        parameters.Add("@P_USERNAME", UserName);
                        parameters.Add("@P_IP_ADDRESS", IPAddress);
                        parameters.Add("@P_DATE_NOW", DateTime.Now);
                        jsonResponse = connection.Query<JsonResponse>("FT_TRANSACTIONPROCESSING_SAVE_UPDATE_BATCH_AFTER_TRANSACTION", param: parameters, tran, commandType: CommandType.StoredProcedure).FirstOrDefault();
                        if (jsonResponse.IsSuccess)
                            tran.Commit();
                        else
                            tran.Rollback();

                    }
                    catch (Exception ex)
                    {
                        jsonResponse.IsSuccess = false;
                        jsonResponse.ResponseData = ex;
                        tran.Rollback();
                        jsonResponse.HasError = true;
                        AppLogger.LogErrorToText(ex);
                    }
                }
                
            }
            return jsonResponse;
        }
        private List<ATTEBatchProcessing> ProcessTransactionData(List<ATTEBatchProcessing> transactionData, string SourceBankName, string SourceBankNumber, string sourceAccountName,
            string DivCode, string CompCode, string BatchID, string UserName, string IPAddress)
        {
            List<ATTEBatchProcessing> allTransctionresponse = new List<ATTEBatchProcessing>();
            int batchSize = NPSHelper.MaxBatchSize();
            int currentIndex = 0;
            // Process data in batches
            while (currentIndex<transactionData.Count)
            {
                int remainingCount = transactionData.Count - currentIndex;
                int currentBatchSize = Math.Min(batchSize, remainingCount);
                List<ATTEBatchProcessing> batch = transactionData.Skip(currentIndex).Take(currentBatchSize).ToList();
                var thisBatchResponse = NPSHelper.TransactionProcessing(batch, SourceBankName, SourceBankNumber, sourceAccountName);
                SaveTransactionProcessingData(thisBatchResponse, DivCode, CompCode, BatchID, UserName, IPAddress);
                allTransctionresponse.AddRange(thisBatchResponse);
                currentIndex += currentBatchSize;
            }
            return allTransctionresponse;
        }
        public JsonResponse TransactionProcessing(string DivCode, string CompCode, string BatchID, string SourceBankName, string SourceBankNumber, string sourceAccountName, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            List<ATTEBatchProcessing> transactionData = new List<ATTEBatchProcessing>();
            JsonResponse transactionDataReq = GetDataForTransactionProcessing(DivCode, CompCode, BatchID, UserName, IPAddress);
            if (!transactionDataReq.IsSuccess)
            {
                return transactionDataReq;
            }
            transactionData = transactionDataReq.ResponseData as List<ATTEBatchProcessing>;
            if (transactionData.Count > 0)
            {
                var transactionDataResponseFromAPI = ProcessTransactionData(transactionData, SourceBankName, SourceBankNumber, sourceAccountName, DivCode, CompCode, BatchID, UserName, IPAddress);
                transactionDataResponseFromAPI.LogData("TransactionProcessing", "API Transaction Processing Return Data");
                jsonResponse = UpdateBatchAfterTransactionProcessing(transactionDataResponseFromAPI, DivCode, CompCode, BatchID, UserName, IPAddress);
            }
            else
            {
                jsonResponse = CloseBatch(DivCode, CompCode, BatchID, UserName, IPAddress, "NO DATA AVAILABLE FOR THE CURRENT DIVIDEND AND THE CURRENT BATCH !!! ");
                AppLogger.LogData("NO DATA AVAILABLE FOR THE CURRENT DIVIDEND AND THE CURRENT BATCH !!! ", "TransactionProcessing", " Trasaction Processing Batch Close");
                jsonResponse.IsSuccess = false;
                jsonResponse.Message = "NO DATA AVAILABLE FOR THE CURRENT DIVIDEND AND THE CURRENT BATCH !!! <br/> BATCH WILL BE CLOSED";
            }
            return jsonResponse;
        }
        #endregion
    }
}
