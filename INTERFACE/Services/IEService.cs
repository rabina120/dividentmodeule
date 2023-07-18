using Entity.Common;
using Entity.Esewa;
using System.Threading.Tasks;

namespace Interface.Services
{
    public interface IEService
    {
        public Task<ATTDataTableResponse<ATTBatchProcessing>> GetBatchProcessingAsync(ATTDataTableRequest request, string CompCode, string DivCode, string BatchNo, string BatchStatus);
        //public Task<ATTDataTableResponse<ATTDirtyInfromationFromSystem>> LoadDataTable(ATTDataTableRequest request);
    }
}
