using Entity.Common;
using Entity.Esewa;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interface.Reports
{
    public interface IEsewaStatusReport
    {
        JsonResponse GetAllDividends(string CompCode);
        JsonResponse GetAllBatch(string compCode, string divCode);
        Task<List<ATTBatchProcessing>> GetBatchReportData(ATTDataListRequest request, string CompCode, string DivCode, string BatchID, string ReportType, string ReportSubType);


        JsonResponse GenerateReportData(string CompCode, string DivCode, string Batch, string ReportType, string ReportSubType,string exportTo, string UserName,string IpAddress);
    }
}
