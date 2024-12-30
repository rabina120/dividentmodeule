//using Dapper;
//using Entity.Common;
//using ENTITY.FundTransfer;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Options;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Threading.Tasks;

//namespace REPOSITORY.FundTransfer.Service
//{
//    public interface ITransactionRepository
//    {
//        Task<List<ATTEBatchProcessing>> GetPendingBatchesAsync(ATTDataListRequest request, string CompCode, string DivCode, string BatchID, string BatchStatus);
//      //  Task UpdateBatchStatusAsync(string batchId, ATTNPSBulkCheckStatusResponse response);
//    }

//    public class TransactionRepository : ITransactionRepository

//    {

//        IOptions<ReadConfig> _connectionString;

//        private IConfiguration _configuration;
//        string _keys;

//        public TransactionRepository(IOptions<ReadConfig> connectionString, IConfiguration configuration)
//        {
//            _connectionString = connectionString;
//            _configuration = configuration;
//            _keys = "EsewaKeys";
//        }        
//        public async Task<List<ATTEBatchProcessing>> GetPendingBatchesAsync(ATTDataListRequest request, string CompCode, string DivCode, string BatchID, string BatchStatus)
//        {
//            try
//            {
//                using (var connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
//                {
//                    connection.Open();
//                    var parameters = new DynamicParameters();
//                    //parameters.Add("SearchValue", request.SearchValue.Trim(), DbType.String);
//                    parameters.Add("PageNo", request.PageNo, DbType.Int32);
//                    parameters.Add("PageSize", request.PageSize, DbType.Int32);
//                    parameters.Add("SortColumn", request.SortColumn, DbType.Int32);
//                    parameters.Add("SortDirection", request.SortDirection, DbType.String);
//                    parameters.Add("CompCode", CompCode.Trim(), DbType.String);
//                    parameters.Add("DivCode", DivCode, DbType.String);
//                    parameters.Add("BatchNo", BatchID.Trim(), DbType.String);
//                    parameters.Add("BatchStatus", BatchStatus.Trim(), DbType.String);

//                    var batchProcessings = (await connection.QueryAsync<ENTITY.FundTransfer.ATTEBatchProcessing>(
//                        "FT_TRANSACTION_GETACCOUNTVALIDATEDDATA",
//                        parameters,
//                        commandType: CommandType.StoredProcedure)).ToList();

//                    return batchProcessings;
//                }
//            }
//            catch (Exception ex)
//            {
//                throw new Exception(ex.Message, ex);
//            }
//        }

        
//    }
//}
