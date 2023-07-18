using Entity.Common;

namespace Interface.DividendManagement
{
    public interface IHoldersHistory
    {
        JsonResponse GetHoldersHistory(string CompCode, string DivType, string ShareType, string ShHolderNo, int occupation, string UserName, string IPAddress);
    }
}
