

using Entity.Common;
using Entity.Dividend;
using System.Collections.Generic;

namespace Interface.DividendProcessing
{
    public interface ICashDividendPaymentPosting
    {
        JsonResponse GetCashDividendPaymentForApproval(string CompCode, string Divcode);

        JsonResponse PostCashDividendPaymentRequest(List<ATTCashDividend> aTTCashDividends, string ActionType, string UserName);

    }
}
