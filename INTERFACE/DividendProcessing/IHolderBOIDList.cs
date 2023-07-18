using Entity.Common;

namespace Interface.DividendProcessing
{
    public interface IHolderBOIDList
    {
        JsonResponse GetHolderDetails(string CompCode, string HolderNo, string ShareType, string DivType, string UserName);
    }
}
