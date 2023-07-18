using Entity.CDS;
using Entity.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interface.CDS
{
    public interface IBulkCAEntry
    {
        JsonResponse GetCompleteDataFromExcel(List<ATTBulkCAEntry> aTTBulkCAEntries, string UserName, string CompCode, string CertDetailId, string ShOwnerType, string IPAddress);
        JsonResponse SaveBulkCAEntry(List<ATTBulkCAEntry> aTTBulkCAEntries, string CompCode, string TransactionDate, string CertDetail, string UserName, string IPAddress);

        //JsonResponse GetDetails(string CompanyCode, int? Cret_Id, int? ShOwnerType);
        //Task<List<ATTBulkCAEntry>> GetAllDetails(ATTDataListRequest request, string CompanyCode, int? Cret_Id, int? ShOwnerType);

        Task<ATTDataTableResponse<ATTBulkCAEntry>> GetData(ATTDataTableRequest request, string CompanyCode, int? Cret_Id, int? ShOwnerType);

        JsonResponse GenerateReport(string CompanyCode, int? Cret_Id, int? ShOwnerType, string ExportReportType);
    }
}
