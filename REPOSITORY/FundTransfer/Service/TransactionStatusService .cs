//using Entity.Common;
//using ENTITY.FundTransfer;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Options;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.Threading.Tasks;

//namespace REPOSITORY.FundTransfer.Service
//{
//    public interface ITransactionStatusService
//    {
//        //Task CheckAndUpdateTransactionStatuses();
//        Task CheckAndUpdateTransactionStatuses(ATTDataListRequest request, string CompCode, string DivCode, string BatchID, string BatchStatus);

//    }

//    public class TransactionStatusService : ITransactionStatusService
//    {
//        private readonly ITransactionRepository _transactionRepository;
//        IOptions<ReadConfig> _connectionString;

//        private IConfiguration _configuration;
//        string _keys;
      
//        public TransactionStatusService(IOptions<ReadConfig> connectionString, IConfiguration configuration,ITransactionRepository transactionRepository)
//        {
//            _transactionRepository = transactionRepository;
//            _connectionString = connectionString;
//            _configuration = configuration;
//            _keys = "EsewaKeys";
//        }
        
//        public async Task CheckAndUpdateTransactionStatuses(ATTDataListRequest request, string CompCode, string DivCode, string BatchID, string BatchStatus)
//       {
//            try
//            {
//                // Fetch pending batches from the repository
//                var pendingBatches = await _transactionRepository.GetPendingBatchesAsync(request, CompCode, DivCode, BatchID, BatchStatus);

//                using (var connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
//                {
//                    connection.Open();

//                    foreach (var batch in pendingBatches)
//                    {
//                        try
//                        {
//                            var resp = NPSHelper.TransactionStatus(new List<ATTEBatchProcessing> { batch });


//                            // Call the external API to fetch transaction statuses
//                            //var res = NPSHelper.TransactionStatus(batch);
//                            if (!resp.IsSuccess)
//                            {
//                                Console.WriteLine($"Failed to fetch status for batch: {BatchID}");
//                                continue;
//                            }

//                            var apiResponse = (List<ATTEBatchProcessing>)resp.ResponseData;
//                            if (apiResponse == null)
//                            {
//                                Console.WriteLine($"No data returned from API for batch: {BatchID}");
//                                continue;
//                            }

//                            using (var tran = connection.BeginTransaction())
//                            {
//                                try
//                                {
//                                    // Prepare DataTable for batch update
//                                    var dt = new DataTable();
//                                    dt.Columns.Add("WarrantNo", typeof(string));
//                                    dt.Columns.Add("TransactionCode", typeof(string));
//                                    dt.Columns.Add("TransactionMessage", typeof(string));
//                                    dt.Columns.Add("TransactionDetail", typeof(string));

//                                    // Populate DataTable
//                                    foreach (var item in apiResponse)
//                                    {
//                                        dt.Rows.Add(item.sub_token, item.UpdatedTransactionCode, item.UpdatedTransactionMessage, item.UpdatedTransactionDetail);
//                                    }

//                                    // Create and execute the stored procedure
//                                    using (var cmd = new SqlCommand("FT_TRANSACTIONSTATUS_UPDATE", connection, tran))
//                                    {
//                                        cmd.CommandType = CommandType.StoredProcedure;

//                                        cmd.Parameters.AddWithValue("@UDT", dt);
//                                        cmd.Parameters.AddWithValue("@P_COMPCODE", CompCode);
//                                        cmd.Parameters.AddWithValue("@P_DivCode", DivCode);
//                                        cmd.Parameters.AddWithValue("@P_BATCHID", BatchID); // Assuming batch.BatchId is BatchNo
//                                        cmd.Parameters.AddWithValue("@P_IP_ADDRESS", '1'); // Replace with actual IP address variable
//                                        cmd.Parameters.AddWithValue("@P_USERNAME", "NIRMAL");   // Replace with actual username variable
//                                        cmd.Parameters.AddWithValue("@P_DATE_NOW", DateTime.Now);

//                                        using (var reader = await cmd.ExecuteReaderAsync())
//                                        {
//                                            if (reader.Read() && reader.GetString(0) == "true")
//                                            {
//                                                Console.WriteLine($"Batch {BatchID} updated successfully.");
//                                                tran.Commit();
//                                            }
//                                            else
//                                            {
//                                                Console.WriteLine($"Failed to update batch {BatchID}: {reader.GetString(1)}");
//                                                tran.Rollback();
//                                            }
//                                        }

//                                        //return jsonResponse;
//                                    }
//                                }
//                                catch (Exception ex)
//                                {
//                                    tran.Rollback();
//                                    Console.WriteLine($"Error during batch {BatchID} update: {ex.Message}");
//                                }
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            Console.WriteLine($"Error processing batch {BatchID}: {ex.Message}");
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Critical error: {ex.Message}");
//                throw; // Re-throw the exception to ensure visibility at higher levels
//            }
//        }        
//    }
//}
