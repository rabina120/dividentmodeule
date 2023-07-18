

using Entity.Common;
using Entity.Esewa;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interface.ShareHolder
{
    public interface IPaymentSchedulePosting
    {
        JsonResponse DividendBatchDetails(string CompCode, string DivCode);
        Task<List<ATTBatchProcessing>> GetBatchProcessingAsync(ATTDataListRequest request, string CompCode, string BatchNo, string DivCode = null, string PostedData = null, bool isAccountValidate = false, string ProcedureName = null);
        JsonResponse SaveBatchProcessingList(string CompCode, string BatchNo, string DivCode = null, string UserName = null);
        JsonResponse ExportToExcel(string CompCode, string BatchNo);
    }
}
