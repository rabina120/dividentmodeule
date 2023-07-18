
using Entity.Common;

namespace Interface.DividendProcessing
{
    public interface IDividend
    {
        JsonResponse GetDividendTableList(string CompCode);

        JsonResponse GetDividendPaidSummary(string CompCode, string DivCode, string StartDate, string EndDate);

    }
}
