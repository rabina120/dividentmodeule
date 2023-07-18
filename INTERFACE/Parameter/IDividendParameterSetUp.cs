
using Entity.Common;
using Entity.Dividend;

namespace Interface.Parameter
{
    public interface IDividendParameterSetUp
    {

        JsonResponse GetDividendCode(string CompCode);
        string GetDividendCodeString(string CompCode);
        JsonResponse SaveParameterSetup(ATTDividend dividend, string ActionType, string UserName, string IPAddress);

        JsonResponse DeleteParameterSetup(string UserName, string CompCode, string DivCode, string ActionType, string IPAddress);

        JsonResponse GetDividivendDetails(string CompCode, string DivCode, string UserName, string IPAddress);

    }
}
