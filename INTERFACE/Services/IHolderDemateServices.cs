


using Entity.Common;
using Entity.Esewa;
using Entity.HolderInfo;
using System.Threading.Tasks;

namespace Interface.Services
{
    public interface IHolderDemateServices
    {
        public Task<ATTDataTableResponse<ATTBatchProcessing>> GetBatchProcessingAsync(ATTDataTableRequest request, string CompCode, string BatchNo, string DivCode, string PostedData = null, bool isAccountValidate = false, string ProcedureName = null);
        public Task<ATTDataTableResponse<ATTDirtyInfromationFromSystem>> LoadDataTable(ATTDataTableRequest request);
    }
}
