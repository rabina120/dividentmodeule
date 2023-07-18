using Entity.Common;
using Entity.Esewa;
using Interface.Esewa;
using Interface.Services;
using System.Threading.Tasks;

namespace Repository.Services

{
    public class EServiceRepo : IEService
    {
        private readonly ITransactionStatus _transactionStatus;

        public EServiceRepo(ITransactionStatus transactionStatus)
        {
            _transactionStatus = transactionStatus;
        }

        public async Task<ATTDataTableResponse<ATTBatchProcessing>> GetBatchProcessingAsync(ATTDataTableRequest request, string CompCode, string DivCode, string BatchNo, string BatchStatus)
        {
            var req = new ATTDataListRequest()
            {
                PageNo = request.Start,
                PageSize = request.Length,
                SortColumn = request.Order[0].Column,
                SortColumnName = request.Order[0].ColumnName,
                SortDirection = request.Order[0].Dir,
                SearchValue = request.Search != null ? request.Search.Value.Trim() : ""
            };

            var batchProcessings = await _transactionStatus.GetAccountValidatedData(req, CompCode, DivCode, BatchNo, BatchStatus);
            return new ATTDataTableResponse<ATTBatchProcessing>()
            {
                Draw = request.Draw,
                RecordsTotal = batchProcessings.Count > 0 ? batchProcessings[0].TotalCount : 0,
                RecordsFiltered = batchProcessings.Count > 0 ? batchProcessings[0].FilteredCount : 0,
                Data = batchProcessings.ToArray(),

            };
        }

    }
}
