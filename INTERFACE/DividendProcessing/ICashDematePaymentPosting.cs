

using Entity.Common;
using Entity.Dividend;
using System.Collections.Generic;

namespace Interface.DividendProcessing
{
    public interface ICashDematePaymentPosting
    {
        public JsonResponse GetCashDematePaymentForApproval(string CompCode, string Divcode);

        public JsonResponse PostCashDematePaymentRequest(List<ATTDivMasterCDS> attDivMasterCDS, string ActionType, string UserName);

    }
}
