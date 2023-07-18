
using Entity.Common;

namespace Interface.DividendProcessing
{
    public interface IDemateDividend
    {
        JsonResponse GetDemateDividendTableList(string compcode);

        JsonResponse GetDemateDividendInformation(string CompCode, string DivCode, string shholderno, string a, string action,string username,string IPAddress);
    }
}
