
using Entity.Common;
using Entity.Dividend;
using System.Collections.Generic;

namespace Interface.DividendProcessing
{
    public interface ICashIssueDividendPosting
    {
        JsonResponse GetCashDividendForApproval(string CompCode, string FormDate, string ToDate, string Divcode, string UserName, string IP);

        JsonResponse PostCashDividendRequest(List<ATTCashDividend> aTTCashDividends, ATTCashDividend RecordDetails, string ActionType, string UserName, string IP);
    }
}
