using Entity.Common;
using Entity.Esewa;
using Entity.HolderInfo;
using Interface.Services;
using Interface.ShareHolder;
using System.Threading.Tasks;

namespace Repository.Services
{
    public class HolderDemateServiesRepo : IHolderDemateServices
    {
        private readonly IPaymentSchedulePosting _paymentSchedulePosting;
        private readonly IInformationFromSystem _informationFromSystem;

        public HolderDemateServiesRepo(IPaymentSchedulePosting paymentSchedulePosting, IInformationFromSystem informationFromSystem)
        {
            _paymentSchedulePosting = paymentSchedulePosting;
            _informationFromSystem = informationFromSystem;
        }

        public async Task<ATTDataTableResponse<ATTBatchProcessing>> GetBatchProcessingAsync(ATTDataTableRequest request, string CompCode, string BatchNo, string DivCode, string PostedData = null, bool isAccountValidate = false, string ProcedureName = null)
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

            var batchProcessings = await _paymentSchedulePosting.GetBatchProcessingAsync(req, CompCode, BatchNo, DivCode, PostedData, isAccountValidate, ProcedureName);
            return new ATTDataTableResponse<ATTBatchProcessing>()
            {
                Draw = request.Draw,
                RecordsTotal = batchProcessings.Count > 0 ? batchProcessings[0].TotalCount : 0,
                RecordsFiltered = batchProcessings.Count > 0 ? batchProcessings[0].FilteredCount : 0,
                Data = batchProcessings.ToArray(),

            };
        }

        public async Task<ATTDataTableResponse<ATTDirtyInfromationFromSystem>> LoadDataTable(ATTDataTableRequest request)
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

            var dirtyInfromationFromSystems = await _informationFromSystem.LoadDataTable(req);
            return new ATTDataTableResponse<ATTDirtyInfromationFromSystem>()
            {
                Draw = request.Draw,
                RecordsTotal = dirtyInfromationFromSystems.Count > 0 ? dirtyInfromationFromSystems[0].TotalCount : 0,
                RecordsFiltered = dirtyInfromationFromSystems.Count > 0 ? dirtyInfromationFromSystems[0].FilteredCount : 0,
                Data = dirtyInfromationFromSystems.ToArray(),

            };
        }
    }
}
