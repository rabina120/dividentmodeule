

using Entity.Common;
using Entity.Dividend;
using System.Collections.Generic;

namespace Interface.DividendProcessing
{
    public interface ICashDemateIssuePosting
    {
        public JsonResponse GetCashDemateForApproval(string CompCode,string FromDate, string ToDate, string Divcode, string UserName, string IP);

        public JsonResponse PostCashDemateRequest(List<ATTDivMasterCDS> attDivMasterCDS, ATTDivMasterCDS RecordDetails, string ActionType, string UserName, string IP);
    }
}
