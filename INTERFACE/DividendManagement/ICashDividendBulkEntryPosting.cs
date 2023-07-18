
using Entity.Common;

namespace Interface.DividendManagement
{
    public interface ICashDividendBulkEntryPosting
    {
        JsonResponse GetDividendList(string CompCode, string BonusType, string ShareType);
        JsonResponse GetSelectedDividendDetails(string CompCode, string BonusType, string ShareType, string DivCode);
        JsonResponse GenerateData(string CompCode, string BonusType, string ShareType, string DivCode, string UserName, string IPAddress);
        JsonResponse Save(string CompCode, string BonusType, string ShareType, string DivCode, string AcceptOrReject, string UserName, string IPAddress);
    }
}
