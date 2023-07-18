

using Entity.Common;
using Entity.Dividend;
using System.Collections.Generic;

namespace Interface.Parameter
{
    public interface IDividendParameterPosting
    {
        JsonResponse GetDividendForApproval(string CompCode);

        JsonResponse DividendRequestPosting(List<ATTDividend> aTTDividend, string CompCode, string UserName, string ActionType);
    }
}
