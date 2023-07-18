using Entity.Common;
using ENTITY.DemateDividend;
using System.Collections.Generic;
using System.Data;

namespace INTERFACE.DividendManagement
{
    public interface IPoolAccountSplit
    {
        JsonResponse GetHolderInfoForSplit(string BOID, string UserName, string CompCode, string IPAddress, string DivCode, string Action);
        JsonResponse GetCompleteDataFromExcel(List<ATTPoolAccountList> aTTBulkCAEntries, string UserName, string CompCode, string IPAddress);
        JsonResponse UploadPASData(DataTable PasTable, string UserName, string IPAddress, string CompCode, string DivCode, string BOID, string Action);
        JsonResponse GetSplitPostingList(string CompCode, string DivCode, string Username, string IPAddress, int? ParentId);
        JsonResponse SubmitForPostring(List<ATTSplit> postingList, string Username, string IpAddress, string Action, string PostingDate, string Remarks);
    }
}
